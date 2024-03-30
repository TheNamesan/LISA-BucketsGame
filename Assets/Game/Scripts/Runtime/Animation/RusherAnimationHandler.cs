using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class RusherAnimationHandler : AnimationHandler
    {
        public Rusher rusher;

        private void Update()
        {
            ChangeAnimationState();
        }
        public void ChangeAnimationState(bool forcePlaySameState = false)
        {
            if (rusher == null) return;
            if (!anim) return;

            string stateName = GetAnimationStateName();
            if (lastStateName == stateName && !forcePlaySameState) { return; }
            anim.enabled = true;
            anim.Play(stateName, -1, 0);
            lastStateName = stateName;
        }
        public string GetAnimationStateName()
        {
            if (rusher.dead)
            {
                return "RusherDead";
            }
            if (Mathf.Abs(rusher.rb.velocity.x) >= 0.001f)
            {
                return "RusherWalk";
            }
            if (rusher.attacking)
            {
                return "RusherAttack";
            }
            return "RusherIdle";
        }
    }
}
