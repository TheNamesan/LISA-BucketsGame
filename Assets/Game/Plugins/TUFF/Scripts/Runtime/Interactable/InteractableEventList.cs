using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TUFF
{
    [System.Serializable]
    public class InteractableEventList
    {
        public int currentEventIndex = 0;
        public List<EventCommand> content = new List<EventCommand>();
        public void AddContentEvent(EventCommand eventCommand, int insertAt = -1)
        {
            eventCommand.OnInstantiate();
            if (insertAt >= 0) content.Insert(insertAt, eventCommand);
            else content.Add(eventCommand);
        }
    }
}
