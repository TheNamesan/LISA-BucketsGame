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
                    float moveH = 0;
                    
                    if (Mathf.Abs(distanceToPlayer) < 4f) enemyState = EnemyAIState.Alert;
                    if (enemyState == EnemyAIState.Alert) moveH = Mathf.Sign(distanceToPlayer);
                    float velX = moveH * moveSpeed;
                    Vector2 velocity = new Vector2(velX, 0);
                    Vector2 normal = groundNormal;
                    if (moveH > 0) normal = GetNormalFrom(normal, normalRight);
                    else if (moveH < 0) normal = GetNormalFrom(normal, normalLeft);
                    if (normal != Vector2.up) // On Slope
                    {
                        var perp = Vector2.Perpendicular(normal).normalized;
                        velocity = new Vector2(velX, velX) * -perp;
                    }
                    rb.velocity = velocity;
                }
            }
            CapVelocity();
        }
    }
}
