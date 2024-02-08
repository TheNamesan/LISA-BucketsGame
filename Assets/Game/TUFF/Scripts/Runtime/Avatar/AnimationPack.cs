using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Reflection;

namespace TUFF
{
    [CreateAssetMenu(fileName = "AnimationPack", menuName = "TUFF/Animation Pack")]
    public class AnimationPack : ScriptableObject
    {
        [Tooltip("If an animation in this pack is empty, the game will attempt to obtain the animation from the fallback pack.")]
        public AnimationPack fallback;
        [Header("Standing")]
        [Tooltip("Standing animation, right facing.")]
        public AnimationClip standingAnimR;
        [Tooltip("Standing animation, left facing.")]
        public AnimationClip standingAnimL;
        [Tooltip("Standing animation, up facing.")]
        public AnimationClip standingAnimU;
        [Tooltip("Standing animation, down facing.")]
        public AnimationClip standingAnimD;

        [Header("Run")]
        public AnimationClip walkR;
        public AnimationClip walkL;
        public AnimationClip runR;
        public AnimationClip runL;
        public AnimationClip runPrepR;
        public AnimationClip runPrepL;
        public AnimationClip skidR;
        public AnimationClip skidL;
        public AnimationClip runUnprepR;
        public AnimationClip runUnprepL;
        public AnimationClip runTransitionR;
        public AnimationClip runTransitionL;

        [Header("Fall")]
        public AnimationClip fallR;
        public AnimationClip fallL;
        public AnimationClip fallD;
        public AnimationClip jumpU;
        public AnimationClip jumpD;

        [Header("Land")]
        public AnimationClip hardLandR;
        public AnimationClip hardLandL;
        public AnimationClip hardLandD;
        public AnimationClip landR;
        public AnimationClip landL;
        public AnimationClip landU;
        public AnimationClip landD;

        [Header("Climb")]
        public AnimationClip climbN;
        public AnimationClip climbR;
        public AnimationClip climbL;
        public AnimationClip climbU;
        public AnimationClip climbD;
        public AnimationClip climbReturnR;
        public AnimationClip climbReturnL;
        public AnimationClip climbFallR;
        public AnimationClip climbFallL;

        [Header("Alts")]
        public List<AnimationPack> altPacks = new List<AnimationPack>();
        public AnimationClip GetAnim(AnimationNamesType type)
        {
            AnimationClip clip = null;
            switch (type)
            {
                case AnimationNamesType.StandingR: clip = standingAnimR; break;
                case AnimationNamesType.StandingL: clip = standingAnimL; break;
                case AnimationNamesType.StandingU: clip = standingAnimU; break;
                case AnimationNamesType.StandingD: clip = standingAnimD; break;
                case AnimationNamesType.WalkR: clip = walkR; break;
                case AnimationNamesType.WalkL: clip = walkL; break;
                case AnimationNamesType.RunR: clip = runR; break;
                case AnimationNamesType.RunL: clip = runL; break;
                case AnimationNamesType.RunPrepR: clip = runPrepR; break;
                case AnimationNamesType.RunPrepL: clip = runPrepL; break;
                case AnimationNamesType.SkidR: clip = skidR; break;
                case AnimationNamesType.SkidL: clip = skidL; break;
                case AnimationNamesType.RunUnprepR: clip = runUnprepR; break;
                case AnimationNamesType.RunUnprepL: clip = runUnprepL; break;
                case AnimationNamesType.RunTransitionR: clip = runTransitionR; break;
                case AnimationNamesType.RunTransitionL: clip = runTransitionL; break;
                case AnimationNamesType.FallR: clip = fallR; break;
                case AnimationNamesType.FallL: clip = fallL; break;
                case AnimationNamesType.FallD: clip = fallD; break;
                case AnimationNamesType.JumpU: clip = jumpU; break;
                case AnimationNamesType.JumpD: clip = jumpD; break;
                case AnimationNamesType.LandR: clip = landR; break;
                case AnimationNamesType.LandL: clip = landL; break;
                case AnimationNamesType.LandU: clip = landU; break;
                case AnimationNamesType.LandD: clip = landD; break;
                case AnimationNamesType.HardLandR: clip = hardLandR; break;
                case AnimationNamesType.HardLandL: clip = hardLandL; break;
                case AnimationNamesType.HardLandD: clip = hardLandD; break;
                case AnimationNamesType.ClimbN: clip = climbN; break;
                case AnimationNamesType.ClimbR: clip = climbR; break;
                case AnimationNamesType.ClimbL: clip = climbL; break;
                case AnimationNamesType.ClimbU: clip = climbU; break;
                case AnimationNamesType.ClimbD: clip = climbD; break;
                case AnimationNamesType.ClimbReturnR: clip = climbReturnR; break;
                case AnimationNamesType.ClimbReturnL: clip = climbReturnL; break;
                case AnimationNamesType.ClimbFallR: clip = climbFallR; break;
                case AnimationNamesType.ClimbFallL: clip = climbFallL; break;
            }
            if (UseFallback(clip)) clip = fallback.GetAnim(type);
            return clip;
        }
        private bool UseFallback(AnimationClip clip)
        {
            return clip == null && fallback != null && fallback != this;
        }
        //public AnimationClip GetAnim(string animName)
        //{
        //    FieldInfo fieldName = typeof(AnimationPack).GetField(animName);
        //    if (fieldName == null) { Debug.LogWarning("Invalid Field Name"); return null; }
        //    AnimationClip clip = fieldName.GetValue(this) as AnimationClip;
        //    if (clip == null && fallback != null)
        //        clip = fallback.GetAnim(animName);
        //    return clip;
        //}
    }
}
