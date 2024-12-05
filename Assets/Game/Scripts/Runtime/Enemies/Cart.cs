using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

namespace BucketsGame
{
    public class Cart : MovingEntity
    {
        public bool moving { get => m_moving; }
        [Header("Cart properties")]
        [SerializeField] private bool m_moving;
        public float movingStartTimer = 0;

        private void Update()
        {
            if (sprite)
            {
                sprite.flipX = facing == Facing.Left;
                sprite.transform.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, usedNormal));
                //sprite.transform.position = new Vector3(sprite.transform.position.x, groundPoint.y, sprite.transform.position.z);
            }
        }
        private void FixedUpdate()
        {
            GroundCheck();
            WallCheck();
            MoveHandler();
            FallOffMapCheck();
        }
        private void MoveHandler()
        {
            if (!m_moving) { CapVelocity(); return; }
            int moveH = FaceToInt();
            if (grounded)
            {
                movingStartTimer += Time.fixedDeltaTime;
                movingStartTimer = Mathf.Min(movingStartTimer, 1f);
                float speed = moveSpeed * movingStartTimer;
                float velX = moveH * speed;
                Vector2 velocity = new Vector2(velX, 0);
                //if (speed != moveSpeed)
                //{
                //    velX = Mathf.Lerp(speed, moveSpeed, Time.fixedDeltaTime);
                //}

                velocity = new Vector2(velX, 0);
                velocity = GetSlopeVelocity(moveH, velX, velocity, groundNormal);

                rb.velocity = velocity;
                //transform.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, usedNormal));
            }
            bool wallClimbFoundRight = (moveH > 0) && IsVerticalWall(wallRightHit);
            bool wallClimbFoundLeft = (moveH < 0) && IsVerticalWall(wallLeftHit);
            if (!wallClimbFoundRight && !wallClimbFoundLeft) Hitbox(moveH);
            CapVelocity();
        }
        public override bool Hurt(Vector2 launch)
        {
            return Move(launch.x);
        }
        public override bool TryKill(Vector2 launch)
        {
            return Move(launch.x);
        }
        public bool Move(float dir)
        {
            if (m_moving) return false;
            m_moving = true;
            TUFF.AudioManager.instance.PlaySFX(SFXList.instance.cartMoveSFX);

            Hitbox(FaceToInt());
            //col.enabled = false;

            return true;
        }
        private void Hitbox(int openDir)
        {
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            Vector2 position = rb.position;
            Vector2 size = new Vector2(col.bounds.size.x, col.bounds.size.y);
            Vector2 dir = transform.right * openDir;
            float distance = 0f;
            RaycastHit2D[] hits = Physics2D.BoxCastAll(position, size, sprite.transform.eulerAngles.z, dir, distance, hitboxLayers);
            Debug.DrawRay(rb.position, dir * distance, Color.magenta, 1f);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider == this.hurtbox.col) continue;
                if (hits[i].collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (!hurtbox.invulnerable)
                    {
                        bool hitTarget = hurtbox.TryKill(dir);
                        if (hitTarget)
                        {
                            TUFF.AudioManager.instance.PlaySFX(SFXList.instance.cartHitSFX);
                        }
                    }
                }
            }
        }
    }
}

