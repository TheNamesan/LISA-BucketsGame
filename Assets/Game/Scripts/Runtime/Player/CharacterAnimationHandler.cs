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
        public Vector2 pivotLeft = new Vector2(-0.58f, 0.5f);
        public Vector2 pivotRight = new Vector2(0.03f, 0.5f);

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
        public void AngleArms(Vector2 normal, int facingSign)
        {
            normal *= facingSign;

            // Adjust pivots to flipped sprite
            Vector2 leftArmPivot = pivotLeft;
            Vector2 rightArmPivot = pivotRight;
            if (facingSign < 0) { leftArmPivot.x *= -1; rightArmPivot.x *= -1; }
            if (leftArm) leftArm.transform.parent.localPosition = leftArmPivot;
            if (rightArm) rightArm.transform.parent.localPosition = rightArmPivot;

            // Adjust arm positions to flipped parent position
            if (leftArm) leftArm.transform.localPosition = new Vector3(-leftArmPivot.x, -leftArmPivot.y, leftArm.transform.localPosition.z);
            if (rightArm) rightArm.transform.localPosition = new Vector3(-rightArmPivot.x, -rightArmPivot.y, rightArm.transform.localPosition.z);

            // Rotate from pivot
            if (leftArm) leftArm.transform.parent.localRotation = Quaternion.FromToRotation(Vector2.right, normal);
            if (rightArm) rightArm.transform.parent.localRotation = Quaternion.FromToRotation(Vector2.right, normal);
            //if (leftArm) leftArm.transform.localRotation = Quaternion.FromToRotation(Vector2.right, normal);
            //if (rightArm) rightArm.transform.localRotation = Quaternion.FromToRotation(Vector2.right, normal);
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
            //ShowArms(true);
            ShowArms(showArms);
            if (lastStateName == stateName && !forcePlaySameState) { return; }
            if (lastStateName == stateName && forcePlaySameState && animationInWait)
            {
                CancelAnimationWait();
                SetAnimationWait();
            }
            anim.enabled = true;
            anim.Play(stateName, -1, 0);
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
                    if (controller.weapon.animTicks > 0)
                    {
                        if (lastStateName.StartsWith("ShootIdleRight") || lastStateName.StartsWith("RecoverDash")) 
                            CancelAnimationWait();
                        if (animationInWait) { showArm = showArms; return lastStateName; }
                        showArm = true;
                        AngleArms(controller.weapon.shootNormal, controller.FaceToInt());
                        //CancelAnimationWait();
                        SetAnimationWait();
                        return $"ShootIdleRight";
                    }
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
