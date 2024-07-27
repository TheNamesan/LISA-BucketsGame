using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TUFF
{
    public class FadeScreenTrigger : MonoBehaviour
    {
        public Image img;
        private Tween fadeTween;
        public void Awake()
        {
            if (!img) img = GetComponent<Image>();
        }

        public void SetAlpha(float newValue)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, newValue);
        }
        public void FadeIn(float duration)
        {
            FadeIn(duration, null);
        }

        public void FadeIn(float duration, Action onComplete)
        {
            fadeTween?.Pause();
            fadeTween?.Kill();
            fadeTween = img.DOFade(0f, duration).From(1f).SetAutoKill(false).SetUpdate(true)
                .OnComplete(() => { onComplete?.Invoke(); });
        }

        public void FadeOut(float duration)
        {
            FadeOut(duration, null);
        }
        public void FadeOut(float duration, Action onComplete)
        {
            fadeTween?.Pause();
            fadeTween?.Kill();
            fadeTween = img.DOFade(1f, duration).From(0f).SetAutoKill(false).SetUpdate(true)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}
