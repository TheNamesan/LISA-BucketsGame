using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class CharacterAnimationHandler : AnimationHandler
    {
        
        public bool showArms;
        public bool showLegs;
        public SpriteRenderer leftArm;
        public SpriteRenderer rightArm;
        public SpriteRenderer legs;
        public Vector2 pivotLeft = new Vector2(-0.58f, 0.5f);
        public Vector2 pivotRight = new Vector2(0.03f, 0.5f);
        public Vector2 walkPivotLeft = new Vector2(-0.58f, 0.625f);
        public Vector2 walkPivotRight = new Vector2(0.03f, 0.625f);
        //public Vector2 walkHeightOffset = new Vector2(0, 0.125f);
        private float m_startNormalizedTime = 0;
        private float m_armsStartNormalizedTime = 0;
        private bool m_playArms;

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
        public void AngleArms(Vector2 normal, int facingSign, bool walking)
        {
            normal *= facingSign;

            Vector2 offset = Vector2.zero;
            // Adjust pivots to flipped sprite
            Vector2 leftArmPivot = (walking ? walkPivotLeft : pivotLeft);
            Vector2 rightArmPivot = (walking ? walkPivotRight : pivotRight);
            if (walking) offset = walkPivotLeft - pivotLeft;
            if (facingSign < 0) { leftArmPivot.x *= -1; rightArmPivot.x *= -1; }
            if (leftArm) leftArm.transform.parent.localPosition = leftArmPivot;
            if (rightArm) rightArm.transform.parent.localPosition = rightArmPivot;

            // Adjust arm positions to flipped parent position
            if (leftArm) leftArm.transform.localPosition = new Vector3(-leftArmPivot.x + offset.x, -leftArmPivot.y + offset.y, leftArm.transform.localPosition.z);
            if (rightArm) rightArm.transform.localPosition = new Vector3(-rightArmPivot.x + offset.x, -rightArmPivot.y + offset.y, rightArm.transform.localPosition.z);

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
            m_armsStartNormalizedTime = 0;
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
            if (m_playArms) anim.Play("ArmsShoot", 1, m_armsStartNormalizedTime);
            
            lastStateName = stateName;
            m_playArms = false;
        }
        private string GetAnimationStateName(PlayerController controller, CharacterStates state, out bool showArm, out bool showLeg)
        {
            string dir = (controller.facing == Facing.Right ? "Right" : "Left");
            showArm = false;
            showLeg = false;
            FlipSprite(controller.facing);
            switch (state)
            {
                case CharacterStates.Idle:
                    if (controller.weapon.animTicks > 0)
                    {
                        if (lastStateName.StartsWith("ShootIdle") || lastStateName.StartsWith("RecoverDash"))
                            CancelAnimationWait();
                        if (animationInWait) { showArm = showArms; return lastStateName; }
                        showArm = true;
                        // Continue arms animation seemlessly from walk
                        if (lastStateName.StartsWith("ShootWalk"))
                            m_armsStartNormalizedTime = anim.GetCurrentAnimatorStateInfo(1).normalizedTime;
                        AngleArms(controller.weapon.shootNormal, controller.FaceToInt(), false);
                        m_playArms = true;
                        //CancelAnimationWait();
                        SetAnimationWait();
                        return $"ShootIdle";
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
                    if (controller.weapon.animTicks > 0)
                    {
                        if (lastStateName.StartsWith("ShootWalk") || lastStateName.StartsWith("RecoverDash"))
                            CancelAnimationWait();
                        if (animationInWait) { showArm = showArms; showLeg = showLegs; return lastStateName; }
                        showArm = true;
                        showLeg = true;
                        // Continue arms animation seemlessly from idle
                        if (lastStateName.StartsWith("ShootIdle"))
                            m_armsStartNormalizedTime = anim.GetCurrentAnimatorStateInfo(1).normalizedTime;
                        // Continue animation seemlessly from normal walk
                        if (lastStateName.StartsWith("Walk"))
                            m_startNormalizedTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        AngleArms(controller.weapon.shootNormal, controller.FaceToInt(), true);
                        m_playArms = true;
                        //CancelAnimationWait();
                        SetAnimationWait();
                        return $"ShootWalk";
                    }
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
