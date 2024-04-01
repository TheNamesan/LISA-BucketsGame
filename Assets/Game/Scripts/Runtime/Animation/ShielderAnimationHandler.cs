using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class ShielderAnimationHandler : AnimationHandler
    {
        public Shielder target;

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
                return "ShielderDead";
            }
            if (target.firing)
            {
                return "ShielderFire";
            }
            if (Mathf.Abs(target.rb.velocity.x) >= 0.001f)
            {
                return "ShielderWalk";
            }
            
            if (target.attacking)
            {
                return "ShielderAttack";
            }
            return "ShielderIdle";
        }
    }

}
