using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class AnimationHandler : MonoBehaviour
    {
        [Header("References")]
        public Animator anim;
        public SpriteRenderer spriteRenderer;

        [Header("Animator")]
        public string lastStateName = "";
        protected IEnumerator stopHardLandingCDCoroutine;
        protected IEnumerator animWaitCoroutine;
        [SerializeField] protected bool animationInWait = false;
    }
}
