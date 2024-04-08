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
        Tween fadeTween;
        private bool m_onCompleteCalled = false;
        public void Awake()
        {
            img = GetComponent<Image>();
        }

        public void SetAlpha(float newValue)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, newValue);
        }
        public void TriggerFadeIn(float duration)
        {
            FadeIn(duration, null);
        }
        public void TriggerFadeIn(float duration, Action onComplete)
        {
            FadeIn(duration, onComplete);
        }

        private void FadeIn(float duration, Action onComplete)
        {
            fadeTween?.Pause();
            fadeTween?.Kill();
            m_onCompleteCalled = false;
            fadeTween = img.DOFade(0f, duration).From(1f).SetAutoKill(false).SetUpdate(true)
                //.OnComplete(() => { onComplete?.Invoke(); })
                .OnComplete(() => { FadeInUpdate(onComplete); })
                .OnUpdate(() => FadeInUpdate(onComplete));
        }
        private void FadeInUpdate(Action onComplete)
        {
            if (fadeTween.ElapsedPercentage() >= 0.5f && !m_onCompleteCalled)
            {
                onComplete?.Invoke();
                m_onCompleteCalled = true;
            }
        }

        public void TriggerFadeOut(float duration)
        {
            FadeOut(duration, null);
        }
        public void TriggerFadeOut(float duration, Action onComplete)
        {
            FadeOut(duration, onComplete);
        }

        private void FadeOut(float duration, Action onComplete)
        {
            fadeTween?.Pause();
            fadeTween?.Kill();
            m_onCompleteCalled = false;
            fadeTween = img.DOFade(1f, duration).From(0f).SetAutoKill(false).SetUpdate(true)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}
