using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TUFF
{
    public class UnfoldBackgroundAnimator : MonoBehaviour
    {
        public MaskableGraphic background;
        public bool playOnEnable = true;

        public float targetAngle = 120f;
        public float startingAngle = 12f;
        public float duration = 1f;
        public float delay = 0f;

        public Ease ease = Ease.Linear;

        private Tween m_tween = null;
        public void OnEnable()
        {
            if (background == null) background = GetComponent<MaskableGraphic>();
            if (background != null)
                background.material = Instantiate(background.material);
            if (playOnEnable) PlayUnfold();
        }
        public void PlayUnfold()
        {
            if (background == null) Debug.Log("No background reference.");
            KillTween();
            m_tween = DOTween.To(value => OnUpdate(value), startingAngle, targetAngle, duration)
                .SetEase(ease)
                .SetDelay(delay);
        }
        private void OnUpdate(float value)
        {
            if (background != null)
                background.material.SetFloat("_Angle", value);
        }
        public void KillTween()
        {
            m_tween?.Kill();
            m_tween = null;
        }
    }
}

