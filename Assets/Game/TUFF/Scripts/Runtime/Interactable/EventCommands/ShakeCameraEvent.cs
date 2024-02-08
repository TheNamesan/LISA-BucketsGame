using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "EVTShakeCamera", menuName = "TUFF/Events/Shake Camera Event")]
    public class ShakeCameraEvent : EventCommand
    {
        [Tooltip("Reference to the camera to shake.")]
        public CameraFollow targetCamera;
        public CameraShake cameraShake = new CameraShake();
        public override void Invoke()
        {
            if (targetCamera != null)
            {
                //CameraShake.InvokeShake(cameraShake, targetCamera, this);
            }
            else isFinished = true;
        }

        public override void OnInstantiate()
        {
            eventName = "Shake Camera";
            eventColor = new Color(1f, 1f, 0.7f, 1f);
        }
    }
}
