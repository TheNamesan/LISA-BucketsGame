using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class CharacterAnimationHandler : AnimationHandler
    {
        

        public void FlipSprite(Facing facing)
        {
            spriteRenderer.flipX = (facing == Facing.Left);
        }    
        public void ChangeAnimationState(PlayerController controller, CharacterStates state, bool forcePlaySameState = false)
        {
            if (controller == null) return;
            if (!anim) return;

            if (state != controller.lastState)
            {
                CancelAnimationWait();
            }
            string stateName = GetAnimationStateName(controller, state);
            if (lastStateName == stateName && !forcePlaySameState) { return; }
            anim.enabled = true;
            anim.Play(stateName, -1, 0);
            //controller.lastState = state;
            lastStateName = stateName;
        }
        private string GetAnimationStateName(PlayerController controller, CharacterStates state)
        {
            string dir = (controller.facing == Facing.Right ? "Right" : "Left");
            FlipSprite(controller.facing);
            switch (state)
            {
                case CharacterStates.Idle:
                    if (lastStateName.StartsWith("Dash"))
                    {
                        SetAnimationWait();
                        return $"RecoverDash";
                    }
                    if (controller.lastState == CharacterStates.Airborne)
                    {
                        SetAnimationWait();
                        return $"LandRight";
                    }
                    if (animationInWait) return lastStateName;
                    return $"IdleRight";
                case CharacterStates.Walk:
                    return $"WalkRight";
                case CharacterStates.Airborne:
                    if (controller.doubleJumping)
                    {
                        return $"DoubleJumpRight";
                    }
                    return $"FallRight";
                case CharacterStates.Dashing:
                    return $"DashRight";
            }
            return "";

        }
        private void SetAnimationWait()
        {
            if (animWaitCoroutine != null) StopCoroutine(animWaitCoroutine);
            animationInWait = false;
            animWaitCoroutine = WaitForAnimationEnd();
            StartCoroutine(animWaitCoroutine);
            animationInWait = true;
        }
        private IEnumerator WaitForAnimationEnd()
        {
            if (animationInWait) { yield break; };

            yield return new WaitForEndOfFrame();
            float duration = anim.GetCurrentAnimatorStateInfo(0).length;
            //Debug.Log($"time to wait: {duration}");
            yield return new WaitForSeconds(duration);
            animationInWait = false;
            //Debug.Log($"done: {duration}");
        }
        private void CancelAnimationWait()
        {
            if (animWaitCoroutine != null) StopCoroutine(animWaitCoroutine);
            animationInWait = false;
        }
    }
}
