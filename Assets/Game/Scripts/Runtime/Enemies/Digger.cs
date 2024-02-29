using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{

    public class Digger : Enemy
    {
        public bool attacking { get => m_attacking; }
        [Header("Digger Properties")]
        public SpriteRenderer burrowSprite;
        public float jumpForce = 6;
        public float decelerationRate = 3f;
        public float roamSpeed = 4f;
        [SerializeField] private bool m_attacking = false;
        public int attackingAnimTicks = 30;
        public int attackTick = 10;
        [SerializeField] private int m_attackingTicks = 0;
        public bool buried { get => m_buried; }
        [SerializeField] private bool m_buried = false;

        public int buriedDuration = 50;
        [SerializeField] private int m_buriedTicks = 0;
        public int buryCooldown = 50;
        [SerializeField] private int m_buryCooldownTicks = 0;
        public int jumpPrepTime = 10;
        [SerializeField] private int m_jumpPrepTicks = 0;

        private void Start()
        {
            AddAsRoomEnemy();
        }
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
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    sprite.enabled = !m_buried;
                    if (burrowSprite)
                    {
                        burrowSprite.color = (m_jumpPrepTicks > 0 ? Color.red : Color.white);
                        burrowSprite.enabled = m_buried;
                        burrowSprite.transform.localRotation = 
                            Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, usedNormal));
                    }
                }
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
        }
        private void CheckPlayerDistance()
        {
            if (m_dead) return;
            var player = SceneProperties.mainPlayer;
            if (player == null) return;
            float distanceToPlayer = (player.rb.position - rb.position).sqrMagnitude;
            if (!player.dead)
            {
                if (enemyState != EnemyAIState.Alert)
                {
                    EnemyLineOfSight();
                }
            }
            else enemyState = EnemyAIState.Roaming;

            if (enemyState != EnemyAIState.Alert) return;

            if (m_buried)
            {
                //float distanceToPlayerX = player.rb.position.x - rb.position.x;
                // If locking on to player
                if (m_jumpPrepTicks <= 0)
                {
                    var layer = 1 << BucketsGameManager.instance.playerLayer;
                    RaycastHit2D collisionWithPlayer = Physics2D.BoxCast(rb.position, col.size, 0f, Vector2.up, 0f, layer);
                    if (collisionWithPlayer)
                    {
                        m_buriedTicks++;
                        if (m_buriedTicks >= buriedDuration)
                        {
                            PrepareJump();
                            //burrowSprite.enabled = true;
                            
                        }

                    }
                    else m_buriedTicks = 0;
                }
                else // Locked on
                {
                    m_jumpPrepTicks--;
                    if (m_jumpPrepTicks <= 0)
                    {
                        Jump();
                        AttackRaycast();
                    }
                }
            }
            else
            {
                if (m_buryCooldownTicks <= 0)
                {
                    Bury();
                }
            }
        }
        private void TimerHandler()
        {
            if (m_buryCooldownTicks > 0)
                m_buryCooldownTicks--;
        }
        private void Bury()
        {
            m_buried = true;
            m_buriedTicks = 0;
        }
        private void PrepareJump()
        {
            m_jumpPrepTicks = jumpPrepTime;
        }

        private void Jump()
        {
            m_buried = false;
            m_buryCooldownTicks = buryCooldown;
            rb.velocity = new Vector2(0, jumpForce);
            SetAirborne();
        }

        private void AttackRaycast()
        {
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            Vector2 dir = transform.right * FaceToInt();
            Vector2 size = col.size * new Vector2(1.5f, 1.3f);
            RaycastHit2D[] hits = Physics2D.BoxCastAll(rb.position, size, 0f, dir, 0f, hitboxLayers);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (hurtbox.team == Team.Player && !hurtbox.invulnerable)
                    {
                        bool hitTarget = hurtbox.Collision(dir);
                    }
                }
            }
        }
        private void MoveHandler()
        {
            if (m_dead) return;
            if (grounded)
            {
                var player = SceneProperties.mainPlayer;
                if (player != null)
                {
                    float distanceToPlayerX = player.rb.position.x - rb.position.x;
                    int moveH = 0;
                    float speed = 0f;
                    if (enemyState == EnemyAIState.Alert)
                    {
                        moveH = (int)Mathf.Sign(distanceToPlayerX);
                        if (m_buried)
                        {
                            speed = moveSpeed;

                            if ((moveH > 0 && !normalRight) || (moveH < 0 && !normalLeft))
                            {
                                rb.velocity = Vector2.zero;
                                moveH = 0;
                            }
                            
                            if (Mathf.Abs(distanceToPlayerX) <= 0.25f) speed = 0;
                            CheckDoorOpening(moveH);
                        }
                        else rb.velocity = Vector2.zero;
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
                    if (m_attacking)
                    {
                        velocity = Vector2.zero;
                    }
                    else ChangeFacingOnMove(moveH);
                    
                    if (enemyState == EnemyAIState.Alert)
                    {
                        // If velocity.x normal is going in the wrong direction of the player, decelerate.
                        if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(distanceToPlayerX)) velocity *= decelerationRate;
                        rb.velocity += velocity * Time.fixedDeltaTime;
                        rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * Mathf.Min(Mathf.Abs(rb.velocity.x), moveSpeed), rb.velocity.y);
                        if (m_jumpPrepTicks > 0) rb.velocity = Vector2.zero;
                        //var t = Time.fixedDeltaTime * Mathf.Abs(distanceToPlayerX) * 10f;
                        //var minVelocity = velocity * 0.6f;
                        //rb.velocity = Vector2.Lerp(minVelocity, velocity, t);
                    }
                    else rb.velocity = velocity;
                }
            }
            CapVelocity();
        }
        public override bool Hurt(Vector2 launch)
        {
            if (m_dead) return false;
            if (m_buried) return false;
            hp--;
            AlertEnemy();
            if (hp > 0) { HurtTween(); BucketsGameManager.instance.OnEnemyHit(); return true; }
            return Kill(launch);
        }
        public void OnDrawGizmos()
        {
            DrawLineOfSightGizmos();
        }
    }
}
