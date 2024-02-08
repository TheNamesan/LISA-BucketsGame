using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(InvokeUnityEventEvent)), CanEditMultipleObjects]
    public class InvokeUnityEventEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("unityEvent"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var eventCommand = target as InvokeUnityEventEvent;
            var eventCount = eventCommand.unityEvent.GetPersistentEventCount();
            return $"Invoke ({eventCount}) Event{(eventCount == 1 ? "" : "s")}";
        }
    }
}

