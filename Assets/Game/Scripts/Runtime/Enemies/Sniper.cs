using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Sniper : Enemy
    {
        [Header("Sniper Properties")]
        public LineRenderer line;
        public float roamSpeed = 0f;


        [Header("Fire")]
        public int fireRate = 50;
        public bool firing { get => m_firing; }
        public int chargeTime = 100;
        [SerializeField] private bool m_firing = false;
        [SerializeField] private bool m_fired = false;
        [SerializeField] private int m_fireCooldownTicks = 0;
        [SerializeField] private int m_chargeTimeTicks;
        [SerializeField] private Vector3[] m_linePoints = new Vector3[2];
        [SerializeField] private Gradient m_gradient = new();
        [SerializeField] private Vector2 m_crosshairPosition;
        private Vector2 m_shotOrigin { get => rb.position; }
        public RaycastHit2D hasWallInWay;
        public RaycastHit2D hitGround;
        public bool hitTarget;

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
            TimerHandler();
            CheckPlayerDistance();
            AlertLineOfSightCheck();
            MoveHandler();
            
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
        }

        private void AlertLineOfSightCheck()
        {
            Vector2 posA = new();
            Vector2 posB = new();
            Vector2 linePoint = new();
            float widthMultiplier = 0f;
            Color chargeColor = Color.white;
            float alpha = 0.5f;
            var player = SceneProperties.mainPlayer;

            if (player && !m_dead && !player.dead && enemyState == (EnemyAIState.Alert))
            {
                hasWallInWay = Physics2D.Linecast(rb.position, player.rb.position, groundLayers);
                //Debug.DrawLine(rb.position, player.rb.position, (hasWallInWay ? Color.red : Color.green), Time.fixedDeltaTime);
                if (!hasWallInWay)
                {
                    Fire();
                    if (m_firing || m_fired)
                    {
                        m_crosshairPosition = Vector2.Lerp(m_crosshairPosition, player.rb.position, Time.fixedDeltaTime * 5f);
                        posA = rb.position;
                        posB = m_crosshairPosition;
                        linePoint = posB;
                        Vector2 dir = posB - m_shotOrigin;
                        RaycastHit2D expectedHit = Physics2D.Raycast(m_shotOrigin, dir, Mathf.Infinity, groundLayers);
                        if (expectedHit) linePoint = expectedHit.point;
                        float time = (float)m_chargeTimeTicks / chargeTime;
                        
                        chargeColor = Color.Lerp(Color.red, Color.yellow, time);
                        if (m_chargeTimeTicks == 2 || m_chargeTimeTicks == 3 
                            || m_chargeTimeTicks == 6 || m_chargeTimeTicks == 7
                            || m_chargeTimeTicks == 10 || m_chargeTimeTicks == 11
                            || m_chargeTimeTicks == 14 || m_chargeTimeTicks == 15)
                            chargeColor = Color.yellow;
                        widthMultiplier = Mathf.SmoothStep(0.1f, 0.75f, time);
                        alpha = Mathf.SmoothStep(0.75f, 0.5f, time);
                        if (m_fired)
                        {
                            chargeColor = Color.white;
                            widthMultiplier = 0.5f;
                            alpha = 1;
                            UpdateLine(posA, linePoint, widthMultiplier, chargeColor, alpha);
                            AttackRaycast(posB);
                            if (m_fired)
                            {
                                if (!hitTarget && hitGround) linePoint = hitGround.point + (m_shotOrigin - posB).normalized;
                                m_fired = false;
                            }
                        }
                    }
                    //Debug.DrawLine(rb.position, player.rb.position, chargeColor, Time.fixedDeltaTime);
                }

            }
            if (hasWallInWay)
            {
                if (m_firing)
                {
                    m_chargeTimeTicks++;
                    if (m_chargeTimeTicks > chargeTime) StopFire();
                }
            }

            UpdateLine(posA, linePoint, widthMultiplier, chargeColor, alpha);
        }

        private void UpdateLine(Vector2 posA, Vector2 posB, float widthMultiplier, Color chargeColor, float alpha)
        {
            if (line)
            {
                m_linePoints[0] = posA;
                m_linePoints[1] = posB;
                line.widthMultiplier = widthMultiplier;
                line?.SetPositions(m_linePoints);

                m_gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(chargeColor, 0), new GradientColorKey(chargeColor, 1) },
                    new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0), new GradientAlphaKey(alpha, 1) }
                    );
                line.colorGradient = m_gradient;
            }
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
                    if (!hasWallInWay)
                        moveH = (int)Mathf.Sign(distanceToPlayer);
                    //if ((moveH > 0 && !normalRight) || (moveH < 0 && !normalLeft))
                    //    moveH = 0;
                }
                if (enemyState == EnemyAIState.Roaming)
                {
                    speed = roamSpeed;
                    //moveH = FaceToInt();
                    //// Fall Check
                    ////if ((moveH > 0 && !normalRight) || (moveH < 0 && !normalLeft))
                    ////    moveH *= -1;
                    //// Wall Check
                    //if ((moveH > 0 && IsVerticalWall(wallRightHit)) ||
                    //    (moveH < 0 && IsVerticalWall(wallLeftHit)))
                    //{
                    //    moveH *= -1;
                    //}
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
                //rb.velocity = velocity;
            }
            CapVelocity();
        }
        private void Fire()
        {
            if (m_firing || m_fireCooldownTicks > 0) return;
            m_chargeTimeTicks = chargeTime;
            m_firing = true;
            if (SceneProperties.mainPlayer)
                m_crosshairPosition = SceneProperties.mainPlayer.rb.position;
        }
        private void StopFire(bool useCooldown = false)
        {
            if (useCooldown) m_fireCooldownTicks = fireRate;
            m_chargeTimeTicks = 0;
            m_firing = false;
        }
        private void ShootProjectile()
        {
            var player = SceneProperties.mainPlayer;
            if (!player) return;
            m_fired = true;
            // If player is behind enemy, rotate direction in X
            //if (Mathf.Sign(dir.normalized.x) != FaceToInt()) dir.x *= -1;
            //BulletsPool.instance.SpawnBullet(rb.position, dir, Team.Enemy);
        }
        private void AttackRaycast(Vector2 playerPos)
        {
            var player = SceneProperties.mainPlayer;
            if (!player || player.dead) return;
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            var groundLayers = BucketsGameManager.instance.groundLayers;
            Vector2 dir = playerPos - rb.position;
            // RaycastAll so it doesn't hit its own hurtbox
            RaycastHit2D[] hitPlayer = Physics2D.RaycastAll(m_shotOrigin, dir, Mathf.Infinity, hitboxLayers);
            hitGround = Physics2D.Raycast(m_shotOrigin, dir, Mathf.Infinity, groundLayers);
            Debug.DrawRay(m_shotOrigin, dir, Color.magenta, Time.fixedDeltaTime);
            hitTarget = false;
            for (int i = 0; i < hitPlayer.Length; i++)
            {
                if (hitPlayer[i].collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (hurtbox.team == Team.Player && !hurtbox.invulnerable)
                    {
                        hitTarget = hurtbox.Collision(dir);
                    }
                }
            }
            if (!hitTarget && hitGround)
            {
                float rotation = Vector2.SignedAngle(Vector2.right, -hitGround.normal);
                VFXPool.instance.PlayVFX("WallHitVFX", hitGround.point, false, rotation);
                if (hitGround.collider.TryGetComponent(out TUFF.TerrainProperties props))
                {
                    props.WallHit();
                }
            }
        }
        private void FireTimer()
        {
            if (m_fireCooldownTicks > 0)
            {
                m_fireCooldownTicks--;
            }
            if (m_firing && m_chargeTimeTicks > 0 && !hasWallInWay)
            {
                m_chargeTimeTicks--;
                //if (m_chargeTimeTicks == fireTick) // Shoot Projectile
                //{
                //    ShootProjectile();
                //}
                if (m_chargeTimeTicks <= 0)
                { 
                    ShootProjectile();
                    StopFire(true);
                } 
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
            hp--;
            if (hp > 0) return true;
            m_dead = true;
            StopFire();
            SetAirborne();
            launch *= 40f;
            rb.velocity = (launch);
            BucketsGameManager.instance.OnEnemyKill();
            return true;
        }
        public void OnDrawGizmos()
        {
            DrawLineOfSightGizmos();
        }

        
    }
}
