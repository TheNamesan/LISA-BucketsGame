using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TUFF
{
    public class InvokeUnityEventEvent : EventCommand
    {
        [Tooltip("Events to invoke.")]
        public UnityEvent unityEvent = new UnityEvent();
        public override void Invoke()
        {
            unityEvent?.Invoke();
            isFinished = true;
        }
        public override void OnInstantiate()
        {
            eventName = "Invoke Unity Event";
        }
    }
}