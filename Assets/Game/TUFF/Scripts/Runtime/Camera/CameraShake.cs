using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace TUFF
{
    [System.Serializable]
    public class CameraShake
    {
        [Tooltip("Duration of the shake in seconds.")]
        public float timeDuration = 1f;
        [Tooltip("The shake strength on each axis. Defines the maximum positions the shake can reach.")]
        public Vector2 shakeStrength = Vector2.one;
        [Tooltip("How much will the shake vibrate.")]
        public int vibrato = 10;
        [Tooltip("How much the shake will randomize its direction. Values of 90 and lower work best.")]
        public float randomness = 90;
        [Tooltip("If true, the shake will smoothly snap all values to integers.")]
        public bool snapping = false;
        [Tooltip("If true, the shake will automatically fade out smoothly within its duration, otherwise it will not.")]
        public bool enableFadeOut = true;
        [Tooltip("If true, the camera will stop following the player when the shake ends.")]
        public bool disableCameraFollow = false;
        [Tooltip("Unity Event to call when the shake ends.")]
        public UnityEvent onShakeEnd;

        public static void InvokeShake(CameraShake cameraShake, CameraFollow target, EventAction commandCallback = null)
        {
            if (commandCallback != null)
            {
                UnityAction action = null;
                action = () => {
                    commandCallback.isFinished = true;
                    cameraShake.onShakeEnd.RemoveListener(action);
                };
                cameraShake.onShakeEnd.AddListener(action);
            }
            target.ShakeCamera(cameraShake);
        }
    }
}
