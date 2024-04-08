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
        private Material m_material;

        private void Awake()
        {
            CheckAddedMaterial();
        }

        private void CheckAddedMaterial()
        {
            if (sprite && m_material == null)
            {
                m_material = Instantiate(sprite.material);
                sprite.material = m_material;
            }
        }
        public void Invoke(Vector3 position, Quaternion rotation, Sprite sprite, bool flip, Color color, float duration, bool asAddedColor)
        {
            CheckAddedMaterial();
            gameObject.SetActive(true);
            GameUtility.KillTween(ref tween);
            if (this.sprite)
            {
                this.sprite.sprite = sprite;
                this.sprite.flipX = flip;
                if (asAddedColor)
                {
                    this.sprite.color = Color.white;
                    m_material?.SetColor("_AddedColorA", color);
                }
                else
                { 
                    this.sprite.color = color;
                    m_material?.SetColor("_AddedColorA", Color.black);
                }
            }
            transform.position = position;
            transform.rotation = rotation;
            m_inUse = true;
            tween = this.sprite.DOFade(0f, duration).From(color).SetEase(Ease.Linear)
                .OnComplete(OnComplete);
        }
        public void Invoke(Vector3 position, Quaternion rotation, Sprite sprite, bool flip, Color color, float duration)
        {
            Invoke(position, rotation, sprite, flip, color, duration, false);   
        }
        private void OnComplete()
        {
            m_inUse = false;
            gameObject.SetActive(false);
        }
    }
}

