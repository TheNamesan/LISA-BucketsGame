using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(SwitchCameraFollowAction))]
    public class SwitchCameraFollowActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("targetCamera"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("disableCameraFollow"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as SwitchCameraFollowAction;
            if (action.targetCamera == null) return "No target set";
            return $"{(action.disableCameraFollow ? "Disable " : "Enable")} {action.targetCamera.gameObject.name} Follow";
        }
    }
}

