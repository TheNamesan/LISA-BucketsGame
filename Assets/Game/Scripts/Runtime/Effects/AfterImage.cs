using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BucketsGame
{
    public class AfterImage : MonoBehaviour
    {
        private Tween tween;
        public SpriteRenderer sprite;
        public bool inUse { get => m_inUse; }
        private bool m_inUse;
        
        public void Invoke(Vector3 position, Sprite sprite, bool flip, Color color, float duration)
        {
            gameObject.SetActive(true);
            GameUtility.KillTween(ref tween);
            if (this.sprite)
            {
                this.sprite.sprite = sprite;
                this.sprite.color = color;
                this.sprite.flipX = flip;
            }
            transform.position = position;
            m_inUse = true;
            tween = this.sprite.DOFade(0f, duration).From(color).SetEase(Ease.Linear)
                .OnComplete(OnComplete);
        }
        private void OnComplete()
        {
            m_inUse = false;
            gameObject.SetActive(false);
        }
    }
}

