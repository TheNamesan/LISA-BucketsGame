using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public enum NextEventCondition
    {
        StartImmediately = 0,
        WaitForCall = 1,
    }

    [System.Serializable]
    public class EventCommand : ScriptableObject
    {
        [SerializeReference] [HideInInspector] public InteractableEventList parent = null;
        [HideInInspector] public NextEventCondition nextEventCondition;
        [HideInInspector] public bool isFinished = false;

        public string eventName = "EventCommand";
        public Color eventColor = Color.white;
        public virtual void Invoke()
        {

        }
        public virtual void OnInstantiate()
        {

        }
        public virtual EventAction Port()
        {
            var port = new EventAction();
            port.eventName = eventName;
            return port;
        }
    }
}
