using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class BattleAnimation : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Animator controlling the animation. Used for queing a pause when a Crit hits.")]
        public Animator anim;

        [Header("Pivot")]
        public AnimationPivotType targetPartyPivot = AnimationPivotType.Center;
        public AnimationPivotType targetEnemyPivot = AnimationPivotType.Center;

        [Header("Events")]
        [Tooltip("If true, automatically runs all events and ends the animation on Initialize. Enable if there's no animator or animations to play for this skill.")]
        public bool autoRunEvents = false;
        public List<BattleAnimationEvent> events = new List<BattleAnimationEvent>();
        [System.NonSerialized] public TargetedSkill callRef;

        public bool isFinished { get { return m_isFinished; } }
        protected bool m_isFinished = false;

        protected bool queuePause = false;
        protected float animOrgSpeed = 0f;
        protected IEnumerator pauseCoroutine;

        public void RunEvent(int index) //Called from Animation
        {
            if (index >= events.Count || index < 0) return;
            if (events[index] == null) return;
            RunEvent(callRef, events[index]);
        }
        public static void RunEvent(TargetedSkill targetedSkill, BattleAnimationEvent animEvent)
        {
            if (animEvent == null) return;
            List<Targetable> orgTargets = null;
            ScopeData orgScopeData = null;
            if (targetedSkill != null)
            {
                orgTargets = new List<Targetable>(targetedSkill.targets);
                orgScopeData = targetedSkill.scopeData;
                if (animEvent.overrideScopeData)
                {
                    targetedSkill?.OverrideScopeData(animEvent.scopeDataOverride);
                }
                targetedSkill?.CheckTargetIsRandom();
            }
            animEvent.onEventRun.Invoke();
            if (animEvent.playSFX && !animEvent.playSFXOnHit)
            {
                BattleManager.PlayAnimationEventSFX(animEvent);
            }
            if (animEvent.userMotion)
            {
                if (targetedSkill != null)
                {
                    var user = targetedSkill.user;
                    if (user != null)
                    {
                        user.imageReference.PlayMotion(animEvent.motionType);
                    }
                }
            }
            if (animEvent.hit)
            {
                BattleManager.instance.HitTarget(animEvent);
            }
            else
            {
                if (animEvent.flashTarget && targetedSkill != null && targetedSkill.targets != null)
                {
                    for (int i = 0; i < targetedSkill.targets.Count; i++)
                    {
                        BattleManager.TintTarget(animEvent.skillOrigin.targets[i], animEvent.flashTargetData.flashColor, animEvent.flashTargetData.flashDuration);
                    }
                }
            }
            if (animEvent.flashScreen)
            {
                BattleManager.TintScreen(animEvent.flashScreenData.flashColor, animEvent.flashScreenData.flashDuration);
            }
            if (animEvent.overrideScopeData)
            {
                targetedSkill?.OverrideScopeData(orgScopeData, orgTargets);
            }
            
        }

        public void EndAnimation() //Called from Animation
        {
            m_isFinished = true;
            if (callRef != null)
            {
                callRef?.AnimationEnded();
            }
            else Destroy(gameObject);
        }
        public void InitiateAnimation(TargetedSkill targetedSkill)
        {
            m_isFinished = false;
            callRef = targetedSkill;
            if (callRef != null) callRef.runningAnimation = true;
            for(int i=0; i < events.Count; i++)
            {
                events[i].InitiateAnimationEvent(this, callRef);
            }
            if (autoRunEvents)
            {
                StartCoroutine(AutoRunEvents());
            }
        }
        private void Update()
        {
            if (queuePause)
            {
                if (UIController.instance.skipButtonHold)
                {
                    StopPause();
                }
            }
        }
        protected IEnumerator AutoRunEvents()
        {
            for (int i = 0; i < events.Count; i++)
            {
                RunEvent(i);
                if (queuePause)
                {
                    ResetPauseRoutine(events[i]);
                    yield return StartCoroutine(pauseCoroutine);
                }
            }
            yield return null;
            EndAnimation();
        }
        public void QueuePause()
        {
            QueuePause(null);
        }
        /// <summary>
        /// Allows custom Crit pause timer
        /// </summary>
        /// <param name="hitInfo">Reference to the event's hit data.</param>
        public void QueuePause(BattleAnimationEvent hitInfo)
        {
            if (UIController.instance.skipButtonHold) return;
            if (anim != null && !autoRunEvents)
            {
                ResetPauseRoutine(hitInfo);
                StartCoroutine(pauseCoroutine);
            }
            else queuePause = false;
        }
        private void ResetPauseRoutine(BattleAnimationEvent hitInfo)
        {
            StopPause();
            pauseCoroutine = Pause(hitInfo);
        }
        protected IEnumerator Pause(BattleAnimationEvent hitInfo = null)
        {
            queuePause = true;
            if (anim != null)
            {
                animOrgSpeed = anim.speed;
                Debug.Log(animOrgSpeed);
                anim.speed = 0f;
            }
            float pauseTime = (hitInfo != null ? (hitInfo.overrideCritPauseTimer ? hitInfo.critPauseTimer : TUFFSettings.critPauseTimer) : TUFFSettings.critPauseTimer);
            yield return new WaitForSeconds(pauseTime);
            Resume(animOrgSpeed);
        }
        protected void StopPause()
        {
            if (pauseCoroutine != null) StopCoroutine(pauseCoroutine);
            if (queuePause) Resume(animOrgSpeed);
        }
        protected void Resume(float orgSpeed)
        {
            if (anim != null)
            {
                anim.speed = orgSpeed;
            }
            queuePause = false;
        }

        public void OverrideEvent(BattleAnimationEvent battleAnimationEvent, int index)
        {
            if (index < 0 || index >= events.Count) return;
            if (battleAnimationEvent == null) return;
            events[index] = battleAnimationEvent;
            events[index].InitiateAnimationEvent(this, callRef);
        }
        public AnimationPivotType GetPivot(Targetable target)
        {
            if (target == null) return AnimationPivotType.Center;
            if (target is PartyMember) return targetPartyPivot;
            return targetEnemyPivot;
        }
        protected void Reset() //Sets variable default values for new entries on List
        {
            events = new List<BattleAnimationEvent>()
            {
                new BattleAnimationEvent()
            };
            events[0].SFXList = new List<SFX>()
            {
                new SFX()
            };
            events[0].formula = new HitFormula();
            events[0].formula.formulaGroups = new List<HitFormulaGroup>()
            {
                new HitFormulaGroup()
            };
            events[0].formula.formulaGroups[0].formulaOperations = new List<HitFormulaOperation>()
            {
                new HitFormulaOperation()
            };
            events[0].effects = new List<Effect>()
            {
                new Effect()
            };
        }
    }
}

