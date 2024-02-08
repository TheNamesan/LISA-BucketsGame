using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    public enum ComboDialInput
    {
        W = 0,
        A = 1,
        S = 2,
        D = 3
    }
    public class ComboDial : MonoBehaviour
    {
        public GameObject comboDialHUDPrefab;

        [Header("References")]
        public GameObject neutralPose;

        [Header("Dial Skills")]
        public Skill QSkill;
        public Skill WSkill;
        public Skill ASkill;
        public Skill SSkill;
        public Skill DSkill;

        [Header("Combo Dial Skills")]
        public List<Command> comboDialSkills = new List<Command>();
        [Tooltip("Max number of inputs.")]
        public int maxInputs = 5;

        private BattleAnimation skillAnim;
        [HideInInspector] public ComboDialHUD comboDialHUD; 
        [SerializeField] protected string comboInput;
        public int queuedSkillsIndex = 0;
        [SerializeField] protected List<SkillsLearned> validMoves = new List<SkillsLearned>();
        public List<TargetedSkill> queuedSkills = new List<TargetedSkill>();
        protected TargetedSkill comboMoveSkill = null;

        [Header("Sounds")]
        public AudioClip inputSFX;
        public AudioClip successSFX;

        public virtual void Awake()
        {
            skillAnim = GetComponent<BattleAnimation>();
            if (WSkill == null && ASkill == null && SSkill == null && DSkill == null)
            {
                Debug.LogWarning("No Dial Skills assigned.");
                EndSkill();
                return;
            }
            if (maxInputs <= 0) maxInputs = 1;
            queuedSkillsIndex = 0;
            comboInput = "";
            queuedSkills = new List<TargetedSkill>();
            comboMoveSkill = null;
            if (neutralPose != null) neutralPose.SetActive(true);
            var comboGO = Instantiate(comboDialHUDPrefab, BattleManager.instance.hud.overlayInfo);
            comboDialHUD = comboGO.GetComponent<ComboDialHUD>();
            comboDialHUD.ResetInputsInfo();
            comboDialHUD.ResetDialInput();
            comboDialHUD.InitializeComboDialHUD(this);
        }
        public void Start()
        {
            GetValidMoves();
            StartCoroutine(RunQueuedSkills());
        }
        public virtual IEnumerator RunQueuedSkills()
        {
            while (queuedSkillsIndex < maxInputs)
            {
                while (queuedSkills.Count - 1 < queuedSkillsIndex)
                {   
                    yield return null;
                    if (BattleManager.instance.CheckWin()) EndSkill();
                } 
                //Debug.Log(queuedSkillsIndex + " " + queuedSkills.Count);
                if (neutralPose != null) neutralPose.SetActive(false);
                yield return StartCoroutine(queuedSkills[queuedSkillsIndex].InvokeSkill());
                queuedSkillsIndex++;
                if(comboMoveSkill != null && queuedSkillsIndex == queuedSkills.Count - 1) break;
            }
            if (neutralPose != null) neutralPose.SetActive(false);
            if (comboMoveSkill != null)
            {
                yield return StartCoroutine(queuedSkills[queuedSkillsIndex].InvokeSkill()); 
            }
            EndSkill();
        }
        public void Update()
        {
            CheckDialInput();
        }
        public virtual void CheckDialInput()
        {
            if (UIController.instance.QDown && QSkill != null) { AddToComboInput("Q", QSkill); }
            else if (UIController.instance.WDown && WSkill != null) { AddToComboInput("W", WSkill); }
            else if (UIController.instance.ADown && ASkill != null) { AddToComboInput("A", ASkill); }
            else if (UIController.instance.SDown && SSkill != null) { AddToComboInput("S", SSkill); }
            else if (UIController.instance.DDown && DSkill != null) { AddToComboInput("D", DSkill); }
        }
        public virtual void AddToComboInput(string input, Skill skill)
        {
            if (comboMoveSkill != null) return;
            if (comboInput.Length >= maxInputs) return;
            var targetedSkill = QueueAsTargetedSkill(skill);
            comboInput += input;
            comboDialHUD.AddInputIcon(targetedSkill.skill.icon);
            CheckComboMoves();
        }

        private TargetedSkill QueueAsTargetedSkill(Skill skill)
        {
            var targetedSkill = new TargetedSkill(skill, skillAnim.callRef.targets, skillAnim.callRef.user, onFinished: ReactivateNeutralPose);
            targetedSkill.targets = BattleManager.instance.CheckTargetIsStillValid(targetedSkill);
            QueueCommand(targetedSkill, queuedSkills.Count);
            return targetedSkill;
        }

        public virtual void GetValidMoves() 
        {
            if (skillAnim.callRef == null) return;
            var user = skillAnim.callRef.user;
            var job = user.GetJob();
            if (job == null || comboDialSkills.Count <= 0) return;
            for (int i = 0; i < comboDialSkills.Count; i++)
            {
                if (comboDialSkills[i] == null) continue;
                if (user.GetCommands().IndexOf(comboDialSkills[i]) < 0) continue; // if Command is not in Job, continue
                if (comboDialSkills[i].commandType == CommandType.Single)
                {
                    if (ValidateMove(comboDialSkills[i].skills[0]))
                    {
                        Debug.Log($"MOVE: {comboDialSkills[i].skills[0].skill.GetName()}. COMBO: {comboDialSkills[i].skills[0].skill.comboDialMove}");
                        validMoves.Add(comboDialSkills[i].skills[0]);
                        break;
                    }
                    continue;
                }
                else if (comboDialSkills[i].commandType == CommandType.Group)
                {
                    var group = comboDialSkills[i].skills;
                    for (int j = 0; j < group.Count; j++)
                    {
                        if (ValidateMove(group[j]))
                        {
                            Debug.Log($"MOVE: {group[j].skill.GetName()}. COMBO: {group[j].skill.comboDialMove}");
                            validMoves.Add(group[j]);
                        }
                    }
                }
            }
        }
        public virtual void CheckComboMoves()
        {
            for(int i = 0; i < validMoves.Count; i++)
            {
                if(validMoves[i].skill.comboDialMove == comboInput)
                {
                    comboMoveSkill = QueueAsTargetedSkill(validMoves[i].skill);
                    comboDialHUD.AddSuccessInput(comboMoveSkill.skill as Skill);
                    PlaySFX(successSFX);
                    return;
                }
            }
            //if (!skillAnim.callRef.CanAffordSP() || !skillAnim.callRef.CanAffordTP()) return false;
            PlaySFX(inputSFX);
        }
        protected void ReactivateNeutralPose()
        {
            if (neutralPose == null) return;
            if (queuedSkillsIndex >= maxInputs - 1) return;
            if (comboMoveSkill != null && queuedSkillsIndex >= queuedSkills.Count - 1) return;
            BattleManager.SetAnimationPosition(skillAnim.GetComponent<RectTransform>(), queuedSkills[queuedSkillsIndex]);
            //if (queuedSkills[queuedSkillsIndex].targets[0] != null)
                //skillAnim.transform.position = queuedSkills[queuedSkillsIndex].targets[0].imageReference.transform.position;
            neutralPose.SetActive(true);
        }
        protected virtual void QueueCommand(TargetedSkill targetedSkill, int index)
        {
            index = Mathf.Clamp(index, 0, queuedSkills.Count);
            queuedSkills.Insert(index, targetedSkill);
        }
        public virtual bool ValidateMove(SkillsLearned skillLearned)
        {
            if (skillLearned == null) return false;
            if (skillLearned.skill == null) return false;
            if (skillLearned.skill.comboDialMove == "") return false;
            if (!skillLearned.CanBeUsed(skillAnim.callRef.user)) return false;
            
            return true;
        }
        public void EndSkill()
        {
            if(comboDialHUD != null) Destroy(comboDialHUD.gameObject);
            skillAnim.EndAnimation();
        }
        protected void PlaySFX(AudioClip clip)
        {
            AudioManager.instance.PlaySFX(clip, 1f, 1f);
        }
    }
}

