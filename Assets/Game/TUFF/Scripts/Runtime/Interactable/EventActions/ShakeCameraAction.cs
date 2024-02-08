using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ShakeCameraAction : EventAction
    {
        [Tooltip("Reference to the camera to shake.")]
        public CameraFollow targetCamera;
        public CameraShake cameraShake = new CameraShake();
        public ShakeCameraAction()
        {
            eventName = "Shake Camera";
            eventColor = new Color(1f, 1f, 0.7f, 1f);
        }
        public override void Invoke()
        {
            if (targetCamera != null)
            {
                CameraShake.InvokeShake(cameraShake, targetCamera, this);
            }
            else isFinished = true;
        }
    }
}

