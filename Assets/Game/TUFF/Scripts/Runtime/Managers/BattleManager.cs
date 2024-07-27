using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace TUFF
{
    public enum BattleState
    {
        START = 0,
        PLAYERACTIONS = 1,
        BATTLE = 2,
        WON = 3,
        LOST = 4,
        ESCAPED = 5
    }
    public class BattleManager : MonoBehaviour
    {
        public BattleHUD hud;
        public BattleState battleState = BattleState.START;
        public int turn = 0;
        [SerializeField] private Battle battle;
        public int queuedSkillsIndex = 0;
        public List<TargetedSkill> queuedSkills = new List<TargetedSkill>();
        public List<TargetedSkill> nextTurnQueuedSkills = new List<TargetedSkill>();
        public UnityEvent onActionEnd = new UnityEvent();
        public UnityEvent onTurnEnd = new UnityEvent();
        public PartyMember[] activeParty = new PartyMember[0];
        public List<EnemyInstance> enemies = new List<EnemyInstance>();
        public int magsCollected = 0;
        public int expCollected = 0;

        [Header("Battle Toggles")]
        public bool disableStatChanges = false;
        public bool disableStateChanges = false;

        [HideInInspector] public Dictionary<InventoryItem, int> rewards = new Dictionary<InventoryItem, int>();
        private EventAction eventCallback = null;

        public static BattleManager instance;

        public bool InBattle { get => m_inBattle; }
        private bool m_inBattle = false;
        public bool CanEscape { get => m_canEscape; }
        private bool m_canEscape = false;
        public void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        public void TestBattle(Battle battle)
        {
            InitiateBattle(battle, true);
        }
        public void InitiateBattle(Battle battle, bool canEscape = false, EventAction evtCallback = null)
        {
            if (battle == null) { 
                Debug.LogWarning("Battle is null!"); 
                if (evtCallback != null) evtCallback.isFinished = true;
                return; 
            }
            UIController.instance.TriggerBattleStart();
            StartCoroutine(LoadBattle(battle, evtCallback, canEscape));
        }
        private IEnumerator LoadBattle(Battle battle, EventAction evtCallback, bool canEscape)
        {
            m_inBattle = true;
            m_canEscape = canEscape;

            if (battle.autoPlayBGM) AudioManager.instance.PlayMusic(battle.bgm);
            //Move To BattleHUD
            eventCallback = evtCallback;
            this.battle = Instantiate(battle, hud.battleStage);
            this.battle.name = "BattleContent";
            //End Move To BattleHUD
            GameManager.instance.DisablePlayerInput(true);
            AudioManager.instance.ChangeAmbienceVolume(0);
            battleState = BattleState.START;
            turn = 0;
            rewards.Clear();
            magsCollected = 0;
            expCollected = 0;
            GetActivePartyMembers();
            InitiateEnemyInstances();
            ResetTargetablesActedCheck();
            ResetQueuedCommands();
            ResetNextTurnQueuedCommands();
            hud.battleStage.gameObject.SetActive(false);
            hud.InitializeHUD();
            ForceUpdateHUD(true);

            while (!UIController.instance.BattleStartIsFinished())
            {
                yield return null;
            }
            hud.battleStage.gameObject.SetActive(true);
            UIController.instance.HideBattleStart();
            hud.gameObject.SetActive(true);
            StartCoroutine(StartTurn());
        }
        private IEnumerator StartTurn()
        {
            yield return CheckBattleEvents();
            UncheckBattleConditionsPlayedState(SpanType.OncePerTurn);
            turn += 1;

            ResetTargetablesActedCheck();
            ResetQueuedCommands();
            AssignNextTurnQueuedCommands();
            ResetNextTurnQueuedCommands();
            UpdateTargetablesStates();
            StartPlayerActions();
        }
        private void StartPlayerActions()
        {
            battleState = BattleState.PLAYERACTIONS;
            ForceUpdateHUD(true);
            hud.StartPlayerActions();
        }
        private IEnumerator CheckBattleEvents()
        {
            var events = battle.battleEvents;
            for (int i = 0; i < events.Length; i++)
            {
                if (events[i].conditions.ValidateConditions())
                    yield return events[i].actionList.PlayActions();
            }
        }
        private void UncheckBattleConditionsPlayedState(SpanType spanType)
        {
            var elements = System.Array.FindAll(battle.battleEvents, e => e.conditions.span == spanType);
            for (int i = 0; i < elements.Length; i++)
                elements[i].conditions.SetPlayed(false);
        }
        private void GetActivePartyMembers()
        {
            activeParty = new PartyMember[PlayerData.activePartyMaxSize];
            for (int i = 0; i < activeParty.Length; i++)
            {
                activeParty[i] = GameManager.instance.playerData.GetActivePartyMember(i);
                activeParty[i]?.OnBattleStart();
            }
        }
        private void InitiateEnemyInstances()
        {
            enemies = new List<EnemyInstance>();
            for (int i = 0; i < battle.initialEnemies.Count; i++)
            {
                enemies.Add(new EnemyInstance(battle.initialEnemies[i]));
                enemies[i]?.OnBattleStart();
            }
        }
        public bool IsPartOfQueuedUnitedSkill(Targetable user)
        {
            for (int i = 0; i < queuedSkills.Count; i++)
            {
                if (queuedSkills[i].user is UnitedPartyMember united)
                {
                    if (united.userA == user || united.userB == user)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public Targetable CheckForUnitedSkillUser(IBattleInvocation invocation, Targetable user)
        {
            if (invocation is Skill skill && user is PartyMember)
            {
                if (skill.isUnitedSkill)
                {
                    var userA = PlayerData.instance.GetPartyMember(skill.unitedUserA);
                    var userB = PlayerData.instance.GetPartyMember(skill.unitedUserB);
                    user = new UnitedPartyMember(userA, userB);
                }
            }
            return user;
        }
        public void QueueCommandNextTurn(TargetedSkill targetedSkill)
        {
            if (targetedSkill == null) return;
            targetedSkill.user = CheckForUnitedSkillUser(targetedSkill.skill, targetedSkill.user);
            nextTurnQueuedSkills.Add(targetedSkill);
        }
        public void QueueCommand(TargetedSkill targetedSkill)
        {
            if (targetedSkill == null) return;
            targetedSkill.user = CheckForUnitedSkillUser(targetedSkill.skill, targetedSkill.user);
            queuedSkills.Add(targetedSkill);
        }
        public void QueueCommand(IBattleInvocation skill, List<Targetable> targets, Targetable user, UnitHUD unitCommandCallback = null)
        {
            user = CheckForUnitedSkillUser(skill, user);
            queuedSkills.Add(new TargetedSkill(skill, targets, user, unitCommandCallback));
        }
        public void QueueCommand(IBattleInvocation skill, List<Targetable> targets, Targetable user, int index, UnitHUD unitCommandCallback = null)
        {
            user = CheckForUnitedSkillUser(skill, user);
            index = Mathf.Clamp(index, 0, queuedSkills.Count);
            queuedSkills.Insert(index, new TargetedSkill(skill, targets, user, unitCommandCallback));
        }
        /// <summary>
        /// Gets the index in list of the queued command from the specified user.
        /// </summary>
        /// <returns>Index in list of the queued command.</returns>
        public int IndexOfQueuedCommandFromUser(Targetable user) // Change this to work with act times
        {
            return queuedSkills.FindIndex(q => q.user == user);
        }
        public bool HasQueuedCommandFromUser(Targetable user) => IndexOfQueuedCommandFromUser(user) >= 0;
        
        public void QueueOrReplaceCommand(IBattleInvocation skill, List<Targetable> targets, Targetable user, UnitHUD unitCommandCallback = null)
        {
            int index = IndexOfQueuedCommandFromUser(user);
            if (index >= 0)
            {
                queuedSkills[index] = new TargetedSkill(skill, targets, user, unitCommandCallback);
                Debug.Log("Replace");
                return;
            }
            QueueCommand(skill, targets, user, unitCommandCallback);
        }
        public void RemoveCommand(int index)
        {
            if (index >= queuedSkills.Count || index < 0) return;
            queuedSkills.RemoveAt(index);
        }
        public void RemoveLastCommand()
        {
            if (queuedSkills.Count <= 0) return;
            queuedSkills.RemoveAt(queuedSkills.Count - 1);
        }
        private void ResetQueuedCommands()
        {
            queuedSkillsIndex = 0;
            if (queuedSkills == null) queuedSkills = new List<TargetedSkill>();
            else queuedSkills.Clear();
        }
        private void ResetNextTurnQueuedCommands()
        {
            if (nextTurnQueuedSkills == null) nextTurnQueuedSkills = new List<TargetedSkill>();
            else nextTurnQueuedSkills.Clear();
        }
        private void AssignNextTurnQueuedCommands()
        {
            queuedSkills = new List<TargetedSkill>(nextTurnQueuedSkills);
        }
        private void SortQueuedSkillBySpeed()
        {
            if (queuedSkills.Count <= 1) return;
            queuedSkills.Sort((skl1, skl2) => skl2.attackSpeed.CompareTo(skl1.attackSpeed));
        }
        public void EscapeBattle()
        {
            battleState = BattleState.ESCAPED;
            EndBattle();
        }
        public void RunBattleActions()
        {
            QueueEnemyActions();
            QueueTargetablesForcedActions();
            SortQueuedSkillBySpeed(); // Organize actions by Speed
            battleState = BattleState.BATTLE;
            StartCoroutine(RunBattleActionsCoroutine());
        }
        IEnumerator RunBattleActionsCoroutine() //Need to optimize some code here...
        {
            yield return new WaitForSeconds(1f);
            // Play all the queued invocations
            for (queuedSkillsIndex = 0; queuedSkillsIndex < queuedSkills.Count; queuedSkillsIndex++)
            {
                yield return StartCoroutine(queuedSkills[queuedSkillsIndex].InvokeSkill());
                if (queuedSkills[queuedSkillsIndex].markIgnore) continue;
                onActionEnd.Invoke();
                yield return new WaitForSeconds(0.5f);
                hud.ShowDescriptionDisplay(false);
                if (queuedSkills[queuedSkillsIndex].user != null) CheckActionEndStatesDuration(queuedSkills[queuedSkillsIndex].user);
                CheckGameOver();
                CheckWin();
                yield return CheckActEndEvents();
                UncheckBattleConditionsPlayedState(SpanType.OncePerAct);
                if (CheckBattleEnd()) break; 
            }
            // If battle ended from party KO or Enemy KO, end battle
            if (CheckBattleEnd())
            {
                EndBattle();
                yield break;
            }
            // Remove states and apply HP, SP and TP regens
            CheckTargetablesTurnEndStatesDuration();
            ApplyTargetablesRegens();
            // Check if enemies or party died from negative HP regens
            if (CheckGameOver() || CheckWin() || CheckBattleEnd())
            {
                yield return CheckActEndEvents();
                EndBattle();
                yield break;
            }
            yield return CheckActEndEvents();
            if (CheckWin() || CheckBattleEnd())
            {
                EndBattle();
                yield break;
            }
            onTurnEnd.Invoke();
            StartCoroutine(StartTurn());
        }
        private IEnumerator CheckActEndEvents()
        {
            yield return CheckBattleEvents();
            yield return PlayEnemyDeaths();
        }
        protected IEnumerator PlayEnemyDeaths()
        {
            bool won = CheckWin();
            var KOdEnemies = enemies.FindAll(e => (e.isKOd && !e.playedKOAnimation));
            //var KOdEnemies = GetUnplayedKOAnimations();
            for (int i = 0; i < KOdEnemies.Count; i++)
            {
                //Say text here
                hud.ShowWindowsDynamic(false);
                AddEnemyRewards(KOdEnemies[i].enemyRef);
                KOdEnemies[i].RemoveAllStates();
                if (won && i == KOdEnemies.Count - 1) PlayWinAudio();
                yield return KOdEnemies[i].PlayKOAnimation();
                yield return KOdEnemies[i].PlayKOMotion();
            }
        }
        protected List<EnemyInstance> GetUnplayedKOAnimations()
        {
            var list = new List<EnemyInstance>();
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].isKOd && !enemies[i].playedKOAnimation)
                    list.Add(enemies[i]);
            }
            return list;
        }
        public void DequeueSkillsFromUser(Targetable user)
        {
            int index = queuedSkills.FindIndex(q => q.user == user);
            if (index >= 0) queuedSkills[index].markIgnore = true;
            //for (int i = queuedSkillsIndex; i < queuedSkills.Count; i++)
            //{
            //    if (queuedSkills[i].user != user) continue;
            //    queuedSkills[i].markIgnore = true;
            //}
        }
        public BattleAnimation PlayBattleAnimation(TargetedSkill targetedSkill)
        {
            if (targetedSkill == null) return null;
            if (targetedSkill.skill == null) return null;
            if (targetedSkill.skill.animation == null) return null;
            return PlayAnimation(targetedSkill.skill.animation, targetedSkill);
        }
        private BattleAnimation PlayAnimation(BattleAnimation battleAnimation, TargetedSkill targetedSkill)
        {
            GameObject skillAnimGO;
            BattleAnimation skillAnim;
            CreateAnimation(battleAnimation, targetedSkill, out skillAnimGO, out skillAnim);
            var target = targetedSkill.targets[0];
            SetAnimationPosition(skillAnimGO.GetComponent<RectTransform>(), skillAnim.callRef, battleAnimation.GetPivot(target));

            return skillAnim;
        }
        /// <summary>
        /// No TargetedSkill data version.
        /// </summary>
        public BattleAnimation PlayAnimation(BattleAnimation battleAnimation, Vector3 position)
        {
            return PlayAnimation(battleAnimation, position, AnimationPivotType.Center);
        }
        public BattleAnimation PlayAnimation(BattleAnimation battleAnimation, Vector3 position, AnimationPivotType pivot)
        {
            GameObject skillAnimGO;
            BattleAnimation skillAnim;
            CreateAnimation(battleAnimation, null, out skillAnimGO, out skillAnim);
            //skillAnimGO.GetComponent<RectTransform>().position = position;
            AssignAnimationPositionFromPivot(skillAnim.transform as RectTransform, pivot, position);
            return skillAnim;
        }
        private void CreateAnimation(BattleAnimation battleAnimation, TargetedSkill targetedSkill, out GameObject skillAnimGO, out BattleAnimation skillAnim)
        {
            skillAnimGO = Instantiate(battleAnimation.gameObject, hud.battleStage);
            skillAnim = skillAnimGO.GetComponent<BattleAnimation>();
            skillAnim.InitiateAnimation(targetedSkill);
            //hud.AddToBattleStage(skillAnimGO.transform);
        }
        public static void SetAnimationPosition(RectTransform rectTransform, TargetedSkill targetedSkill, AnimationPivotType pivot = AnimationPivotType.Center)
        {
            if (targetedSkill == null) return;
            if (targetedSkill.targets == null) return;
            if (targetedSkill.targets.Count <= 0) return;
            if (IsGroupScope(targetedSkill.scopeData.scopeType) || IsRandomScope(targetedSkill.scopeData.scopeType)) return;

            var target = targetedSkill.targets[0];
            var basePosition = target.imageReference.GetCameraPosition();
            AssignAnimationPositionFromPivot(rectTransform, pivot, basePosition);
            //rectTransform.position = targetedSkill.targets[0].imageReference.rect.position;
        }
        public static void AssignAnimationPositionFromPivot(RectTransform rectTransform, AnimationPivotType pivot, Vector3 basePosition)
        {
            rectTransform.position = basePosition;
            switch (pivot)
            {
                case AnimationPivotType.Top:
                    rectTransform.anchoredPosition += (Vector2.up * 560f);
                    break;
                case AnimationPivotType.Bottom:
                    rectTransform.anchoredPosition += (Vector2.up * -560f);
                    break;
                case AnimationPivotType.Screen:
                    rectTransform.anchoredPosition = (Vector2.zero);
                    break;
            }
        }
        public bool GetPatternSkillAndTargets(Targetable user, List<ActionPattern> actionPatterns, out Skill skill, out List<Targetable> targets, int fromTurn)
        {
            skill = GetSkillFromPatternPool(actionPatterns, user, fromTurn);
            if (skill == null) { targets = null; return false; }
            var validTargets = GetInvocationValidTargets(user, skill.scopeData);
            targets = GetDefaultTargets(validTargets, skill.scopeData);
            return true;
        }
        private void QueueEnemyActions()
        {
            if (CheckBattleEnd()) return;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].CanControlAct()) continue;
                if (HasQueuedCommandFromUser(enemies[i])) continue; // Change this to work with act times
                Skill skill;
                List<Targetable> targets = new List<Targetable>();
                if (!GetPatternSkillAndTargets(enemies[i], enemies[i].enemyRef.actionPatterns, out skill, out targets, turn)) continue;
                QueueCommand(skill, targets, enemies[i]);
            }
        }
        private void QueueTargetablesForcedActions()
        {
            if (CheckBattleEnd()) return;
            for (int i = 0; i < GameManager.instance.playerData.GetActivePartySize(); i++)
            {
                var user = activeParty[i];
                if (!user.CanAct()) continue;
                var actionPatterns = user.GetForcedActions();
                Skill skill;
                List<Targetable> targets = new List<Targetable>();
                if (!GetPatternSkillAndTargets(user, actionPatterns, out skill, out targets, turn)) continue;
                QueueCommand(skill, targets, user);
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                var user = enemies[i];
                if (!user.CanAct()) continue;
                var actionPatterns = user.GetForcedActions();
                Skill skill;
                List<Targetable> targets = new List<Targetable>();
                if (!GetPatternSkillAndTargets(user, actionPatterns, out skill, out targets, turn)) continue;
                QueueCommand(skill, targets, user);
            }
        }
        public static List<Targetable> GetDefaultTargets(List<Targetable> validTargets, ScopeData scopeData)
        {
            if (validTargets.Count == 0) return null;
            var scope = scopeData.scopeType;
            if (scope == ScopeType.TheUser) // if TheUser, pick itself
            {
                return new List<Targetable>() { validTargets[0] };
            }
            if (IsSingleScope(scope))
            {
                return RollForTargets(validTargets);
            }
            if (IsGroupScope(scope) || IsRandomScope(scope))
            {
                return validTargets;
            }

            return null;
        }
        public static List<Targetable> RollForTargets(List<Targetable> validTargets, int numberOfTargets = 1)
        {
            List<Targetable> targets = new List<Targetable>();

            int totalValue = 0;
            int[] targetRates = new int[validTargets.Count];
            for (int i = 0; i < validTargets.Count; i++)
            {
                targetRates[i] = LISAUtility.Truncate(validTargets[i].GetTargetRate() * 100f);
                //Debug.Log(targetRates[i]);
                if (targetRates[i] <= 0) targetRates[i] = 1;
                totalValue += targetRates[i];
            }
            if (numberOfTargets < 0) numberOfTargets = 0;
            int[] targetValue = new int[numberOfTargets];
            for (int i = 0; i < numberOfTargets; i++)
            {
                int minValue = 1;
                int maxValue = 0;
                targetValue[i] = Random.Range(1, totalValue + 1);
                Debug.Log($"Rolled: {targetValue[i]}. Total: {totalValue}");
                for (int j = 0; j < validTargets.Count; j++)
                {
                    maxValue += targetRates[j];
                    if (targetValue[i] >= minValue && targetValue[i] <= maxValue)
                    {
                        targets.Add(validTargets[j]);
                        break;
                    }
                }
            }
            return targets;
        }
        private Skill GetSkillFromPatternPool(List<ActionPattern> actionPatterns, Targetable user, int fromTurn)
        {
            if (user == null) return null;
            if (actionPatterns.Count <= 0) return null;
            int minValue = 1;
            int maxValue = 0;
            int totalValue = 0;

            List<ActionPattern> validActionPatterns = new List<ActionPattern>();
            for (int i = 0; i < actionPatterns.Count; i++) // Get all actionPatterns which's conditions are met).
            {
                var actionPattern = actionPatterns[i];
                // If condition is met, has validTargets and attackRate is more than 0, add value to list, if not continue.
                if (GetInvocationValidTargets(user, actionPattern.skill.scopeData).Count > 0 && 
                    actionPattern.conditions.ValidateConditions(user, fromTurn) &&
                    actionPattern.attackRate > 0)
                {
                    validActionPatterns.Add(actionPattern);
                    totalValue += actionPattern.attackRate;
                }
            }
            //Debug.Log($"totalValue: {totalValue}");
            int targetValue = Random.Range(1, totalValue + 1);
            //Debug.Log($"targetValue: {targetValue}");
            for (int i = 0; i < validActionPatterns.Count; i++)
            {
                maxValue += validActionPatterns[i].attackRate;
                //Debug.Log($"maxValue: {maxValue}; minValue: {minValue}");
                if (targetValue >= minValue && targetValue <= maxValue)
                {
                    return validActionPatterns[i].skill;
                }
                minValue += validActionPatterns[i].attackRate;
            }
            return null;
        }
        public static bool IsValidTargetFromConditionScope(ConditionScopeType conditionScope, Targetable targetable)
        {
            var isKOd = targetable.isKOd;
            switch (conditionScope)
            {
                case ConditionScopeType.OnlyAlive:
                    if (!isKOd) return true;
                    break;
                case ConditionScopeType.OnlyKO:
                    if (isKOd) return true;
                    break;
                case ConditionScopeType.AliveAndKO:
                    return true;
                default:
                    return false;
            }
            return false;
        }
        public static List<Targetable> CheckValidTargetFromConditionScope(ConditionScopeType conditionScope, Targetable targetable, List<Targetable> targetListToAddTo = null)
        {
            if (targetable == null) return null;
            if (targetListToAddTo == null) targetListToAddTo = new List<Targetable>();
            if (IsValidTargetFromConditionScope(conditionScope, targetable))
                targetListToAddTo.Add(targetable);
            return targetListToAddTo;
        }
        /// <summary>
        /// Returns all possible targets based on the skill's scope and caster
        /// </summary>
        /// <param name="user"></param>
        /// <param name="skill"></param>
        /// <returns>List of all possible targets for the skill</returns>
        public List<Targetable> GetInvocationValidTargets(Targetable user, ScopeData scopeData)
        {
            var validTargets = new List<Targetable>();
            if (user == null) return validTargets;
            if (scopeData == null) return validTargets;
            var scope = scopeData.scopeType;
            if (scope == ScopeType.TheUser)
            {
                if (!user.isKOd) validTargets.Add(user);
                return validTargets;
            }
            var activePartySize = GameManager.instance.playerData.GetActivePartySize();
            var excludeUser = scopeData.excludeUser;
            if (user is PartyMember)
            {
                if (IsEnemyScope(scope) || IsAnyoneScope(scope))
                {
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (enemies[i] == null) continue;
                        if (excludeUser && enemies[i] == user) continue;
                        CheckValidTargetFromConditionScope(scopeData.targetCondition, enemies[i], validTargets);
                    }
                }
                if (IsAllyScope(scope) || IsAnyoneScope(scope))
                {
                    for (int i = 0; i < activePartySize; i++)
                    {
                        if (activeParty[i] == null) continue;
                        if (excludeUser && activeParty[i] == user) continue;
                        CheckValidTargetFromConditionScope(scopeData.targetCondition, activeParty[i], validTargets);
                    }
                }
            }
            else if (user is EnemyInstance)
            {
                if (IsEnemyScope(scope) || IsAnyoneScope(scope))
                {
                    for (int i = 0; i < activePartySize; i++)
                    {
                        if (activeParty[i] == null) continue;
                        if (excludeUser && activeParty[i] == user) continue;
                        CheckValidTargetFromConditionScope(scopeData.targetCondition, activeParty[i], validTargets);
                    }
                }
                if (IsAllyScope(scope) || IsAnyoneScope(scope))
                {
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (enemies[i] == null) continue;
                        if (excludeUser && enemies[i] == user) continue;
                        CheckValidTargetFromConditionScope(scopeData.targetCondition, enemies[i], validTargets);
                    }
                }
            }
            Debug.Log("VALID TARGETS: " + validTargets.Count);
            return validTargets;
        }
        /// <summary>
        /// Checks if the user from the TargetedSkill has enough SP or TP to use the skill.
        /// </summary>
        /// <param name="targetedSkill">TargetedSkill to check</param>
        /// <param name="payCost">If true, charges the corresponding SP, TP or other resources.</param>
        /// <returns>If true, the user could afford the skill's cost and was charged successfully.</returns>
        public bool CanUseSkill(TargetedSkill targetedSkill, bool payCost = true) => CanUseSkill(targetedSkill.skill, targetedSkill.user, payCost);
        //{
        //    if (TUFFSettings.DebugSkillsCostNoResources()) return true;
        //    if (targetedSkill.skill is Skill skl)
        //    {
        //        if (!targetedSkill.CanAffordSP() || !targetedSkill.CanAffordTP() || !targetedSkill.CanAffordItem() || !targetedSkill.UnitedUsersCanAct()
        //            || !targetedSkill.CanAffordUP())
        //            return false;
        //        if (payCost) targetedSkill.user.PaySkillCost(skl);
        //    }
        //    else if (targetedSkill.skill is Item itm)
        //    {
        //        if (Inventory.instance.items[itm.id] <= 0)
        //            return false;
        //        if (payCost) targetedSkill.user.PayItemCost(itm);
        //    }
        //    ForceUpdateHUD();
        //    return true;
        //}
        public bool CanUseSkill(IBattleInvocation invocation, Targetable user, bool payCost = true)
        {
            if (TUFFSettings.DebugSkillsCostNoResources()) return true;
            if (invocation is Skill skill)
            {
                if (user.HasSkillSeal(skill)) return false;
                if (!skill.CanAffordSP(user) || !skill.CanAffordTP(user) || !skill.CanAffordItem(user) || !skill.UnitedUsersCanAct()
                    || !skill.CanAffordUP())
                    return false;
                if (payCost) user.PaySkillCost(skill);
            }
            else if (invocation is Item item)
            {
                if (Inventory.instance.items[item.id] <= 0)
                    return false;
                if (payCost) user.PayItemCost(item);
            }
            ForceUpdateHUD();
            return true;
        }
        public bool HasUnitedSkillUsersInActiveParty(Skill skill)
        {
            if (skill.isUnitedSkill)
            {
                var userA = PlayerData.instance.GetPartyMember(skill.unitedUserA);
                var userB = PlayerData.instance.GetPartyMember(skill.unitedUserB);
                bool userAInActiveParty = PlayerData.instance.IsInActiveParty(userA.unitRef);
                bool userBInActiveParty = PlayerData.instance.IsInActiveParty(userB.unitRef);
                return userAInActiveParty && userBInActiveParty;
            }
            return false;
        }
        public List<Targetable> CheckTargetIsStillValid(TargetedSkill targetedSkill)
        {
            if (targetedSkill.user == null || targetedSkill.skill == null) return null;
            var validTargets = GetInvocationValidTargets(targetedSkill.user, targetedSkill.scopeData);
            if (validTargets.Count <= 0) return null;
            var scope = targetedSkill.scopeData.scopeType;
            if (IsGroupScope(scope))
                return validTargets;
            if (IsRandomScope(scope))
                return RollForTargets(validTargets);
            else
            {
                int singleTargetIdx = ContainsTarget(validTargets, targetedSkill.targets[0]);
                if (singleTargetIdx >= 0) return new List<Targetable>() { validTargets[singleTargetIdx] };
                return new List<Targetable>() { validTargets[Random.Range(0, validTargets.Count - 1)] }; // If the TargetedSkill's skill is no longer valid, pick one at random from the valid ones.
            }
        }
        public static int ContainsTarget(List<Targetable> validTargets, Targetable target)
        {
            if (target == null) return -1;
            for (int i = 0; i < validTargets.Count; i++)
            {
                if (validTargets[i] == target) return i;
            }
            return -1;
        }
        public static void PlayAnimationEventSFX(BattleAnimationEvent animEvent)
        {
            for (int i = 0; i < animEvent.SFXList.Count; i++)
            {
                if (animEvent.SFXList[i] == null) continue;
                AudioManager.instance?.PlaySFX(animEvent.SFXList[i]);
            }
        }
        public void HitTarget(BattleAnimationEvent animEvent)
        {
            if (animEvent == null) { Debug.Log("event is null"); return; }
            if (animEvent.skillOrigin == null) { Debug.Log("origin is null"); return; }
            if (animEvent.skillOrigin.user == null) { Debug.Log("user is null"); return; }
            if (animEvent.skillOrigin.targets == null) { Debug.Log("targets is null"); return; }
            StartCoroutine(HitTargets(animEvent));
        }
        protected IEnumerator HitTargets(BattleAnimationEvent animEvent)
        {
            float delay = animEvent.multihitTimingDelay;
            var targets = animEvent.skillOrigin.targets;
            var user = animEvent.skillOrigin.user;
            int first = (animEvent.multihitTiming != MultihitTimingType.LastToFirst ? 0 : targets.Count - 1);
            int order = (animEvent.multihitTiming != MultihitTimingType.LastToFirst ? +1 : -1);
            for (int i = first; (animEvent.multihitTiming != MultihitTimingType.LastToFirst ? i < targets.Count : i >= 0); i += order)
            {
                if (IsHit(animEvent, i)) //Calculate Hitrate, if true, call TakeHit
                {
                    if (animEvent.playSFXOnHit) PlayAnimationEventSFX(animEvent);
                    animEvent.skillOrigin.targets[i].TakeHit(animEvent, i);
                }
                else //else Display Miss
                {
                    if (animEvent.ignoreHitEffects) CheckHitEffects(animEvent, i); //If ignoreHit, check Effects as normal
                    if (animEvent.flashTarget && animEvent.ignoreHitFlashTarget)
                    {
                        TintTarget(animEvent.skillOrigin.targets[i], animEvent.flashTargetData.flashColor, animEvent.flashTargetData.flashDuration);
                    }
                    DisplayMiss(animEvent.skillOrigin.targets[i]);
                }
                if (animEvent.TPGain != 0) animEvent.skillOrigin.user.RecoverTP(animEvent.TPGain, true);
                if (animEvent.UPGain != 0 && user is PartyMember) PlayerData.instance.battleData.RecoverUP(animEvent.UPGain);
                if (animEvent.multihitTiming != MultihitTimingType.AllAtOnce) yield return new WaitForSeconds(delay);
            }
        }
        public void ApplyTargetablesRegens()
        {
            for (int i = 0; i < GameManager.instance.playerData.GetActivePartySize(); i++)
            {
                if (activeParty[i] == null) continue;
                activeParty[i].ApplyRegens();
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                enemies[i].ApplyRegens();
            }
        }
        public void UpdateTargetablesStates()
        {
            for (int i = 0; i < GameManager.instance.playerData.GetActivePartySize(); i++)
            {
                if (activeParty[i] == null) continue;
                activeParty[i].UpdateStates();
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                enemies[i].UpdateStates();
            }
        }
        public void CheckBattleEndStateRemovals()
        {
            for (int i = 0; i < GameManager.instance.playerData.GetActivePartySize(); i++)
            {
                if (activeParty[i] == null) continue;
                activeParty[i].RemoveStatesWithAtBattleEndCondition();
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                enemies[i].RemoveStatesWithAtBattleEndCondition();
            }
        }
        protected bool CheckGameOver()
        {
            for (int i = 0; i < GameManager.instance.playerData.GetActivePartySize(); i++)
            {
                if (!activeParty[i].isKOd) return false; // At least one Party Member is alive
            }
            battleState = BattleState.LOST;
            return true;
        }
        public bool CheckWin()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].isKOd) return false; // All enemies down
            }
            battleState = BattleState.WON;
            return true;
        }
        public bool CheckBattleEnd()
        {
            return (battleState == BattleState.WON || battleState == BattleState.LOST || battleState == BattleState.ESCAPED);
        }
        public void UnloadBattle()
        {
            m_inBattle = false;
            m_canEscape = false;
            if (battle != null) Destroy(battle.gameObject);
            turn = 0;
            rewards.Clear();
            magsCollected = 0;
            expCollected = 0;
            //GameManager.instance.DisablePlayerInput(false);
            ResetTargetablesActedCheck();
            ResetQueuedCommands();
            ResetNextTurnQueuedCommands();
            hud.UnloadHUD();
            hud.gameObject.SetActive(false);
        }
        public void EndBattle()
        {
            if (battleState == BattleState.WON)
            {
                StartCoroutine(OnBattleWon());
            }
            if (battleState == BattleState.LOST)
            {
                StartCoroutine(OnBattleLost());
            }
            if (battleState == BattleState.ESCAPED)
            {
                StartCoroutine(OnBattleEscape());
            }
        }
        private IEnumerator EndBattleFade()
        {
            CheckBattleEndStateRemovals();
            UIController.instance.UIFadeScreen.FadeOut(0.25f);
            yield return new WaitForSeconds(0.25f);
            if (eventCallback != null)
            { 
                if (eventCallback is StartBattleAction startBattleAction)
                {
                    startBattleAction.OnBattleEnd(battleState);
                }
                else eventCallback.isFinished = true; 
            }
            else if (!CommonEventManager.interactableEventPlaying) GameManager.instance.DisablePlayerInput(false);

            UnloadBattle();
            AudioManager.instance.RestoreAmbienceVolume();
            UIController.instance.UIFadeScreen.FadeIn(0.25f);
        }
        private IEnumerator OnBattleWon()
        {
            GetEnemyRewards();
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(hud.ShowResults()); //Show Rewards and EXP
            StartCoroutine(EndBattleFade());
        }
        private IEnumerator OnBattleLost()
        {
            if (eventCallback != null && eventCallback is StartBattleAction startBattleAction)
            {
                if (startBattleAction.continueOnLose)
                {
                    AudioManager.instance.StopMusic(1f);
                    yield return new WaitForSeconds(1f);
                    PlayerData.instance.RecoverAllFromKO();
                    StartCoroutine(EndBattleFade());
                    yield break;
                }
            }
            GameManager.instance.GameOver();
            while (GameManager.gameOver)
            {
                yield return null;
            }
            CheckBattleEndStateRemovals();
            UnloadBattle();
        }
        private IEnumerator OnBattleEscape()
        {
            AudioManager.instance.StopMusic(1f);
            AudioManager.instance.PlaySFX(TUFFSettings.escapeSFX);
            yield return new WaitForSeconds(1f);
            StartCoroutine(EndBattleFade());
        }
        public void AddEnemyRewards(Enemy enemy)
        {
            magsCollected += enemy.mags;
            expCollected += enemy.EXP;

            var drops = enemy.drops;
            for (int j = 0; j < drops.Count; j++)
            {
                var chance = drops[j].dropChance;
                bool trigger = RollChance(chance);
                if (!trigger) continue;
                AddReward(drops[j].GetItem());
            }
        }
        public void AddReward(InventoryItem item)
        {
            if (rewards.ContainsKey(item)) rewards[item] += 1;
            else rewards.Add(item, 1);
        }
        private void GetEnemyRewards()
        {
            foreach (KeyValuePair<InventoryItem, int> i in rewards)
            {
                PlayerData.instance.AddToInventory(i.Key, i.Value);
            }
            PlayerData.instance.AddMags(magsCollected);
            PlayerData.instance.AddEXPToParty(expCollected);
        }
        private void PlayWinAudio()
        {
            AudioManager.instance.StopMusic(1f); 
            AudioManager.instance.PlaySFX(TUFFSettings.battleVictorySFX);
        }
        public void DisplayHit(BattleAnimationEvent hitInfo, int value, int targetIndex, bool isCrit = false)
        {
            hud.DisplayHit(hitInfo, value, targetIndex, isCrit);
            switch (hitInfo.damageType)
            {
                case DamageType.HPDamage:
                    if (isCrit)
                    {
                        AudioManager.instance.PlaySFX(TUFFSettings.critDamageSFX);
                        break;
                    }
                    if (hitInfo.skillOrigin.targets[targetIndex] is EnemyInstance)
                    {
                        AudioManager.instance.PlaySFX(TUFFSettings.enemyDamageSFX);
                    }
                    else if (hitInfo.skillOrigin.targets[targetIndex] is PartyMember)
                        AudioManager.instance.PlaySFX(TUFFSettings.unitDamageSFX);
                    break;
                case DamageType.HPRecover:
                    AudioManager.instance.PlaySFX(TUFFSettings.recoverySFX);
                    break;
                case DamageType.SPRecover:
                    AudioManager.instance.PlaySFX(TUFFSettings.recoverySFX);
                    break;
                case DamageType.TPRecover:
                    AudioManager.instance.PlaySFX(TUFFSettings.recoverySFX);
                    break;
                default:
                    break;
            }
            ForceUpdateHUD();
        }
        public void DisplayRegen(Targetable target, int value, DamageType damageType)
        {
            hud.DisplayRegen(target, value, damageType);
            ForceUpdateHUD();
        }
        public void DisplayMiss(Targetable target)
        {
            hud.AddMissToHitDisplayGroup(target);
            AudioManager.instance.PlaySFX(TUFFSettings.missSFX);
            ForceUpdateHUD();
        }
        public void DisplayState(Targetable target, State state, bool removal = false)
        {
            hud?.DisplayState(target, state, removal);
            ForceUpdateHUD();
        }
        public void DisplayVulnerability(Targetable target, int elementIndex, VulnerabilityType vulType)
        {
            hud?.DisplayVulnerability(target, elementIndex, vulType);
        }
        public void ForceUpdateHUD(bool resetPartyCommandIcons = false)
        {
            hud.UpdateUnitsHUD(activeParty, resetPartyCommandIcons);
            hud.UpdateEnemiesHUD();
            hud.UpdateUPBar();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chance"></param>
        /// <param name="randomMinInclusive"></param>
        /// <param name="randomMaxExclusive"></param>
        /// <returns>Returns true if chance was triggered.</returns>
        public static bool RollChance(int chance, int randomMinInclusive = 0, int randomMaxExclusive = 100)
        {
            // If random number (between min and max - 1) is lower than chance, return true
            bool triggeredChance = (Random.Range(randomMinInclusive, randomMaxExclusive) < chance);
            return triggeredChance;
        }
        public static bool IsHit(BattleAnimationEvent hitInfo, int targetIndex)
        {
            if (hitInfo.certainHit) return true; //if certainHit, always hit

            float userHitChance = hitInfo.skillOrigin.user.GetHitRate(); //get user's hit rate
            userHitChance *= (hitInfo.successPercent * 0.01f); //Multiply user's hit rate by successPercent
            float targetEvadeChance = hitInfo.skillOrigin.targets[targetIndex].GetEvasionRate(); //get target's evasion
            int hitChance = LISAUtility.Truncate(Mathf.Clamp(userHitChance - targetEvadeChance, 0f, 1f) * 100); //Limit hit between 0 and 100%.
            if (hitChance <= 0) return false; //if hitChance is 0, cant hit.
            //int random = Random.Range(0, 100); 
            //bool trigger = random < hitChance; 
            if (RollChance(hitChance)) return true; //if random number(between 0 and 99) is lower than hitChance, it's a hit
            return false;
        }
        public static bool IsCriticalHit(BattleAnimationEvent hitInfo, int targetIndex, bool ignoreCritEvade = false)
        {
            if (!hitInfo.canCrit) return false;
            float userCritChance = hitInfo.skillOrigin.user.GetCritRate();
            float targetCritEvade = 0;
            if (!ignoreCritEvade) targetCritEvade = hitInfo.skillOrigin.targets[targetIndex].GetCritEvasionRate();
            int critChance = LISAUtility.Truncate(Mathf.Clamp(userCritChance - targetCritEvade, 0f, 1f) * 100);
            if (critChance <= 0) return false;
            //int random = Random.Range(0, 100);
            //bool trigger = random < critChance;
            if (RollChance(critChance)) return true;
            return false;
        }
        public static void CheckHitEffects(BattleAnimationEvent hitInfo, int targetIndex)
        {
            for (int i = 0; i < hitInfo.effects.Count; i++)
            {
                if (hitInfo.effects[i] == null) continue;
                var effect = hitInfo.effects[i];
                var user = hitInfo.skillOrigin.user;
                var target = hitInfo.skillOrigin.targets[targetIndex];
                switch (effect.effectType)
                {
                    case EffectType.RecoverHP:
                        int maxValue = LISAUtility.Truncate(target.GetMaxHP() * (effect.recoverPercent * 0.01f));
                        int flatValue = effect.recoverFlat;
                        int value = maxValue + flatValue;
                        target.ApplyHealingModifiers(hitInfo, ref value);
                        target.RecoverHP(value, false, true);
                        break;

                    case EffectType.RecoverSP:
                        maxValue = LISAUtility.Truncate(target.GetMaxSP() * (effect.recoverPercent * 0.01f));
                        flatValue = effect.recoverFlat;
                        target.RecoverSP(maxValue + flatValue, false, true);
                        break;

                    case EffectType.RecoverTP:
                        maxValue = LISAUtility.Truncate(target.GetMaxTP() * (effect.recoverPercent * 0.01f));
                        flatValue = effect.recoverFlat;
                        target.RecoverTP(maxValue + flatValue, false, true);
                        break;

                    case EffectType.AddState:
                        if (!RollForStateApply(effect.state, user, target, effect.stateTriggerChance)) continue;
                        break;

                    case EffectType.RemoveState:
                        if (effect.state == null) continue;
                        if (RollChance(effect.stateTriggerChance)) target.RemoveState(effect.state);
                        break;

                    case EffectType.SpecialEffect:
                        if (effect.specialEffect == SpecialEffectType.RemoveKO)
                        {
                            target.RemoveKO();
                        }
                        break;
                    case EffectType.LearnSkill:
                        if (user is PartyMember member) member.LearnSkill(effect.skill, true);
                        break;
                    case EffectType.ForgetSkill:
                        if (user is PartyMember member1) member1.LearnSkill(effect.skill, false);
                        break;
                    case EffectType.CommonEvent:
                        if (effect.commonEvent != null)
                        {
                            CommonEventManager.instance.QueueCommonEvent(effect.commonEvent);
                            CommonEventManager.instance.RunEvents();
                        }
                        break;
                    case EffectType.QueueSkill:
                        if (effect.skillToQueue != null)
                        {
                            BattleManager.instance.QueueCommand(effect.skillToQueue, hitInfo.skillOrigin.targets, user, BattleManager.instance.queuedSkillsIndex + 1, hitInfo.skillOrigin.commandIconCallback);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Rolls for a state to be applied to a target considering vulnerabilities, potencies and immunities.
        /// </summary>
        /// <param name="state">State to apply</param>
        /// <param name="user">Targetable casting the state. If null, potency of the user will not be considered</param>
        /// <param name="target">Targetable to apply the state to.</param>
        /// <param name="baseTriggerChance">Base percentage chance of state triggering before vulnerabilities and potencies</param>
        /// <returns>Returns true if state was applied.</returns>
        public static bool RollForStateApply(State state, Targetable user, Targetable target, int baseTriggerChance)
        {
            if (state == null) return false;
            if (target == null) return false;
            if (target.IsImmuneToState(state)) return false;
            float totalMultiplier = GetTotalStateVulnerability(state, user, target);
            int chance = LISAUtility.Truncate(baseTriggerChance * totalMultiplier);
            Debug.Log("Chance: " + chance);
            if (RollChance(chance)) target.ApplyState(state);
            return true;
        }
        public static float GetTotalElementVulnerability(int elementIndex, Targetable user, Targetable target)
        {
            if (elementIndex < 0) return 1f;
            float potMult = (user != null ? user.GetElementPotency(elementIndex) : 1f);
            float vulMult = target.GetElementVulnerability(elementIndex);
            float totalMult = vulMult * potMult;
            Debug.Log("elementIndex: " + elementIndex + " , Total: " + totalMult);
            return totalMult;
        }
        public static float GetTotalStateVulnerability(State state, Targetable user, Targetable target)
        {
            float potMult = (user != null ? user.GetStatePotency(state) : 1f);
            float vulMult = target.GetStateVulnerability(state);
            return vulMult * potMult;
        }
        public static float GetTotalHitTypeVulnerability(HitType hitType, Targetable user, Targetable target)
        {
            float potMult = hitType == HitType.PhysicalAttack ? user.GetPhysicalDamagePotency() : user.GetSpecialDamagePotency();
            float vulMult = hitType == HitType.PhysicalAttack ? target.GetPhysicalDamageVulnerability() : target.GetSpecialDamageVulnerability();
            return vulMult * potMult;
        } 
        public static float GetTotalHealingPotency(Targetable user, Targetable target)
        {
            float dealtMult = user.GetHealingDealtPotency();
            float receivedMult = target.GetHealingReceivedPotency();
            return dealtMult * receivedMult;
        }
        public static float GetTotalItemPotency(Targetable user, Targetable target)
        {
            float dealtMult = user.GetItemDealtPotency();
            float receivedMult = target.GetItemReceivedPotency();
            return dealtMult * receivedMult;
        }
        public static void TintTarget(Targetable target, Color color, float duration)
        {
            if (target == null) return;
            target.imageReference?.Flash(color, duration);
        }
        public static void TintScreen(Color color, float duration)
        {
            instance.hud.TintScreen(color, duration);
        }
        public void CheckActionEndStatesDuration(Targetable user)
        {
            for (int i = 0; i < user.states.Count; i++)
            {
                if (user.states[i].state.autoRemovalTiming == AutoRemovalTiming.ActionEnd)
                {
                    if (user.states[i].ReduceStateTurnDuration(1)) i--;
                }
            }
        }
        public void CheckTargetablesTurnEndStatesDuration()
        {
            for (int i = 0; i < GameManager.instance.playerData.GetActivePartySize(); i++)
            {
                if (activeParty[i] == null) continue;
                var targetable = activeParty[i];
                for (int j = 0; j < targetable.states.Count; j++)
                {
                    if (targetable.states[j].state.autoRemovalTiming == AutoRemovalTiming.TurnEnd)
                    {
                        if (targetable.states[j].ReduceStateTurnDuration(1)) j--;
                    }
                    else if (targetable.states[j].state.autoRemovalTiming == AutoRemovalTiming.ActionEnd && !targetable.acted)
                    {
                        if (targetable.states[j].ReduceStateTurnDuration(1)) j--;
                    }
                }
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                var targetable = enemies[i];
                for (int j = 0; j < targetable.states.Count; j++)
                {
                    if (targetable.states[j].state.autoRemovalTiming == AutoRemovalTiming.TurnEnd)
                    {
                        if (targetable.states[j].ReduceStateTurnDuration(1)) j--;
                    }
                    else if (targetable.states[j].state.autoRemovalTiming == AutoRemovalTiming.ActionEnd && !targetable.acted)
                    {
                        if (targetable.states[j].ReduceStateTurnDuration(1)) j--;
                    }
                }
            }
        }
        private void ResetTargetablesActedCheck()
        {
            for (int i = 0; i < GameManager.instance.playerData.GetActivePartySize(); i++)
            {
                if (activeParty[i] == null) continue;
                activeParty[i].acted = false;
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                enemies[i].acted = false;
            }
        }
        public static float GetStatChange(List<Feature> features, StatChangeType changeType, float initialValue = 1)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.StatChange) continue;
                if (features[i].statChange != changeType) continue;
                initialValue *= features[i].statChangeValue * 0.01f;
            }
            return Mathf.Clamp(initialValue, 0, 10);
        }
        public static float GetExtraRateChange(List<Feature> features, ExtraRateChangeType changeType, float initialValue = 0)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.ExtraRateChange) continue;
                if (features[i].extraRateChange != changeType) continue;
                initialValue += features[i].extraRateChangeValue * 0.01f;
            }
            return initialValue;
        }
        public static float GetSpecialRateChange(List<Feature> features, SpecialRateChangeType changeType, float initialValue = 1)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.SpecialRateChange) continue;
                if (features[i].specialRateChange != changeType) continue;
                initialValue *= features[i].specialRateChangeValue * 0.01f;
            }
            return Mathf.Clamp(initialValue, 0, 10);
        }
        public static float GetElementVulnerability(List<Feature> features, int elementIndex, float initialValue = 1)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.ElementVulnerability) continue;
                if (features[i].element != elementIndex) continue;
                initialValue *= features[i].elementValue * 0.01f;
            }
            return Mathf.Clamp(initialValue, 0, 10);
        }
        public static float GetElementPotency(List<Feature> features, int elementIndex, float initialValue = 1)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.ElementPotency) continue;
                if (features[i].element != elementIndex) continue;
                initialValue *= features[i].elementValue * 0.01f;
            }
            return Mathf.Clamp(initialValue, 0, 10);
        }
        public static float GetStateVulnerability(List<Feature> features, State state, float initialValue = 1)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.StateVulnerability) continue;
                if (features[i].state != state) continue;
                initialValue *= features[i].stateValue * 0.01f;
            }
            return Mathf.Clamp(initialValue, 0f, 100f);
        }
        public static float GetStatePotency(List<Feature> features, State state, float initialValue = 1)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.StatePotency) continue;
                if (features[i].state != state) continue;
                initialValue *= features[i].stateValue * 0.01f;
            }
            return Mathf.Clamp(initialValue, 0f, 100f);
        }
        public static VulnerabilityType GetVulnerabilityType(float multiplier)
        {
            if (multiplier <= 0) return VulnerabilityType.Immune;
            else if (multiplier < 1) return VulnerabilityType.Resist;
            else if (multiplier > 1) return VulnerabilityType.Weakpoint;
            return VulnerabilityType.Normal;
        }
        public static VulnerabilityType GetVulnerabilityTypeFromSkill(IBattleInvocation invocation, Targetable user, Targetable target, ref int elementIndexFound) // Remove out
        {
            var evts = invocation.GetAllEvents();
            for (int i = 0; i < evts.Count; i++) 
            {
                if (evts[i].ElementIndex >= 0) {
                    Debug.Log("Found it!: " + evts[i].ElementIndex);
                    elementIndexFound = evts[i].ElementIndex;
                    Debug.Log("User: " + user.GetName() + ", Target: " + target.GetName());
                    float mult = GetTotalElementVulnerability(evts[i].ElementIndex, user, target);
                    var vulType = BattleManager.GetVulnerabilityType(mult);
                    if (vulType != VulnerabilityType.Normal)
                        return vulType;
                }
            }
            return VulnerabilityType.Normal;
        }
        public static int GetAttackSpeed(List<Feature> features, int initialValue = 0)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.AttackSpeed) continue;
                initialValue += features[i].attackSpeed;
            }
            return initialValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The index of the special feature.</returns>
        public static int GetSpecialFeatureIndex(List<Feature> features, SpecialFeatureType featureType)
        {
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].featureType != FeatureType.SpecialFeature) continue;
                if (features[i].specialFeature == featureType) return i;
            }
            return -1;
        }
        public static bool IsSingleScope(ScopeType scope)
        {
            return scope == ScopeType.OneEnemy || scope == ScopeType.OneAlly || scope == ScopeType.Anyone;
        }
        public static bool IsGroupScope(ScopeType scope)
        {
            return scope == ScopeType.AllEnemies || scope == ScopeType.AllAllies || scope == ScopeType.Everyone;
        }
        public static bool IsRandomScope(ScopeType scope)
        {
            return scope == ScopeType.RandomEnemies || scope == ScopeType.RandomAllies || scope == ScopeType.RandomAnyone;
        }
        public static bool IsEnemyScope(ScopeType scope)
        {
            return scope == ScopeType.OneEnemy || scope == ScopeType.AllEnemies || scope == ScopeType.RandomEnemies;
        }
        public static bool IsAllyScope(ScopeType scope)
        {
            return scope == ScopeType.OneAlly || scope == ScopeType.AllAllies || scope == ScopeType.RandomAllies;
        }
        public static bool IsAnyoneScope(ScopeType scope)
        {
            return scope == ScopeType.Anyone || scope == ScopeType.Everyone || scope == ScopeType.RandomAnyone;
        }
    }
}
