using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Flyer : Enemy
    {
        [Header("Flyer Properties")]
        public Sprite bulletSprite;
        public float roamSpeed = 3f;
        public float approachDistance = 10f;
        public float deadGravity = 2f;

        [Header("Fire")]
        public int fireAnimDuration = 35;
        public int fireTick = 15;
        public int fireRate = 50;
        public bool firing { get => m_firing; }
        [SerializeField] private bool m_firing = false;
        [SerializeField] private int m_fireAnimTicks = 0;
        [SerializeField] private int m_fireCooldownTicks = 0;

        private void Update()
        {
            if (sprite)
            {
                if (m_dead)
                {
                    sprite.color = Color.red;
                    rb.constraints = RigidbodyConstraints2D.None;
                }
                else
                {
                    //if (m_attacking)
                    //{
                    //    if (m_attackingTicks == attackTick) sprite.color = new Color(255, 127, 0, 255);
                    //    else sprite.color = Color.green;
                    //}
                    //else sprite.color = Color.white;
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
                sprite.flipX = facing == Facing.Left;
            }
        }
        private void FixedUpdate()
        {
            if (!m_dead) rb.sharedMaterial = BucketsGameManager.instance.aliveMat;
            else rb.sharedMaterial = BucketsGameManager.instance.deadMat;
            if (m_dead) rb.gravityScale = deadGravity;
            else rb.gravityScale = gravityScale;
            GroundCheck();
            WallCheck();
            CheckPlayerDistance();
            MoveHandler();
            TimerHandler();
        }
        private void CheckPlayerDistance()
        {
            if (m_dead) return;
            
            var player = SceneProperties.mainPlayer;
            if (player == null) return;
            float distanceToPlayer = Vector2.Distance(player.rb.position, rb.position);//(player.rb.position - rb.position).sqrMagnitude;
            if (!player.dead)
            {
                if (enemyState != EnemyAIState.Alert)
                {
                    EnemyLineOfSight();
                }
            }
            else enemyState = EnemyAIState.Roaming;

            if (enemyState != EnemyAIState.Alert) return;
            if (Mathf.Abs(distanceToPlayer) <= approachDistance)
            {
                RaycastHit2D hasWallInWay = Physics2D.Linecast(rb.position, player.rb.position, groundLayers);
                Debug.DrawLine(rb.position, player.rb.position, (hasWallInWay ? Color.red : Color.green), Time.fixedDeltaTime);
                if (!hasWallInWay) Fire();
            }
            //if (m_attacking) // Run Tick
            //{
            //    m_attackingTicks--;
            //    if (m_attackingTicks <= 0) m_attacking = false;
            //    else if (m_attackingTicks == attackTick) // Attack Raycast
            //    {
            //        var hitboxLayers = GameManager.instance.hurtboxLayers;
            //        Vector2 dir = transform.right * FaceToInt();
            //        RaycastHit2D[] hits = Physics2D.BoxCastAll(rb.position, new Vector2(1f, 1f), 0f, dir, 0.25f, hitboxLayers);
            //        for (int i = 0; i < hits.Length; i++)
            //        {
            //            if (hits[i].collider.TryGetComponent(out Hurtbox hurtbox))
            //            {
            //                if (hurtbox.team == Team.Player && !hurtbox.invulnerable)
            //                {
            //                    bool hitTarget = hurtbox.Collision(dir);
            //                }
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    if (Mathf.Abs(distanceToPlayer) <= 0.65f) // Attack
            //    {
            //        m_attacking = true;
            //        m_attackingTicks = attackingAnimTicks;
            //    }
            //}
        }

        private void MoveHandler()
        {
            if (m_dead) return;
            
            
            var player = SceneProperties.mainPlayer;
            if (player != null)
            {
                float distanceToPlayer = player.rb.position.x - rb.position.x;
                int moveH = 0;
                float speed = 0f;
                if (enemyState == EnemyAIState.Alert)
                {
                    speed = moveSpeed;
                    //if (Mathf.Abs(distanceToPlayer) <= approachDistance)
                    //    speed = approachSpeed;
                    moveH = (int)Mathf.Sign(distanceToPlayer);
                    //if ((moveH > 0 && !normalRight) || (moveH < 0 && !normalLeft))
                    //    moveH = 0;
                }
                if (enemyState == EnemyAIState.Roaming)
                {
                    speed = roamSpeed;
                    moveH = FaceToInt();
                    // Fall Check
                    //if ((moveH > 0 && !normalRight) || (moveH < 0 && !normalLeft))
                    //    moveH *= -1;
                    // Wall Check
                    if ((moveH > 0 && IsVerticalWall(wallRightHit)) ||
                        (moveH < 0 && IsVerticalWall(wallLeftHit)))
                    {
                        moveH *= -1;
                    }
                }
                float velX = moveH * speed;
                Vector2 velocity = new Vector2(velX, 0);
                //Vector2 normal = groundNormal;
                //velocity = GetSlopeVelocity(moveH, velX, velocity, normal);

                //if (m_firing)
                //{
                //    velocity = Vector2.zero;
                //}
                //else ChangeFacingOnMove(moveH);
                ChangeFacingOnMove(moveH);
                float expectedVelocity = rb.velocity.x + (velocity.x * Time.fixedDeltaTime * 2f);
                if (Mathf.Abs(expectedVelocity) <= Mathf.Abs(velocity.x))
                    rb.AddForce(velocity, ForceMode2D.Force);
                //else Debug.Log(expectedVelocity + " > " + velocity.x);
                Debug.DrawRay(rb.position, rb.velocity, Color.white, Time.fixedDeltaTime);
                //rb.velocity = velocity;
            }
            
            CapVelocity();
        }
        private void Fire()
        {
            if (m_firing || m_fireCooldownTicks > 0) return;
            m_fireAnimTicks = fireAnimDuration;
            m_firing = true;
        }
        private void StopFire(bool useCooldown = false)
        {
            if (useCooldown) m_fireCooldownTicks = fireRate;
            m_fireAnimTicks = 0;
            m_firing = false;
        }
        private void ShootProjectile()
        {
            var player = SceneProperties.mainPlayer;
            if (!player) return;
            Vector2 dir = player.rb.position - rb.position;
            Vector2 size = Vector2.one;
            // If player is behind enemy, rotate direction in X
            //if (Mathf.Sign(dir.normalized.x) != FaceToInt()) dir.x *= -1;
            BulletsPool.instance.SpawnBullet(rb.position, bulletSprite, size, dir, Team.Enemy);
        }
        private void FireTimer()
        {
            if (m_fireCooldownTicks > 0)
            {
                m_fireCooldownTicks--;
            }
            if (m_firing && m_fireAnimTicks > 0)
            {
                m_fireAnimTicks--;
                if (m_fireAnimTicks == fireTick) // Shoot Projectile
                {
                    ShootProjectile();
                }
                if (m_fireAnimTicks <= 0) StopFire(true);
            }
        }
        private void TimerHandler()
        {
            if (m_dead) return;
            FireTimer();
        }
        public override bool Hurt(Vector2 launch)
        {
            if (m_dead) return false;
            m_dead = true;
            StopFire();
            SetAirborne();
            launch *= 40f;
            rb.velocity = (launch);
            BucketsGameManager.instance.OnEnemyKill();
            return true;
        }
    }
}

