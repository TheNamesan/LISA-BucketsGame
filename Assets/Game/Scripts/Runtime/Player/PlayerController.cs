using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public enum Facing
    {
        Right = 0, 
        Left = 1
    }
    public class PlayerController : MonoBehaviour
    {
        public Rigidbody2D rb;
        public BoxCollider2D col;
        public WeaponBehaviour weapon;
        public Vector2 closestContactPointD { get => col.ClosestPoint((Vector2)col.bounds.center + Vector2.down * col.bounds.size); }
        public GamePlayerInput input;
        public Facing facing = Facing.Right;

        public float moveSpeed = 6;

        [Header("Jump")]
        public float jumpForce = 10;
        public int extraJumps = 1;
        public float maxFallSpeed = -10;
        private int m_jumps = 0;

        [Header("Ground Collision")]
        public LayerMask groundLayers;
        public bool grounded = false;
        public Collider2D groundCollider = null;
        public Vector2 groundPoint;
        public Vector2 groundNormal;
        public Vector2 GroundNormalPerpendicular { get => Vector2.Perpendicular(groundNormal).normalized; }

        [Header("Dash")]
        public float dashSpeed = 15;
        public int dashTicksDuration = 25;
        public bool dashing = false;
        private int m_dashTicks = 0;
        private int m_dashDirection = 0;

        private void Update()
        {

        }
        private void FixedUpdate()
        {
            GroundCheck();
            InputCheck();
            MoveHandler();
            TimerHandler();
            input.jumpDown = false;
            input.dashDown = false;
            input.shootDown = false;
        }

        private void InputCheck()
        {
            if (input.dashDown && m_dashTicks <= 0) // Dash Input
            {
                DashHandler();
            }
            ShootHandler();
            if (input.focus) // Tmp?
            {
                Time.timeScale = 0.25f;
            }
            else Time.timeScale = 1;
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
                //Vector2 normal = Physics2D.BoxCast(closestContactPointD, new Vector2(collisionBoxSize.x, 1f), 
                //    0f, Vector2.down, collisionBoxDistance, groundLayers).normal;
                Vector2 normal = collision.normal;
                var collider = collision.collider;
                if (grounded) 
                {
                    boxColor = Color.yellow;
                }
                if (!grounded) // If mid-air before, touch land
                {
                    TouchLand();
                    boxColor = Color.green;
                }
                UpdateGroundData(collider, collision.point, normal);
                Debug.DrawRay(collision.point, normal, Color.green);
            }
            else // OnCollisionExit
            {
                grounded = false;
                UpdateGroundData(null);
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
                weapon?.Shoot(DistanceToMouse().normalized);
            }
        }
        private void TouchLand()
        {
            grounded = true;
            m_jumps = extraJumps; // Restore mid-air jumps
        }
        private void UpdateGroundData(Collider2D collider, Vector2 point = new Vector2(), Vector2 normal = new Vector2())
        {
            groundCollider = collider;
            if (!groundCollider) //If no collider, set default normal
            {
                groundPoint = Vector2.zero;
                groundNormal = Vector2.up;
            }
            else
            {
                groundPoint = point;
                groundNormal = normal;
            }
        }
        private void MoveHandler()
        {
            int moveH = (int)input.inputH;
            int moveV = (int)input.inputV;

            if (!grounded) // Mid-air
            {
                DashCancelCheck(moveH);
                float velX = GetVelX(moveH);
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
                ChangeFacingOnMove(moveH);
            }
            else // In Ground
            {
                DashCancelCheck(moveH);
                float velX = GetVelX(moveH);
                float velY = moveV * jumpForce;
                Vector2 finalVel = new Vector2(velX, rb.velocity.y);
                finalVel *= new Vector2(1, -GroundNormalPerpendicular.y);
                rb.velocity = finalVel;
                Jump(velY);
                ChangeFacingOnMove(moveH);
            }
            CapVelocity();
        }

        private void DashCancelCheck(int moveH)
        {
            if (dashing && moveH != 0 && m_dashDirection != moveH) { StopDash(); } // Cancel Dash
        }

        private void ChangeFacingOnMove(float moveH)
        {
            if (moveH > 0) ChangeFacing(Facing.Right);
            if (moveH < 0) ChangeFacing(Facing.Left);
        }
        private void TimerHandler()
        {
            DashTimer();
        }
        private float GetVelX(int moveH)
        {
            float velocity = moveH * moveSpeed; // Walk Speed
            if (dashing) velocity = dashSpeed * m_dashDirection; // Dash Speed
            return velocity;
        }
        private void DashHandler()
        {
            m_dashTicks = dashTicksDuration;
            m_dashDirection = FacingToInt(facing);
            dashing = true;
        }

        private void DashTimer()
        {
            if (!dashing) return;
            m_dashTicks--;
            if (m_dashTicks < 0) StopDash();
        }
        private void StopDash()
        {
            m_dashTicks = 0;
            m_dashDirection = 0;
            dashing = false;
        }
        private void Jump(float velY, bool useExtraJumps = false)
        {
            if (input.jumpDown) // Jump
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
        public void ChangeFacing(Facing newFacing)
        {
            facing = newFacing;
        }
        public int FacingToInt(Facing faceDir)
        {
            return faceDir == Facing.Right ? 1 : -1;
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

