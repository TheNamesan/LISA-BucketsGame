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
            if (!m_dead) rb.sharedMaterial = GameManager.instance.aliveMat;
            else rb.sharedMaterial = GameManager.instance.deadMat;
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
    }
}
