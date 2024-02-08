using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class MoveCameraAction : EventAction
    {
        [Tooltip("Reference to the camera to move.")]
        public CameraFollow targetCamera;
        public CameraMove cameraMove = new CameraMove();
        public MoveCameraAction()
        {
            eventName = "Move Camera";
            eventColor = new Color(0.9f, 1f, 0.5f, 1f);
        }
        public override void Invoke()
        {
            if (targetCamera != null)
            {
                CameraMove.InvokeMovement(cameraMove, targetCamera, this);
            }
            else isFinished = true;
        }
    }
}