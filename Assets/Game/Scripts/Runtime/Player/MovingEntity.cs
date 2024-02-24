using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUFF;

namespace BucketsGame
{
    public enum Facing { Right = 0, Left = 1 }
    public class MovingEntity : MonoBehaviour
    {
        public Rigidbody2D rb;
        public BoxCollider2D col;
        public SpriteRenderer sprite;
        public Hurtbox hurtbox;
        public Vector2 closestContactPointD { get => col.ClosestPoint((Vector2)col.bounds.center + Vector2.down * col.bounds.size); }
        public Vector2 closestContactPointR { get => col.ClosestPoint((Vector2)col.bounds.center + Vector2.right * col.bounds.size); }
        public Vector2 closestContactPointL { get => col.ClosestPoint((Vector2)col.bounds.center + Vector2.left * col.bounds.size); }
        public Facing facing = Facing.Right;
        public float moveSpeed = 6;

        
        public LayerMask groundLayers { get => BucketsGameManager.instance.groundLayers; }
        public LayerMask oneWayGroundLayers { get => BucketsGameManager.instance.oneWayLayers; }
        [Header("Ground Collision")]
        public bool grounded = false;
        public bool ignoreOneWay = false;
        public Collider2D groundCollider = null;
        public TerrainProperties groundProperties = null;
        [SerializeField] protected List<Collider2D> ignoredOneWays = new();
        public Vector2 groundPoint;
        public Vector2 GroundNormalPerpendicular { get => Vector2.Perpendicular(groundNormal).normalized; }
        public RaycastHit2D normalRight;
        public RaycastHit2D normalLeft;
        public RaycastHit2D wallRightHit;
        public RaycastHit2D wallLeftHit;
        public bool IsOnSlope { get => groundNormal != Vector2.up; }
        public Vector2 groundNormal;
        [SerializeField] protected bool groundIsOneWay;
        public float gravityScale = 2;
        public float maxFallSpeed = -15;

        public int hp = 1;
        public bool dead { get => m_dead; }
        [SerializeField] protected bool m_dead = false;
        protected readonly Vector2 NORMAL_LIMIT = new Vector2(1, 0);

        public bool OnScreen 
        { get 
            {
                var screenPos = SceneProperties.instance.camManager.GetWorldToScreenPoint(rb.position);
                return gameObject.activeInHierarchy && 
                    screenPos.x > 0f && screenPos.x < Screen.width && 
                    screenPos.y > 0f && screenPos.y < Screen.height;
            }
        }

        protected virtual void GroundCheck()
        {
            if (m_dead) return;
            LayerMask layers = groundLayers;
            if (!ignoreOneWay) layers = layers | oneWayGroundLayers;

            //Vertical Collision
            float sizeMult = 0.1f;
            Vector2 collisionBoxSize = new Vector2(col.bounds.size.x, Physics2D.defaultContactOffset * sizeMult);
            float collisionBoxDistance = collisionBoxSize.y * 10f;//(rb.velocity.y > -10 ? collisionBoxSize.y * 10f : collisionBoxSize.y * 200f);
            RaycastHit2D collision = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance, layers);

            Color boxColor = Color.red;

