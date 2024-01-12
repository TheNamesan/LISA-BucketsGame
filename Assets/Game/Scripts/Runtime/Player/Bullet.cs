using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Bullet : MonoBehaviour
    {
        public Rigidbody2D rb;
        public float velocity = 27;
        private const int m_maxTicksLife = 250;
        private int m_ticks = 0;
        public void Fire(Vector2 normal)
        {
            transform.localRotation = Quaternion.FromToRotation(Vector2.right, normal);
            m_ticks = 0; // Reset Ticks
        }
        private void FixedUpdate()
        {
            Movement();
            Ticks();
        }

        private void Ticks()
        {
            m_ticks++;
            if (m_ticks >= m_maxTicksLife) ReturnToPool();
        }

        private void Movement()
        {
            if (!rb) return;
            var vel = transform.right * velocity;
            rb.velocity = vel;

            RaycastHit2D hit = Physics2D.CircleCast(rb.position, transform.localScale.x, rb.transform.up, 0, (1 << 6));
            if (hit)
            {
                
                ReturnToPool();
            } 
        }
        private void ReturnToPool()
        {
            Destroy(gameObject); // Return to pool
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            //if (other.gameObject.layer != 7) 
            // ReturnToPool();
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            //if (collision.gameObject.layer != 7)
            //    ReturnToPool();
        }
    }
}