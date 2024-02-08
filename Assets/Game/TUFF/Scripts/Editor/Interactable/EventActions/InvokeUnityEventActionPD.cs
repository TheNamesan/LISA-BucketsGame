using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(InvokeUnityEventAction))]
    public class InvokeUnityEventActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("unityEvent"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as InvokeUnityEventAction;
            var eventCount = action.unityEvent.GetPersistentEventCount();
            return $"Invoke ({eventCount}) Event{(eventCount == 1 ? "" : "s")}";
        }
    }
}

