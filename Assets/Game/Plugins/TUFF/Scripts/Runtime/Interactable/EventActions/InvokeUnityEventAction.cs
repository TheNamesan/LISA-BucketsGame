using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TUFF
{
    [System.Serializable]
    public class InvokeUnityEventAction : EventAction
    {
        [Tooltip("Events to invoke.")]
        public UnityEvent unityEvent = new UnityEvent();
        public InvokeUnityEventAction()
        {
            eventName = "Invoke Unity Event";
        }
        public override void Invoke()
        {
            unityEvent?.Invoke();
            isFinished = true;
        }
    }
}

