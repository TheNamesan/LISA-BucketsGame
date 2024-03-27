using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class CharacterAnimationHandler : AnimationHandler
    {
        public Transform scarfPivot;
        public bool showArms;
        public bool showLegs;
        public SpriteRenderer leftArm;
        public SpriteRenderer rightArm;
        public SpriteRenderer legs;
        public Vector2 pivotLeft = new Vector2(-0.58f, 0.5f);
        public Vector2 pivotRight = new Vector2(0.03f, 0.5f);
        public Vector2 walkPivotLeft = new Vector2(-0.58f, 0.625f);
        public Vector2 walkPivotRight = new Vector2(0.03f, 0.625f);
        public Vector2 fallPivotLeft = new Vector2(-0.3125f, 0.7075f);
        public Vector2 fallPivotRight = new Vector2(0.34375f, 0.645f);
        public Vector2 scarfNormalPosition = new Vector2(0, 0.72f);
        public Vector2 scarfShootPosition = new Vector2(-0.27f, 0.72f);
        public Vector2 scarfDeadPosition = new Vector2(0f, 0.18f);
        private float m_startNormalizedTime = 0;

        public void FlipSprite(Facing facing)
        {
            bool flip = (facing == Facing.Left);
            spriteRenderer.flipX = flip;
            if (leftArm) leftArm.flipX = flip;
            if (rightArm) rightArm.flipX = flip;
            if (legs) legs.flipX = flip;
        }    
        public void ShowArms(bool show)
        {
            showArms = show;
            if (leftArm) leftArm.gameObject.SetActive(show);
            if (rightArm) rightArm.gameObject.SetActive(show);
        }
        public void ShowLegs(bool show)
        {
            showLegs = show;
            if (legs) legs.gameObject.SetActive(show);
        }
        public void SetScarfPivot(int index, int faceDir = 1)
        {
            if (!scarfPivot) return;
            Vector2 pivot = new Vector2();
            if (index == 0) pivot = scarfNormalPosition;
            if (index == 1) pivot = scarfShootPosition;
            if (index == 2) pivot = scarfDeadPosition;
            pivot.x *= faceDir;
            scarfPivot.transform.localPosition = pivot;
        }
        public void AngleArms(Vector2 normal, int facingSign, CharacterStates state)
        {
            Debug.Log($"Normal: {normal}, Facing: {facingSign}");
            normal *= facingSign;

            Vector2 offsetLeft = Vector2.zero;
            Vector2 offsetRight = Vector2.zero;
            // Adjust pivots to flipped sprite
            Vector2 leftArmPivot = (pivotLeft);
            Vector2 rightArmPivot = (pivotRight);

            if (state == CharacterStates.Walk)
            {
                leftArmPivot = (walkPivotLeft);
                rightArmPivot = (walkPivotRight);
                offsetLeft = walkPivotLeft - pivotLeft;
                offsetRight = walkPivotLeft - pivotLeft;
            }
            else if (state == CharacterStates.Airborne)
            {
                leftArmPivot = (fallPivotLeft);
                rightArmPivot = (fallPivotRight);
                offsetLeft = fallPivotLeft - pivotLeft;
                offsetRight = fallPivotRight - pivotRight;

                offsetLeft.x *= facingSign;
                offsetRight.x *= facingSign;
            }
            if (facingSign < 0) { leftArmPivot.x *= -1; rightArmPivot.x *= -1; }
            if (leftArm) leftArm.transform.parent.localPosition = leftArmPivot;
            if (rightArm) rightArm.transform.parent.localPosition = rightArmPivot;

            // Adjust arm positions to flipped parent position
            if (leftArm) leftArm.transform.localPosition = new Vector3(-leftArmPivot.x + offsetLeft.x, -leftArmPivot.y + offsetLeft.y, leftArm.transform.localPosition.z);
            if (rightArm) rightArm.transform.localPosition = new Vector3(-rightArmPivot.x + offsetRight.x, -rightArmPivot.y + offsetRight.y, rightArm.transform.localPosition.z);

            //normal = Vector2.right;
            // Rotate from pivot
            if (leftArm) leftArm.transform.parent.localRotation = Quaternion.FromToRotation(Vector2.right, normal);
            if (rightArm) rightArm.transform.parent.localRotation = Quaternion.FromToRotation(Vector2.right, normal);

            //if (leftArm) leftArm.transform.localRotation = Quaternion.FromToRotation(Vector2.right, normal);
            //if (rightArm) rightArm.transform.localRotation = Quaternion.FromToRotation(Vector2.right, normal);
        }
        public void ChangeAnimationState(PlayerController controller, CharacterStates state, bool forcePlaySameState = false, float fromNormalizedTime = 0)
        {
            if (controller == null) return;
            if (!anim) return;

            if (state != controller.lastState)
            {
                showArms = false;
                CancelAnimationWait();
            }
            m_startNormalizedTime = fromNormalizedTime;

            string stateName = GetAnimationStateName(controller, state, out bool showArm, out bool showLeg);
            //ShowArms(true);
            
            if (lastStateName == stateName && !forcePlaySameState) { return; }
            if (lastStateName == stateName && forcePlaySameState && animationInWait)
            {
                CancelAnimationWait();
                SetAnimationWait();
            }
            anim.enabled = true;
            ShowArms(showArm);
            ShowLegs(false);
            anim.Play(stateName, 0, m_startNormalizedTime);
            
            lastStateName = stateName;
        }
        public void PlayArmsAnimation(Vector2 normal, int facingSign, CharacterStates state, float from = 0)
        {
            anim.Play("ArmsShoot", 1, from);
            AngleArms(normal, facingSign, state); // This is just here as a fallback :P
        }
        private string GetAnimationStateName(PlayerController controller, CharacterStates state, out bool showArm, out bool showLeg)
        {
            string dir = (controller.facing == Facing.Right ? "Right" : "Left");
            showArm = false;
            showLeg = false;
            if (!controller.flipLock) FlipSprite(controller.facing);
            SetScarfPivot(0);
            if (controller.dead || controller.stunned)
            {
                SetScarfPivot(2);
                if (!controller.grounded)
                { 
                    return "Dead"; 
                }
                return "Dead";
            }
            switch (state)
            {
                case CharacterStates.Idle:
                    if (controller.weapon.animTicks > 0)
                    {
                        // the ShootSomething is necessary so it repeats the animation when shooting again
                        if (lastStateName.StartsWith("ShootIdle") || lastStateName.StartsWith("RecoverDash"))
                            CancelAnimationWait();
                        if (animationInWait) { showArm = showArms; return lastStateName; }
                        showArm = true;
                        int faceDir = (!controller.flipLock ? controller.FaceToInt() : controller.flipLockDir) ;
                        SetScarfPivot(1, faceDir);
                        AngleArms(controller.weapon.shootNormal, faceDir, state);
                        SetAnimationWait(0.44f);
                        return $"ShootIdle";
                    }
                    else if (lastStateName.StartsWith("Shoot")) CancelAnimationWait(); // Failsafe
                    if (lastStateName.StartsWith("Dash"))
                    {
                        SetAnimationWait();
                        return $"RecoverDash";
                    }
                    if (controller.lastState == CharacterStates.Airborne && !controller.ignoreLandAnim)
                    {
                        SetAnimationWait();
                        return $"LandRight";
                    }
                    if (animationInWait) { showArm = showArms; return lastStateName; }
                    return $"IdleRight";
                case CharacterStates.Walk:
                    if (controller.weapon.animTicks > 0)
                    {
                        bool changedWalking = ((lastStateName == ("ShootWalk") && controller.walkingBackwards)
                            || (lastStateName == ("ShootWalkBack") && !controller.walkingBackwards));
                        // the ShootSomething is necessary so it repeats the animation when shooting again
                        if (lastStateName.StartsWith("ShootWalk") || lastStateName.StartsWith("RecoverDash") || changedWalking)
                            CancelAnimationWait();
                        if (animationInWait) { showArm = showArms; return lastStateName; }
                        showArm = true;
                        // Continue animation seemlessly from normal walk
                        if (lastStateName.StartsWith("Walk") || changedWalking)
                            m_startNormalizedTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        int faceDir = (!controller.flipLock ? controller.FaceToInt() : controller.flipLockDir);
                        SetScarfPivot(1, faceDir);
                        AngleArms(controller.weapon.shootNormal, faceDir, state);
                        SetAnimationWait(0.44f);
                        if (controller.walkingBackwards) { return $"ShootWalkBack"; }
                        return $"ShootWalk";
                    }
                    else if (lastStateName.StartsWith("Shoot")) CancelAnimationWait(); // Failsafe
                    if (lastStateName.StartsWith("Dash"))
                    {
                        SetAnimationWait();
                        return $"RecoverDash";
                    }
                    if (animationInWait) { showArm = showArms; showLeg = showLegs; return lastStateName; }
                    // Continue body animation seemlessly from ShootWalk
                    if (lastStateName.StartsWith("ShootWalk"))
                        m_startNormalizedTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    return $"WalkRight";
                case CharacterStates.Airborne:
                    if (controller.weapon.animTicks > 0)
                    {
                        //the ShootSomething is necessary so it repeats the animation when shooting again
                        if (lastStateName.StartsWith("ShootFall") || lastStateName.StartsWith("RecoverDash"))
                            CancelAnimationWait();
                        if (animationInWait) { showArm = showArms; return lastStateName; }
                        showArm = true;
                        // Continue animation seemlessly from normal fall
                        if (lastStateName.StartsWith("Fall"))
                            m_startNormalizedTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        int faceDir = (!controller.flipLock ? controller.FaceToInt() : controller.flipLockDir);
                        AngleArms(controller.weapon.shootNormal, faceDir, state);
                        SetAnimationWait(0.44f);
                        return $"ShootFall";
                    }
                    else if (lastStateName.StartsWith("Shoot")) CancelAnimationWait(); // Failsafe
                    if (controller.wallClimb)
                    {
                        return $"WallSlide";
                    }
                    if (controller.doubleJumping)
                    {
                        return $"DoubleJumpRight";
                    }
                    if (lastStateName.StartsWith("ShootFall"))
                        m_startNormalizedTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
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
        private void SetAnimationWait(float duration)
        {
            if (animWaitCoroutine != null) StopCoroutine(animWaitCoroutine);
            animationInWait = false;
            animWaitCoroutine = WaitForAnimationEnd(duration);
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
        private IEnumerator WaitForAnimationEnd(float duration)
        {
            if (animationInWait) { yield break; };

            yield return new WaitForEndOfFrame();
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
