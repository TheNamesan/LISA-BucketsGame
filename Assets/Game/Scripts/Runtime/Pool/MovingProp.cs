using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class MovingProp : PoolObject
    {
        public SpriteRenderer spriteRenderer;
        public Rigidbody2D rb;
        public BoxCollider2D col;
        private float m_time = 0;
        private bool m_despawnOnTime = false;

        public void Spawn(Vector2 force, Sprite sprite)
        {
            Spawn(force, sprite, -1f, new Vector2(1.6f, 0.16f), 1f);
        }
        public void Spawn(Vector2 force, Sprite sprite, float activeTime, Vector2 size, float gravityScale)
        {
            m_inUse = true;
            gameObject.SetActive(true);
            spriteRenderer.sprite = sprite;
            m_time = activeTime;
            col.size = size;
            rb.gravityScale = gravityScale;
            if (m_time < 0) m_despawnOnTime = false;
            else m_despawnOnTime = true;
            Vector2 pos = col.ClosestPoint(rb.position + new Vector2(99f, 99f));
            rb.AddForceAtPosition(force, pos, ForceMode2D.Impulse);
            //rb.velocity = (force);
        }
        private void FixedUpdate()
        {
            rb.velocity *= 0.99f;
        }
        private void Update()
        {
            if (m_despawnOnTime)
            {
                m_time -= Time.deltaTime;
                if (m_time <= 0)
                {
                    ReturnToPool(); 
                }
            }
        }
    }
}
