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
        public float jumpForce = 10;
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
            InputCheck();
            MoveHandler();
            TimerHandler();
            ExpectedPosition();
            input.jumpDown = false;
            input.dashDown = false;
            input.shootDown = false;
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
        }
        private float GetVelX(int moveH)
        {
            float velocity = moveH * moveSpeed; // Walk Speed
            if (dashing) velocity = dashSpeed * m_dashDirection; // Dash Speed
            return velocity;
        }
        private void DashHandler()
        {
            if (input.dashDown && m_dashTicks <= 0) // Dash Input
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
            hurtbox?.SetInvulnerable(false);
            if (!grounded) ChangeState(CharacterStates.Airborne);
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
                    if (!dashing) ChangeState(CharacterStates.Airborne);
                }
                Debug.Log("Jump!");
                rb.velocity = new Vector2(rb.velocity.x, velY);
                SetAirborne(); //Setting this here so slope fixes get ignored
            }
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

