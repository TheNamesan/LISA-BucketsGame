using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUFF;
using DG.Tweening;

namespace BucketsGame
{
    public enum PlayableCharacters { Buckets = 0, Rusher = 1 }
    public enum CharacterStates { Idle = 0, Walk = 1, Falling = 2, Airborne = 3, Dashing = 4 };
    public class PlayerController : MovingEntity
    {
        public Tween movementTween;
        //public Rigidbody2D rb;
        //public BoxCollider2D col;
        public WeaponBehaviour weapon;
        //public SpriteRenderer sprite;
        public CharacterAnimationHandler animHandler;
        public AfterImagesHandler afterImagesHandler;
        public AfterImagesHandler leftArmAfterImagesHandler;
        public AfterImagesHandler rightArmAfterImagesHandler;
        //public Hurtbox hurtbox;
        
        //public Vector2 closestContactPointD { get => col.ClosestPoint((Vector2)col.bounds.center + Vector2.down * col.bounds.size); }
        public GamePlayerInput input;
        public CharacterStates lastState = CharacterStates.Idle;
        //public Facing facing = Facing.Right;

        //public bool dead { get => m_dead; }
        //private bool m_dead = false;
        protected Vector2 lastPosition;
        protected Vector2 lastVelocity;

        public bool flipLock = false;
        public int flipLockDir = 0;
        public bool walkingBackwards { get {
                int moveH = (int)input.inputH;
                if (flipLock && moveH != 0 && moveH != Mathf.Sign(weapon.shootNormal.x))
                {
                    return true;
                }
                return false;
        }}
        [Header("Jump")]
        public float jumpForce = 12;
        //public float gravityScale = 2;
        public int extraJumps = 1;
        public int midairDashes = 1;
        public bool doubleJumping = false;
        [SerializeField] private int m_jumps = 0;
        [SerializeField] private int m_midairDashes = 0;

        [Header("Speed")]
        public float adrenalineSpeedScale = 1.4f;
        public float slowDownSpeedScale = 0.65f;
        public float slowDownJumpScale = 0.5f;
        public int slowDownDuration = 50;
        [SerializeField] private int m_slowDownTicks = 0;

        [Header("Dash")]
        public float dashSpeed = 25;
        public int dashTicksDuration = 25;
        [Tooltip("Amount of physics ticks it takes to be able to change directions at the start of the dash.")]
        public int dashDirectionBufferDuration = 2;
        
        public bool dashing = false;
        public bool canBufferDashDirection { get => dashTicksDuration - m_dashTicks <= dashDirectionBufferDuration; }
        [SerializeField] private int m_dashTicks = 0;
        public int dashCooldownTicksDuration = 25;
        public int dashCooldownTicks { get => m_dashCooldownTicks; }
        [SerializeField] private int m_dashCooldownTicks = 0;
        public Vector2 dashDirection { get => m_dashDirection; }
        [SerializeField] private Vector2 m_dashDirection = new Vector2();

        [Header("Wall Jump")]
        public bool wallClimb = false;
        public bool wallJumping = false;
        public Vector2 wallJumpSpeed = new Vector2(25, 14);
        public int wallJumpTicksDuration = 10;
        [SerializeField] private int m_wallJumpTicks = 0;
        [SerializeField] private int m_wallJumpDirection = 0;

        [Header("Stun")]
        public int stunDuration = 62;
        public bool stunned { get => m_stunTicks > 0; }
        [SerializeField] private int m_stunTicks = 0;

        public bool ignoreLandAnim = false;

        [Header("Rusher")]
        public float rusherDashSpeed = 30;

        private void OnEnable()
        {
            lastPosition = rb.position;
            lastVelocity = rb.velocity;
        }
        private void Start()
        {
            ResetMidairMoves();
            ignoreLandAnim = true;
            GroundCheck();
            //GroundedAnimationStateCheck(); // If this is ON, weird stuff happens when resetting
        }
        private void Update()
        {
            /*if (sprite)
            {
                if (grounded) sprite.color = Color.white; // Tmp
                else sprite.color = Color.green;
                if (hurtbox && hurtbox.invulnerable) sprite.color = Color.blue;
                //if (m_dead) rb.constraints = RigidbodyConstraints2D.None;
                //else rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }*/
            if (afterImagesHandler)
            {
                bool active = dashing || wallJumping || BucketsGameManager.instance.focusMode;
                afterImagesHandler.enabled = active;
                if (leftArmAfterImagesHandler)
                {
                    leftArmAfterImagesHandler.m_time = afterImagesHandler.m_time;
                    leftArmAfterImagesHandler.enabled = active; 
                }
                if (rightArmAfterImagesHandler)
                {
                    rightArmAfterImagesHandler.m_time = afterImagesHandler.m_time;
                    rightArmAfterImagesHandler.enabled = active; 
                    
                }
            }
            GroundedAnimationStateCheck();
        }
        private void FixedUpdate()
        {
            AssignDeadMaterial();
            lastPosition = rb.position;
            IgnoreOneWayCheck();
            GroundCheck();
            WallCheck();
            InputCheck();
            AttackRaycast();
            MoveHandler();
            TimerHandler();
            ExpectedPosition();
            FallOffMapCheck();

            lastVelocity = rb.velocity;
        }

