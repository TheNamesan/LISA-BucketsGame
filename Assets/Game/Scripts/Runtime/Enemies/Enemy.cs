using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public enum EnemyAIState
    {
        Roaming = 0,
        Alert = 1
    }
    public class Enemy : MovingEntity
    {
        public EnemyAIState enemyState = EnemyAIState.Roaming;
        [Header("Line Of Sight")]
        public float coneAngle = 45f;
        public float coneAngleOffset = 0f;
        public float coneDistance = 8.5f;
        public int coneAccuracy = 12;
        
        private void Update()
        {
            if (sprite)
            {
                if (m_dead)
                {
                    sprite.color = Color.red;
                    rb.constraints = RigidbodyConstraints2D.None;
                }
                else rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }    
        }
        private void FixedUpdate()
        {
            if (!m_dead) rb.sharedMaterial = BucketsGameManager.instance.aliveMat;
            else rb.sharedMaterial = BucketsGameManager.instance.deadMat;
            GroundCheck();
            MoveHandler();
        }
        private void MoveHandler()
        {
            if (m_dead) return;
            if (grounded)
            {
                if (SceneProperties.mainPlayer != null)
                {
                    var player = SceneProperties.mainPlayer;
                    float distanceToPlayer = player.rb.position.x - rb.position.x;
                    int moveH = 0;
                    
                    if (Mathf.Abs(distanceToPlayer) < 4f) enemyState = EnemyAIState.Alert;
                    if (enemyState == EnemyAIState.Alert) moveH = (int)Mathf.Sign(distanceToPlayer);
                    float velX = moveH * moveSpeed;
                    Vector2 velocity = new Vector2(velX, 0);
                    Vector2 normal = groundNormal;
                    velocity = GetSlopeVelocity(moveH, velX, velocity, normal);
                    rb.velocity = velocity;
                    ChangeFacingOnMove(moveH);
                }
            }
            CapVelocity();
        }
        public override bool Hurt(Vector2 launch)
        {
            if (m_dead) return false;
            m_dead = true;
            SetAirborne();
            launch *= 40f;
            rb.velocity = (launch);
            BucketsGameManager.instance.OnEnemyKill();
            return true;
        }
        protected virtual void EnemyLineOfSight()
        {
            LayerMask layers = BucketsGameManager.instance.groundLayers | (1 << BucketsGameManager.instance.playerLayer);

            int max = coneAccuracy + 1;
            for (int i = 0; i < max; i++)
            {
                float a = -coneAngle * 0.5f;
                float angle = -coneAngleOffset + Mathf.Lerp(-a, a, Mathf.InverseLerp(0, max, i));
                float rad = Mathf.Deg2Rad * angle;
                Vector2 normal = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                Vector2 dir = (normal * FaceToInt());
                float distance = coneDistance;
                RaycastHit2D los = Physics2D.Raycast(rb.position, dir, distance, layers);
                Color color = Color.white;
                if (los)
                {
                    if (los.collider.gameObject.layer == BucketsGameManager.instance.playerLayer)
                    {
                        color = Color.green;
                        AlertEnemy();
                        Debug.DrawRay(rb.position, dir.normalized * distance, color, Time.fixedDeltaTime);
                        break;
                    }
                }
                Debug.DrawRay(rb.position, dir.normalized * distance, color, Time.fixedDeltaTime);
            }
        }
        protected virtual void AlertEnemy()
        {
            enemyState = EnemyAIState.Alert;
        }
    }
}
