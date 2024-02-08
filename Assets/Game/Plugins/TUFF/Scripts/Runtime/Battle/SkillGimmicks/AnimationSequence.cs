using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    public enum AnimSequenceElementType
    {
        PlayAnimation = 0,
        WaitForAnimationEnd = 1,
        WaitForSeconds = 2
    }
    /// <summary>
    /// Runs a sequence of Battle Animations without the Hit or Skill data. 
    /// </summary>
    public class AnimationSequence : MonoBehaviour
    {
        public List<AnimationSequenceElement> sequence = new List<AnimationSequenceElement>();
        protected BattleAnimation battleAnim;
        protected int sequenceIndex = 0;
        protected BattleAnimation lastAnimation = null;

        protected void Awake()
        {
            battleAnim = GetComponent<BattleAnimation>();
        }
        protected void Start()
        {
            StartCoroutine(RunAnimationSequence());
        }

        protected IEnumerator RunAnimationSequence()
        {
            if(battleAnim == null) { EndSkill();  yield break; }
            for(sequenceIndex = 0; sequenceIndex < sequence.Count; sequenceIndex++)
            {
                if(sequence[sequenceIndex].elementType == AnimSequenceElementType.PlayAnimation)
                {
                    if (sequence[sequenceIndex].animation == null) continue;
                    var anim = BattleManager.instance.PlayAnimation(sequence[sequenceIndex].animation, battleAnim.transform.position);
                    lastAnimation = anim;
                }
                else if(sequence[sequenceIndex].elementType == AnimSequenceElementType.WaitForAnimationEnd)
                {
                    if (lastAnimation == null) continue;
                        yield return new WaitUntil(() => lastAnimation.isFinished);
                }
                else if (sequence[sequenceIndex].elementType == AnimSequenceElementType.WaitForSeconds)
                {
                    yield return new WaitForSeconds(sequence[sequenceIndex].waitSeconds);
                }
            }
            EndSkill();
            yield break;
        }

        public void EndSkill()
        {
            battleAnim?.EndAnimation();
        }
    }
    [System.Serializable]
    public class AnimationSequenceElement
    {
        public AnimSequenceElementType elementType = AnimSequenceElementType.PlayAnimation;
        public BattleAnimation animation;
        public float waitSeconds = 0f;
    }
}
