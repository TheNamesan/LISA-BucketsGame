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
        public SpriteRenderer sprite;
        public Vector2 closestContactPointD { get => col.ClosestPoint((Vector2)col.bounds.center + Vector2.down * col.bounds.size); }
        public GamePlayerInput input;
        public Facing facing = Facing.Right;

        public float moveSpeed = 6;

        [Header("Jump")]
        public float jumpForce = 10;
        public float gravityScale = 2;
        public int extraJumps = 1;
        public float maxFallSpeed = -10;
        private int m_jumps = 0;

        [Header("Ground Collision")]
        public LayerMask groundLayers;
        public bool grounded = false;
        public Collider2D groundCollider = null;
        public Vector2 groundPoint;
        public Vector2 groundNormal;
        public Vector2 groundNormalSide;

        public Vector2 GroundNormalPerpendicular { get => Vector2.Perpendicular(groundNormal).normalized; }
        public Vector2 GroundNormalSidePerpendicular { get => Vector2.Perpendicular(groundNormalSide).normalized; }
        RaycastHit2D normalRight;
        RaycastHit2D normalLeft;
        public bool IsOnSlope { get => groundNormal != Vector2.up; }
        

        [Header("Dash")]
        public float dashSpeed = 15;
        public int dashTicksDuration = 25;
        public bool dashing = false;
        private int m_dashTicks = 0;
        private int m_dashDirection = 0;

        private void Update()
        {
            if (sprite)
            {
                if (grounded) sprite.color = Color.white; // Tmp
                else sprite.color = Color.green;
            }
        }
        private void FixedUpdate()
        {
            GroundCheck();
            InputCheck();
            MoveHandler();
            TimerHandler();
            ExpectedPosition();
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
            //if (input.focus) // Tmp?
            //{
            //    Time.timeScale = 0.25f;
            //}
            //else Time.timeScale = 1;
        }
        private void ExpectedPosition()
        {
            Debug.DrawRay(closestContactPointD + rb.velocity * Time.fixedDeltaTime, Vector3.up, Color.blue);
        }
        private void GroundCheck()
        {
            //Vertical Collision
            float sizeMult = 0.1f;
            Vector2 collisionBoxSize = new Vector2(col.bounds.size.x, Physics2D.defaultContactOffset * sizeMult);
            float collisionBoxDistance = collisionBoxSize.y * 10f;//(rb.velocity.y > -10 ? collisionBoxSize.y * 10f : collisionBoxSize.y * 200f);
            RaycastHit2D collision = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance, groundLayers);

            Color boxColor = Color.red;

            // This is a fix used when reaching the top of a slope
            if (!collision && IsOnSlope && grounded) // If was on slope climbing up, attempt to find expected ground
            {
                RaycastHit2D snapAttempt = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance * 100f, groundLayers);
                if (snapAttempt)
                { 
                    collision = snapAttempt;
                    rb.velocity = Vector2.zero; // Important!
                    SnapToGround(sizeMult, collision, instant: true); // The instant is important so it doesn't cancel the speed in MoveHandler (rb.MovePosition is the issue)
                    collision = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance, groundLayers);
                }
            }
            if (collision)
            {
                var collider = collision.collider;
                Vector2 normal = collision.normal;
                float distance = col.size.y;
                //RaycastHit2D normalHitHR = Physics2D.Raycast(closestContactPointD, Vector2.right, distance, groundLayers);
                //RaycastHit2D normalHitHL = Physics2D.Raycast(closestContactPointD, Vector2.left, distance, groundLayers);
                RaycastHit2D normalHitVRay = Physics2D.Raycast(closestContactPointD, Vector2.down, distance, groundLayers);
                RaycastHit2D normalHitV = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, distance, groundLayers);
                Vector2 offset = new Vector2(moveSpeed * Time.fixedDeltaTime, 0);
                Vector2 rightOrigin = closestContactPointD + offset;
                normalRight = Physics2D.BoxCast(rightOrigin, collisionBoxSize, 0f, Vector2.down, distance, groundLayers);
                Vector2 leftOrigin = closestContactPointD - offset;
                normalLeft = Physics2D.BoxCast(leftOrigin, collisionBoxSize, 0f, Vector2.down, distance, groundLayers);

                //if (normalHitHR)
                //{
                //    groundNormalSide = normalHitHR.normal;
                //}
                //if (normalHitHL)
                //{
                //    groundNormalSide = normalHitHL.normal;
                //}
                if (normalRight)
                {
                    Debug.DrawRay(normalRight.point, normalRight.normal, Color.red); //
                }
                if (normalLeft)
                {
                    Debug.DrawRay(normalLeft.point, normalLeft.normal, Color.red); //
                }
                if (normalHitV)
                {
                    normal = normalHitV.normal;
                }
                if (normalHitVRay)
                {
                    //var boxDiff = new Vector2(Mathf.Abs(normalHitV.normal.x), Mathf.Abs(normalHitV.normal.y)) - Vector2.up;
                    //var rayDiff = new Vector2(Mathf.Abs(normalHitVRay.normal.x), Mathf.Abs(normalHitVRay.normal.y)) - Vector2.up;
                    var boxDiff = Vector2.Distance(normalHitV.normal, Vector2.up);
                    var rayDiff = Vector2.Distance(normalHitVRay.normal, Vector2.up);
                    if (boxDiff < rayDiff) // Keep this!!!!
                    {
                        Debug.Log($"box: {boxDiff} < ray: {rayDiff}");
                        normal = normalHitVRay.normal; // If this takes priority, it allows climbing down normally
                    }
                }

                if (grounded) 
                {
                    boxColor = Color.yellow;
                }
                if (!grounded) // If mid-air before, touch land
                {
                    SnapToGround(sizeMult, collision);
                    TouchLand();
                    boxColor = Color.green;
                }
                UpdateGroundData(collider, collision.point, normal);
                Debug.DrawRay(collision.point, normal, Color.green);
            }
            else // OnCollisionExit
            {
                SetAirborne();
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

        private void SetAirborne()
        {
            //Debug.Log("Going airborne");
            grounded = false;
            UpdateGroundData(null);
            EnableGravity(true);
        }

        private void SnapToGround(float sizeMult, RaycastHit2D collision, float offsetForce = 10, bool instant = false)
        {
            int layerMask = 1 << collision.collider.gameObject.layer;
            float distance = Vector2.Distance(closestContactPointD, rb.position) * 1.25f;
            RaycastHit2D hitW = Physics2D.Raycast(rb.position, Vector2.down, distance, layerMask);
            Vector2 landingPosition = (hitW ? hitW.point : collision.point);
            if (landingPosition == collision.point) Debug.Log("Using collision point");
            float offset = Physics2D.defaultContactOffset * sizeMult * offsetForce;
            float moveToY = landingPosition.y + Vector2.Distance(closestContactPointD, rb.position) + offset;
            if (!instant)
            {
                if (rb.velocity.y > -1f) { rb.MovePosition(new Vector2(rb.position.x, moveToY)); }  // if velocity is too small, smooth out the positioning
                else rb.position = new Vector2(rb.position.x, moveToY); // else position the player instantly
            }
            else rb.position = new Vector2(rb.position.x, moveToY); // else position the player instantly
        }

        private void EnableGravity(bool enable)
        {
            if (enable) rb.gravityScale = gravityScale;
            else rb.gravityScale = 0;
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
            EnableGravity(false);
        }
        private void UpdateGroundData(Collider2D collider, Vector2 point = new Vector2(), Vector2 normal = new Vector2())
        {
            groundCollider = collider;
            if (!groundCollider) //If no collider, set default normal
            {
                groundPoint = Vector2.zero;
                groundNormal = Vector2.up;
                groundNormalSide = Vector2.up;
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
                Vector2 finalVel = new Vector2(velX, 0); // This 0 can fix a lot of jank lol
                Vector2 normal = groundNormal;
                if (moveH > 0) normal = GetNormalFrom(normal, normalRight);
                else if (moveH < 0) normal = GetNormalFrom(normal, normalLeft);
                if (normal != Vector2.up) // On Slope
                {
                    var perp = Vector2.Perpendicular(normal).normalized;
                    finalVel = new Vector2(velX, velX) * -perp;
                }
                //if (IsOnSlope) // On Slope
                //{
                //    finalVel = new Vector2(velX, velX) * -GroundNormalPerpendicular;
                //}
                rb.velocity = finalVel;
                Jump(velY);
                ChangeFacingOnMove(moveH);
            }
            CapVelocity();
        }

        private Vector2 GetNormalFrom(Vector2 normal, RaycastHit2D ray)
        {
            if (ray)
            {
                if (ray.point.y - groundPoint.y > 0.05f)
                {
                    Debug.Log(ray.point.y - groundPoint.y);
                    normal = ray.normal;
                }
            }

            return normal;
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
                SetAirborne(); //Setting this here so slope fixes get ignored
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

