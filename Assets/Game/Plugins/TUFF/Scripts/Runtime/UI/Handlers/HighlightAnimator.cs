using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TUFF
{
    public class HighlightAnimator : MonoBehaviour
    {
        [Tooltip("Reference to the Image component for effects.")]
        public Image imageVFX;
        protected Tween colorTween;
        [HideInInspector] public bool isHighlighted = false;

        public virtual void Highlight(bool highlight)
        {
            if (imageVFX == null) return;
            if (highlight)
            {
                imageVFX.enabled = true;
                imageVFX.color = new Color(imageVFX.color.r, imageVFX.color.g, imageVFX.color.b, 0.15f);
            }
            else
            {
                if (isHighlighted) imageVFX.DOFade(0f, 0.25f);
            }
            isHighlighted = highlight;
        }
    }
}

