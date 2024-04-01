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

        public void Spawn(Vector2 force, Sprite sprite)
        {
            m_inUse = true;
            gameObject.SetActive(true);
            spriteRenderer.sprite = sprite;
            Vector2 pos = col.ClosestPoint(rb.position + new Vector2(99f, 99f));
            rb.AddForceAtPosition(force, pos, ForceMode2D.Impulse);
            //rb.velocity = (force);
        }
        private void FixedUpdate()
        {
            rb.velocity *= 0.99f;
        }
    }
}
