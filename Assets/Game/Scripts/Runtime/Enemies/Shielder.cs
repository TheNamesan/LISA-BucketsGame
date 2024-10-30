using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Shielder : Enemy
    {

        [Header("Shielder Properties")]
        public ShielderAnimationHandler animationHandler;
        public float roamSpeed = 3f;
        public float approachSpeed = 2f;
        public float approachDistance = 10f;
        public Sprite spearSprite;
        public Sprite doorSprite;
        public bool attacking { get => m_attacking; }
        public bool vulnerable { get => m_vulnerable; }

        [Header("Attack")]
        [SerializeField] private bool m_attacking = false;
        [SerializeField] private bool m_vulnerable = false;
        public int attackingAnimTicks = 30;
        public int attackTick = 10;
        [SerializeField] private int m_attackingTicks = 0;

        [Header("Fire")]
        public float bulletVelocity = 30;
        public int fireAnimDuration = 35;
        public int fireTick = 15;
        public int fireRate = 25;
        public bool firing { get => m_firing; }
        [SerializeField] private bool m_firing = false;
        [SerializeField] private int m_fireAnimTicks = 0;
        [SerializeField] private int m_fireCooldownTicks = 0;

        [Header("Stun")]
        public float stunPushbackSpeed = 12f;
        public int stunnedDuration = 30;
        public int stunnedTicks { get => m_stunnedTicks; }
        public bool stunned { get => m_stunnedTicks > 0; }
        [SerializeField] private int m_stunnedTicks = 0;
        [SerializeField] private int m_stunDirection = 0;

        [Header("Pain Mode")]
        public float painApproachSpeed = 3.5f;
        public int painFireRate = 15;
        public float painStunPushbackSpeed = 4f;

        private void Start()
        {
            AddAsRoomEnemy();
        }
        private void Update()
        {
            if (sprite)
            {
                //if (m_dead)
                //{
                //    sprite.color = Color.red;
                //    rb.constraints = RigidbodyConstraints2D.None;
                //}
                //else
                //{
                //    if (m_vulnerable)
                //    {
                //        sprite.color = Color.blue;
                //    }
                //    else if (m_attacking)
                //    {
                //        if (m_attackingTicks == attackTick) sprite.color = new Color(255, 127, 0, 255);
                //        else sprite.color = Color.green;
                //    }
                //    else sprite.color = Color.white;
                //    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                //}
                sprite.flipX = facing == Facing.Left;
            }
        }
        private void FixedUpdate()
        {
            if (!m_dead) rb.sharedMaterial = BucketsGameManager.instance.aliveMat;
            else rb.sharedMaterial = BucketsGameManager.instance.deadMat;
            GroundCheck();
            WallCheck();
            CheckPlayerDistance();
            MoveHandler();
            TimerHandler();
            FallOffMapCheck();
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
            else AssignOriginalState();

            if (enemyState != EnemyAIState.Alert) return;
            if (m_attacking) // Run Tick
            {
                m_attackingTicks--;
                if (m_attackingTicks <= 0)
                {
                    StopFire(true);
                    m_attacking = false;

                }
                else if (m_attackingTicks == attackTick) // Attack Raycast
                {
                    AttackRaycast();
                }
            }
            else if (!m_firing && !stunned && Mathf.Abs(distanceToPlayer) <= 1f && !player.stunned) // Attack
            {
                Attack();
            }
            else if (Mathf.Abs(distanceToPlayer) <= approachDistance && m_stunnedTicks <= 0) // Prepare to shoot
            {
                bool wallInWay = false;
                RaycastHit2D[] hasWallInWayHitAll = Physics2D.LinecastAll(rb.position, player.rb.position, groundLayers);
                for (int i = 0; i < hasWallInWayHitAll.Length; i++)
                {
                    if (hasWallInWayHitAll[i].collider.TryGetComponent(out TUFF.TerrainProperties props))
                    {
                        if (props.enemyBulletsGoThrough) continue;
                    }
                    wallInWay = true;
                    break;
                }
                Debug.DrawLine(rb.position, player.rb.position, (wallInWay ? Color.red : Color.green), Time.fixedDeltaTime);
                if (!wallInWay) Fire();
            }
        }
        private void AttackRaycast()
        {
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            Vector2 dir = transform.right * FaceToInt();
            RaycastHit2D[] hits = Physics2D.BoxCastAll(rb.position, new Vector2(1f, 1f), 0f, dir, 0.35f, hitboxLayers);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (hurtbox.team == Team.Player)
                    {
                        if (hurtbox.callback is PlayerController player)
                        {
                            bool hitTarget = player.Stun(dir);
                            if (hitTarget) TUFF.AudioManager.instance.PlaySFX(SFXList.instance.shielderShoveHitSFX);
                        }
                    }
                }
            }
        }
        private void Attack()
        {
            m_attacking = true;
            m_attackingTicks = attackingAnimTicks;
            TUFF.AudioManager.instance.PlaySFX(SFXList.instance.shielderShoveSFX);
        }

        private void MoveHandler()
        {
            if (m_dead)
            {
                if (grounded) rb.velocity *= 0.95f;
                return;
            }
            if (grounded)
            {
                var player = SceneProperties.mainPlayer;
                if (player != null)
                {
                    float distanceToPlayerX = player.rb.position.x - rb.position.x;
                    float distanceToPlayer = Vector2.Distance(player.rb.position, rb.position);
                    int moveH = 0;
                    float speed = 0f;
                    if (enemyState == EnemyAIState.Alert)
                    {
                        speed = moveSpeed;
                        if (Mathf.Abs(distanceToPlayerX) <= approachDistance)
                        {
                            speed = (BucketsGameManager.IsPainMode() ? painApproachSpeed : approachSpeed);
                        }
                            
                        if (distanceToPlayer <= 0.3f) speed = 0;
                        moveH = (int)Mathf.Sign(distanceToPlayerX);
                        bool enterDoor = !m_attacking && !stunned && !m_firing;
                        if (CheckIfDoorIsFaster(player, distanceToPlayer, ref moveH, enterDoor))
                        {
                            speed = (BucketsGameManager.IsPainMode() ? painApproachSpeed : approachSpeed);
                        }
                            
                        else moveH = (int)Mathf.Sign(distanceToPlayerX);
                        if ((moveH > 0 && !normalRight) || (moveH < 0 && !normalLeft))
                            moveH = 0;
                        CheckDoorOpening(moveH);
                    }
                    if (enemyState == EnemyAIState.Roaming)
                    {
                        speed = roamSpeed;
                        moveH = FaceToInt();
                        // Fall Check
                        if ((moveH > 0 && !normalRight) || (moveH < 0 && !normalLeft))
                            moveH *= -1;
                        // Wall Check
                        if ((moveH > 0 && IsVerticalWall(wallRightHit)) ||
                            (moveH < 0 && IsVerticalWall(wallLeftHit)))
                        {
                            moveH *= -1;
                        }
                    }
                    float velX = moveH * speed;
                    Vector2 velocity = new Vector2(velX, 0);
                    Vector2 normal = groundNormal;
                    velocity = GetSlopeVelocity(moveH, velX, velocity, normal);

                    if (m_stunnedTicks > 0)
                    {
                        float pushBackSpeed = (BucketsGameManager.IsPainMode() ? painStunPushbackSpeed : stunPushbackSpeed);
                        // Plus 0.2 few frames where shielder is stopped.
                        float x = Mathf.Lerp(0, pushBackSpeed * m_stunDirection, ((float)m_stunnedTicks / stunnedDuration) * 2f);
                        if ((x > 0 && !normalRight) || (x < 0 && !normalLeft))
                            x = 0;
                        //Debug.Log(x);
                        velocity.Set(x, 0);
                    }
                    else if (m_attacking || m_firing)
                    {
                        velocity = Vector2.zero;
                    }
                    else ChangeFacingOnMove(moveH);
                    rb.velocity = velocity;
                }
            }
            CapVelocity();
        }
        private void Stun(Vector2 direction)
        {
            if (m_dead) return;
            m_stunnedTicks = stunnedDuration;

            m_stunDirection = (int)Mathf.Sign(direction.normalized.x);
            ChangeFacing((m_stunDirection > 0 ? Facing.Left : Facing.Right));
            StopFire();
            TUFF.AudioManager.instance.PlaySFX(SFXList.instance.shielderBlockSFX);
            if (animationHandler) animationHandler.ChangeAnimationState(true);
            if (!SceneProperties.mainPlayer.dead)
            {
                AlertEnemy();
            }
        }
        protected override void AlertEnemy()
        {
            if (enemyState != EnemyAIState.Alert) StopFire(true);
            base.AlertEnemy();
        }
        private void Fire()
        {
            if (m_firing || m_fireCooldownTicks > 0 || !OnScreen) return;
            m_fireAnimTicks = fireAnimDuration;
            m_firing = true;
        }
        private void StopFire(bool useCooldown = false)
        {
            if (useCooldown) m_fireCooldownTicks = (BucketsGameManager.IsPainMode() ? painFireRate : fireRate);
            m_vulnerable = false;
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
            float faceAngle = Vector2.SignedAngle(Vector2.right, dir.normalized);
            //Debug.Log("Angle: " + faceAngle);
            if (Mathf.Sign(dir.normalized.x) != FaceToInt())
            {
                // If the Y value of the vector is too low, it is a blind spot.
                if (Mathf.Abs(dir.normalized.y) <= 0.3f)
                    dir.x *= -1;
            }
            Vector2 position = rb.position + dir.normalized;
            BulletsPool.instance.SpawnBullet(position, dir, BulletType.Spear, Team.Enemy);
            TUFF.AudioManager.instance.PlaySFX(SFXList.instance.shielderShootSFX);
        }
        private void StunTimer()
        {
            if (m_stunnedTicks > 0) m_stunnedTicks--;
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
                    m_vulnerable = true;
                }
                if (m_fireAnimTicks <= 0)
                {
                    StopFire(true);
                    FacePlayer();

                }
            }
        }

        private void FacePlayer()
        {
            if (SceneProperties.mainPlayer)
            {
                float distanceToPlayerX = SceneProperties.mainPlayer.rb.position.x - rb.position.x;
                ChangeFacingOnMove((int)Mathf.Sign(distanceToPlayerX));
            }
        }

        private void TimerHandler()
        {
            if (m_dead) return;
            StunTimer();
            FireTimer();
        }
        public override bool Hurt(Vector2 launch)
        {
            if (m_dead) return false;
            if (!m_vulnerable)
            {
                Stun(launch.normalized);
                return true;
            }
            hp--;
            AlertEnemy();
            HurtTween();
            if (hp > 0) { BucketsGameManager.instance.OnEnemyHit(); return true; }
            return Kill(launch);
        }
        public override bool Kill(Vector2 launch)
        {
            if (m_dead) return false;
            m_dead = true;
            StopFire();
            SetAirborne();
            launch *= 40f;
            rb.velocity = (launch);
            BucketsGameManager.instance.OnEnemyKill();
            Vector2 spearSpawnPos = rb.position + Vector2.right * 0.5f;
            Vector2 doorSpawnPos = rb.position - Vector2.right * 0.5f;
            RaycastHit2D spearAdjust = Physics2D.Raycast(rb.position, Vector2.right, 0.5f, BucketsGameManager.instance.groundLayers);
            RaycastHit2D doorAdjust = Physics2D.Raycast(rb.position, Vector2.left, 0.5f, BucketsGameManager.instance.groundLayers);
            if (spearAdjust) spearSpawnPos = spearAdjust.point + new Vector2(-0.1f, 0);
            if (doorAdjust) doorSpawnPos = doorAdjust.point + new Vector2(0.1f, 0);
            MovingPropPool.instance.SpawnProp(spearSpawnPos, 0f, launch * 1.5f, spearSprite);
            MovingPropPool.instance.SpawnProp(doorSpawnPos, 0f, launch * 0.5f, doorSprite);
            return true;
        }
        public void OnDrawGizmos()
        {
            DrawLineOfSightGizmos();
        }
    }
}
