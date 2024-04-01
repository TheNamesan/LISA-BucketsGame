using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUFF;

namespace BucketsGame
{
    public enum Team
    {
        Player = 0,
        Enemy = 1
    }
    
    public class Bullet : PoolObject
    {
        public Animator anim;
        public SFX hitSFX;
        public BulletType type;

        public Rigidbody2D rb;
        public CircleCollider2D col;
        public SpriteRenderer spriteRenderer;
        public float velocity = 27;
        public LayerMask groundLayers;
        public Team team = Team.Player;
        private const int m_maxTicksLife = 250;
        private int m_ticks = 0;
        private Vector2 m_lastPosition;

        public void Fire(Vector2 normal, float velocity, float radius, string animName, Vector2 spriteSize, SFX hitSFX, BulletType bulletType, Team team = Team.Player)
        {
            gameObject.SetActive(true);
            this.team = team;
            //spriteRenderer.sprite = sprite;
            type = bulletType;
            spriteRenderer.transform.localScale = new Vector3(spriteSize.x, spriteSize.y, 
                spriteRenderer.transform.localScale.z);
            this.velocity = velocity;
            col.radius = radius;
            this.hitSFX = hitSFX;
            PlayAnimation(animName);
            transform.localRotation = Quaternion.FromToRotation(Vector2.right, normal);
            m_ticks = 0; // Reset Ticks
            m_inUse = true;
            // Place last position a little behind
            m_lastPosition = (transform.position - (transform.right * velocity * Time.fixedDeltaTime));
        }
        
        private void OnEnable()
        {
            m_lastPosition = rb.position;
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
        private void PlayAnimation(string animName)
        {
            if (!anim) return;
            anim.Play(animName, -1, 0f);
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
            
            bool hitWall = false;
            if (hitGround)
            {
                hitWall = OnWallHit(hitGround, hitGround.point, hitGround.normal, false);
            }
            if (!hitWall)
            {
                RaycastHit2D hitOneWay = Physics2D.CircleCast(m_lastPosition, radius, distance, distance.magnitude, BucketsGameManager.instance.oneWayLayers);
                if (hitOneWay)
                {
                    hitWall = OnWallHit(hitOneWay, hitOneWay.point, hitOneWay.normal, true);
                }
            }
            if (hitWall) return;
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            RaycastHit2D[] hitAll = Physics2D.CircleCastAll(rb.position, radius, rb.transform.up, 0, hitboxLayers);
            for (int i = 0; i < hitAll.Length; i++)
            {
                if (hitAll[i].collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (hurtbox.team != team && !hurtbox.invulnerable)
                    {
                        // If target is not dead and hits, despawn bullet
                        bool hitTarget = hurtbox.Collision(rb.velocity.normalized);
                        if (hitTarget) {
                            if (type == BulletType.Firebomb) FirebombEffects(hitAll[i].point);
                            AudioManager.instance.PlaySFX(hitSFX); 
                            ReturnToPool(); 
                            return; }
                    }
                }
            }
        }

        private bool OnWallHit(RaycastHit2D hitGround, Vector2 point, Vector2 normal, bool isOneWay)
        {
            //Debug.Log(normal);
            // I'm flipping the normal to align with the sprite
            float rotation = Vector2.SignedAngle(Vector2.right, -normal);
            //Debug.Log(rotation);

            if (hitGround)
            {
                if (hitGround.collider.TryGetComponent(out TUFF.TerrainProperties props))
                {
                    if (props.playerBulletsGoThrough && team == Team.Player) return false;
                    if (props.enemyBulletsGoThrough && team == Team.Enemy) return false;
                    bool ignoreSFX = (type == BulletType.Magician); // LOL FIX;
                    props.WallHit(ignoreSFX);
                }
                else if (isOneWay) return false;
                if (hitGround.collider.TryGetComponent(out Door door))
                {
                    door.Open(rb.velocity.normalized.x, team == Team.Player);
                }
            }
            if (type == BulletType.Firebomb) FirebombEffects(point);
            else if (type == BulletType.Spear) VFXPool.instance.PlayVFX("SpearVFX", point, false, rb.rotation, "Platforms");
            else VFXPool.instance.PlayVFX("WallHitVFX", point, false, rotation);
            AudioManager.instance.PlaySFX(hitSFX);
            ReturnToPool();
            return true;
        }

        private void FirebombEffects(Vector2 point)
        {
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            float radius = 1.5f;
            RaycastHit2D[] hitAll = Physics2D.CircleCastAll(point + Vector2.up * radius * 0.5f, radius, rb.transform.up, 0, hitboxLayers);
            for (int i = 0; i < hitAll.Length; i++)
            {
                if (hitAll[i].collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (hurtbox.team != team && !hurtbox.invulnerable)
                    {
                        bool hitTarget = hurtbox.Collision(rb.velocity.normalized);
                    }
                }
            }
            VFXPool.instance.PlayVFX("ExplosionVFX", point, false);
            AudioManager.instance.PlaySFX(SFXList.instance.firebombHitSFX);
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