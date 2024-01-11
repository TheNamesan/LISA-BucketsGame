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
            Debug.Log(SceneProperties.cam.WorldToScreenPoint(rb.position));
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
        }
        private void ReturnToPool()
        {
            Destroy(gameObject); // Return to pool
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != 7) 
             ReturnToPool();
        }

    }
}