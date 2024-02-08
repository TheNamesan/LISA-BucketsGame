using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(EventCommand), true), CanEditMultipleObjects]
    public class EventCommandEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var command = target as EventCommand;
            serializedObject.Update();
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(serializedObject.FindProperty("parent").type);
            var eventName = serializedObject.FindProperty("eventName");
            EditorGUILayout.PropertyField(eventName);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eventColor"));
            command.name = eventName.stringValue;
            InspectorGUIContent();
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
        public virtual void InspectorGUIContent() {}
        public virtual void OnEditorInstantiate() {}

        public virtual float GetSummaryHeight()
        {
            return 20f;
        }

        public virtual void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, "No Summary");
        }

        public string GetEventName()
        {
            var command = target as EventCommand;
            return command.eventName;
        }
    }
}
