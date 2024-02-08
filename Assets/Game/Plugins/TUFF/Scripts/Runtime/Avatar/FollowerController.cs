using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TUFF
{
    public enum FollowingState
    {
        Sleep = 0,
        Awake = 1
    }
    public enum WaypointAction
    {
        None = 0,
        Jump = 1
    }
    public class FollowerController : MonoBehaviour
    {
        public OverworldCharacterController controller;
        public OverworldCharacterController target;
        public FollowingState state = FollowingState.Sleep;
        public AIWaypoint lastWaypoint;
        public Vector2 lastPosition;
        public Vector2 lastTargetPosition;
        public Vector2 lastTargetFixedPosition;
        public float reachDistance = 1f;
        public List<CharacterInputStream> targetInputQueue = new();
        public CharacterInputStream lastInput;
        public List<Vector2> registedPositions = new();
        public List<Vector2> rbTargetPositions = new();
        public List<OverworldCharacterController.QueuedAction> targetQueuedAction = new();
        public WaypointList waypoints;

        private float horizontalInput = 0;
        private float m_updateTimer = 0;

        public float mimicStartRate = 0.25f;
        private float mimicStartTicks = 0;
        private bool mimicInitialized;

        public bool mimic = false;

        public void Awake()
        {
            
        }
        private void OnEnable()
        {
            if (target)
            {
                transform.position = target.transform.position;
                //target.onFixedUpdate.AddListener(FixedLogic);
                //target.onUpdate.AddListener(Logic);
                //target.onInputUpdate.AddListener(StartTimer);
                target.onInputUpdate.AddListener(Logic);
                //target.onFixedUpdate.AddListener(Logic);
            }
            mimicStartTicks = mimicStartRate;
        }
        private void OnDisable()
        {
            if (target)
            {
                //target.onFixedUpdate.RemoveListener(FixedLogic);
                //target.onUpdate.RemoveListener(Logic);
                //target.onInputUpdate.RemoveListener(StartTimer);
                target.onInputUpdate.RemoveListener(Logic);
            }
            mimicInitialized = false;
        }
        void Logic()
        {
            if (controller == null) return;
            if (target == null) return;
            //controller.nextInput = target.nextInput;
            //controller.FixedUpdate(); // Important here!!!
            ////StartCoroutine(LogicCoroutine(target.nextInput));
            //return;

            bool hasInput = target.nextInput.horizontalInput != 0 || target.nextInput.verticalInput != 0;

            bool hasMove = target.moveH != 0 || target.moveV != 0;
            bool isAirborne = !target.grounded;


            bool cheese = target.rb.position != target.lastPosition;

            var targetInput = target.lastInput;

            bool hasVelocity = Mathf.Abs(target.rb.velocity.x) >= 0.00001f || Mathf.Abs(target.rb.velocity.y) >= 0.00001f;
            bool hadVelocity = Mathf.Abs(target.lastVelocity.x) >= 0.00001f || Mathf.Abs(target.lastVelocity.y) >= 0.00001f;

            bool selfMove = (targetInput.HasInput() && (hasVelocity || hadVelocity));
            bool imposedMove = (!target.grounded || !target.wasGrounded || target.hardLanded || target.jumping);
            bool selfImposedMove = (!controller.grounded || !controller.wasGrounded || controller.hardLanded || controller.jumping);
            bool targetRunPrep = (target.runMode == CharacterRunMode.Prep && (targetInput.runButtonHold || target.runMomentum || target.runCanceled));
            bool selfRunPrep = (controller.runMode == CharacterRunMode.Prep && (controller.nextInput.runButtonHold || controller.runMomentum || controller.runCanceled));
            bool stopppedRunButton = target.lastInput.runButtonHold && !target.input.runButtonHold;
            bool selfStopppedRunButton = controller.lastInput.runButtonHold && !controller.input.runButtonHold;
            
            bool positionNotSame = false;
            if (mimicStartTicks <= 0)
            {
                if (registedPositions.Count > 0) 
                    positionNotSame = registedPositions[^1] != target.lastPosition;
            }
            else
            {   
                RunTimer();
                targetInputQueue.Add(targetInput);
                registedPositions.Add(target.lastPosition);
                if (mimicStartTicks <= 0){
                    registedPositions.RemoveAt(0);
                    targetInputQueue.RemoveAt(0);
                }
            }
            bool update = selfMove || imposedMove || selfImposedMove || targetRunPrep || selfRunPrep || stopppedRunButton ||
                selfStopppedRunButton || positionNotSame;
            if (update && mimicStartTicks <= 0)
            {
                state = FollowingState.Awake;
                //RunTimer();

                targetInputQueue.Add(targetInput);
                registedPositions.Add(target.lastPosition);

                controller.nextInput = targetInputQueue[0];
                targetInputQueue.RemoveAt(0);

                Vector2 min = registedPositions[0] - Vector2.one * 0.01f;
                Vector2 max = registedPositions[0] + Vector2.one * 0.01f;

                Vector2 o = controller.rb.position;
                bool withinRangeX = LISAUtility.WithinValueRange(min.x, max.x, o.x);
                bool withinRangeY = LISAUtility.WithinValueRange(min.y, max.y, o.y);
                if (!withinRangeX || !withinRangeY)
                {
                    //Debug.LogWarning($"DESYNC AHHH!!! {o} vs {registedPositions[0]}");
                    //mimicStartTicks = mimicStartRate;
                    //controller.rb.MovePosition(registedPositions[0]); // EZ SYNC LMFAO
                }
                registedPositions.RemoveAt(0);
                controller.FixedUpdate(); // Important here!!!
                lastInput = controller.nextInput;
            }
            //targetInputQueue.Add(targetInput);
            //else registedPositions.Add(target.lastPosition);
            //if (targetInputQueue.Count > 0 && registedPositions.Count > 0) // If list has pending inputs
            //{
            //    if (mimicStartTicks <= 0)
            //    {
                        
                        
            //        //controller.nextInput = targetInputQueue[0];

            //        //targetInputQueue.RemoveAt(0);

            //        //Vector2 min = registedPositions[0] - Vector2.one * 0.01f;
            //        //Vector2 max = registedPositions[0] + Vector2.one * 0.01f;

            //        //Vector2 o = controller.rb.position;
            //        //bool withinRangeX = LISAUtility.WithinValueRange(min.x, max.x, o.x);
            //        //bool withinRangeY = LISAUtility.WithinValueRange(min.y, max.y, o.y);
            //        //if (!withinRangeX || !withinRangeY) {
            //        //    Debug.LogWarning($"DESYNC AHHH!!! {o} vs {registedPositions[0]}");
            //        //    //mimicStartTicks = mimicStartRate;
            //        //    //controller.rb.MovePosition(registedPositions[0]); // EZ SYNC LMFAO
            //        //} 
            //        //registedPositions.RemoveAt(0);
            //        //controller.FixedUpdate(); // Important here!!!
            //        //lastInput = controller.nextInput;
            //    }
            //}
            else
            {
                //Debug.Log("nah");
                if (controller.nextInput.HasInput())
                {
                    state = FollowingState.Sleep;
                    controller.nextInput = new CharacterInputStream();
                    controller.FixedUpdate(); // Important here!!!
                }
            }
        }

        private void RunTimer()
        {
            mimicStartTicks -= Time.fixedDeltaTime;
            if (mimicStartTicks < 0)
            {
                mimicStartTicks = 0;
            }
        }

        private IEnumerator LogicCoroutine(CharacterInputStream newInput)
        {
            for (int i = 0; i < 10; i++)
                yield return new WaitForFixedUpdate();
            controller.nextInput = newInput;
            //controller.FixedUpdate(); // Important here!!!
            yield break;
            //controller.UpdateLogic(); // Important!!!
        }
        void FixedLogic()
        {
        }
        private void Update()
        {
            //if (mimic && targetInputQueue.Count > 0)
            //{
            //    controller.nextInput = targetInputQueue[0];
            //    targetInputQueue.RemoveAt(0);
            //}
        }
        private void LateUpdate()
        {
        }
        void FixedUpdate()
        {
            //StartTimer();
            //if (mimic && targetFixedInputQueue.Count > 0)
            //{
            //    controller.nextInput = targetFixedInputQueue[0];
            //    targetFixedInputQueue.RemoveAt(0);
            //}
            //added = false;
            //inputAssigned = false;
        }


        private void RecordPositions()
        {
            if (controller) lastPosition = controller.rb.position;
            if (target) lastTargetPosition = target.rb.position;
        }

        private void PathFindingLogic()
        {
            if (controller == null) return;
            if (target == null) return;
            if (waypoints.Count <= 0) return;
            AIWaypoint curWaypoint = waypoints[0];
            Vector2 point = curWaypoint.position;
            //waypoints.RemoveAt(0);
            var distanceX = point.x - controller.rb.position.x;
            var distanceY = point.y - controller.rb.position.y;
            {
                controller.nextInput.runButtonHold = target.runMomentum;
                //if (lastWaypoint.waypointAction == WaypointAction.Jump)
                //{
                //    horizontalInput = 0;
                //    CheckJump(distanceY);
                //}
                //else
                {
                    //CheckJump(distanceY);
                    if (Mathf.Abs(distanceX) >= 0.0001f && Mathf.Abs(distanceY) < 1f)
                    {
                        Debug.Log($"Last position: {lastPosition.x}");
                        Debug.Log($"Cur position: {controller.rb.position.x}");
                        Debug.Log($"Point: {point.x}");
                        if (LISAUtility.WithinValueRange(lastPosition.x, controller.rb.position.x, point.x))
                        {
                            Debug.Log("NIGEL STOPS!!");
                            waypoints.RemoveAt(0);
                        }
                        else horizontalInput = 1 * LISAUtility.Sign(distanceX);
                    }
                    else
                    {
                        //horizontalInput = 0;
                        waypoints.RemoveAt(0);
                    }
                }
            }
            Color color = (state == FollowingState.Awake ? Color.yellow : Color.white);
            Debug.DrawLine(controller.rb.position, point, color);
            lastWaypoint = curWaypoint;
        }

        private void CheckJump(float distanceY)
        {
            if (distanceY >= 0.25f)
            {
                JumpInput(1);
            }
            if (distanceY <= -0.25f)
            {
                JumpInput(-1);
            }
        }

        private void JumpInput(int direction)
        {
            controller.nextInput.interactionButtonDown = true;
            controller.nextInput.verticalInput = direction;
        }

        private bool WithinSleepRange()
        {
            if (controller == null) return false;
            if (target == null) return false;
            var targetDistance = Vector2.Distance(target.rb.position, controller.rb.position);
            if (Mathf.Abs(targetDistance) <= reachDistance)
            {
                return true;
            }
            return false;
        }

        public void UpdatePathFollowing()
        {
            //m_updateTimer += Time.fixedDeltaTime;
            //if (m_updateTimer >= updateRate)
            //    m_updateTimer = 0f;
            //else return;
            //if (state == FollowingState.Sleep) return;
            //if (target == null) return;
            //if (target.rb.position == lastTargetPosition) return;
            //AddWaypoint(target.rb.position);
            
        }
        public void AddWaypoint(Vector2 point)
        {
            waypoints.AddNew(point);
        }
    }
    [System.Serializable]
    public class WaypointList : List<AIWaypoint>
    {
        public void AddNew(Vector2 position)
        {
            AddNew(position, WaypointAction.None);
        }
        public void AddNew(Vector2 position, WaypointAction action)
        {
            Add(new AIWaypoint(position, action));
        }
    }
    [System.Serializable]
    public struct AIWaypoint
    {
        
        public Vector2 position;
        public AIWaypoint(Vector2 position, WaypointAction action)
        { this.position = position; waypointAction = action; }
        public AIWaypoint(Vector2 position) : this(position, WaypointAction.None) { }
        public WaypointAction waypointAction;
    }
}

