using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class CharacterAnimationHandler : AnimationHandler
    {
        public bool showArms;
        public SpriteRenderer leftArm;
        public SpriteRenderer rightArm;

        public void FlipSprite(Facing facing)
        {
            bool flip = (facing == Facing.Left);
            spriteRenderer.flipX = flip;
            if (leftArm) leftArm.flipX = flip;
            if (rightArm) rightArm.flipX = flip;
        }    
        public void ShowArms(bool show)
        {
            if (leftArm) leftArm.gameObject.SetActive(show);
            if (rightArm) rightArm.gameObject.SetActive(show);
        }
        public void ChangeAnimationState(PlayerController controller, CharacterStates state, bool forcePlaySameState = false)
        {
            if (controller == null) return;
            if (!anim) return;

            if (state != controller.lastState)
            {
                showArms = false;
                CancelAnimationWait();
            }
            string stateName = GetAnimationStateName(controller, state, out bool showArm);
            showArms = showArm;
            ShowArms(showArms);
            if (lastStateName == stateName && !forcePlaySameState) { return; }
            anim.enabled = true;
            anim.Play(stateName, -1, 0);
            //controller.lastState = state;
            lastStateName = stateName;
        }
        private string GetAnimationStateName(PlayerController controller, CharacterStates state, out bool showArm)
        {
            string dir = (controller.facing == Facing.Right ? "Right" : "Left");
            showArm = false;
            FlipSprite(controller.facing);
            switch (state)
            {
                case CharacterStates.Idle:
                    if (lastStateName.StartsWith("Dash"))
                    {
                        SetAnimationWait();
                        return $"RecoverDash";
                    }
                    if (controller.weapon.animTicks > 0)
                    {
                        if (animationInWait) { showArm = showArms; return lastStateName; }
                        showArm = true;
                        SetAnimationWait();
                        return $"ShootIdleRight";
                    }
                    if (controller.lastState == CharacterStates.Airborne)
                    {
                        SetAnimationWait();
                        return $"LandRight";
                    }
                    if (animationInWait) { showArm = showArms; return lastStateName; }
                    return $"IdleRight";
                case CharacterStates.Walk:
                    if (lastStateName.StartsWith("Dash"))
                    {
                        SetAnimationWait();
                        return $"RecoverDash";
                    }
                    if (animationInWait) return lastStateName;
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
