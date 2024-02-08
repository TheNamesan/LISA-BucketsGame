using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TUFF
{
    public class FlashImageHandler : MonoBehaviour
    {
        public Image image;
        [HideInInspector] public Color originalColor = new Color();
        protected bool originalColorObtained = false;
        protected Tween colorTween;
        void Awake()
        {
            if (!originalColorObtained) ObtainOriginalColor();
        }

        public void Flash(Color color, float duration)
        {
            if (image == null) return;
            if (!originalColorObtained) ObtainOriginalColor();
            KillTween();
            var orgColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            image.color = color;
            colorTween = image.DOFade(0f, duration).OnComplete(() => image.color = orgColor);
        }
        protected void ObtainOriginalColor()
        {
            if (image != null)
            {
                originalColor = image.color;
                originalColorObtained = true;
            }
        }
        public void KillTween()
        {
            colorTween?.Complete();
            colorTween?.Kill();
            colorTween = null;
        }
        private void OnDestroy()
        {
            KillTween();
        }
    }
}

