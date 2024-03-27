using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class AfterImagesHandler : MonoBehaviour
    {
        public SpriteRenderer targetSprite;
        public float rateOverTime = 20;
        public Color targetColor = new Color(0, 1f, 1f, 0.5f);
        public float m_time = 0;
        private void Update()
        {
            CheckAfterImages();
        }
        private void CheckAfterImages()
        {
            if (!targetSprite) return;
            if (!targetSprite.gameObject.activeInHierarchy) return;
            m_time -= Time.deltaTime;
            if (m_time <= 0)
            {
                {
                    var pos = targetSprite.transform.position;
                    var rot = targetSprite.transform.rotation;
                    var sprite = targetSprite.sprite;
                    bool flip = targetSprite.flipX;
                    Color color = targetColor;
                    float duration = 1f;
                    AfterImagesPool.instance.CallAfterImage(pos, rot, sprite, flip, color, duration);
                }
                float rate = (rateOverTime == 0 ? 1f : rateOverTime);
                m_time = 1f / rate;
            }
        }
        private void CallAfterImages()
        {

        }
    }
}

