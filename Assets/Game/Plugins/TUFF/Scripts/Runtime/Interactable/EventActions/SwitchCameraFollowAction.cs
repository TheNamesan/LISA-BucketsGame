using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class SwitchCameraFollowAction : EventAction
    {
        [Tooltip("Reference to the camera")]
        public CameraFollow targetCamera;
        [Tooltip("If true, will stop the camera from following the Player. Else it will re-enable it.")]
        public bool disableCameraFollow = false;
        public SwitchCameraFollowAction()
        {
            eventName = "Switch Camera Follow";
            eventColor = EventGUIColors.movement;
        }
        public override void Invoke()
        {
            if (targetCamera != null)
            {
                targetCamera.DisableCameraFollow(disableCameraFollow);
            }
            isFinished = true;
        }
    }
}

