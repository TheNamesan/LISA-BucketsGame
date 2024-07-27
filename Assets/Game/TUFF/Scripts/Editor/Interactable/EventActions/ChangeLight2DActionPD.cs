using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeLight2DAction))]
    public class ChangeLight2DActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var light2D = targetProperty.FindPropertyRelative("light2D");
            EditorGUILayout.PropertyField(light2D);

            // Enabled
            DrawProperties("keepEnabled", "enabled");

            //Color
            DrawProperties("keepColor", "color");

            //Intensity
            DrawProperties("keepIntensity", "intensity");
        }
        public void DrawProperties(string keepValueName, string propertyToShowName)
        {
            var keepValue = targetProperty.FindPropertyRelative(keepValueName);
            EditorGUILayout.PropertyField(keepValue);
            if (!keepValue.boolValue)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative(propertyToShowName));
            }
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeLight2DAction;
            if (action.light2D == null) return "No Light2D set";
            string name = action.light2D.name;
            string enabled = "";
            if (!action.keepEnabled)
            {
                enabled = $"[Enabled: {action.enabled}]";
            }
            string color = "";
            if (!action.keepColor)
            {
                color = $"[Color: {action.color}]";
            }
            string intensity = "";
            if (!action.keepIntensity)
            {
                intensity = $"[Intensity: {action.intensity}]";
            }
            return $"Set {name}: {enabled}{color}{intensity}";
        }
    }
}

