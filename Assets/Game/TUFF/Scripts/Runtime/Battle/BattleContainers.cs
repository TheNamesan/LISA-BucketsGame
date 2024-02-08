using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace TUFF
{
    [System.Serializable]
    public class EnemyReference
    {
        [Tooltip("Reference to the Enemy asset from the Database.")]
        public Enemy enemy;
        [Tooltip("Reference to the GraphicEffectsMask component on the stage.")]
        public GraphicHandler imageReference;
    }
    [System.Serializable]
    public class TargetedSkill
    {
        public IBattleInvocation skill;
        public ScopeData scopeData;
        public List<Targetable> targets = new List<Targetable>();
        public Targetable user;
        public UnitHUD commandIconCallback;
        public int attackSpeed = 0;
        public int repeats = 1;
        public bool markIgnore = false;
        public bool runningAnimation = false;

        public UnityEvent onFinished = new UnityEvent();

        public bool isFinished
        { 
            get { return m_isFinished; }
        }
        private bool m_isFinished = false;
        public BattleAnimation animationInstance = null;
        private int m_repeatCount = 0;
        public Random.State rngState;
        
        public TargetedSkill(Random.State rngState, IBattleInvocation skill, List<Targetable> targets, Targetable user, UnitHUD commandIconCallback = null, UnityAction onFinished = null)
        {
            this.rngState = rngState;
            this.skill = skill;
            this.targets = targets;
            this.user = user;
            this.commandIconCallback = commandIconCallback;
            scopeData = skill.ScopeData;
            attackSpeed = (user != null ? user.GetAttackSpeed() : 0) + (skill != null ? skill.speed : 0);
            repeats = (skill != null ? skill.repeats : 1); // Add user attack times here
            if (onFinished != null) this.onFinished.AddListener(onFinished);
        }
        public TargetedSkill(IBattleInvocation skill, List<Targetable> targets, Targetable user, UnitHUD commandIconCallback = null, UnityAction onFinished = null) : this(Random.state, skill, targets, user, commandIconCallback, onFinished) { }
        public IEnumerator InvokeSkill()
        {
            m_isFinished = false;
            if (!markIgnore) targets = BattleManager.instance.CheckTargetIsStillValid(this);
            if (markIgnore) EndSkill();
            else if (!user.CanAct())
            {
                BattleManager.instance.DequeueSkillsFromUser(user);
                EndSkill();
            }
            else if (skill == null) EndSkill();
            else if (user == null) { Debug.LogWarning("Skill has no user."); EndSkill(); }
            else if (targets == null) { Debug.LogWarning("Skill has no target list."); EndSkill(); }
            else if (targets.Count <= 0) { Debug.LogWarning("Skill has no targets."); EndSkill(); }
            else if (skill.animation == null && skill.startEvents.Count <= 0 && skill.endEvents.Count <= 0) EndSkill();
            else if (!BattleManager.instance.CanUseSkill(this)) EndSkill();
            else if (skill != null)
            {
                Random.State prevState = Random.state;
                Random.state = rngState;
                for (m_repeatCount = 0; m_repeatCount < repeats; m_repeatCount++)
                {
                    if (!markIgnore) targets = BattleManager.instance.CheckTargetIsStillValid(this);
                    if (targets == null) break;
                    if (skill is Skill skil)
                    {
                        BattleManager.instance.hud.ShowDescriptionDisplay(true);
                        BattleManager.instance.hud.descriptionDisplay.DisplayText(skil.GetUseMessage(user.GetName()));
                    }
                    RunStartEvents();
                    if (skill.animation != null)
                    {
                        animationInstance = BattleManager.instance.PlayBattleAnimation(this);
                        while (runningAnimation) yield return null;
                        if (animationInstance != null) Object.Destroy(animationInstance.gameObject);
                    }
                    RunEndEvents();
                    while (CommonEventManager.instance.isRunning) yield return null;
                }
                if (user != null) user.acted = true;
                EndSkill();
                Random.state = prevState;
            } 
            while (!m_isFinished)
            {
                yield return null;
            }
            yield return new WaitForSeconds(skill.endDelay);
        }
        public void AnimationEnded()
        {
            runningAnimation = false;
        }
        public void EndSkill()
        {
            if (commandIconCallback != null)
                commandIconCallback.UpdateCommandIcon(null);

            if (targets != null)
                foreach (Targetable target in targets) target?.UpdatePrevHP();

            onFinished?.Invoke();
            m_isFinished = true;
        }
        public void CheckTargetIsRandom()
        {
            if (BattleManager.IsRandomScope(scopeData.scopeType))
            {
                int noOfTargets = scopeData.randomNumberOfTargets;
                targets = BattleManager.RollForTargets(BattleManager.instance.GetInvocationValidTargets(user, scopeData), noOfTargets);
            }
        }
        public void OverrideScopeData(ScopeData newScopeData, List<Targetable> newTargetList = null)
        {
            if (user == null) return;
            scopeData = newScopeData;
            if (newTargetList == null)
            {
                var validTargets = BattleManager.instance.GetInvocationValidTargets(user, scopeData);
                newTargetList = BattleManager.GetDefaultTargets(validTargets, scopeData);
            }
            targets = newTargetList;
        }
        public void RunStartEvents()
        {
            if (skill != null && user != null && targets != null && !markIgnore)
            {
                if (targets.Count > 0)
                    for (int i = 0; i < skill.startEvents.Count; i++)
                    {
                        skill.startEvents[i].InitiateAnimationEvent(skill.animation, this);
                        BattleAnimation.RunEvent(this, skill.startEvents[i]);
                    }
            }
        }
        public void RunEndEvents()
        {
            if (skill != null && user != null && targets != null && !markIgnore)
            {
                if (targets.Count > 0)
                {
                    for (int i = 0; i < skill.endEvents.Count; i++)
                    {
                        skill.endEvents[i].InitiateAnimationEvent(skill.animation, this);
                        BattleAnimation.RunEvent(this, skill.endEvents[i]);
                    }
                }
            }
        }
        

        public virtual bool CanAffordSP()
        {
            if (skill is Skill skl) return skl.CanAffordSP(user);//user.SP >= ((Skill)skill).SPCost;
            return true;
        }
        public virtual bool CanAffordTP()
        {
            if (skill is Skill skl) skl.CanAffordTP(user);//return user.TP >= ((Skill)skill).TPCost;
            return true;
        }
        public virtual bool CanAffordItem( )
        {
            if (skill is Skill skl) return skl.CanAffordItem(user);
            return true;
        }
        public virtual bool CanAffordUP()
        {
            if (skill is Skill skl) return skl.CanAffordUP();
            return true;
        }
        public virtual bool UnitedUsersCanAct()
        {
            if (skill is Skill skl) return skl.UnitedUsersCanAct();
            return true;
        }
    }
}

