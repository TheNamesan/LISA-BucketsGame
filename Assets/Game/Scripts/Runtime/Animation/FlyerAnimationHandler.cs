using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class FlyerAnimationHandler : AnimationHandler
    {
        public Flyer target;

        private void Update()
        {
            ChangeAnimationState();
        }
        public void ChangeAnimationState(bool forcePlaySameState = false)
        {
            if (target == null) return;
            if (!anim) return;

            string stateName = GetAnimationStateName();
            if (lastStateName == stateName && !forcePlaySameState) { return; }
            anim.enabled = true;
            anim.Play(stateName, -1, 0);
            lastStateName = stateName;
        }
        public string GetAnimationStateName()
        {
            if (target.dead)
            {
                if (!target.grounded) return "FlyerDeadAir";
                return "FlyerDead";
            }
            else if (target.firing)
            {
                return "FlyerAttack";
            }
            return "FlyerIdle";
        }
    }
}
