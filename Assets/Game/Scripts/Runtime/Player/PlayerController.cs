using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class PlayerController : MonoBehaviour
    {
        public Rigidbody2D rb;
        public BoxCollider2D col;
        public WeaponBehaviour weapon;
        public Vector2 closestContactPointD { get => col.ClosestPoint((Vector2)col.bounds.center + Vector2.down * col.bounds.size); }
        public GamePlayerInput input;

        public float moveSpeed = 6;

        [Header("Jump")]
        public float jumpForce = 10;
        public int extraJumps = 1;
        public float maxFallSpeed = -10;
        private int m_jumps = 0;

        [Header("Ground Collision")]
        public LayerMask groundLayers;
        public bool grounded = false;

        void Update()
        {

        }
        private void FixedUpdate()
        {
            GroundCheck();
            ShootHandler();
            MoveHandler();
            input.jumpPress = false;
            input.shootDown = false;
        }

        private void GroundCheck()
        {
            //Vertical Collision
            float sizeMult = 0.1f;
            Vector2 collisionBoxSize = new Vector2(col.bounds.size.x, Physics2D.defaultContactOffset * sizeMult);
            float collisionBoxDistance = collisionBoxSize.y * 10f;//(rb.velocity.y > -10 ? collisionBoxSize.y * 10f : collisionBoxSize.y * 200f);
            RaycastHit2D collision = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance, groundLayers);

            Color boxColor = Color.red;
            if (collision)
            {
                boxColor = Color.green;
                if (!grounded) // If grounded before, touch land
                    TouchLand();

                Debug.DrawRay(collision.point, collision.normal, Color.yellow);
            }
            else // OnCollisionExit
            {
                grounded = false;
            }

            float displayTime = 0f;
            Vector2 boxCenter = closestContactPointD;
            Vector2 boxExtents = collisionBoxSize * 0.5f;
            Debug.DrawLine(new Vector2(boxCenter.x + boxExtents.x, boxCenter.y - boxExtents.y),
                new Vector2(boxCenter.x + boxExtents.x, boxCenter.y + boxExtents.y), boxColor, displayTime);
            Debug.DrawLine(new Vector2(boxCenter.x + boxExtents.x, boxCenter.y + boxExtents.y),
               new Vector2(boxCenter.x - boxExtents.x, boxCenter.y + boxExtents.y), boxColor, displayTime);
            Debug.DrawLine(new Vector2(boxCenter.x - boxExtents.x, boxCenter.y + boxExtents.y),
               new Vector2(boxCenter.x - boxExtents.x, boxCenter.y - boxExtents.y), boxColor, displayTime);
            Debug.DrawLine(new Vector2(boxCenter.x - boxExtents.x, boxCenter.y - boxExtents.y),
               new Vector2(boxCenter.x + boxExtents.x, boxCenter.y - boxExtents.y), boxColor, displayTime);
        }
        private void ShootHandler()
        {
            if (input.shootDown)
            {
                Debug.Log(DistanceToMouse().normalized);
                weapon?.Shoot(DistanceToMouse().normalized);
            }
        }
        private void TouchLand()
        {
            grounded = true;
            m_jumps = extraJumps; // Restore mid-air jumps
        }
        private void MoveHandler()
        {
            int moveH = (int)input.inputH;
            int moveV = (int)input.inputV;

            if (!grounded) // Mid-air
            {
                float velX = moveH * moveSpeed;
                float velY = moveV * jumpForce;
                rb.velocity = new Vector2(velX, rb.velocity.y);
                Jump(velY, true); // Double Jump
                if (moveV < 0) // Fast Fall
                {
                    rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
                }
                else if (moveV == 0 && rb.velocity.y > 0) // Cancel Jump
                {
                    Debug.Log("Cancel Jump!");
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                }
            }
            else // In Ground
            {
                float velX = moveH * moveSpeed;
                float velY = moveV * jumpForce;
                rb.velocity = new Vector2(velX, rb.velocity.y);
                Jump(velY);
            }
            CapVelocity();
        }

        private void Jump(float velY, bool useExtraJumps = false)
        {
            if (input.jumpPress) // Jump
            {
                // If jumping mid-air and not enough extra jumps, abort
                if (useExtraJumps)
                {
                    if (m_jumps <= 0) return;
                    else m_jumps--;
                }
                Debug.Log("Jump!");
                rb.velocity = new Vector2(rb.velocity.x, velY);

            }
        }

        private void CapVelocity()
        {
            if (rb.velocity.y < maxFallSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
            }
        }
        public Vector2 DistanceToMouse()
        {
            return input.MousePointWorld - rb.position;
        }
        public float AngleToMouse()
        {
            return Vector2.SignedAngle(rb.position, input.MousePointWorld);
        }
    }
}

