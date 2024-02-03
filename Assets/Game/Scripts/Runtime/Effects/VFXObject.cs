using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class VFXObject : PoolObject
    {
        public Animator anim;
        private IEnumerator durationCoroutine = null;
        public void Play(string animName)
        {
            gameObject.SetActive(true);
            if (!anim) return;
            if (durationCoroutine != null) StopCoroutine(durationCoroutine);
            anim.Play(animName, -1, 0f);
            durationCoroutine = WaitForAnimationEnd();
            StartCoroutine(durationCoroutine);
        }
        private IEnumerator WaitForAnimationEnd()
        {
            yield return new WaitForEndOfFrame();
            float duration = anim.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
            ReturnToPool();
        }
        public override void ReturnToPool()
        {
            if (durationCoroutine != null) StopCoroutine(durationCoroutine);
            m_inUse = false;
            gameObject.SetActive(false);
        }
    }
}
