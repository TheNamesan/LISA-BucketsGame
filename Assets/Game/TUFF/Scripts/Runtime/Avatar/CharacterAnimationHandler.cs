using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public enum AnimationNamesType
    {
        StandingR, StandingL, StandingU, StandingD,
        WalkR, WalkL,
        RunR, RunL,
        RunPrepR, RunPrepL, SkidR, SkidL,
        RunUnprepR, RunUnprepL,
        RunTransitionR, RunTransitionL,
        FallR, FallL, FallD, 
        JumpU, JumpD,
        LandR, LandL, LandU, LandD,
        HardLandR, HardLandL, HardLandD, 
        ClimbN,
        ClimbR, ClimbL, ClimbU, ClimbD, 
        ClimbReturnR, ClimbReturnL,
        ClimbFallR, ClimbFallL,
        FreeAnim, Empty
    }
    public class CharacterAnimationHandler : MonoBehaviour
    {
        public readonly string[] animNames =
        {
            "StandingR", "StandingL", "StandingU", "StandingD",
            "WalkR", "WalkL",
            "RunR", "RunL",
            "RunPrepR", "RunPrepL", "SkidR", "SkidL",
            "RunUnprepR", "RunUnprepL",
            "RunTransitionR", "RunTransitionL",
            "FallR", "FallL", "FallD",
            "JumpU", "JumpD",
            "LandR", "LandL", "LandU", "LandD",
            "HardLandR", "HardLandL", "HardLandD",
            "ClimbN",
            "ClimbR", "ClimbL", "ClimbU", "ClimbD",
            "ClimbReturnR", "ClimbReturnL",
            "ClimbFallR", "ClimbFallL",
            "FreeAnim", "Empty"
        };
        public Animator anim;
        public SpriteRenderer spriteRenderer;
        public AnimationPack pack;
        public int alt = -1;

        [Header("Animator")]
        public string lastStateName = "";
        protected IEnumerator stopHardLandingCDCoroutine;
        protected IEnumerator animWaitCoroutine;
        [SerializeField] bool animationInWait = false;


        private AnimatorOverrideController overrideControl;
        private AnimationClipOverrides clipOverrides;
        private bool m_initialized = false;
        private bool m_lockBuffer = false;
        void Start()
        {
            // Initialize Override Variables
            if (!m_initialized)
            {
                Initialize();
                LoadAnimationPack(pack);
            }
        }

        private void Initialize()
        {
            if (anim != null)
            {
                overrideControl = new AnimatorOverrideController(anim.runtimeAnimatorController);
                overrideControl.name = $"Override {anim.name}";
                anim.runtimeAnimatorController = overrideControl;
                Debug.Log("Overrides Count: " + overrideControl.overridesCount);
                clipOverrides = new AnimationClipOverrides(overrideControl.overridesCount); // Calling Overrides Count pre allocates the overrides
                overrideControl.GetOverrides(clipOverrides);
                m_initialized = true;
            }
        }

        public void ChangeAnimationState(OverworldCharacterController controller, CharacterStates state, bool forcePlaySameState = false)
        {
            if (controller == null) return;
            if (state != controller.lastState)
            {
                CancelAnimationWait();
            }
            string stateName = GetAnimationStateName(controller, state);
            if (lastStateName == stateName && !forcePlaySameState) { return; }
            
            anim.enabled = true;
            lastStateName = stateName;

            if (m_lockBuffer) { /*Debug.Log($"LOCK BUFFER OFF, TRIED PLAYING: {stateName}");*/ m_lockBuffer = false; }
            else
            { 
                anim.Play(stateName, -1, 0);
                //Debug.Log($"Playing Anim: {stateName}");
            }
            //controller.lastState = state;
        }
        public string GetAnimationStateName(OverworldCharacterController c, CharacterStates state)
        {
            if (c == null) return "";
            var name = GetAdvancedPackAnimName(c, state);
            return name;
        }

        private string GetAdvancedPackAnimName(OverworldCharacterController c, CharacterStates state)
        {
            string postfix = (c.faceDirection == FaceDirections.East ? "Right" : "Left");
            switch (state)
            {
                case CharacterStates.Standing:
                    bool playLand = lastStateName.StartsWith("Jump") || lastStateName.StartsWith("Falling") || lastStateName.StartsWith("ClimbFalling");
                    if (playLand) // Landing
                    {
                        SetAnimationWait();
                        if (c.faceDirection == FaceDirections.East) return "LandingRight";
                        if (c.faceDirection == FaceDirections.West) return "LandingLeft";
                        if (c.faceDirection == FaceDirections.North) return "Landing";
                        if (c.faceDirection == FaceDirections.South) return "LandingDown";
                    }
                    if (c.faceDirection == FaceDirections.East || c.faceDirection == FaceDirections.West)
                    {
                        if (animationInWait) return lastStateName;
                        if ((lastStateName == $"Run{postfix}" || lastStateName == $"RunTransition{postfix}") &&
                            c.runCanceled)
                        {
                            SetAnimationWait();
                            return "Skid" + postfix;
                        }
                        if (c.runButtonHoldTime > 0 || c.runMomentum)
                        {
                            //if (lastStateName != "RunPrep" + postfix) SetAnimationWait();
                            return "RunPrep" + postfix;
                        }
                        if (lastStateName == "RunPrep" + postfix && c.IsRunPrepTimerReached())
                        {
                            SetAnimationWait();
                            return "RunUnprep" + postfix;
                        }
                        return "Standing" + postfix;
                    }
                    if (animationInWait) return lastStateName;
                    if (c.faceDirection == FaceDirections.North) return "StandingUp";
                    if (c.faceDirection == FaceDirections.South) return "StandingDown";
                    return "Standing" + postfix;
                case CharacterStates.Walk:
                    if (c.InRunningState())
                    {
                        if (animationInWait) return lastStateName;
                        if (lastStateName == "RunTransition" + postfix || lastStateName == "Run" + postfix)
                            return "Run" + postfix;
                        if (lastStateName != "RunTransition" + postfix)
                        {
                            SetAnimationWait();
                            return "RunTransition" + postfix;
                        }

                        return "Run" + postfix;
                    }
                    if (animationInWait) return lastStateName;
                    if (lastStateName == "Run" + postfix)
                    {
                        SetAnimationWait();
                        return "RunTransition" + postfix;
                    }
                    return "Walk" + postfix;
                case CharacterStates.Falling:
                    if (c.fellAfterClimbing)
                    {
                        if (c.faceDirection == FaceDirections.East) return "ClimbFallingRight";
                        if (c.faceDirection == FaceDirections.West) return "ClimbFallingLeft";
                    }
                    if (c.faceDirection == FaceDirections.East) return "FallingRight";
                    if (c.faceDirection == FaceDirections.West) return "FallingLeft";
                    if (c.faceDirection == FaceDirections.South) return "Falling";
                    return "Falling";
                case CharacterStates.Jump:
                    if (c.jumpDirection > 0) return "JumpDown";
                    return "Jump";
                case CharacterStates.HardLanding:
                    if (c.faceDirection == FaceDirections.East) return "HardLandingRight";
                    if (c.faceDirection == FaceDirections.West) return "HardLandingLeft";
                    if (c.faceDirection == FaceDirections.North) return "HardLandingNorth";
                    if (c.faceDirection == FaceDirections.South) return "HardLanding";
                    return "HardLanding";
                case CharacterStates.Climbing:
                    bool inLadderMovement = (c.climbMode == CharacterClimbMode.Ladder &&
                        (c.ladderTickTimer > 0 || c.ladderCDTimer > 0));
                    if (c.moveV != 0)
                    {
                        CancelAnimationWait();
                        if (c.moveV >= 0) return "Climbing";
                        else return "ClimbingDown";
                    }
                    if (inLadderMovement) return lastStateName;
                    if (c.moveV == 0)
                    {
                        if (animationInWait) return lastStateName;
                        if (!PlayerData.instance.charProperties.disableRopeJump)
                        {
                            if (lastStateName == "ClimbingRight" && c.moveH <= 0)
                            {
                                if (lastStateName != "ClimbingReturnRight")
                                    SetAnimationWait();
                                return "ClimbingReturnRight";
                            }
                            if (lastStateName == "ClimbingLeft" && c.moveH >= 0)
                            {
                                if (lastStateName != "ClimbingReturnLeft")
                                    SetAnimationWait();
                                return "ClimbingReturnLeft";
                            }
                            if (c.moveH > 0)
                            {
                                if (lastStateName != "ClimbingRight")
                                    SetAnimationWait();
                                return "ClimbingRight";
                            }
                            if (c.moveH < 0)
                            {
                                if (lastStateName != "ClimbingLeft")
                                    SetAnimationWait();
                                return "ClimbingLeft";
                            }
                        }
                    }
                    return "ClimbingNeutral";
                default: return "";
            }
        }

        private void CancelAnimationWait()
        {
            if (animWaitCoroutine != null) StopCoroutine(animWaitCoroutine);
            animationInWait = false;
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

        public void LoadAnimationPack(AnimationPack newpack)
        {
            if (overrideControl == null || clipOverrides == null) Initialize();
            OverrideAnimationClips(newpack);
            pack = newpack;
            alt = -1;
            Debug.Log("===Override Animator===");
        }
        public void UsePackAlt(int index)
        {
            if (overrideControl == null || clipOverrides == null) Initialize();
            if (pack == null) { Debug.LogWarning("No Pack assigned"); return; }
            if (index >= pack.altPacks.Count) return;
            AnimationPack targetPack = null;
            if (index < 0) targetPack = pack;
            else targetPack = pack.altPacks[index];
            alt = index;
            OverrideAnimationClips(targetPack);
        }

        protected void OverrideAnimationClips(AnimationPack newpack)
        {
            if (newpack == null) { Debug.LogWarning("Pack is empty"); return; }
            for (int idx = 0; idx < clipOverrides.Count; idx++)
            {
                var clip = clipOverrides[idx].Key;
                //Debug.Log("===NAME: " + clip.name);
                for (int i = 0; i < animNames.Length; i++)
                {
                    if (clip.name.Equals(animNames[i])) // If current clip has the correct name
                    {
                        if (clip.name.Equals(animNames[(int)AnimationNamesType.FreeAnim]) ||
                            clip.name.Equals(animNames[(int)AnimationNamesType.Empty]))
                            continue;
                        // Valid name, try to replace
                        var type = (AnimationNamesType)i;
                        var newClip = newpack.GetAnim(type); // Get animation matching the name type

                        if (newClip == null) { Debug.LogWarning("clip is empty: " + type); break; }
                        //Debug.Log($"Assigning animation: {newClip.name} to {clipOverrides[i]}" );
                        //clipOverrides[clip] = newClip;
                        clipOverrides.AssignNewValue(idx, newClip);
                        break;
                    }
                }
            }
            overrideControl.ApplyOverrides(clipOverrides);
        }
        public void PlayAnimation(AnimationClip clip)
        {
            if (overrideControl == null || clipOverrides == null) Initialize();
            if (clip == null) { Debug.LogWarning("Clip is empty"); return; }
            if (anim != null)
            {
                string id = animNames[(int)AnimationNamesType.FreeAnim];
                clipOverrides[id] = clip;
                overrideControl.ApplyOverrides(clipOverrides);
                anim.Play("FreeAnim", -1, 0);
                //Debug.Log("LOCK BUFFER ON");
                m_lockBuffer = true; // Used to avoid animations from being interrupted when an ActionList plays without yielding first
            }
            else { Debug.LogWarning("No Animator component assigned"); return; }
        }
        public void ChangeSprite(Sprite sprite)
        {
            if (overrideControl == null || clipOverrides == null) Initialize();
            if (anim != null)
            {
                anim.keepAnimatorStateOnDisable = true;
                anim.enabled = false;
                //anim.Play("Empty", -1, 0);
            }
            else { Debug.LogWarning("No Animator component assigned"); }
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
            }
            else { Debug.LogWarning("No Sprite Renderer component assigned!"); }

        }
        public void RestoreState(OverworldCharacterController controller)
        {
            if (!controller) return;
            m_lockBuffer = false;
            ChangeAnimationState(controller, controller.lastState, true);
        }
    }


    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity) { }

        public AnimationClip this[string name]
        {
            get { return this.Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.name.Equals(name));
                if (index >= 0) this.AssignNewValue(index, value);
            }
        }
        public AnimationClip this[AnimationClip clip]
        {
            get { return this.Find(x => x.Key.Equals(clip)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.Equals(clip));
                if (index >= 0) this.AssignNewValue(index, value);
            }
        }
        public void AssignNewValue(int index, AnimationClip value)
        {
            if (index < 0 || index >= Count) return;
            this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}
