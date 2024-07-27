using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TUFF
{
    public class TintScreenTrigger : MonoBehaviour
    {
        public Image img;
        private Tween fadeTween;
        public void Awake()
        {
            if (!img) img = GetComponent<Image>();
        }
        public void Tint(Color color, float duration)
        {
            Tint(color, duration, null);
        }

        public void Tint(Color color, float duration, Action onComplete)
        {
            fadeTween?.Pause();
            fadeTween?.Kill();
            fadeTween = img.DOColor(color, duration).OnComplete(() => { onComplete?.Invoke(); });
        }
    }
}