        protected override void GroundCheck()
        {
            //if (m_dead) return;
            if (ignoreLandAnim) Debug.Log("HI");
            LayerMask layers = groundLayers;
            if (!ignoreOneWay) layers = layers | oneWayGroundLayers;

            //Vertical Collision
            float sizeMult = 0.1f;
            Vector2 collisionBoxSize = new Vector2(col.bounds.size.x, Physics2D.defaultContactOffset);
            float collisionBoxDistance = Physics2D.defaultContactOffset;//collisionBoxSize.y * 10f;//(rb.velocity.y > -10 ? collisionBoxSize.y * 10f : collisionBoxSize.y * 200f);   
            Vector2 hitOrigin = closestContactPointD;//+ (ignoreLandAnim ? new Vector2(0f, 0.01f) : Vector2.zero);

            RaycastHit2D solidGroundHit = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance, groundLayers);
            if (ignoreLandAnim) Debug.Log(solidGroundHit.collider);
            RaycastHit2D oneWayHit = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance, oneWayGroundLayers);
            if (oneWayHit && ignoreOneWay)
            {
                var collider = oneWayHit.collider;
                if (!ignoredOneWays.Contains(collider))
                {
                    ignoredOneWays.Add(collider);
                    Physics2D.IgnoreCollision(col, collider, true);
                }
            }

