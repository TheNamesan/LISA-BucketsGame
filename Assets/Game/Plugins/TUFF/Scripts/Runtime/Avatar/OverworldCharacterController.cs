using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TUFF
{
    public enum FaceDirections { East = 0, West = 1, North = 2, South = 3 };
    public enum CharacterStates { Standing = 0, Walk = 1, Falling = 2, Jump = 3, HardLanding = 4, Climbing = 5 };
    public enum CharacterRunMode { Default = 0, Prep = 1 }
    public enum CharacterClimbMode { Default = 0, Ladder = 1 }
    public enum CharacterHorizontalDirection { Right = 0, Left = 1 }
    public class OverworldCharacterController : MonoBehaviour
    {
        [Header("References")]
        public CharacterAnimationHandler animHandler;
        public Rigidbody2D rb;
        public Collider2D col;
        public SpriteRenderer sprite;
        [SerializeField] ClimbableDetector climbDetect;

        [Header("Character Properties")]
        public float characterHeight = 1f;

        [Header("Sounds")]
        [SerializeField] AudioClip jump;
        [SerializeField] AudioClip hardLanding;
        [SerializeField] AudioClip land;

        [Header("Horizontal Movement")]
        [Tooltip("Avatar speed when walking.")]
        [SerializeField] float moveSpeed = 4;
        [Tooltip("Avatar speed when running (pressing Run button).")]
        [SerializeField] float runSpeed = 7;
        [Tooltip("Determines the behaviour for character run movement.\n" +
            "Default: Character runs any time the run button is pressed.\n" +
            "Prep: Character needs to stand still while holding the run button before they can run.")]
        public CharacterRunMode runMode;
        protected Vector2 touchingWalkableNormal = Vector2.zero;
        protected Vector2 lastTouchingWalkableNormal = Vector2.zero;
        protected Vector2 touchingWalkablePerp = Vector2.zero;
        protected Vector2 nextTouchingWalkableNormal = Vector2.zero;
        protected Vector2 collisionBoxSize = Vector2.zero;
        protected float collisionBoxDistance = 0f;
        protected float maxNormalDirection = 1.01f;
        [Tooltip("Player can currently control their horizontal input value.")]
        [SerializeField] bool enableHorizontalVelocity;

        [Tooltip("Player can currently control their vertical input value.")]
        [SerializeField] bool enableVerticalVelocity;

        [Header("Gravity")]
        [Tooltip("Avatar gravity under normal conditions (multiplies value set by Physics.gravity).")]
        [SerializeField] float gravityScale;
        [Tooltip("Avatar current gravity applied.")]
        [SerializeField] float gravity;
        [Tooltip("Used when instantiating the Avatar.")]
        public bool muteLandSound = true;

        [Header("Jumping")]
        [Tooltip("Avatar jump up force.")]
        [SerializeField] float jumpForce;
        [Tooltip("Avatar jump down force.")]
        [SerializeField] float jumpDownForce;
        [Tooltip("Avatar horizontal jump force in the X axis.")]
        [SerializeField] float horizontalJumpXForce = 4;
        [Tooltip("Avatar horizontal jump force in the Y axis.")]
        [SerializeField] float horizontalJumpYForce = 6;
        public UnityEvent<Vector2> onJump = new();


        public enum HJumpDir { None = 0, Right = 1, Left = -1 }
        [Tooltip("Avatar current horizontal jumping direction.")]
        [SerializeField] HJumpDir hJumpDir = HJumpDir.None;
        [Tooltip("Avatar current horizontal jumping velocity.")]
        [SerializeField] float hJumpXVelocity = 0;
        [Tooltip("Avatar is currently touching the ground (ground is defined by walkableLayer).")]
        public bool grounded;
        public bool wasGrounded;
        [Tooltip("Avatar is currently in the Hard Landing animation.")]
        public bool hardLanded = false;
        [Tooltip("Avatar's Hard Landing isn't finished.")]
        [SerializeField] bool inHardLandedAnimCD = false;
        [Tooltip("Avatar is currently jumping.")]
        public bool jumping;
        [Tooltip("Avatar current jump direction. 0 Up, 1 Down")]
        public int jumpDirection = 1;
        [Tooltip("The distance to check if the player can jump up in Unity units.")]
        [SerializeField] float jumpDetectionRange;
        [Tooltip("The distance to check if the player can jump down in Unity units.")]
        [SerializeField] float jumpDownDetectionRange;
        [Tooltip("Avatar expected landing position when jumping down.")]
        [SerializeField] Vector2 jumpExpectedLanding;
        protected Vector2 landingNormal;
        //protected IEnumerator jumpCoroutine;
        //protected IEnumerator jumpDownCoroutine;

        [Header("Falling")]
        [SerializeField] bool falling = false;
        [Tooltip("Avatar falling force.")]
        [SerializeField] float fallForce;
        [Tooltip("Position in World Space where the player began to fall.")]
        public Vector3 fallStart;
        [Tooltip("Distance between fallStart and touching the ground position to count as Hard Falling.")]
        [SerializeField] float fallDistanceTilDamage;
        [Tooltip("The amount of damage the Player receives when Hard Falling. The damage applied increments as the distance to fallStart increases.")]
        [SerializeField] int fallDamagePerDistance;
        public Vector2 lastVelocity = Vector2.zero;
        float lastXVelocity;

        [Header("Ground Collision")]
        [Tooltip("The layers that count as ground for the Avatar.")]
        [SerializeField] LayerMask walkableLayers;
        [Tooltip("The layers that count as wall for the Avatar. The layers will stop the player from jumping when in line of sight.")]
        [SerializeField] LayerMask wallLayers;
        [Tooltip("The layers to ignore when checking for ground when jumping up or down.")]
        [SerializeField] LayerMask ignoredLayersAtJumpCheck;

        [Tooltip("Avatar currently touching Walkable.")]
        [SerializeField] Collider2D touchingWalkable;
        [Tooltip("Avatar touched Walkable before falling.")]
        [SerializeField] Collider2D touchingWalkableBeforeFalling;
        [Tooltip("Current Touching Walkable's terrain properties.")]
        [SerializeField] TerrainProperties touchingWalkableProperties;
        [Tooltip("Amount of seconds to wait to ignore Walkable collision when jumping down.")]
        [SerializeField] float ignoreWalkableForSeconds;
        [Tooltip("Avatar is currently ignoring walkable while jumping down.")]
        [SerializeField] bool ignoreWalkable;
        [HideInInspector] public bool ignoreGroundCheck = false;
        [HideInInspector] public bool ignoreFallCheck = false;
        [SerializeField] protected float distancePerStep = 1.15f;
        public float distanceForNextStep = 0f;
        public Vector2 lastPosition;

        [Header("Climbing")]
        [Tooltip("Determines the behaviour for character climb movement.\n" +
            "Default: Character climbs ropes normally.\n" +
            "Ladder: Character makes pauses at fixed intervals when climbing.")]
        public CharacterClimbMode climbMode;
        [Tooltip("The layers that count as ground for the Avatar.")]
        [SerializeField] LayerMask climbableLayers;
        [Tooltip("Avatar is currently climbing.")]
        [SerializeField] bool climbing;
        [Tooltip("Avatar can't currently climb.")]
        [SerializeField] bool disableClimbableCollision;
        [Tooltip("Avatar fell from a rope.")]
        public bool fellAfterClimbing;
        [Tooltip("Avatar speed when climbing.")]
        [SerializeField] float climbingSpeed;
        [Tooltip("Avatar speed when climbing and holding Run button.")]
        [SerializeField] float runningClimbingSpeed;
        [Tooltip("Avatar current taken input in the Y axis when climbing.")]
        [SerializeField] float climbVerticalInput;
        [Tooltip("Avatar current taken input in the X axis for climbing vertically.")]
        [SerializeField] float climbHorizontalInput;
        [SerializeField] float climbFallGravityDelay = 0.2f;
        public int ladderTickMax = 20;
        public int ladderCDMax = 20;
        [HideInInspector] public int ladderTickTimer = 0;
        [HideInInspector] public int ladderCDTimer = 0;
        [SerializeField] int ladderDirection = 0;
        protected IEnumerator climbFallAirborneCoroutine;

        public enum ClimbAction { None = 0, GettingOn = 1, GettingOff = 2, Climbing = 3 };
        [Tooltip("Avatar current action for climbing.")]
        [SerializeField] ClimbAction climbAction = ClimbAction.None;
        [SerializeField] int climbDir;
        [Tooltip("Rope Top Point")]
        [SerializeField] Vector2 ropeTop = new Vector2();
        [Tooltip("Rope Bottom Point")]
        [SerializeField] Vector2 ropeBottom = new Vector2();
        [Tooltip("Rope Left Point")]
        [SerializeField] Vector2 ropeLeft = new Vector2();
        [Tooltip("Rope Right Point")]
        [SerializeField] Vector2 ropeRight = new Vector2();
        [SerializeField] Vector2 ropeCenter = new Vector2();

        //public string lastStateName = "";
        public CharacterStates lastState = CharacterStates.Standing;

        public FaceDirections faceDirection { get { return m_faceDirection; } }
        [Header("Facing")]
        [Tooltip("Avatar current facing.")]
        [SerializeField] FaceDirections m_faceDirection = FaceDirections.East;
        int nextXFacing = 0;
        int nextYFacing = 0;
        int faceX = 1; //Used for animator changes
        int faceY = 0; //Used for animator changes

        [Header("Interaction")]
        [Tooltip("Distance checked to find interactables in the Avatar's facing direction.")]
        [SerializeField] public float interactionDistance = 1f;


        [Header("Run")]
        [SerializeField] public bool runPrepped = false;
        [SerializeField] public bool runMomentum = false;
        [SerializeField] public bool runCanceled = false;
        [SerializeField] public Vector2 runDirection = new Vector2();
        [SerializeField] double runButtonHoldTime = 0d;
        [SerializeField] double runButtonReleaseTime = 0d;
        [SerializeField] double timeForRunPrep = 0.35d;
        [Tooltip("Time to keep the runMomentum variable after releasing the run key.")]
        [SerializeField] double runMomentumReleaseGracePeriod = 0.0d;
        [SerializeField] double runMomentumCancelCD = 0.35d;
        [SerializeField] double runMomentumCancelCDTimer = 0.0d;

        public enum QueuedAction { None = 0, JumpUp = 1, JumpDown = 2, Climb = 3, Interaction = 4, Pause = 99 };
        [Header("Player Actions")]
        public QueuedAction queuedAction = QueuedAction.None;

        [Tooltip("Input")]
        [SerializeField] public CharacterInputStream input = new();
        public CharacterInputStream nextInput = new();
        public CharacterInputStream lastInput { get => m_lastInput; }
        private CharacterInputStream m_lastInput = new();

        [Header("True Input")]
        [Tooltip("Avatar true horizontal input used")]
        public float moveH = 0;
        [Tooltip("Avatar true vertical input used")]
        public float moveV = 0;
        [SerializeField] float lastMoveH = 0;
        [SerializeField] float lastMoveV = 0;

        [Header("Coroutine")]
        protected IEnumerator stopHardLandingCDCoroutine;

        public UnityEvent onUpdate = new();
        public UnityEvent onFixedUpdate = new();
        public UnityEvent onInputUpdate = new();

        [SerializeField] float timeJump = 0;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (!animHandler) animHandler = GetComponent<CharacterAnimationHandler>();
            if (sprite == null) sprite = transform.GetComponentInChildren<SpriteRenderer>();
            if (!col) col = GetComponent<Collider2D>();
            climbFallAirborneCoroutine = EnableGravityDelay();
        }
        private void OnEnable()
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer, true);
        }

        void Start()
        {
            ignoreWalkable = false;
            lastPosition = rb.position;
            SetFallStart();
            muteLandSound = true;
            CheckGround();
            wasGrounded = grounded;
        }

        protected void Update()
        {
            UpdateLogic();
        }

        public void UpdateLogic()
        {
            AnimatorSpeed();
            VelocityZeroCorrection();
            onUpdate?.Invoke();
        }

        public void FixedUpdate()
        {
            lastXVelocity = rb.velocity.x;
            Vector2 last = lastPosition;
            CheckJumpDownExpectedLanding();
            CheckQueues();
            CheckGround();
            ClimbHandler();
            DequeueAction();
            MoveHandler();
            onFixedUpdate?.Invoke();
            queuedAction = QueuedAction.None;
            Vector2 current = rb.position;
            //Debug.Log($"{current != last}. Cur: {current}. Last: {last}");
            m_lastInput = input;
            input = nextInput; // Put here to keep input consistant with followers
            //Debug.Log($"{gameObject.name}: {current}");
            onInputUpdate?.Invoke();
            lastPosition = rb.position;
            lastVelocity = rb.velocity;
            InputUpdate();
            nextInput.interactionButtonDown = false;
            nextInput.runButtonDown = false;
            nextInput.pauseButtonDown = false;
            wasGrounded = grounded;
        }

        private void LateUpdate()
        {
            
        }

        private void VelocityZeroCorrection() // Attempts to fix sprite clipping when crossing other characters
        {
            float min = 0.0001f;
            if (Mathf.Abs(rb.velocity.x) <= min)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            if (Mathf.Abs(rb.velocity.y) <= min)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }

        

        private void QueueAction(QueuedAction actionToQueue)
        {
            queuedAction = actionToQueue;
        }

        private void DequeueAction()
        {
            if (queuedAction == QueuedAction.None) return;
            switch (queuedAction)
            {
                case QueuedAction.JumpUp:
                    JumpCheck();
                    break;
                case QueuedAction.JumpDown:
                    JumpDownCheck();
                    break;
                case QueuedAction.Climb:
                    CheckClimb();
                    break;
                case QueuedAction.Interaction:
                    CheckInteractable();
                    break;
                case QueuedAction.Pause:
                    CheckPause();
                    break;
            }
        }

        private void CheckGround()
        {
            Vector2 closestContactPoint = col.ClosestPoint((Vector2)col.bounds.center + Vector2.down * col.bounds.size);
            Vector2 boxCenter = closestContactPoint;
            float DCOMultiplier = 0.1f;
            collisionBoxSize = new Vector2(col.bounds.size.x, Physics2D.defaultContactOffset * DCOMultiplier);
            Vector2 boxExtents = collisionBoxSize * 0.5f;
            collisionBoxDistance = (rb.velocity.y > -10 ? collisionBoxSize.y * 10f : collisionBoxSize.y * 200f) /*+ (WasOnSlope() ? 0.3f : 0f)*/; //Has a higher distance check if velocity is higher (mainly when falling or jumping down)
            RaycastHit2D collision = Physics2D.BoxCast(closestContactPoint, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance, walkableLayers);
            Vector2 nextOrigin = closestContactPoint;
            RaycastHit2D nextCollision = Physics2D.BoxCast(nextOrigin, collisionBoxSize, 0f, Vector2.down, 0.15f/*(IsOnSlope() ? 0.5f : 0.15f)*/, walkableLayers);

            bool slopeFound = false;
            Collider2D collider = collision.collider;
            Vector2 normal = collision.normal;
            Vector2 point = collision.point;
            Color boxColor;
            /*if (nextCollision && !collision)
            {
                collider = nextCollision.collider;
                normal = nextCollision.normal;
                point = nextCollision.point;
            }*/

            //Debug.DrawRay(slopeNormalHit.point, slopeNormalHit.normal, Color.yellow);
            //Debug.DrawLine(closestContactPoint + rb.velocity * Time.deltaTime, slopeNormalHit.point, Color.blue);

            if ((collision /*|| (nextCollision && grounded)*/) && !IsWalkableLayerCollisionDisabled()) //OnCollisionEnter/Stay
            {
                if (touchingWalkable == collider || grounded)
                {
                    boxColor = Color.yellow;
                }
                //if (grounded && !collision/*!collision && nextCollision*/)
                //{
                //    Debug.Log(nextCollision.normal);
                //    //RaycastHit2D hitW = Physics2D.BoxCast(rb.position, boxSize, 0f, Vector2.down, Vector2.Distance(closestContactPoint, rb.position) * 1.5f, walkableLayers);
                //    Vector2 landingPosition = nextCollision.point;
                //    float moveBuffer = (climbing ? Vector2.Distance(closestContactPoint, landingPosition) : Physics2D.defaultContactOffset * DCOMultiplier); //if climbing, it places the player a little higher, since it's most likely it will collide deeper into the ground
                //    float moveToY = landingPosition.y + Vector2.Distance(closestContactPoint, rb.position) - moveBuffer * 250f;
                //    if (rb.velocity.y > -runSpeed) rb.MovePosition(new Vector2(rb.position.x, moveToY)); //if velocity is too small, smooth out the positioning
                //    else rb.position = new Vector2(rb.position.x, moveToY); //else position the player instantly
                //    boxColor = Color.yellow;
                //    collider = nextCollision.collider;
                //    normal = nextCollision.normal;
                //    point = nextCollision.point;
                //}
                if (grounded && touchingWalkable != collider)
                {
                    Debug.Log(collider + " (" + collision.collider + ")," + slopeFound + "," + touchingWalkableNormal);
                    SetTouchingWalkable(collider);
                    boxColor = Color.yellow;
                }
                if (!grounded && touchingWalkable == null /*&& normal.y > 0*/ && rb.velocity.y < 0.1f && !ignoreGroundCheck)
                {
                    SnapToGround(closestContactPoint, DCOMultiplier, collider, point);
                    TouchLand(collider);
                    boxColor = Color.green;
                }
                else
                {
                    boxColor = Color.magenta;
                }

                lastTouchingWalkableNormal = touchingWalkableNormal;
                if (!collision && nextCollision)
                {
                    touchingWalkableNormal = nextCollision.normal;
                    touchingWalkablePerp = Vector2.Perpendicular(touchingWalkableNormal).normalized;
                }
                else
                {
                    touchingWalkableNormal = collision.normal;
                    touchingWalkablePerp = Vector2.Perpendicular(touchingWalkableNormal).normalized;
                }


                //Debug.DrawLine(transform.position, collision.point, Color.yellow);
                //Debug.DrawRay(collision.point, touchingWalkableNormal, Color.cyan);
                //Debug.DrawRay(collision.point, touchingWalkablePerp, Color.green);
                if (nextCollision) { Debug.DrawLine(nextOrigin, nextCollision.point, new Color32(67, 171, 255, 255)); }
            }
            else //OnCollisionExit
            {
                if (IsWalkableLayerCollisionDisabled())
                {
                    boxColor = Color.white;
                }
                else boxColor = Color.red;
                if (!climbing && grounded && !ignoreFallCheck)
                {
                    Debug.Log("Ungrounded && WasOnSlope: " + WasOnSlope());
                    SetupFall();
                }
                else
                {
                    touchingWalkableBeforeFalling = touchingWalkable;
                    SetTouchingWalkable(null);
                }
            }

            float displayTime = 0f;
            Debug.DrawLine(new Vector2(boxCenter.x + boxExtents.x, boxCenter.y - boxExtents.y),
                new Vector2(boxCenter.x + boxExtents.x, boxCenter.y + boxExtents.y), boxColor, displayTime);
            Debug.DrawLine(new Vector2(boxCenter.x + boxExtents.x, boxCenter.y + boxExtents.y),
               new Vector2(boxCenter.x - boxExtents.x, boxCenter.y + boxExtents.y), boxColor, displayTime);
            Debug.DrawLine(new Vector2(boxCenter.x - boxExtents.x, boxCenter.y + boxExtents.y),
               new Vector2(boxCenter.x - boxExtents.x, boxCenter.y - boxExtents.y), boxColor, displayTime);
            Debug.DrawLine(new Vector2(boxCenter.x - boxExtents.x, boxCenter.y - boxExtents.y),
               new Vector2(boxCenter.x + boxExtents.x, boxCenter.y - boxExtents.y), boxColor, displayTime);

            ignoreGroundCheck = false;
            ignoreFallCheck = false;
        }

        private void SnapToGround(Vector2 closestContactPoint, float DCOMultiplier, Collider2D collider, Vector2 point)
        {
            RaycastHit2D hitW = Physics2D.Raycast(rb.position, Vector2.down, Vector2.Distance(closestContactPoint, rb.position) * 1.25f, 1 << collider.gameObject.layer);
            Vector2 landingPosition = (hitW ? hitW.point : point);
            float moveBuffer = (climbing ? Vector2.Distance(closestContactPoint, landingPosition) : Physics2D.defaultContactOffset * DCOMultiplier); //if climbing, it places the player a little higher, since it's most likely it will collide deeper into the ground
            float moveToY = landingPosition.y + Vector2.Distance(closestContactPoint, rb.position) + moveBuffer;
            if (rb.velocity.y > -1f) { rb.MovePosition(new Vector2(rb.position.x, moveToY)); }  //if velocity is too small, smooth out the positioning
            else rb.position = new Vector2(rb.position.x, moveToY); //else position the player instantly
        }

        private void AnimatorSpeed()
        {
            GroundedAnimationStateCheck();
            ClimbingAnimationStateCheck();
        }
        public void SetSceneChangeFrameConditions()
        {
            //muteLandSound = true;
            ignoreFallCheck = true;
        }
        private void GroundedAnimationStateCheck()
        {
            if (!grounded || jumping || hardLanded || climbing || falling) return;

            if (Mathf.Abs(rb.velocity.x) > 0.000001f && moveH != 0)
            {
                SetHardLanded(false);
                ChangeState(CharacterStates.Walk);
            }
            else { ChangeState(CharacterStates.Standing); }
        }
        private void ClimbingAnimationStateCheck()
        {
            if (climbing)
            {
                ChangeState(CharacterStates.Climbing);
            } 
        }

        private void CheckJumpDownExpectedLanding()
        {
            if (ignoreWalkable)
            {
                Vector2 closestContactPoint = col.ClosestPoint(rb.position - new Vector2(0, characterHeight));
                if ((jumpDirection >= 1 && closestContactPoint.y <= jumpExpectedLanding.y) ||
                    (jumpDirection <= 0 && closestContactPoint.y >= jumpExpectedLanding.y))
                {
                    ignoreWalkable = false;
                    jumpExpectedLanding = Vector2.zero;

                    Debug.Log("top kek");
                    DisableWalkableLayersCollision(false);
                }
                else
                {
                    float offset = 0.5f;
                    Vector2 a = closestContactPoint + new Vector2(-offset, 0);
                    Vector2 b = closestContactPoint + new Vector2(+offset, 0);
                    Debug.DrawLine(a, b, Color.cyan, 0.5f);
                    Debug.DrawLine(closestContactPoint, jumpExpectedLanding, Color.white, 0.5f);
                } 
            }
        }

        private void InputUpdate()
        {
            
            //if (GameManager.disablePlayerInput)
            //{
            //    runButtonDown = false;
            //    runButtonHold = false;
            //}
            // Horizontal Input
            //if ()
            //if (!GameManager.disablePlayerInput)
            //{
            //    horizontalInput = playerHorizontalInput;
            //    horizontalInputTap = playerHorizontalInputTap;
            //}

            //if (!GameManager.disablePlayerInput)
            //{
                if (input.horizontalInputTap > 0)
                {
                    nextXFacing = 1;
                }
                else if (input.horizontalInputTap < 0)
                {
                    nextXFacing = -1;
                }
            //}
            if (Mathf.Abs(input.horizontalInputTap) > 0)
            {
                input.horizontalInputTap = 0;
            }
            // Vertical Input

            //if (!GameManager.disablePlayerInput)
            //{
                if (input.verticalInput > 0)
                {
                    nextYFacing = 1;
                }
                else if (input.verticalInput < 0)
                {
                    nextYFacing = -1;
                }
            //}
            if (Mathf.Abs(input.verticalInputTap) > 0)
            {
                input.verticalInputTap = 0;
            }
            // Z
            if (input.interactionButtonDown)
            {
                bool disallowedByRunMode = false;
                if (runMode == CharacterRunMode.Prep && (InRunningState() || runCanceled)) disallowedByRunMode = true;
                if (!climbing && !disallowedByRunMode)
                {
                    if (input.verticalInput > 0) QueueAction(QueuedAction.JumpUp);
                    else if (input.verticalInput < 0) QueueAction(QueuedAction.JumpDown);
                }
                if (queuedAction == QueuedAction.None)
                    QueueAction(QueuedAction.Interaction);
                //Debug.Log(queuedAction);
            }
            // Shift
            if (PlayerData.instance.charProperties.disableRun) input.runButtonHold = false;
            if (input.runButtonHold && runMode == CharacterRunMode.Prep)
            {
                if (!inHardLandedAnimCD && hardLanded)
                    RecoverFromHardLanding();
            }
            if (input.runButtonHold && !GameManager.disablePlayerInput) //tmp disable player input
            {
                if (lastState == CharacterStates.Standing && faceX != 0)
                {
                    runButtonHoldTime += Time.deltaTime;
                    if (IsRunPrepTimerReached()) { 
                        runPrepped = true;
                        runMomentum = false;
                        runButtonReleaseTime = 0;
                    }
                }
                else {
                    runButtonHoldTime = 0; 
                }
            }
            else
            {
                runButtonHoldTime = 0;
            }
            if (runPrepped && lastState == CharacterStates.Standing && !input.runButtonHold)
            {
                runPrepped = false;
                runMomentum = true;
                runDirection.x = faceX;
            }
            if (!runMomentum) runButtonReleaseTime = 0;
            if (input.verticalInput != 0 || (input.horizontalInput != 0 && input.horizontalInput != runDirection.x))
            {
                StopRunMomentum();
            }
            bool groundedSpeedIsLow = runMomentum && grounded && Mathf.Abs(rb.velocity.x) <= 0.0001f;
            if (runCanceled || groundedSpeedIsLow)
            {
                runMomentumCancelCDTimer += Time.deltaTime;
                if (runMomentumCancelCDTimer >= runMomentumCancelCD)
                {
                    runCanceled = false;
                    runMomentum = false;
                    runMomentumCancelCDTimer = 0d;
                }
            }
            else runMomentumCancelCDTimer = 0d;
            // Pause
            if (input.pauseButtonDown)
            {
                input.pauseButtonDown = false;
                if (queuedAction == QueuedAction.None && FollowerInstance.player.controller == this)
                    QueueAction(QueuedAction.Pause);
            }
        }

        public void StopRunMomentum()
        {
            if (runMomentum)
            {
                runMomentum = false;
                runCanceled = true;
                moveH = 0;
                runMomentumCancelCDTimer = 0d;
            }
            runPrepped = false;
            runButtonHoldTime = 0;
            runButtonReleaseTime = 0;
        }

        private bool CheckInteractable()
        {
            if (GameManager.disablePlayerInput) return false;
            if (grounded && rb.velocity.y == 0 && input.horizontalInput == 0)
            {
                if (hardLanded) return false;
                Debug.DrawRay(transform.position, Vector2.right * interactionDistance * faceX, Color.cyan, 3);
                RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.right * faceX, interactionDistance, LayerMask.GetMask("Interactable"));
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i])
                    {
                        InteractableObject interactable
                            = hit[i].transform.GetComponent<InteractableObject>();
                        if (interactable == null)
                        {
                            continue;
                        }
                        TriggerType type = interactable.GetCurrentIndexType();
                        if (type == TriggerType.ActionButton)
                        {
                            if (interactable.TriggerInteractable()) return true;
                        }
                        else if (type == TriggerType.ActionFaceUp)
                        {
                            if (m_faceDirection == FaceDirections.North)
                                if (interactable.TriggerInteractable()) return true;
                        }
                        else if (type == TriggerType.ActionFaceDown)
                        {
                            if (m_faceDirection == FaceDirections.South)
                                if (interactable.TriggerInteractable()) return true;
                        }
                        //return true;
                    }
                }
            }
            else if (climbing)
            {
                CheckClimbFall();
                return true;
            }
            return false;
        }

        private void CheckQueues()
        {
            if (queuedAction != QueuedAction.None) return;
            CheckClimbQueue();
        }
        private void CheckClimbQueue()
        {
            //if (GameManager.disablePlayerInput) return;
            if (climbing || hardLanded || jumping) return;
            bool prepRun = (runMode == CharacterRunMode.Prep && runMomentum && !grounded);
            if (!(input.verticalInput != 0 || (!grounded && input.horizontalInput != 0) || prepRun)) return;
            if (climbDetect.climbable == null) return;
            if (disableClimbableCollision && climbDetect.IntersectsLastClimbPoints()) return;
            QueueAction(QueuedAction.Climb);
        }
        private void CheckClimb()
        {
            if (climbing || hardLanded || jumping) return;
            bool prepRun = (runMode == CharacterRunMode.Prep && runMomentum && !grounded);
            if (!(input.verticalInput != 0 || (!grounded && input.horizontalInput != 0) || prepRun) ) return;
            if (climbDetect.climbable == null) return;
            if (disableClimbableCollision && climbDetect.IntersectsLastClimbPoints()) return;

            Vector2 closestContactPoint = col.ClosestPoint((Vector3)rb.position - new Vector3(0, characterHeight, 0));
            Vector2 upperPoint = closestContactPoint + Vector2.up * col.bounds.size.y;
            UpdateRopePoints();

            ropeCenter = climbDetect.pointsCenter;

            if (grounded) // Find rope while on ground
            {
                bool ropeIsDown = (closestContactPoint.y + 0.01f >= ropeTop.y);
                if (input.verticalInput < 0 && !ropeIsDown)
                {
                    // If walkable layer is on the way down
                    RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.down, characterHeight * 0.75f, walkableLayers);
                    float offset = 0.1f;
                    float half = offset * 0.5f;
                    bool noRopeBelow = LISAUtility.WithinValueRange(ropeBottom.y + half, ropeBottom.y - half, hit.point.y);
                    if (noRopeBelow)
                    {
                        return; 
                    }
                }
                else if (input.verticalInput > 0 && ropeIsDown)
                {
                    return;
                }
                rb.velocity = Vector2.zero;
                rb.MovePosition(new Vector2(ropeCenter.x, rb.position.y));
                SetClimbAction(ClimbAction.GettingOn, ropeIsDown ? -1 : 1);
                SetupClimb();
            }
            else if (!grounded) // Find rope while in the air
            {
                rb.velocity = Vector2.zero;
                rb.MovePosition(new Vector2(ropeCenter.x, rb.position.y));
                closestContactPoint = col.ClosestPoint((Vector3)rb.position - new Vector3(0, characterHeight, 0));
                upperPoint = closestContactPoint + Vector2.up * col.bounds.size.y;
                if (upperPoint.y > ropeTop.y)
                {
                    rb.MovePosition(new Vector2(ropeCenter.x, ropeTop.y + Vector2.Distance(upperPoint, rb.position) + Physics2D.defaultContactOffset));
                }
                else if (closestContactPoint.y < ropeBottom.y)
                {
                    rb.MovePosition(new Vector2(ropeCenter.x, ropeBottom.y + Vector2.Distance(closestContactPoint, rb.position) - Physics2D.defaultContactOffset));
                }
                SetClimbAction(ClimbAction.Climbing, 0);
                SetupClimb();
            }
            ChangeFaceDirection(FaceDirections.North);
        }

        public void UpdateRopePoints()
        {
            if (climbDetect == null) return;
            if (climbDetect.climbablePoints.Length == 0) return;
            ropeTop = Vector2.Lerp(climbDetect.climbablePoints[2], climbDetect.climbablePoints[1], 0.5f); // Top
            ropeBottom = Vector2.Lerp(climbDetect.climbablePoints[3], climbDetect.climbablePoints[0], 0.5f); // Bottom
            ropeLeft = Vector2.Lerp(climbDetect.climbablePoints[3], climbDetect.climbablePoints[2], 0.5f); // Left
            ropeRight = Vector2.Lerp(climbDetect.climbablePoints[0], climbDetect.climbablePoints[1], 0.5f); // Right
        }

        private void SetupClimb()
        {
            falling = false;
            CancelLadder();
            SetClimbing(true);
            SetGrounded(false);
            EnableGravity(false);
            DisableWalkableLayersCollision(true);
            enableVerticalVelocity = true;
            fellAfterClimbing = false;
            StopCoroutine(climbFallAirborneCoroutine);
            disableClimbableCollision = false;
            hJumpXVelocity = 0;
            hJumpDir = HJumpDir.None;
            climbDetect?.terrainProperties?.OnEnter(this);
        }

        protected void MoveHandler()
        {
            lastMoveH = moveH;
            lastMoveV = moveV;
            moveH = enableHorizontalVelocity ? input.horizontalInput : 0;
            moveV = enableVerticalVelocity ? input.verticalInput : rb.velocity.y;
            //if (GameManager.disablePlayerInput)
            //{
            //    moveH = 0;
            //    moveV = 0;
            //}
            if (hardLanded)
            {
                if (!inHardLandedAnimCD && (moveH != 0 || moveV != 0))
                    RecoverFromHardLanding();
                else
                {
                    rb.velocity = new Vector3(0, rb.velocity.y);
                    return;
                }
            }
            if (climbing) //Climbing
            {
                Vector2 closestContactPoint = col.ClosestPoint(rb.position - new Vector2(0, characterHeight));
                Vector2 upperPoint = closestContactPoint + Vector2.up * col.bounds.size.y;

                if (closestContactPoint.y <= ropeBottom.y)
                {
                    if (moveV < 0) moveV = 0;
                }
                else if (upperPoint.y >= ropeTop.y)
                {
                    if (moveV > 0) moveV = 0;
                }
                if (climbAction == ClimbAction.GettingOn)
                {
                    Debug.Log("GettingOn");
                    if (climbDir > 0) //Rope is Up
                    {
                        if (closestContactPoint.y >= ropeBottom.y)
                        {
                            SetClimbAction(ClimbAction.Climbing, 0);
                        }
                        else moveV = climbDir;
                    }
                    else if (climbDir < 0) //Rope is Down
                    {
                        if (upperPoint.y <= ropeTop.y)
                        {
                            SetClimbAction(ClimbAction.Climbing, 0);
                        }
                        else moveV = climbDir;
                    }
                }
                else if (climbAction == ClimbAction.GettingOff)
                {
                    Debug.Log("GettingOff");
                    if (climbDir > 0)  //Landing Up
                    {
                        RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.down, Vector2.Distance(closestContactPoint, rb.position) * 1.5f, walkableLayers);
                        if (hit)
                        {
                            rb.velocity = Vector2.zero;
                            rb.MovePosition(new Vector2(rb.position.x, hit.point.y + Vector2.Distance(closestContactPoint, rb.position)) + Vector2.up * 0.005f);
                        }
                        else SetClimbAction(ClimbAction.Climbing, 0);
                    }
                    else if (climbDir < 0) //Landing Down
                    {
                        RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.down, Vector2.Distance(closestContactPoint, rb.position) * 1.25f, walkableLayers);
                        if (hit)
                        {
                            moveV = -1;
                        }
                        else SetClimbAction(ClimbAction.Climbing, 0);
                    }
                }
                else if (climbAction == ClimbAction.Climbing)
                {
                    if (climbMode == CharacterClimbMode.Ladder) // Ladder logic;
                    {
                        if (ladderTickTimer <= 0)
                        { 
                            if (ladderCDTimer <= 0)
                            {
                                if (moveV != 0 && ladderDirection == 0) // Wait for direction
                                {
                                    ladderDirection = (int)moveV;
                                    ladderTickTimer = ladderTickMax; // Set up first timer
                                }
                            }
                            else
                            {
                                ladderCDTimer -= 1;
                                if (ladderCDTimer <= 0)
                                {
                                    if (moveV != 0 && ladderDirection == 0) // Wait for direction
                                    {
                                        ladderDirection = (int)moveV;
                                        ladderTickTimer = ladderTickMax; // Set up first timer
                                    }
                                }
                                else moveV = 0;
                            }
                        }
                        else
                        {
                            ladderTickTimer -= 1; 
                            if (ladderTickTimer <= 0)
                            {
                                ladderDirection = 0;
                                ladderCDTimer = ladderCDMax; // Set up second timer
                            }
                            moveV = ladderDirection;
                        }
                    }
                }
                float move = moveV * climbingSpeed/*(runButtonHold ? runningClimbingSpeed : climbingSpeed)*/;
                var expectedPosition = closestContactPoint + (Vector2.up * move * Time.fixedDeltaTime);
                float distanceDownToRb = Vector2.Distance(closestContactPoint, rb.position);
                if (move == 0)
                {
                    if (nextXFacing > 0)
                        ChangeFaceDirection(FaceDirections.East);
                    else if (nextXFacing < 0)
                        ChangeFaceDirection(FaceDirections.West);
                }
                else ChangeFaceDirection(FaceDirections.North);
                if (move < 0 && expectedPosition.y < ropeBottom.y && climbAction != ClimbAction.GettingOff) // Rope is below
                {
                    RaycastHit2D hit = Physics2D.Raycast(closestContactPoint, Vector2.down, Vector2.Distance(closestContactPoint, expectedPosition), walkableLayers);
                    if (hit)
                    {
                        move = 0;
                        rb.velocity = Vector2.zero;
                        rb.MovePosition(new Vector2(rb.position.x, hit.point.y + distanceDownToRb - Physics2D.defaultContactOffset * 1f));
                        DisableWalkableLayersCollision(false);
                    }
                }
                Debug.DrawLine(closestContactPoint, expectedPosition, Color.cyan); //Next expected position
                //if (climbAction == ClimbAction.GettingOn)
                //{
                //    Debug.Log($"{gameObject.name}: {rb.position}. T: {Time.fixedTime}");
                //}
                rb.velocity = new Vector3(0, move);
            }
            else if (grounded && !jumping) // Walking & Standing
            {
                //rb.velocity = new Vector3(rb.velocity.x , 0);
                if (runMode == CharacterRunMode.Prep)
                {
                    if (InRunningState()) moveH = runDirection.x;
                    if (runCanceled) { moveH = 0; nextYFacing = 0; };
                }
                AdjustFacingOnMoveDirection(moveH);
                float movementSpeed = (InRunningState() ? runSpeed : moveSpeed);
                Vector2 closestContactPoint = col.ClosestPoint((Vector2)col.bounds.center + Vector2.down * col.bounds.size);
                Vector2 velocity = Vector2.zero;
                /*if (IsOnSlope()) //if on slope
                {
                    velocity = new Vector3(-touchingWalkablePerp.x, -touchingWalkablePerp.y) * movementSpeed * moveH;
                }
                else*/
                {
                    
                    float speed = movementSpeed * moveH;
                    velocity = new Vector3(speed, 0);
                }
                Vector2 expectedContactPosition = closestContactPoint + velocity * Time.fixedDeltaTime;
                RaycastHit2D slopeNormalHit = Physics2D.BoxCast(expectedContactPosition, collisionBoxSize, 0f, Vector2.down, collisionBoxDistance, walkableLayers);
                Debug.DrawRay(slopeNormalHit.point, slopeNormalHit.normal, Color.yellow);
                Debug.DrawLine(expectedContactPosition, slopeNormalHit.point, Color.blue);
                //Debug.DrawLine(closestContactPoint, expectedPosition, Color.white, 1f);
                /*if (moveH != 0 && slopeNormalHit && IsOnSlope() && slopeNormalHit.normal.y >= 1f
                    && expectedContactPosition.y > slopeNormalHit.point.y)
                {
                    //Debug.Log("too high up: " + expectedContactPosition.y + "vs" + slopeNormalHit.point.y);
                    float moveBuffer = Physics2D.defaultContactOffset;
                    float moveToY = slopeNormalHit.point.y + Vector2.Distance(closestContactPoint, rb.position) - moveBuffer;
                    Vector2 direction = (slopeNormalHit.point - closestContactPoint).normalized;
                    float magnitude = Vector2.Distance(closestContactPoint, slopeNormalHit.point);
                    //velocity = new Vector2(velocity.x, direction.y);
                    rb.MovePosition(new Vector2(expectedContactPosition.x, moveToY));
                    velocity = Vector2.zero;
                    //velocity = new Vector2(velocity.x, moveToY);

                }*/
                Debug.DrawLine(closestContactPoint, expectedContactPosition, Color.red, 0f);
                rb.velocity = velocity;
                
                if (moveH == 0) //if moving
                {
                    if (nextYFacing > 0)
                        ChangeFaceDirection(FaceDirections.North);
                    else if (nextYFacing < 0)
                        ChangeFaceDirection(FaceDirections.South);
                }
                else //Step Logic
                {
                    //Debug.Log(lastPosition + ", " + rb.position);
                    //distanceForNextStep += Mathf.Abs(rb.position.x - lastPosition.x);
                    //if (distanceForNextStep >= distancePerStep)
                    //{
                    //    Step();
                    //}
                }
            }
            else if (hJumpXVelocity != 0) //Horizontal Fall
            {
                rb.velocity = new Vector3(hJumpXVelocity, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector3(0, rb.velocity.y);
            }
            nextXFacing = 0;
            nextYFacing = 0;
        }

        private void AdjustFacingOnMoveDirection(float horizontal)
        {
            if (horizontal > 0)
            {
                ChangeFaceDirection(FaceDirections.East);
            }
            else if (horizontal < 0)
            {
                ChangeFaceDirection(FaceDirections.West);
            }
        }
        protected void CancelLadder()
        {
            ladderDirection = 0;
            ladderTickTimer = 0;
            ladderCDTimer = 0;
        }
        private void CheckClimbFall()
        {
            ClimbFall();
            //RaycastHit2D hit;
            //Vector2 closestContactPoint = col.ClosestPoint((Vector2)rb.position - new Vector2(0, characterHeight));
            //hit = Physics2D.Linecast(new Vector2(transform.position.x, transform.position.y + characterHeight * 0.25f),
            //            new Vector2(closestContactPoint.x, closestContactPoint.y - 0.25f), walkableLayers);
            //if (!hit)
            //{
            //    ClimbFall();
            //    return;
            //}
            //else
            //{
            //    Debug.Log("TooCloseToGround");
            //}
        }

        void ClimbHandler()
        {
            if (climbing)
            {
                UpdateRopePoints();
                RaycastHit2D hit;
                Vector2 closestContactPoint = col.ClosestPoint((Vector2)rb.position - new Vector2(0, characterHeight));
                float distanceDownToRb = Vector2.Distance(closestContactPoint, rb.position);
                Vector2 upperPoint = closestContactPoint + Vector2.up * col.bounds.size.y;

                Debug.DrawLine(rb.position, closestContactPoint, Color.green);
                /*if (interactionButtonDown) //Move to Update
                {
                    QueueAction(QueuedAction.ClimbFall);
                }*/
                if (climbAction == ClimbAction.GettingOn) return;
                else if (climbAction == ClimbAction.GettingOff) //Smooth Climb GettingOff
                {
                    if (climbDir < 0) //Landing Down
                    {
                        hit = Physics2D.Raycast(rb.position, Vector2.down, distanceDownToRb * 1.25f, walkableLayers);
                        if (!hit) return;
                        if (rb.position.y - distanceDownToRb <= hit.point.y)
                        {
                            rb.position = new Vector2(rb.position.x, hit.point.y + distanceDownToRb + Physics2D.defaultContactOffset * 0.51f);
                            rb.velocity = Vector2.zero;
                        }
                    }
                    else if (climbDir > 0) //Landing Up
                    {
                        hit = Physics2D.Raycast(rb.position, Vector2.down, distanceDownToRb * 1.25f, walkableLayers);
                        if (!hit) return;
                        if (rb.position.y - distanceDownToRb >= hit.point.y)
                        {
                            rb.position = new Vector2(rb.position.x, hit.point.y + distanceDownToRb + Physics2D.defaultContactOffset * 0.51f);
                            rb.velocity = Vector2.zero;
                            DisableWalkableLayersCollision(false);
                        }
                    }
                    return;
                }
                if (closestContactPoint.y < ropeBottom.y)
                {
                    //Debug.Log("Out of rope downwards!");
                    CancelLadder();
                    hit = Physics2D.Raycast(rb.position, Vector2.down, distanceDownToRb * 1.25f, walkableLayers);
                    if (hit)
                    {
                        rb.velocity = Vector2.zero;
                        DisableWalkableLayersCollision(false);
                        enableVerticalVelocity = false;
                        SetFallStart();
                        ChangeFaceDirection(FaceDirections.South);
                        SetClimbAction(ClimbAction.GettingOff, -1);
                    }
                }
                else if (upperPoint.y > ropeTop.y)
                {
                    Debug.Log("Out of rope upwards!");
                    CancelLadder();
                    hit = Physics2D.Raycast(rb.position, Vector2.down, distanceDownToRb * 1.25f, walkableLayers);
                    if (hit)
                    {
                        rb.velocity = Vector2.zero;
                        
                        enableVerticalVelocity = false;
                        SetFallStart();
                        ChangeFaceDirection(FaceDirections.North);
                        SetClimbAction(ClimbAction.GettingOff, 1);
                    }
                }
            }
        }

        private void SetClimbAction(ClimbAction action, int direction)
        {
            climbAction = action;
            climbDir = (int)Mathf.Sign(direction);
        }

        private void ClimbFall()
        {
            if (PlayerData.instance.charProperties.disableRopeJump) return;
            rb.velocity = Vector2.zero;
            CancelLadder();
            ChangeFaceDirection(FaceDirections.South);
            SetClimbing(false);
            StartCoroutine(ReenableCollisionDelay());
            //DisableWalkableLayersCollision(false);
            fellAfterClimbing = true;
            disableClimbableCollision = true;
            climbDetect.SetLastPoints();
            enableVerticalVelocity = false;
            
            SetupFall();
        }

        public void ChangeFaceDirection(FaceDirections newFacing)
        {
            m_faceDirection = newFacing;
            FaceDirectionToInt();
            GroundedAnimationStateCheck();
            ClimbingAnimationStateCheck();
        }

        void FaceDirectionToInt()
        {
            switch (m_faceDirection)
            {
                case FaceDirections.East:
                    faceX = 1;
                    faceY = 0;
                    break;

                case FaceDirections.West:
                    faceX = -1;
                    faceY = 0;
                    break;

                case FaceDirections.North:
                    faceX = 0;
                    faceY = 1;
                    break;

                case FaceDirections.South:
                    faceX = 0;
                    faceY = -1;
                    break;
            }
        }

        void EnableGravity(bool activateGravity)
        {
            gravity = activateGravity ? gravityScale : 0;
            rb.gravityScale = gravity;
            if (!activateGravity)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            enableHorizontalVelocity = !activateGravity;
        }
        //protected IEnumerator JumpTransition()
        //{
        //    yield return new WaitForSeconds(0.03f);
        //    JumpHandler();
        //}
        private void JumpCheck()
        {
            if (jumping || climbing || !grounded) return;
            if (CheckInteractable()) return;
            Vector2 closestContactPoint = col.ClosestPoint(rb.position - new Vector2(0, characterHeight));
            Vector2 size = new Vector2(col.bounds.size.x, Physics2D.defaultContactOffset * 0.1F);
            if (WallCheck(size, Vector2.up, jumpDetectionRange)) return;
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, size, 0, Vector2.up, jumpDetectionRange, ~ignoredLayersAtJumpCheck);
            if (hit.collider != null)
            {
                if (((1 << hit.collider.gameObject.layer) & walkableLayers) == 0) return;
                jumpDirection = 0;
                ChangeFaceDirection(FaceDirections.North);
                SetJumping(true, jumpDirection);
                landingNormal = hit.normal;
                jumpExpectedLanding = hit.point - Vector2.up * Vector2.Distance(closestContactPoint, rb.position);
                timeJump = Time.realtimeSinceStartup;
                Debug.DrawRay(transform.position, Vector3.up * hit.distance, Color.yellow, 10);
                onJump?.Invoke(rb.position);
                //if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);
                //jumpCoroutine = JumpTransition();
                //StartCoroutine(jumpCoroutine);
                JumpHandler();
                
            }
            else
            {
                Debug.DrawRay(transform.position, Vector3.up * jumpDetectionRange, Color.white, 10);
            }
        }
        //protected IEnumerator JumpDownTransition()
        //{
        //    yield return new WaitForSeconds(0.03f);
        //    JumpDownHandler();
        //}
        private void JumpDownCheck()
        {
            if (jumping || climbing || !grounded) return;
            if (CheckInteractable()) return;
            Vector2 closestContactPoint = col.ClosestPoint(rb.position - new Vector2(0, characterHeight));
            Vector2 rayOrigin = transform.position + Vector3.down * (characterHeight * 0.5f) + Vector3.down;
            Vector2 size = new Vector2(col.bounds.size.x, Physics2D.defaultContactOffset * 0.1F);
            if (WallCheck(size, Vector2.down, transform.position.y - rayOrigin.y - 0.01f)) return;
            RaycastHit2D hit = Physics2D.BoxCast(rayOrigin, size, 0, Vector2.down, jumpDownDetectionRange, ~ignoredLayersAtJumpCheck);
            if (hit.collider != null)
            {
                if (((1 << hit.collider.gameObject.layer) & walkableLayers) == 0) return;
                jumpDirection = 1;
                ChangeFaceDirection(FaceDirections.South);
                SetJumping(true, jumpDirection);
                landingNormal = hit.normal;
                Vector2 colSize = Vector2.up * col.bounds.size.x;
                jumpExpectedLanding = rayOrigin + Vector2.up * (Vector2.Distance(closestContactPoint, rb.position) + Physics2D.defaultContactOffset);
                timeJump = Time.realtimeSinceStartup;
                Debug.DrawRay(transform.position + Vector3.down * (characterHeight * 0.5f) + Vector3.down, Vector3.down * hit.distance, Color.yellow, 10);
                onJump?.Invoke(rb.position);
                JumpDownHandler();
                //if (jumpDownCoroutine != null) StopCoroutine(jumpDownCoroutine);
                //jumpDownCoroutine = JumpDownTransition();
                //StartCoroutine(jumpDownCoroutine);
            }
            else
            {
                Debug.DrawRay(transform.position + Vector3.down * (characterHeight * 0.5f) + Vector3.down, Vector3.down * jumpDownDetectionRange, Color.white, 10);
            }
        }

        /// <summary>
        /// Checks for colliders with the Wall layer.
        /// </summary>
        /// <returns>True if raycast found a collider with a Wall layer.</returns>
        private bool WallCheck(Vector2 size, Vector2 direction, float distance)
        {
            RaycastHit2D inTheWayCheck = Physics2D.BoxCast(transform.position, size, 0, direction, distance, wallLayers);
            if (inTheWayCheck.collider != null)
            {
                //Debug.Log("There's something on the way: " + inTheWayCheck.collider.gameObject.name);
                Debug.DrawRay(transform.position, direction * distance, Color.cyan, 1f);
                return true;
            }
            return false;
        }
        private void CheckPause()
        {
            if (grounded && !hardLanded)
            {
                UIController.instance.InvokePauseMenu();
            }
        }

        void DisableWalkableLayersCollision(bool input)
        {
            if (!col) return;
            col.isTrigger = input;
            
            //for (int i = 0; i < 32; i++)
            //{
            //    if (walkableLayers == (walkableLayers | (1 << i)))
            //    {
            //        Physics2D.IgnoreLayerCollision(gameObject.layer, i, input);
            //    }
            //}
        }
        // Prevents Character getting stuck when jumping off a rope near a platform
        private IEnumerator ReenableCollisionDelay()
        {
            if (!col) yield break;
            yield return new WaitForFixedUpdate();
            col.isTrigger = false;
        }

        private bool IsWalkableLayerCollisionDisabled()
        {
            if (!col) return true;
            return col.isTrigger;
            //for (int i = 0; i < 32; i++)
            //{
            //    if (walkableLayers == (walkableLayers | (1 << i)))
            //    {
            //        if (Physics2D.GetIgnoreLayerCollision(gameObject.layer, i))
            //            return true;
            //    }
            //}
            //return false;
        }

        public void JumpHandler() //Called from "Jump" animation
        {
            if (grounded && jumping)
            {
                StopRunMomentum(); //runMomentum = false;
                PlaySFX(jump, 0f);
                EnableGravity(true);
                rb.velocity = Vector3.up * jumpForce;
                SetGrounded(false);
                SetFallStart();
                if (landingNormal.y < 1f)
                {
                    SetTouchingWalkable(null);
                    DisableWalkableLayersCollision(true);
                    ignoreWalkable = true;
                }
            }
        }

        public void JumpDownHandler() //Called from "JumpDown" animation
        {
            if (grounded && jumping)
            {
                StopRunMomentum();
                PlaySFX(jump, 0f);
                EnableGravity(true);
                rb.velocity = Vector3.up * jumpDownForce;
                SetGrounded(false);
                SetFallStart();
                touchingWalkableBeforeFalling = touchingWalkable;
                DisableWalkableLayersCollision(true);
                ignoreWalkable = true;
            }
        }

        public void FallHandler() //Called from "Falling" animation
        {
            //Debug.Log("FallHandler");
            if (falling || climbing) return;
            Vector2 force = Vector2.zero;
            if (hJumpDir != HJumpDir.None)
            {
                force = new Vector2(horizontalJumpXForce * ((int)hJumpDir), horizontalJumpYForce);
                hJumpDir = HJumpDir.None;
            }
            else
            {
                force = Vector3.up * fallForce;
            }
            Fall(force);
        }

        protected void Fall(Vector2 force)
        {
            if (grounded || climbing) return;
            falling = true;
            PlaySFX(jump, 0f);
            if (fellAfterClimbing)
            {
                StopCoroutine(climbFallAirborneCoroutine);
                climbFallAirborneCoroutine = EnableGravityDelay();
                StartCoroutine(climbFallAirborneCoroutine);
            }
            else EnableGravity(true);
            hJumpXVelocity = force.x;
            rb.velocity = new Vector2(0, force.y);
        }
        protected IEnumerator EnableGravityDelay() //Bruh
        {
            yield return new WaitForSeconds(climbFallGravityDelay);
            EnableGravity(true);
        }

        public void ForceFall(Vector2 force, bool forceHardLanding = false)
        {
            SetupFallConditions();
            if (forceHardLanding)
            {
                fallStart = transform.position;
                fallStart.y += fallDistanceTilDamage + force.y;
            }
            ignoreGroundCheck = true;
            ignoreFallCheck = true;
            Fall(force);
        }

        public void PlaySFX(AudioClip audioClip, float pitchVariation)
        {
            AudioManager.instance.PlaySFX(audioClip, 1f, Random.Range(1f - pitchVariation, 1f + pitchVariation));
        }
        public void ClimbSounds(int input) //Called from "Climb" animation
        {
            climbDetect.terrainProperties?.Step(transform.position);
        }
        public void StepEffect()
        {
            touchingWalkableProperties?.Step(transform.position, new Vector3Int(0, -1, 0));
        } 
        public void Step()
        {
            distanceForNextStep = 0f;
            //Debug.Log("Step");
        }
        public void RecoverFromHardLanding() //Called from hardLanded
        {
            SetHardLanded(false);
            SetGrounded(true);
        }
        public void SetFallStart()
        {
            fallStart = transform.position;
        }
        public void SetGrounded(bool input)
        {
            grounded = input;
        }
        private IEnumerator StopHardLandingCD()
        {
            yield return new WaitForEndOfFrame();
            float duration = animHandler.anim.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
            inHardLandedAnimCD = false;
        }
        public void SetHardLanded(bool input)
        {
            hardLanded = input;
            if (hardLanded)
            {
                if (stopHardLandingCDCoroutine != null) StopCoroutine(stopHardLandingCDCoroutine);
                StopRunMomentum(); //runMomentum = false;
                inHardLandedAnimCD = true;
                stopHardLandingCDCoroutine = StopHardLandingCD();
                StartCoroutine(stopHardLandingCDCoroutine);
                ChangeState(CharacterStates.HardLanding);
            }
            else inHardLandedAnimCD = false;
        }
        public void SetJumping(bool input, int direction)
        {
            jumping = input;
            jumpDirection = direction;
            if(jumping) ChangeState(CharacterStates.Jump, true);
        }
        public void SetClimbing(bool input)
        {
            climbing = input;
            if (climbing) {
                StopRunMomentum(); //runMomentum = false;
                ChangeState(CharacterStates.Climbing); 
            }
            if (!climbing) SetClimbAction(ClimbAction.None, 0); 
        }

        private void TouchLand(Collider2D colliderTouched)
        {
            Land();
            SetTouchingWalkable(colliderTouched);
        }
        private void SetupFall()
        {
            if (fellAfterClimbing)
            {
                if (input.horizontalInput != 0)
                {
                    hJumpDir = (input.horizontalInput > 0 ? HJumpDir.Right : HJumpDir.Left);
                    if (hJumpDir == HJumpDir.Right) ChangeFaceDirection(FaceDirections.East);
                    else ChangeFaceDirection(FaceDirections.West);
                }
                else ChangeFaceDirection(FaceDirections.South);
            }
            else if (InRunningState() && !jumping)
            {
                if (lastXVelocity != 0)
                {
                    hJumpDir = (lastXVelocity > 0 ? HJumpDir.Right : HJumpDir.Left);
                    if (hJumpDir == HJumpDir.Right) ChangeFaceDirection(FaceDirections.East);
                    else if (hJumpDir == HJumpDir.Left) ChangeFaceDirection(FaceDirections.West);
                }
                else ChangeFaceDirection(FaceDirections.South);
            }
            else ChangeFaceDirection(FaceDirections.South);
            SetupFallConditions();
            FallHandler();
        }
        protected void SetupFallConditions()
        {
            touchingWalkableBeforeFalling = touchingWalkable;
            muteLandSound = false;
            SetJumping(false, 1);
            SetGrounded(false);
            SetHardLanded(false);
            SetFallStart();
            SetTouchingWalkable(null);
            ChangeState(CharacterStates.Falling);
            CheckQueues(); // Put here to check for climbables first, then skip jump sfx if found
            DequeueAction();
        }
        public void Land()
        {
            //Debug.Log("Land");
            Debug.Log("Landed Time: " + (Time.realtimeSinceStartup - timeJump));
            if (climbing) SetFallStart();
            falling = false;
            enableVerticalVelocity = true;
            disableClimbableCollision = false;
            fellAfterClimbing = false;
            StopCoroutine(climbFallAirborneCoroutine);
            hJumpXVelocity = 0;
            hJumpDir = HJumpDir.None;
            rb.velocity = Vector2.zero;
            DisableWalkableLayersCollision(false);
            if (fallStart.y - transform.position.y > fallDistanceTilDamage) //HardFall
            {
                GameManager.instance.DealDamageToParty((int)Mathf.Round(fallStart.y - transform.position.y) * fallDamagePerDistance);
                PlaySFX(hardLanding, 0f);
                SetHardLanded(true);
            }
            else if (!muteLandSound && !climbing) PlaySFX(land, 0f);
            
            SetGrounded(true);
            muteLandSound = false;
            SetJumping(false, jumpDirection);
            SetClimbing(false);
            EnableGravity(false);
            SetFallStart();
        }
        private void SetTouchingWalkable(Collider2D collider2D)
        {
            touchingWalkable = collider2D;
            if (collider2D != null)
            {
                if (collider2D.transform.TryGetComponent(out touchingWalkableProperties))
                {
                    touchingWalkableProperties.OnEnter(this);
                }
            }
            else
            {
                touchingWalkableProperties = null;
                touchingWalkableNormal = Vector2.zero;
                touchingWalkablePerp = Vector2.zero;
            }
        }
        private void ChangeState(CharacterStates state, bool forcePlaySameAnim = false)
        {
            animHandler.ChangeAnimationState(this, state, forcePlaySameAnim);
            lastState = state;
        }
        public void ReplaceAnimator(RuntimeAnimatorController runtimeAnimatorController)
        {
            //anim.runtimeAnimatorController = runtimeAnimatorController;
        }
        public bool InRunningState()
        {
            if (runMode == CharacterRunMode.Default) return input.runButtonHold;
            else if (runMode == CharacterRunMode.Prep)
            {
                return runMomentum;
            }
            return false;
        }
        public bool IsRunPrepTimerReached()
        {
            return runButtonHoldTime >= timeForRunPrep;
        }
        public bool IsRunPrepGraceTimerReached()
        {
            return runButtonReleaseTime >= runMomentumReleaseGracePeriod;
        }
        public bool IsOnSlope()
        {
            return touchingWalkableNormal.y < maxNormalDirection && touchingWalkableNormal.y < 1f;
        }
        public bool WasOnSlope()
        {
            return lastTouchingWalkableNormal.y < maxNormalDirection && lastTouchingWalkableNormal.y < 1f;
        }

        public void ChangeClimbMode(CharacterClimbMode climbMode)
        {
            this.climbMode = climbMode;
        }
    }
}