            // This is a fix used when reaching the top of a slope
            if (!collision && IsOnSlope && grounded) // If was on slope climbing up, attempt to find expected ground
            {
                RaycastHit2D snapAttempt = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance * 100f, layers);
                if (snapAttempt)
                {
                    collision = snapAttempt;
                    rb.velocity = Vector2.zero; // Important!
                    SnapToGround(sizeMult, collision, instant: true); // The instant is important so it doesn't cancel the speed in MoveHandler (rb.MovePosition is the issue)
                    collision = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance, layers);
                }
            }
            if (collision)
            {
                var collider = collision.collider;
                Vector2 normal = collision.normal;
                float distance = col.size.y;
                //RaycastHit2D normalHitHR = Physics2D.Raycast(closestContactPointD, Vector2.right, distance, groundLayers);
                //RaycastHit2D normalHitHL = Physics2D.Raycast(closestContactPointD, Vector2.left, distance, groundLayers);
                RaycastHit2D normalHitVRay = Physics2D.Raycast(closestContactPointD, Vector2.down, distance, layers);
                RaycastHit2D normalHitV = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, distance, layers);
                Vector2 offset = new Vector2(moveSpeed * Time.fixedDeltaTime, 0);
                Vector2 rightOrigin = closestContactPointD + offset;
                normalRight = Physics2D.BoxCast(rightOrigin, collisionBoxSize, 0f, Vector2.down, distance, layers);
                Vector2 leftOrigin = closestContactPointD - offset;
                normalLeft = Physics2D.BoxCast(leftOrigin, collisionBoxSize, 0f, Vector2.down, distance, layers);

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
                        //Debug.Log($"box: {boxDiff} < ray: {rayDiff}");
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
        protected virtual void WallCheck()
        {
            if (m_dead) return;
            //Vertical Collision
            float sizeMult = 0.1f;
            Vector2 collisionBoxSize = new Vector2(Physics2D.defaultContactOffset * sizeMult, col.bounds.size.y);
            float collisionBoxDistance = collisionBoxSize.x * 10f;
            wallRightHit = Physics2D.BoxCast(closestContactPointR, collisionBoxSize, 0f, Vector2.right, collisionBoxDistance, groundLayers);
            wallLeftHit = Physics2D.BoxCast(closestContactPointL, collisionBoxSize, 0f, Vector2.left, collisionBoxDistance, groundLayers);
            if (wallRightHit) Debug.DrawRay(wallRightHit.point, wallRightHit.normal, Color.green);
            if (wallLeftHit) Debug.DrawRay(wallLeftHit.point, wallLeftHit.normal, Color.green);

            Color boxColor = Color.red;
        }
        protected virtual bool IsVerticalWall(RaycastHit2D wallHit)
        {
            if (!wallHit) return false;
            return 1 - Mathf.Abs(wallHit.normal.x) <= 0.00001f; // Is about 1
        }

        protected virtual void SetAirborne()
        {
            grounded = false;
            UpdateGroundData(null);
            EnableGravity(true);
        }
        protected virtual void SnapToGround(float sizeMult, RaycastHit2D collision, float offsetForce = 10, bool instant = false)
        {
            int layerMask = 1 << collision.collider.gameObject.layer;
            float distance = Vector2.Distance(closestContactPointD, rb.position) * 1.25f;
            RaycastHit2D hitW = Physics2D.Raycast(rb.position, Vector2.down, distance, layerMask);
            Vector2 landingPosition = (hitW ? hitW.point : collision.point);
            //if (landingPosition == collision.point) Debug.Log("Using collision point");
            float offset = Physics2D.defaultContactOffset * sizeMult * offsetForce;
            float moveToY = landingPosition.y + Vector2.Distance(closestContactPointD, rb.position) + offset;
            if (!instant)
            {
                var pos = new Vector2(rb.position.x + rb.velocity.x * Time.deltaTime, moveToY);
                if (rb.velocity.y > -1f) { rb.MovePosition(pos); }  // if velocity is too small, smooth out the positioning
                else rb.position = pos; // else position the player instantly
            }
            else rb.position = new Vector2(rb.position.x, moveToY); // else position the player instantly
        }
        protected virtual void EnableGravity(bool enable)
        {
            if (enable) rb.gravityScale = gravityScale;
            else rb.gravityScale = 0;
        }
        protected virtual void TouchLand()
        {
            grounded = true;
            EnableGravity(false);
        }
        protected virtual void UpdateGroundData(Collider2D collider, Vector2 point = new Vector2(), Vector2 normal = new Vector2())
        {
            groundCollider = collider;
            // If collider has Terrain Properties component
            if (collider && collider.TryGetComponent(out TerrainProperties props))
            {
                groundProperties = props;
            }
            if (!groundCollider) //If no collider, set default normal
            {
                groundPoint = Vector2.zero;
                groundNormal = Vector2.up;
                groundProperties = null;
            }
            else
            {
                groundPoint = point;
                groundNormal = normal;
            }
        }
        public virtual bool Hurt(Vector2 launch)
        {
            return Kill(launch);
        }
        public virtual bool Kill(Vector2 launch)
        {
            if (m_dead) return false;
            m_dead = true;
            SetAirborne();
            launch *= 40f;
            rb.velocity = (launch);
            return true;
        }
        protected virtual void CapVelocity()
        {
            if (rb.velocity.y < maxFallSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
            }
        }
        protected virtual Vector2 GetSlopeVelocity(int moveH, float velX, Vector2 velocity, Vector2 normal)
        {
            if (moveH > 0) normal = GetNormalFrom(normal, normalRight);
            else if (moveH < 0) normal = GetNormalFrom(normal, normalLeft);
            if (normal != Vector2.up) // On Slope
            {
                if (normal.y >= NORMAL_LIMIT.y)
                {
                    var perp = Vector2.Perpendicular(normal).normalized;
                    velocity = new Vector2(velX, velX) * -perp;
                }
            }
            return velocity;
        }
        protected Vector2 GetNormalFrom(Vector2 normal, RaycastHit2D ray)
        {
            if (ray)
            {
                
                // If difference in Y is relatively high and
                // Normal.y is positive (angle between 0 and 180)
                
                if (ray.point.y - groundPoint.y > 0.05f && ray.normal.y >= NORMAL_LIMIT.y)
                {
                    Debug.Log($"{gameObject.name}: {ray.point.y - groundPoint.y}({ray.normal.y})");
                    normal = ray.normal;
                }
            }
            return normal;
        }
        protected virtual void ChangeFacingOnMove(float moveH)
        {
            if (moveH > 0) ChangeFacing(Facing.Right);
            if (moveH < 0) ChangeFacing(Facing.Left);
        }
        public virtual void ChangeFacing(Facing newFacing)
        {
            facing = newFacing;
        }
        public virtual int FaceToInt()
        {
            return (facing == Facing.Right ? 1 : -1);
        }
        public virtual void A_StepEffect()
        {
            if (groundProperties)
            {
                groundProperties.Step(transform.position, new Vector3Int(0, -1, 0));
            }
        }
    }
}