            if (ignoreLandAnim) Debug.Log(closestContactPointD);
            RaycastHit2D collision = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance, layers);
            bool nextGroundIsOneWay = oneWayHit && (collision.collider == oneWayHit.collider);
            if (groundCollider != null && collision)
            {
                if (solidGroundHit && solidGroundHit.collider == groundCollider)
                {
                    collision = solidGroundHit;
                    layers = groundLayers;
                }
            }


            Color boxColor = Color.red;


            // This is a fix used when reaching the top of a slope
            // Added dashing alongside IsOnSlope as sometimes moving too fast would put you airborne if you tried to slide down a slope
            if (!collision && (IsOnSlope || dashing) && grounded) // If was on slope climbing up, attempt to find expected ground
            {
                var distance = collisionBoxDistance * 100f;
                RaycastHit2D snapAttempt = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, distance, layers);
                if (snapAttempt)
                {
                    collision = snapAttempt;
                    //rb.velocity = Vector2.zero; // Important!
                    rb.velocity = new Vector2(rb.velocity.x, 0f); // Important!
                    SnapToGround(sizeMult, collision, instant: true); // The instant is important so it doesn't cancel the speed in MoveHandler (rb.MovePosition is the issue)
                    collision = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance, layers);
                }
            }
            if (collision)
            {
                var collider = collision.collider;

                //if (groundCollider != null && collider != groundCollider)
                //{
                //    Debug.Log($"Collider change! One Way: {nextGroundIsOneWay}");
                //    // If current ground is not one way, it takes priority over normal checks
                //    if (!groundIsOneWay)
                //    {
                //        layers = groundLayers;
                //    }
                //    //if (nextGroundIsOneWay && !groundIsOneWay)
                //    //{
                //    //    layers = groundLayers;
                //    //}
                //    //else layers = oneWayGroundLayers;
                //}


                Vector2 normal = collision.normal;
                float distance = col.size.y;
                RaycastHit2D normalHitVRay = Physics2D.Raycast(closestContactPointD, Vector2.down, distance, layers);
                RaycastHit2D normalHitV = Physics2D.BoxCast(closestContactPointD, collisionBoxSize, 0f, Vector2.down, distance, layers);
                Vector2 offset = new Vector2((dashing ? GetDashSpeed() : moveSpeed) * Time.fixedDeltaTime, 0);
                Vector2 rightOrigin = closestContactPointD + offset;
                normalRight = Physics2D.BoxCast(rightOrigin, collisionBoxSize, 0f, Vector2.down, distance, layers);
                Vector2 leftOrigin = closestContactPointD - offset;
                normalLeft = Physics2D.BoxCast(leftOrigin, collisionBoxSize, 0f, Vector2.down, distance, layers);

                

                if (normalRight)
                {
                    Debug.DrawRay(normalRight.point, normalRight.normal, (normalLeft.normal.y >= 0 ? Color.red : Color.magenta)); //
                }
                if (normalLeft)
                {
                    Debug.DrawRay(normalLeft.point, normalLeft.normal, (normalLeft.normal.y >= 0 ? Color.red : Color.magenta)); //
                }
                if (normalHitV)
                {
                    normal = normalHitV.normal;
                }
                if (normalHitVRay)
                {
                    //var boxDiff = new Vector2(Mathf.Abs(normalHitV.normal.x), Mathf.Abs(normalHitV.normal.y)) - Vector2.up;
                    //var rayDiff = new Vector2(Mathf.Abs(normalHitVRay.normal.x), Mathf.Abs(normalHitVRay.normal.y)) - Vector2.up;
                    float boxDiff = Vector2.Distance(normalHitV.normal, Vector2.up);
                    float rayDiff = Vector2.Distance(normalHitVRay.normal, Vector2.up);
                    if (boxDiff < rayDiff) 
                    // (Mathf.Abs(boxDiff - rayDiff) >= 0.001f) // Keep this!!!!
                    {
                        Debug.Log($"box: {boxDiff} < ray: {rayDiff}");
                        normal = normalHitVRay.normal; // If this takes priority, it allows climbing down normally
                    }
                }

                if (grounded)
                {
                    boxColor = Color.yellow;
                }
                // Using lastVelocity here because otherwise the player never hits the ground
                // if falling on a slide while velX is more than 0.
                if (!grounded && lastVelocity.y <= 0) // If mid-air before, touch land
                {
                    var snapHit = collision;
                    if (nextGroundIsOneWay)
                    {
                        var origin = new Vector2(collision.point.x, rb.position.y);
                        var dist = Mathf.Abs(collision.point.y - rb.position.y);
                        LayerMask correctionLayer = 1 << collision.collider.gameObject.layer;
                        RaycastHit2D correction = Physics2D.Raycast(origin, Vector2.down, dist, correctionLayer);
                        Debug.DrawRay(origin, Vector2.down * dist, (correction ? Color.green : Color.blue));
                        if (correction) { Debug.Log("Corrected"); snapHit = correction; }
                    }
                    SnapToGround(sizeMult, snapHit);
                    TouchLand();
                    boxColor = Color.green;
                }
                
                UpdateGroundData(collider, collision.point, normal);
                Debug.DrawRay(collision.point, normal, new Color32(255, 165, 0, 255));
            }
            else // OnCollisionExit
            {
                if (ignoreLandAnim) Debug.Log("no collision :(");
                SetAirborne();
            }


            this.groundIsOneWay = nextGroundIsOneWay; // !!!


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

            ignoreLandAnim = false;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
        }
        private void IgnoreOneWayCheck()
        {
            if (input.inputV < 0)
            {
                ignoreOneWay = true;
            }
            else { 
                ignoreOneWay = false;
                for (int i = 0; i < ignoredOneWays.Count; i++)
                {
                    if (ignoredOneWays[i] == null) continue;
                    Physics2D.IgnoreCollision(col, ignoredOneWays[i], false);
                }
                ignoredOneWays.Clear();                
            }
        }
        private void AssignDeadMaterial()
        {
            if (!m_dead) rb.sharedMaterial = BucketsGameManager.instance.aliveMat;
            else rb.sharedMaterial = BucketsGameManager.instance.deadMat;
        }

        private void InputCheck()
        {
            if (m_dead || stunned) return;
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
            if (!dashing) ChangeState(CharacterStates.Airborne);
            grounded = false;
            UpdateGroundData(null);
            EnableGravity(true);
        }
        private void AttackRaycast()
        {
            if (!BucketsGameManager.IsRusher()) return;
            if (!dashing) return;
            
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            Vector2 dir = transform.right * FaceToInt();
            RaycastHit2D[] hits = Physics2D.BoxCastAll(rb.position, new Vector2(1f, 1f), 0f, dir, 0.25f, hitboxLayers);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (hurtbox.team == Team.Enemy && !hurtbox.invulnerable)
                    {
                        bool hitTarget = hurtbox.TryKill(dir);
                        if (hitTarget)
                        { 
                            TUFF.AudioManager.instance.PlaySFX(SFXList.instance.rusherPunchHitSFX);
                        }
                    }
                }
            }
        }
        private void ShootHandler()
        {
            if (m_dead || stunned) return;
            if (BucketsGameManager.IsRusher()) return;
            bool disabledByAction = wallClimb || wallJumping || dashing;
            bool shoot = ShootButton();
            if (weapon && shoot && !disabledByAction)
            {
                Vector2 aimNormal = DistanceToMouse().normalized;
                bool shot = weapon.Shoot(aimNormal);
                if (shot)
                {
                    ChangeFacingToShootDirection(aimNormal);
                }
            }
        }

        private void ChangeFacingToShootDirection(Vector2 aimNormal, bool ignoreAnims = false)
        {
            // Change character's facing and lock it to the aiming direction
            flipLockDir = (aimNormal.x > 0 ? 1 : -1);
            
            flipLock = false;
            ChangeFacing(flipLockDir > 0 ? Facing.Right : Facing.Left);
            
            flipLock = true;
            // Shoot Animation
            // This below is so it doesn't replay the arms shoot anim when landing
            if (!ignoreAnims) 
                animHandler?.PlayArmsAnimation(weapon.shootNormal, flipLockDir, lastState);
            //if (grounded || (!grounded && weapon.animTicks <= 0)) 
            bool forcePlaySameAnim = grounded || weapon.animTicks <= 0; 
                ChangeState(lastState, forcePlaySameAnim);
            
        }

        protected override void TouchLand()
        {
            grounded = true;
            ResetMidairMoves();
            wallClimb = false;
            StopWallJump();
            EnableGravity(false);
            if (weapon.animTicks > 0) ChangeFacingToShootDirection(weapon.shootNormal, true);
            if (!ignoreLandAnim) AudioManager.instance.PlaySFX(SFXList.instance.landSFX);
        }
        private void MoveHandler()
        {
            int moveH = (int)input.inputH;
            int moveV = (int)input.inputV;
            if (m_dead || stunned)
            {
                if (grounded) rb.velocity *= 0.95f;
                return; 
            }
            if (!grounded) // Mid-air
            {
                if (m_dead || stunned) return;
                DashCancelCheck(moveH);
                float velX = GetVelX(moveH);
                float velY = moveV * jumpForce * (m_slowDownTicks > 0 ? slowDownJumpScale : 1f);
                if (((moveH > 0 || wallJumping) && (IsVerticalWall(wallRightHit)) ||
                    ((moveH < 0 || wallJumping) && IsVerticalWall(wallLeftHit))) && !dashing) // Wall Climb
                {
                    StopWallJump();
                    CancelFacingLock();
                    //ChangeFacingOnMove(moveH);
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
                if (stunned) moveH = (int)Mathf.Sign(rb.velocity.x);
                float velX = GetVelX(moveH);
                float velY = moveV * jumpForce * (m_slowDownTicks > 0 ? slowDownJumpScale : 1f);
                // Check door opening
                CheckDoorOpening(moveH);
                Vector2 finalVel = new Vector2(velX, 0); // This 0 can fix a lot of jank lol
                Vector2 normal = groundNormal;
                if (stunned)
                {
                    //velX = rb.velocity.x;
                    //finalVel = velX;
                    Debug.Log(moveH);
                }
                finalVel = GetSlopeVelocity(moveH, velX, finalVel, normal);
                if (stunned) Debug.Log(finalVel);
                rb.velocity = finalVel;
                //if (stunned) rb.velocity = new Vector2(rb.velocity.x * 0.95f, rb.velocity.y);
                Jump(velY);
                
                ChangeFacingOnMove(moveH);
            }
            CapVelocity();
        }

        private void CheckDoorOpening(int moveH)
        {
            Door targetDoor = null;
            int openDir = 0;
            if ((moveH > 0 || dashing) && (IsVerticalWall(wallRightHit)))
            {
                if (wallRightHit.collider.TryGetComponent(out Door door)) { openDir = 1; targetDoor = door; }
                    
            }
            else if ((moveH < 0 || dashing) && IsVerticalWall(wallLeftHit))
            {
                if (wallLeftHit.collider.TryGetComponent(out Door door)) { openDir = -1; targetDoor = door; }
            }
            if (targetDoor) targetDoor.Open(openDir, true);
        }

        private void DashCancelCheck(int moveH)
        {
            if (BucketsGameManager.IsRusher())
            {
                m_dashDirection.x = moveH; return;
            }
            if (canBufferDashDirection && moveH != 0)
            {
                m_dashDirection.x = moveH; return;
            }
            if (dashing && moveH != 0 && m_dashDirection.x != moveH) 
            { StopDash(true); } // Cancel Dash
        }

        private void TimerHandler()
        {
            DashTimer();
            WallJumpTimer();
            StunTimer();
            if (m_slowDownTicks > 0)
            {
                //int value = (grounded ? 2 : 1);
                m_slowDownTicks--;
                if (m_slowDownTicks < 0) m_slowDownTicks = 0;
            }
            // Set to 1 so the walking forwards anim is not visible for 1 frame
            if (weapon.animTicks <= 0)
            {
                flipLock = false;
            }
        }
        private float GetVelX(int moveH)
        {
            if (m_dead || stunned) return rb.velocity.x;
            float speed = moveSpeed;
            if (m_slowDownTicks > 0) speed *= slowDownSpeedScale;
            if (BucketsGameManager.instance.focusMode) speed *= adrenalineSpeedScale;
            float velocity = moveH * speed; // Walk Speed
            if (dashing) // Dash Speed
            {
                velocity = Mathf.Lerp(speed, GetDashSpeed(), (float)m_dashTicks / dashTicksDuration) * m_dashDirection.x;
            }
            
            return velocity;
        }
        private void DashHandler()
        {
            bool zeroTicks = m_dashTicks <= 0;
            bool button = input.dashDown;
            if (BucketsGameManager.IsRusher())
            {
                zeroTicks = true;
                bool shoot = input.shootDown;
                button = button || shoot;
            }
            if (button && zeroTicks && !wallJumping && !wallClimb) // Dash Input
            {
                bool useMidairDashes = !grounded;
                if (BucketsGameManager.IsRusher())
                    useMidairDashes = false;
                Dash(useMidairDashes);
            }
        }

        private bool ShootButton()
        {
            return input.shootDown || (input.shoot && TUFF.GameManager.instance.configData.bucketsAutoFire);
        }

        private void Dash(bool useMidairDashes = false)
        {
            if (useMidairDashes)
            {
                if (m_midairDashes <= 0) return;
                else
                {
                    m_midairDashes--;
                    doubleJumping = false;
                }
            }
            // Cancel facing lock
            CancelFacingLock();
            m_dashTicks = dashTicksDuration;
            m_dashDirection = new Vector2(FaceToInt(), input.inputV);
            dashing = true;
            hurtbox?.SetInvulnerable(true);
            ChangeState(CharacterStates.Dashing, true);
            if (grounded) VFXPool.instance.PlayVFX("DashVFX", sprite.transform.position, facing == Facing.Left);
            AudioManager.instance.PlaySFX(SFXList.instance.dashSFX);
            if (BucketsGameManager.IsRusher())
                AudioManager.instance.PlaySFX(SFXList.instance.rusherPunchSFX);
            BucketsGameManager.instance.OnDash();
        }

        private void CancelFacingLock()
        {
            flipLock = false;
            if (weapon.animTicks > 0)
            {
                int moveH = (int)input.inputH;
                weapon?.CancelAnim();
                //ChangeFacingOnMove(moveH);
            }
        }

        private void WallJump()
        {
            m_wallJumpTicks = wallJumpTicksDuration;
            m_wallJumpDirection = -FaceToInt();
            wallJumping = true;
            ResetMidairMoves();
            AudioManager.instance.PlaySFX(SFXList.instance.wallJumpSFX);
            hurtbox?.SetInvulnerable(true);
        }
        private void ResetMidairMoves()
        {
            doubleJumping = false;
            m_jumps = extraJumps; // Restore mid-air jumps
            m_midairDashes = midairDashes; // Restore mid-air dashes
        }
        private void DashTimer()
        {
            if (!dashing && m_dashCooldownTicks > 0) 
            {
                m_dashCooldownTicks--;
                return; 
            }
            if (!dashing) return;
            m_dashTicks--;
            if (m_dashTicks < 0) StopDash(true);
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
        private void StunTimer()
        {
            if (m_stunTicks > 0)
                m_stunTicks--;
        }
        private void StopDash(bool setCooldown = false)
        {
            m_dashTicks = 0;
            m_dashDirection = new();
            if (setCooldown) m_dashCooldownTicks = dashCooldownTicksDuration;
            dashing = false;
            hurtbox?.SetInvulnerable(false);
            if (!grounded) ChangeState(CharacterStates.Airborne);
        }
        private void StopWallJump()
        {
            m_wallJumpTicks = 0;
            m_wallJumpDirection = 0;
            if (wallJumping) hurtbox?.SetInvulnerable(false);
            wallJumping = false;
        }
        /// <summary>
        /// Returns true if jump force was applied.
        /// </summary>
        private bool Jump(float velY, bool useExtraJumps = false)
        {
            if (m_dead || stunned) return false;
            if (input.jumpDown) // Jump
            {
                // If jumping mid-air and not enough extra jumps, abort
                if (useExtraJumps)
                {
                    if (m_jumps <= 0) return false;
                    else
                    {
                        doubleJumping = true;
                        if (!BucketsGameManager.IsRusher()) 
                            m_jumps--;
                        if (!dashing) ChangeState(CharacterStates.Airborne, true);
                        VFXPool.instance.PlayVFX("DoubleJumpVFX", sprite.transform.position, facing == Facing.Left);
                    }
                }
                SetAirborne(); //Setting this here so slope fixes get ignored
                Debug.Log("Jump!");
                rb.velocity = new Vector2(rb.velocity.x, velY);
                wallClimb = false;
                AudioManager.instance.PlaySFX(SFXList.instance.jumpSFX);
                return true;
            }
            return false;
        }
        public override bool Kill(Vector2 launch)
        {
            if (m_dead) return false;
            m_dead = true;
            SetAirborne();
            StopDash();
            launch *= 40f;
            rb.velocity = (launch);
            AudioManager.instance.PlaySFX(SFXList.instance.playerDeadSFX);
            BucketsGameManager.instance.OnPlayerDead();
            return true;
        }
        public void Slowdown()
        {
            if (m_dead) return;
            m_slowDownTicks = slowDownDuration;
        }
        public bool Stun(Vector2 launch)
        {
            if (m_dead) return false;
            if (stunned) return true;
            //SetAirborne();
            SetStunned();
            StopDash();
            launch *= 20f;
            rb.velocity = (launch);
            return true;
        }
        public void SetStunned()
        {
            m_stunTicks = stunDuration;
        }
        public override void ChangeFacing(Facing newFacing)
        {
            facing = newFacing;
            if (!flipLock) animHandler?.FlipSprite(facing);
            //GroundedAnimationStateCheck();
        }

        public void ChangeState(CharacterStates state, bool forcePlaySameAnim = false)
        {
            animHandler.ChangeAnimationState(this, state, forcePlaySameAnim);
            lastState = state;
        }
        private void GroundedAnimationStateCheck()
        {
            //if (!grounded || jumping || hardLanded || climbing || falling) return;
            if (!grounded || dashing) return;

            if (Mathf.Abs(rb.velocity.x) >= 0.0001f && input.inputH != 0)
            {
                //if (lastState != CharacterStates.Walk) weapon.CancelAnim();
                ChangeState(CharacterStates.Walk);
            }
            else { 
                //if (lastState != CharacterStates.Idle) weapon.CancelAnim(); 
                ChangeState(CharacterStates.Idle); }  
        }
        public Vector2 DistanceToMouse()
        {
            return input.MousePointWorld - rb.position;
        }
        public Vector2 DistanceToPoint(Vector2 mousePos)
        {
            return mousePos - rb.position;
        }
        public float AngleToMouse()
        {
            return Vector2.SignedAngle(rb.position, input.MousePointWorld);
        }
        public void ForceMovementRight()
        {
            GameUtility.KillTween(ref movementTween);
            float duration = 1.5f;
            movementTween = DOTween.To(() => input.inputH, value => input.inputH = 1f, 0f, duration)
                .OnComplete(() => input.inputH = 0);
        }
        public void ForceSmallMovementRight()
        {
            GameUtility.KillTween(ref movementTween);
            float duration = 0.5f;
            movementTween = DOTween.To(() => input.inputH, value => input.inputH = 1f, 0f, duration)
                .OnComplete(() => input.inputH = 0);
        }
        public float GetDashSpeed()
        {
            if (BucketsGameManager.IsRusher())
                return rusherDashSpeed;
            
            return dashSpeed;
        }
    }
}

