using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Magician : Enemy
    {
        [Header("Magician Properties")]
        public Vector2 teleportPositionPadding = new Vector2();
        public Transform sineMove;
        public Vector2 roomBoundsMin { get => SceneProperties.instance.TUFFSceneProperties.min + teleportPositionPadding; }
        public Vector2 roomBoundsMax { get => SceneProperties.instance.TUFFSceneProperties.max - teleportPositionPadding; }
        public Vector2 normalizedRoomPosition { get 
            {
                float normX = Mathf.InverseLerp(roomBoundsMin.x, roomBoundsMax.x, rb.position.x);
                float normY = Mathf.InverseLerp(roomBoundsMin.y, roomBoundsMax.y, rb.position.x);
                return new Vector2(normX, normY);
            }
        }

        [Header("Dash")]
        public float dashSpeed = 25;
        public int dashTicksDuration = 20;
        [SerializeField] private int m_dashTicks = 0;
        
        [SerializeField] private int m_dashDirection = 0;
        public bool dashing { get => m_dashTicks > 0; }
        public int dashDirection { get => m_dashDirection; }

        [Header("HP")]
        public int dashes = 7;
        [SerializeField] private int m_dashesLeft = 0;

        public PlayerController player { get => SceneProperties.mainPlayer; }
        private void Awake()
        {
            ResetDashes();
        }

        private void ResetDashes()
        {
            m_dashesLeft = dashes;
        }

        private void Start()
        {
            //AddAsRoomEnemy();
        }
        private void FixedUpdate()
        {
            if (!m_dead) rb.sharedMaterial = BucketsGameManager.instance.aliveMat;
            else rb.sharedMaterial = BucketsGameManager.instance.deadMat;

            GroundCheck();
            WallCheck();
            MoveHandler();
            TimerHandler();
        }
        private float GetVelX(int moveH)
        {
            if (m_dead) return rb.velocity.x;
            float speed = moveSpeed;
            float velocity = moveH * speed; // Walk Speed
            if (dashing) // Dash Speed
            {
                velocity = Mathf.Lerp(speed, dashSpeed, (float)m_dashTicks / dashTicksDuration) * m_dashDirection;
            }

            return velocity;
        }
        private void MoveHandler()
        {
            if (m_dead) return;
            var player = SceneProperties.mainPlayer;
            if (!player) return;
            float distanceToPlayer = player.rb.position.x - rb.position.x;
            if (!grounded)
            {
                // Floating anim
                float amplitude = 0.5f;
                float value = Mathf.Sin(Time.time * 2f) * amplitude;
                sineMove.transform.localPosition = new Vector2(sineMove.transform.localPosition.x, value);

                int moveH = 0;
                float speed = moveSpeed;
                moveH = (int)Mathf.Sign(distanceToPlayer);

                float velX = GetVelX(moveH);
                Vector2 velocity = new Vector2(velX, 0);
                ChangeFacingOnMove(moveH);

                if (dashing)
                {
                    rb.velocity = velocity;
                }
                else
                {
                    float expectedVelocity = rb.velocity.x + (velocity.x * Time.fixedDeltaTime * 2f);
                    if (Mathf.Abs(expectedVelocity) <= Mathf.Abs(velocity.x))
                        rb.AddForce(velocity, ForceMode2D.Force);
                }
            }
        }
        private void DoRandomTeleport()
        {
            if (!SceneProperties.instance) return;
            if (!SceneProperties.instance.TUFFSceneProperties) return;
            
            float randomX = Random.Range(roomBoundsMin.x, roomBoundsMax.x);
            float randomY = Random.Range(roomBoundsMin.y, roomBoundsMax.y);
            var newPosition = new Vector2(randomX, randomY);
            rb.position = newPosition;
        }
        private void Dash()
        {
            m_dashTicks = dashTicksDuration;
            m_dashDirection = (normalizedRoomPosition.x >= 0.5f ? -1 : 1);//FaceToInt();
            m_dashesLeft--;
            if (m_dashesLeft < 0) m_dashesLeft = 0;
            TUFF.AudioManager.instance.PlaySFX(SFXList.instance.dashSFX);
        }
        private void StopDash()
        {
            m_dashTicks = 0;
            m_dashDirection = 0;
        }
        private void TimerHandler()
        {
            DashTimer();
        }
        private void DashTimer()
        {
            if (!dashing) return;
            m_dashTicks--;
            if (m_dashTicks < 0) StopDash();
        }
        public override bool Hurt(Vector2 launch)
        {
            if (m_dead) return false;
            //DoRandomTeleport();
            if (!dashing)
            { 
                if (m_dashesLeft > 0) Dash();
                else
                {
                    hp--;
                    if (hp > 0) { HurtTween(); BucketsGameManager.instance.OnEnemyHit();
                        ResetDashes(); return true; }
                    else return Kill(launch);
                }
            }
            return false;
            //hp--;
            //AlertEnemy();
            //if (hp > 0) { BucketsGameManager.instance.OnEnemyHit(); return true; }
            //return Kill(launch);
        }
    }
}
