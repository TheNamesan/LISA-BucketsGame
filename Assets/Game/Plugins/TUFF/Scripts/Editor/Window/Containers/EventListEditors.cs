using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    public class EventListEditors
    {
        public List<EventCommandEditor> editors = new List<EventCommandEditor>();
        public void UpdateEditors(InteractableEventList eventList)
        {
            editors = new List<EventCommandEditor>();
            for (int i = 0; i < eventList.content.Count; i++)
            {
                editors.Add(Editor.CreateEditor(eventList.content[i]) as EventCommandEditor);
                //if (eventListEditors[eventListEditors.Count - 1] == null) Debug.LogWarning($"Editor at index {i} is not of type EventCommandEditor");
            }
        }
    }
}
