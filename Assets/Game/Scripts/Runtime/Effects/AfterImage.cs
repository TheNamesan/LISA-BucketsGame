using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BucketsGame
{
    public class AfterImage : PoolObject
    {
        private Tween tween;
        public SpriteRenderer sprite;

        public void Invoke(Vector3 position, Quaternion rotation, Sprite sprite, bool flip, Color color, float duration)
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
            transform.rotation = rotation;
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

