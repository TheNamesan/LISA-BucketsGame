using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class BarrelBro : Enemy
    {
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
                    sprite.color = Color.white;
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
            AttackRaycast();
            //CheckPlayerDistance();
            MoveHandler();
        }

        private void MoveHandler()
        {
            if (m_dead) return;
            if (grounded)
            {
                var player = SceneProperties.mainPlayer;
                if (player != null)
                {
                    int moveH = FaceToInt();
                    float speed = moveSpeed;

                    if ((moveH > 0 && IsVerticalWall(wallRightHit)) ||
                        (moveH < 0 && IsVerticalWall(wallLeftHit)))
                    {
                        moveH *= -1;
                        TUFF.AudioManager.instance.PlaySFX(SFXList.instance.barrelBroWallHit);
                    }
                    float velX = moveH * speed;
                    Vector2 velocity = new Vector2(velX, 0);
                    Vector2 normal = groundNormal;
                    velocity = GetSlopeVelocity(moveH, velX, velocity, normal);
                    ChangeFacingOnMove(moveH);
                    rb.velocity = velocity;
                }
            }
            CapVelocity();
        }
        private void AttackRaycast()
        {
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            Vector2 dir = transform.right * FaceToInt();
            RaycastHit2D[] hits = Physics2D.BoxCastAll(rb.position, col.bounds.size, 0f, dir, 0f, hitboxLayers);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (hurtbox.team == Team.Player && !hurtbox.invulnerable)
                    {
                        bool hitTarget = hurtbox.Collision(dir);
                        if (hitTarget) TUFF.AudioManager.instance.PlaySFX(SFXList.instance.barrelBroWallHit);
                    }
                }
            }
        }
        public override bool Hurt(Vector2 launch)
        {
            return true;
        }
        public void OnDrawGizmos()
        {
            DrawLineOfSightGizmos();
        }
    }
}
