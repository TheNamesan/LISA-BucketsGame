using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(WaitSecondsEvent)), CanEditMultipleObjects]
    public class WaitSecondsEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("seconds"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
            
        }
        private string GetSummaryText()
        {
            var eventCommand = target as WaitSecondsEvent;
            return $"Wait {eventCommand.seconds} second{(eventCommand.seconds == 1 ? "" : "s")}";
        }
    }
}
