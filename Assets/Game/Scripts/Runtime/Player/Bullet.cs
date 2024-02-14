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
        private Vector2 m_lastPosition;

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
            // Place last position a little behind
            m_lastPosition = (transform.position - (transform.right * velocity * Time.fixedDeltaTime));
        }
        private void FixedUpdate()
        {
            Movement();
            CollisionCheck();
            Ticks();
            m_lastPosition = rb.position;
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
            if (!isActiveAndEnabled) return;
            float radius = col.radius * transform.localScale.x * 0.75f;
            float rad = Mathf.Deg2Rad * transform.eulerAngles.z;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            Vector2 distance = rb.position - m_lastPosition;
            //RaycastHit2D hitGround = Physics2D.CircleCast(rb.position, radius, dir, 0, groundLayers);
            Debug.DrawLine(m_lastPosition, rb.position, Color.magenta, Time.fixedDeltaTime);
            RaycastHit2D hitGround = Physics2D.CircleCast(m_lastPosition, radius, distance, distance.magnitude, groundLayers);
            if (hitGround)
            {
                Vector2 normal = hitGround.normal;
                // Adjust hit normal
                //RaycastHit2D adjustHit = Physics2D.Linecast(m_lastPosition, rb.position, groundLayers);
                //if (adjustHit) { Debug.Log("Using adjust hit"); normal = adjustHit.normal; }
                
                OnWallHit(hitGround, hitGround.point, normal);
            }
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers; // Make the hitbox a seperate class
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

        private void OnWallHit(RaycastHit2D hitGround, Vector2 point, Vector2 normal)
        {
            //Debug.Log(normal);
            // I'm flipping the normal to align with the sprite
            float rotation = Vector2.SignedAngle(Vector2.right, -normal);
            //Debug.Log(rotation);
            VFXPool.instance.PlayVFX("WallHitVFX", point, false, rotation);
            if (hitGround)
            {
                if (hitGround.collider.TryGetComponent(out TUFF.TerrainProperties props))
                {
                    props.WallHit();
                }
                if (hitGround.collider.TryGetComponent(out Door door))
                {
                    door.Open(rb.velocity.normalized.x, team == Team.Player);
                }
            }
            ReturnToPool();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //if (other.gameObject.layer != 7) 
            // ReturnToPool();
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            //OnWallHit(collision);
        }
    }
}