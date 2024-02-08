using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TUFF
{
    public class SelectionBoxAnimator : MonoBehaviour
    {
        public Tween alphaTween = null;
        private Image image;

        public void OnEnable()
        {
            image = GetComponent<Image>();
        }
        public void Update()
        {
            if (image != null && image.isActiveAndEnabled && alphaTween == null)
            {
                alphaTween = image.DOFade(0.5f, 0.5f).From(1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            }
        }
        public void OnDisable()
        {
            KillTween();
        }
        public void OnDestroy()
        {
            KillTween();
        }
        public void KillTween()
        {
            alphaTween?.Kill();
            alphaTween = null;
        }
    }
}

