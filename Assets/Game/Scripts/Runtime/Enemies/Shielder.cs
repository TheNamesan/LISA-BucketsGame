using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Shielder : Enemy
    {
        
        [Header("Shielder Properties")]
        public float roamSpeed = 3f;
        public float approachSpeed = 4f;
        public float approachDistance = 9f;
        public bool attacking { get => m_attacking; }
        public bool vulnerable { get => m_attacking; }
        [SerializeField] private bool m_attacking = false;
        [SerializeField] private bool m_vulnerable = false;
        public int attackingAnimTicks = 30;
        public int attackTick = 10;
        [SerializeField] private int m_attackingTicks = 0;

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
                    if (m_attacking)
                    {
                        if (m_attackingTicks == attackTick) sprite.color = new Color(255, 127, 0, 255);
                        else sprite.color = Color.green;
                    }
                    else sprite.color = Color.white;
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
                sprite.flipX = facing == Facing.Left;
            }
        }
        private void FixedUpdate()
        {
            if (!m_dead) rb.sharedMaterial = GameManager.instance.aliveMat;
            else rb.sharedMaterial = GameManager.instance.deadMat;
            GroundCheck();
            WallCheck();
            CheckPlayerDistance();
            MoveHandler();
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
            if (grounded)
            {
                var player = SceneProperties.mainPlayer;
                if (player != null)
                {
                    float distanceToPlayer = player.rb.position.x - rb.position.x;
                    int moveH = 0;
                    float speed = 0f;
                    if (enemyState == EnemyAIState.Alert)
                    {
                        speed = moveSpeed;
                        if (Mathf.Abs(distanceToPlayer) <= approachDistance)
                            speed = approachSpeed;
                        moveH = (int)Mathf.Sign(distanceToPlayer);
                        if ((moveH > 0 && !normalRight) || (moveH < 0 && !normalLeft))
                            moveH = 0;
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
                    rb.velocity = velocity;
                }
            }
            CapVelocity();
        }
    }
}