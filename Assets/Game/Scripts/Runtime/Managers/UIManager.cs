using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BucketsGame
{
    public class UIManager : MonoBehaviour
    {
        [Header("Bar")]
        public Image focusFill;
        public ImageAnimator focusAnim;
        public ImageAnimator eyeAnim;
        private PlayerController Player { get => SceneProperties.mainPlayer; }
        private float m_timePassed = 0;
        private void OnEnable()
        {
            ScrollBarEffect(0);
        }

        private void ScrollBarEffect(float newTime)
        {
            m_timePassed = newTime;
            if (focusFill && focusFill.material != null) focusFill.material.SetFloat("_PassTime", m_timePassed);
        }

        private void Update()
        {
            UpdateBar();
        }
        private void UpdateBar()
        {
            var focus = GameManager.instance.focusMode;
            if (focusFill)
            {
                focusFill.fillAmount = GameManager.instance.FocusFill();
                if (focus && focusFill.material != null)
                {
                    ScrollBarEffect(m_timePassed + Time.unscaledDeltaTime);
                }
            }
            if (focusAnim)
            {
                focusAnim.Play(focus);
            }
            if (eyeAnim) eyeAnim.Play(focus);
        }
    }
}
