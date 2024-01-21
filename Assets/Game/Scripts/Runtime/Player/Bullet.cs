using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public enum Team
    {
        Player = 0,
        Enemy = 1
    }
    public class Bullet : MonoBehaviour
    {
        public Rigidbody2D rb;
        public float velocity = 27;
        public LayerMask groundLayers;
        public Team team = Team.Player;
        private const int m_maxTicksLife = 250;
        private int m_ticks = 0;
        public void Fire(Vector2 normal, Team team = Team.Player)
        {
            this.team = team;
            transform.localRotation = Quaternion.FromToRotation(Vector2.right, normal);
            m_ticks = 0; // Reset Ticks
        }
        private void FixedUpdate()
        {
            Movement();
            CollisionCheck();
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
            
        }

        private void CollisionCheck()
        {
            RaycastHit2D hitGround = Physics2D.CircleCast(rb.position, transform.localScale.x, rb.transform.up, 0, groundLayers);
            if (hitGround)
            {
                ReturnToPool();
            }
            var hitboxLayers = GameManager.instance.hurtboxLayers;
            RaycastHit2D hit = Physics2D.CircleCast(rb.position, transform.localScale.x, rb.transform.up, 0, hitboxLayers);
            if (hit)
            {
                if (hit.collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (hurtbox.team != team)
                    {
                        hurtbox.Collision(rb.velocity.normalized);
                        ReturnToPool();
                    }
                }
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