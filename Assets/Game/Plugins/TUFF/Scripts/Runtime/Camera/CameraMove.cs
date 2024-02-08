using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace TUFF
{
    public enum MoveCameraType
    {
        MoveDelta = 0,
        MoveToWorldPosition = 1,
        MoveToTransformPosition = 2,
        ReturnToPlayer = 3
    }

    [System.Serializable]
    public class CameraMove
    {
        [Tooltip("Easing movement to use.")]
        public Ease easeType = Ease.Linear;
        [Tooltip("Time to reach the target in seconds.")]
        public float timeDuration = 0.5f;
        [Tooltip("The movement's target.\nMove Delta: Moves the camera a distance from its current position. Disables Camera Following.\nMove To World Position: Moves the camera to the specified world coordinates. Disables Camera Following.\nMove To Transform Position: Moves the camera to the coordinates of the specified GameObject. Disables Camera Following.\nReturn To Player: Returns the Camera to the Player’s Coordinates. Enables Camera Following when finished.")]
        public MoveCameraType moveCameraType;
        public Vector2 moveDelta;
        public Vector2 targetWorldPosition;
        public Transform targetTransform;
        [Tooltip("Unity Event to call when movement ends.")]
        public UnityEvent onMovementEnd;

        public static void InvokeMovement(CameraMove cameraMove, CameraFollow target, EventAction commandCallback = null)
        {
            if (commandCallback != null)
            {
                UnityAction action = null;
                action = () => {
                    commandCallback.isFinished = true;
                    cameraMove.onMovementEnd.RemoveListener(action);
                };
                cameraMove.onMovementEnd.AddListener(action);
            }
            target.MoveCamera(cameraMove);
        }
    }
}

