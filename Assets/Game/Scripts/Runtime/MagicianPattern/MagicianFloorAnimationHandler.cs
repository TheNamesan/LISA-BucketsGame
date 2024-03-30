using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class MagicianFloorAnimationHandler : AnimationHandler
    {
        public MagicianFloor magicianFloor;
        private void Update()
        {
            ChangeAnimationState();
        }
        private void ChangeAnimationState(bool forcePlaySameState = false)
        {
            if (!magicianFloor) return;
            if (!anim) return;

            string stateName = GetAnimationStateName();
            if (lastStateName == stateName && !forcePlaySameState) { return; }
            anim.enabled = true;
            anim.Play(stateName, -1, 0);
            lastStateName = stateName;
        }
        private string GetAnimationStateName()
        {
            if (magicianFloor.telegraphTime > 0)
                return "MagicianFloorTelegraph";
            if (magicianFloor.floorTime > 0 && magicianFloor.telegraphTime <= 0)
                return "MagicianFloorStart";
            
            return "MagicianFloorEnd";
        }
    }
}
