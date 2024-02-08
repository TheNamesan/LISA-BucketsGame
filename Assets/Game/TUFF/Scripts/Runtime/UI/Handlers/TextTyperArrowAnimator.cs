using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    public class TextTyperArrowAnimator : MonoBehaviour
    {
        
        public float animationInterval = 0.26f;
        public Sprite[] sprites = new Sprite[0];
        private float m_frameTimer = 0;
        private Image image;
        private int m_frame = -1;
        void Awake()
        {
            image = GetComponent<Image>();
        }

        void Update()
        {
            m_frameTimer -= Time.deltaTime;
            if (m_frameTimer <= 0)
            {
                m_frameTimer = animationInterval;
                m_frame++;
                if (m_frame >= sprites.Length)
                {
                    m_frame = 0;
                }
                if (sprites.Length > 0)
                {
                    if (image != null) image.sprite = sprites[m_frame];
                }
            }
        }
    }
}
