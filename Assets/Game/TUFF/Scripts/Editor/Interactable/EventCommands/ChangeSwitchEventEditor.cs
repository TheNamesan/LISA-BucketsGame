using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(ChangeSwitchEvent)), CanEditMultipleObjects]
    public class ChangeSwitchEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            var objTarget = serializedObject.FindProperty("target");
            //test.objectReferenceValue = EditorGUILayout.ObjectField(objTarget.displayName, test.objectReferenceValue, typeof(InteractableObject), true);//EditorGUILayout.PropertyField();
            objTarget.objectReferenceValue = EditorGUILayout.ObjectField(objTarget.displayName, objTarget.objectReferenceValue, typeof(InteractableObject), true);//EditorGUILayout.PropertyField();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("newSwitch"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var eventCommand = target as ChangeSwitchEvent;
            if (eventCommand.target == null) return "No target set";
            return $"Set {eventCommand.target.gameObject.name} switch to {eventCommand.newSwitch}";
        }
    }
}
