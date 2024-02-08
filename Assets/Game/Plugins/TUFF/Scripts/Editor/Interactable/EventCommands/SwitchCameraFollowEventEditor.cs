using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(SwitchCameraFollowEvent)), CanEditMultipleObjects]
    public class SwitchCameraFollowEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetCamera"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("disableCameraFollow"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var eventCommand = target as SwitchCameraFollowEvent;
            if (eventCommand.targetCamera == null) return "No target set";
            return $"{(eventCommand.disableCameraFollow ? "Disable " : "Enable")} {eventCommand.targetCamera.gameObject.name} Follow";
        }
    }
}
