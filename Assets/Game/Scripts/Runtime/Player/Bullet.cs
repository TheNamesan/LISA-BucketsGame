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
    public class Bullet : PoolObject
    {
        public Sprite defaultSprite;
        public Vector2 defaultSpriteSize = new Vector2(0.3f, 0.3f);
        
        public Rigidbody2D rb;
        public CircleCollider2D col;
        public SpriteRenderer spriteRenderer;
        public float velocity = 27;
        public LayerMask groundLayers;
        public Team team = Team.Player;
        private const int m_maxTicksLife = 250;
        private int m_ticks = 0;

        public void Fire(Vector2 normal, Team team = Team.Player)
        {
            Fire(normal, defaultSprite, defaultSpriteSize, team);
        }
        public void Fire(Vector2 normal, Sprite sprite, Vector2 size, Team team = Team.Player)
        {
            gameObject.SetActive(true);
            spriteRenderer.sprite = sprite;
            spriteRenderer.transform.localScale = new Vector3(size.x, size.y, spriteRenderer.transform.localScale.z);
            this.team = team;
            transform.localRotation = Quaternion.FromToRotation(Vector2.right, normal);
            m_ticks = 0; // Reset Ticks
            m_inUse = true;
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
            float radius = col.radius * transform.localScale.x;
            RaycastHit2D hitGround = Physics2D.CircleCast(rb.position, radius, rb.transform.up, 0, groundLayers);
            if (hitGround)
            {
                ReturnToPool();
            }
            var hitboxLayers = GameManager.instance.hurtboxLayers; // Make the hitbox a seperate class
            RaycastHit2D hit = Physics2D.CircleCast(rb.position, radius, rb.transform.up, 0, hitboxLayers);
            if (hit)
            {
                if (hit.collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (hurtbox.team != team && !hurtbox.invulnerable)
                    {
                        bool hitTarget = hurtbox.Collision(rb.velocity.normalized);
                        if (hitTarget) ReturnToPool();
                    }
                }
            }
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