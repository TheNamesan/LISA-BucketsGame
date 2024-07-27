using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class PlayAnimationAction : EventAction
    {
        public Animator animator;
        public string animationName;
        public PlayAnimationAction()
        {
            eventName = "Play Animation";
            eventColor = EventGUIColors.character;
        }
        public override void Invoke()
        {
            animator?.Play(animationName, -1, 0);
            isFinished = true;
        }
    }
}

