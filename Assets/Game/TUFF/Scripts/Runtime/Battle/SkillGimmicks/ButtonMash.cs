using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TUFF
{
    public class ButtonMash : MonoBehaviour
    {
        public int timesPressed = 0;
        public bool startMashingOnStart = false;
        [Tooltip("The prompt animation to display when mashing starts.")]
        public BattleAnimation mashingAnim = null;
        [Tooltip("The skill to trigger if no threshold is reached.")]
        public Skill defaultSkill;
        public List<MashThreshold> thresholds = new List<MashThreshold>();
        
        protected TargetedSkill thresholdSkill;
        protected BattleAnimation battleAnim;
        protected bool mashing = false;

        private void Awake()
        {
            battleAnim = GetComponent<BattleAnimation>();
            timesPressed = 0;
        }
        
        void Start()
        {
            thresholdSkill = GetSkillAsTargetedSkill(defaultSkill, battleAnim.callRef);
            if (startMashingOnStart) StartMashing();
        }

        void Update()
        {
            CheckInput();
        }
        public void StartMashing()
        {
            mashing = true;
            var position = (battleAnim != null ? battleAnim.transform.position : Vector3.zero);
            BattleManager.instance.PlayAnimation(mashingAnim, position);
        }
        public void StopMashing()
        {
            mashing = false;
            StartCoroutine(RunThresholdSkill());
        }
        protected virtual IEnumerator RunThresholdSkill()
        {
            if (thresholdSkill == null) { EndSkill(); yield break; }
            if (BattleManager.instance.CheckWin()) { EndSkill(); yield break; }
            yield return StartCoroutine(thresholdSkill.InvokeSkill());
            EndSkill();
        }
        public void EndSkill()
        {
            battleAnim.EndAnimation();
        }
        protected virtual TargetedSkill GetSkillAsTargetedSkill(Skill skill, TargetedSkill originTargetedSkill)
        {
            if (skill == null) return null;
            var targetedSkill = new TargetedSkill(skill, originTargetedSkill.targets, originTargetedSkill.user);
            targetedSkill.targets = BattleManager.instance.CheckTargetIsStillValid(targetedSkill);
            return targetedSkill;
        }
        public virtual void AssignThresholdSkill(Skill skill)
        {
            thresholdSkill = GetSkillAsTargetedSkill(skill, battleAnim.callRef);
        }
        private void CheckInput()
        {
            if (!mashing) return;
            if (UIController.instance.actionButtonDown) ButtonPress();
        }
        private void ButtonPress()
        {
            timesPressed++;
            foreach(MashThreshold threshold in thresholds)
            {
                threshold.CheckThresholdReached(timesPressed, this, battleAnim.transform.position);
            }
        }
    }

    [System.Serializable]
    public class MashThreshold
    {
        public int requiredPresses = 1;
        public SFX reachedSFX = new SFX();
        [Tooltip("The animation to play when the threshold is reached.")]
        public BattleAnimation reachedAnimation;
        public Vector2 animationPositionOffset = Vector2.zero;
        public Vector2 animationScale = Vector2.one;
        [Tooltip("If true, will replace the Skill to trigger when the button mashing stops.")]
        public bool overrideSkill = false;
        [Tooltip("Skill to replace with.")]
        public Skill skill;
        public UnityEvent onThresholdReached = new UnityEvent();
        private bool reached = false;
        
        public void CheckThresholdReached(float timesPressed, ButtonMash buttonMash, Vector3 animationPosition = new Vector3())
        {
            if (reached) return;
            if (timesPressed < requiredPresses) return;
            reached = true;
            AudioManager.instance.PlaySFX(reachedSFX);
            var anim = BattleManager.instance.PlayAnimation(reachedAnimation, animationPosition);
            if (anim != null)
            {
                var rect = anim.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition += animationPositionOffset;
                    anim.transform.localScale = new Vector3(animationScale.x, animationScale.y, anim.transform.localScale.z);
                }
            }
            if (overrideSkill)
            {
                buttonMash.AssignThresholdSkill(skill);
            }
            onThresholdReached?.Invoke();
            Debug.Log("Reached");
        }
    }
}
