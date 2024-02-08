using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class SwitchCameraFollowEvent : EventCommand
    {
        [Tooltip("Reference to the camera")]
        public CameraFollow targetCamera;
        [Tooltip("If true, will stop the camera from following the Player. Else it will re-enable it.")]
        public bool disableCameraFollow = false;
        public override void Invoke()
        {
            if (targetCamera != null)
            {
                targetCamera.DisableCameraFollow(disableCameraFollow);
            }
            isFinished = true;
        }
        public override void OnInstantiate()
        {
            eventName = "Switch Camera Follow";
            eventColor = new Color(0.9f, 1f, 0.5f, 1f);
        }
    }
}
