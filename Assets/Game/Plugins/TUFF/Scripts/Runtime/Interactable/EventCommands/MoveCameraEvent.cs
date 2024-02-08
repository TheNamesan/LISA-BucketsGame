using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "EVTMoveCamera", menuName = "TUFF/Events/Move Camera Event")]
    public class MoveCameraEvent : EventCommand
    {
        [Tooltip("Reference to the camera to move.")]
        public CameraFollow targetCamera;
        public CameraMove cameraMove = new CameraMove();
        public override void Invoke()
        {
            if (targetCamera != null)
            {
                //CameraMove.InvokeMovement(cameraMove, targetCamera, this);
            }
            else isFinished = true;
        }

        public override void OnInstantiate()
        {
            eventName = "Move Camera";
            eventColor = new Color(0.9f, 1f, 0.5f, 1f);
        }
    }
}
