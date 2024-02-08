using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeTransformAction))]
    public class ChangeTransformActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var transform = targetProperty.FindPropertyRelative("transform");
            EditorGUILayout.PropertyField(transform);

            // Position
            var keepPosition = targetProperty.FindPropertyRelative("keepPosition");
            EditorGUILayout.PropertyField(keepPosition);
            if (!keepPosition.boolValue)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("position"));
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("worldPosition"));
            }
            // Rotation
            var keepRotation = targetProperty.FindPropertyRelative("keepRotation");
            EditorGUILayout.PropertyField(keepRotation);
            if (!keepRotation.boolValue)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("rotation"));
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("worldRotation"));
            }
            // Scale
            var keepScale = targetProperty.FindPropertyRelative("keepScale");
            EditorGUILayout.PropertyField(keepScale);
            if (!keepScale.boolValue)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("scale"));
            }
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeTransformAction;
            if (action.transform == null) return "No Transform set";
            string name = action.transform.name;
            string position = "";
            if (!action.keepPosition)
            {
                position = $"[Position: {action.position}{(action.worldPosition ? " (World)" : "")}]";
            }
            string rotation = "";
            if (!action.keepRotation)
            {
                if (!action.keepPosition) rotation = ", ";
                rotation += $"[Rotation: {action.rotation}{(action.worldRotation ? " (World)" : "")}]";
            }
            string scale = "";
            if (!action.keepScale)
            {
                if (!action.keepPosition || !action.keepRotation) scale = ", ";
                scale += $"[Scale: {action.scale}]";
            }

            return $"Set {name}: {position}{rotation}{scale}";
        }
    }
}

