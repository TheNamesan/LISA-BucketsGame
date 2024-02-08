using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public enum FieldOriginType
    {
        FromScene = 0,
        FromPersistentInstance = 1
    }
    [System.Serializable]
    public class EventAction
    {
        [System.NonSerialized] public ActionList parent = null;
        public bool isFinished = false;
        public string eventName = "Event Action";
        public Color eventColor = Color.white;

        public EventAction()
        {
            eventName = "Event Action";
        }
        public virtual void Invoke()
        {
            isFinished = true;
        }
        public virtual void OnInstantiate()
        {
            
        }
        public virtual void OnEnable() { }
        public virtual void OnStart() { }
    }
}

