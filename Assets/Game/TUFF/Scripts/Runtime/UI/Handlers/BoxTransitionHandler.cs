using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace TUFF
{ 
    public enum BoxTransitionState
    {
        Hidden = 0,
        Appearing = 1,
        Visible = 2,
        Dissapearing = 3
    }
    public enum BoxTransitionType
    {
        Instant = 0,
        Fade = 1,
        StretchAndFade = 2
    }
    public class BoxTransitionHandler : MonoBehaviour
    {
        public BoxTransitionType transitionType = BoxTransitionType.Instant;
        private BoxTransitionState m_state = BoxTransitionState.Hidden;
        public BoxTransitionState state { get => m_state; }
        public Tween fadeTween;
        public Tween stretchTween;
        public CanvasGroup canvasGroup;
        public AdjustToOtherRect adjustToOtherRect;
        public Vector2 originalSize = new Vector2();
        public UnityEvent onAppear = new UnityEvent(); // Needs functionality
        public UnityEvent onDissapear = new UnityEvent(); // Needs functionality
        public UnityEvent<BoxTransitionState> onTransitionChange = new();
        private float inDuration = 0.1f;
        private float outDuration = 0.1f;
        private bool m_gotSize = false;
        private RectTransform rectTransform { get { return transform as RectTransform; } }
        
        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            GetOriginalSize();
        }
        public void Appear()
        {
            m_state = BoxTransitionState.Appearing;
            GetOriginalSize();
            onAppear?.Invoke();
            Transition(true, inDuration);
        }

        public void Dissapear()
        {
            m_state = BoxTransitionState.Dissapearing;
            GetOriginalSize();
            onDissapear?.Invoke();
            Transition(false, outDuration);
        }
        private void Transition(bool appear, float duration)
        {
            float fadeStartValue = (appear ? 1f : 0f);
            float fadeEndValue = (appear ? 0f : 1f);
            float stretchStart = (appear ? 0f : originalSize.y); 
            float stretchEnd = (appear ? originalSize.y : 0f);
            BoxTransitionState completeState = (appear ? BoxTransitionState.Visible : BoxTransitionState.Hidden);
            KillTweens();
            if (transitionType == BoxTransitionType.Instant)
            {
                if (canvasGroup) canvasGroup.alpha = fadeEndValue;
                m_state = completeState;
                onTransitionChange.Invoke(completeState);
            }
            else if (transitionType == BoxTransitionType.Fade || transitionType == BoxTransitionType.StretchAndFade)
            {
                if (canvasGroup)
                    fadeTween = canvasGroup.DOFade(fadeStartValue, duration).From(fadeEndValue).OnComplete(() => { m_state = completeState; onTransitionChange.Invoke(completeState); });
            }
            if (transitionType == BoxTransitionType.StretchAndFade)
            {
                if (adjustToOtherRect)
                    stretchTween = DOTween.To(value => adjustToOtherRect.offset.y = value, -stretchEnd, -stretchStart, duration).OnComplete(() => { 
                        m_state = completeState; 
                        if (m_state == BoxTransitionState.Visible) originalSize = rectTransform.sizeDelta;
                        if (m_state == BoxTransitionState.Hidden) adjustToOtherRect.offset.y = 0;
                    });
                else 
                    stretchTween = rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, stretchEnd), duration).From(new Vector2(rectTransform.sizeDelta.x, stretchStart));
            }
        }
        private void GetOriginalSize()
        {
            if (!m_gotSize)
            {
                originalSize = rectTransform.sizeDelta;
                m_gotSize = true;
            }
        }
        private void KillTweens()
        {
            fadeTween?.Kill();
            stretchTween?.Kill();
        }
        private void OnDestroy()
        {
            KillTweens();
        }

    }
}
