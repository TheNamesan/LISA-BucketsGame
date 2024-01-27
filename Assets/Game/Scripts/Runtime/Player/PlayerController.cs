using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    
    public enum CharacterStates { Idle = 0, Walk = 1, Falling = 2, Airborne = 3, Dashing = 4 };
    public class PlayerController : MovingEntity
    {
        //public Rigidbody2D rb;
        //public BoxCollider2D col;
        public WeaponBehaviour weapon;
        //public SpriteRenderer sprite;
        public CharacterAnimationHandler animHandler;
        //public Hurtbox hurtbox;
        
        //public Vector2 closestContactPointD { get => col.ClosestPoint((Vector2)col.bounds.center + Vector2.down * col.bounds.size); }
        public GamePlayerInput input;
        public CharacterStates lastState = CharacterStates.Idle;
        //public Facing facing = Facing.Right;

        //public bool dead { get => m_dead; }
        //private bool m_dead = false;
        private Vector2 lastPosition;

        //public float moveSpeed = 6;

        [Header("Jump")]
        public float jumpForce = 12;
        //public float gravityScale = 2;
        public int extraJumps = 1;
        public int midairDashes = 1;
        //public float maxFallSpeed = -10;
        [SerializeField] private int m_jumps = 0;
        [SerializeField] private int m_midairDashes = 0;

        //[Header("Ground Collision")]
        //public LayerMask groundLayers;
        //public bool grounded = false;
        //public Collider2D groundCollider = null;
        //public Vector2 groundPoint;
        //public Vector2 groundNormal;

        //public Vector2 GroundNormalPerpendicular { get => Vector2.Perpendicular(groundNormal).normalized; }
        //private RaycastHit2D normalRight;
        //private RaycastHit2D normalLeft;
        //public bool IsOnSlope { get => groundNormal != Vector2.up; }

        [Header("Dash")]
        public float dashSpeed = 15;
        public int dashTicksDuration = 25;
        public bool dashing = false;
        [SerializeField] private int m_dashTicks = 0;
        [SerializeField] private int m_dashDirection = 0;

        [Header("Wall Jump")]
        public bool wallClimb = false;
        public bool wallJumping = false;
        public Vector2 wallJumpSpeed = new Vector2(25, 14);
        public int wallJumpTicksDuration = 10;
        [SerializeField] private int m_wallJumpTicks = 0;
        [SerializeField] private int m_wallJumpDirection = 0;

        private void OnEnable()
        {
            lastPosition = rb.position;
        }
        private void Start()
        {
            m_jumps = extraJumps;
            m_midairDashes = midairDashes;
        }
        private void Update()
        {
            if (sprite)
            {
                if (grounded) sprite.color = Color.white; // Tmp
                else sprite.color = Color.green;
                if (hurtbox && hurtbox.invulnerable) sprite.color = Color.blue;
                if (m_dead) rb.constraints = RigidbodyConstraints2D.None;
                else rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            GroundedAnimationStateCheck();
        }
        private void FixedUpdate()
        {
            AssignDeadMaterial();
            lastPosition = rb.position;
            GroundCheck();
            WallCheck();
            InputCheck();
            MoveHandler();
            TimerHandler();
            ExpectedPosition();
            input.jumpDown = false;
            input.dashDown = false;
            input.shootDown = false;
        }
        protected override void GroundCheck()
        {
            if (m_dead) return;
            //Vertical Collision
            float sizeMult = 0.1f;
            float boxSizeX = col.bounds.size.x;
            Vector2 collisionBoxSize = new Vector2(boxSizeX, Physics2D.defaultContactOffset * sizeMult);
            float collisionBoxDistance = collisionBoxSize.y * 10f;//(rb.velocity.y > -10 ? collisionBoxSize.y * 10f : collisionBoxSize.y * 200f);
            RaycastHit2D collision = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance, groundLayers);

            Color boxColor = Color.red;

            // This is a fix used when reaching the top of a slope
            // Added dashing alongside IsOnSlope as sometimes moving too fast would put you airborne if you tried to slide down a slope
            if (!collision && (IsOnSlope || dashing) && grounded) // If was on slope climbing up, attempt to find expected ground
            {
                var distance = collisionBoxDistance * 100f;
                RaycastHit2D snapAttempt = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, distance, groundLayers);
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
                //if (dashing) distance *= 1.5f;
                //RaycastHit2D normalHitHR = Physics2D.Raycast(closestContactPointD, Vector2.right, distance, groundLayers);
                //RaycastHit2D normalHitHL = Physics2D.Raycast(closestContactPointD, Vector2.left, distance, groundLayers);
                RaycastHit2D normalHitVRay = Physics2D.Raycast(closestContactPointD, Vector2.down, distance, groundLayers);
                RaycastHit2D normalHitV = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, distance, groundLayers);
                Vector2 offset = new Vector2((dashing ? dashSpeed : moveSpeed) * Time.fixedDeltaTime, 0);
                Vector2 rightOrigin = closestContactPointD + offset;
                normalRight = Physics2D.BoxCast(rightOrigin, collisionBoxSize, 0f, Vector2.down, distance, groundLayers);
                Vector2 leftOrigin = closestContactPointD - offset;
                normalLeft = Physics2D.BoxCast(leftOrigin, collisionBoxSize, 0f, Vector2.down, distance, groundLayers);

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
        private void AssignDeadMaterial()
        {
            if (!m_dead) rb.sharedMaterial = GameManager.instance.aliveMat;
            else rb.sharedMaterial = GameManager.instance.deadMat;
        }

        private void InputCheck()
        {
            DashHandler();
            ShootHandler();
        }
        private void ExpectedPosition()
        {
            Debug.DrawRay(closestContactPointD + rb.velocity * Time.fixedDeltaTime, Vector3.up, Color.blue);
        }

        protected override void SetAirborne()
        {
            //Debug.Log("Going airborne");
            if (grounded && !dashing) ChangeState(CharacterStates.Airborne);
            grounded = false;
            UpdateGroundData(null);
            EnableGravity(true);
        }

        private void ShootHandler()
        {
            if (m_dead) return;
            if (input.shootDown)
            {
                weapon?.Shoot(DistanceToMouse().normalized);
            }
        }
        protected override void TouchLand()
        {
            grounded = true;
            m_jumps = extraJumps; // Restore mid-air jumps
            m_midairDashes = midairDashes; // Restore mid-air jumps
            wallClimb = false;
            StopWallJump();
            EnableGravity(false);
        }
        private void MoveHandler()
        {
            int moveH = (int)input.inputH;
            int moveV = (int)input.inputV;
            if (m_dead) return;
            if (!grounded) // Mid-air
            {
                DashCancelCheck(moveH);
                float velX = GetVelX(moveH);
                float velY = moveV * jumpForce;
                if (((moveH > 0 || wallJumping) && (IsVerticalWall(wallRightHit)) ||
                    ((moveH < 0 || wallJumping) && IsVerticalWall(wallLeftHit))) && !dashing) // Wall Climb
                {
                    StopWallJump();
                    wallClimb = true;
                }
                if (wallClimb && (IsVerticalWall(wallRightHit) || IsVerticalWall(wallLeftHit))) // Wall Climb speed
                {
                    if (moveV < 0) wallClimb = false;
                    else
                    {
                        rb.velocity = new Vector2(0f, rb.velocity.y * 0.8f);
                        bool jumped = input.jumpDown || input.dashDown; // Wall Jump
                        if (jumped)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, wallJumpSpeed.y);
                            wallClimb = false;
                            WallJump();
                            ChangeFacingOnMove(m_wallJumpDirection);
                        }
                        else return;
                    }
                }
                if (wallJumping) // Wall Jump Speed
                {
                    rb.velocity = new Vector2(wallJumpSpeed.x * m_wallJumpDirection, rb.velocity.y);
                    return;
                }
                else
                {
                    rb.velocity = new Vector2(velX, rb.velocity.y);
                }
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
                finalVel = GetSlopeVelocity(moveH, velX, finalVel, normal);
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

        private void TimerHandler()
        {
            DashTimer();
            WallJumpTimer();
        }
        private float GetVelX(int moveH)
        {
            float velocity = moveH * moveSpeed; // Walk Speed
            if (dashing) velocity = dashSpeed * m_dashDirection; // Dash Speed
            return velocity;
        }
        private void DashHandler()
        {
            if (input.dashDown && m_dashTicks <= 0 && !wallJumping && !wallClimb && !m_dead) // Dash Input
            {
                Dash(!grounded);
            }
        }
        private void Dash(bool useMidairDashes = false)
        {
            if (useMidairDashes)
            {
                if (m_midairDashes <= 0) return;
                else m_midairDashes--;
            }
            m_dashTicks = dashTicksDuration;
            m_dashDirection = FacingToInt(facing);
            dashing = true;
            hurtbox?.SetInvulnerable(true);
            ChangeState(CharacterStates.Dashing, true);
        }
        private void WallJump()
        {
            m_wallJumpTicks = wallJumpTicksDuration;
            m_wallJumpDirection = -FaceToInt();
            wallJumping = true;
            hurtbox?.SetInvulnerable(true);
        }
        private void DashTimer()
        {
            if (!dashing) return;
            m_dashTicks--;
            if (m_dashTicks < 0) StopDash();
        }
        private void WallJumpTimer()
        {
            if (!wallJumping) return;
            m_wallJumpTicks--;
            if (m_wallJumpTicks < 0)
            { 
                StopWallJump();
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }
        private void StopDash()
        {
            m_dashTicks = 0;
            m_dashDirection = 0;
            dashing = false;
            hurtbox?.SetInvulnerable(false);
            if (!grounded) ChangeState(CharacterStates.Airborne);
        }
        private void StopWallJump()
        {
            m_wallJumpTicks = 0;
            m_wallJumpDirection = 0;
            wallJumping = false;
            hurtbox?.SetInvulnerable(false);
        }
        /// <summary>
        /// Returns true if jump force was applied.
        /// </summary>
        private bool Jump(float velY, bool useExtraJumps = false)
        {
            if (input.jumpDown) // Jump
            {
                // If jumping mid-air and not enough extra jumps, abort
                if (useExtraJumps)
                {
                    if (m_jumps <= 0) return false;
                    else m_jumps--;
                    if (!dashing) ChangeState(CharacterStates.Airborne);
                }
                Debug.Log("Jump!");
                rb.velocity = new Vector2(rb.velocity.x, velY);
                wallClimb = false;
                SetAirborne(); //Setting this here so slope fixes get ignored
                return true;
            }
            return false;
        }
        public override bool Hurt(Vector2 launch)
        {
            if (m_dead) return false;
            Debug.Log("Ouch");
            m_dead = true;
            SetAirborne();
            StopDash();
            launch *= 40f;
            rb.velocity = (launch);
            GameManager.instance.OnPlayerDead();
            return true;
        }
        public override void ChangeFacing(Facing newFacing)
        {
            facing = newFacing;
            animHandler?.FlipSprite(facing);
            GroundedAnimationStateCheck();
        }


        private void ChangeState(CharacterStates state, bool forcePlaySameAnim = false)
        {
            animHandler.ChangeAnimationState(this, state, forcePlaySameAnim);
            lastState = state;
        }
        private void GroundedAnimationStateCheck()
        {
            //if (!grounded || jumping || hardLanded || climbing || falling) return;
            if (!grounded || dashing) return;

            if (Mathf.Abs(rb.velocity.x) > 0.000001f && input.inputH != 0)
            {
                ChangeState(CharacterStates.Walk);
            }
            else { ChangeState(CharacterStates.Idle); }
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

