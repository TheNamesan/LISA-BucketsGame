using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(TintScreenAction))]
    public class TintScreenActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("tint"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("duration"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("waitForCompletion"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as TintScreenAction;
            float seconds = action.duration;
            string secText = (seconds == 1 ? "" : "s");
            string wait = (action.waitForCompletion ? " (Wait For Completion)" : "");

            return $"Flash Screen to color {action.tint} in {seconds} second{secText}{wait}";
        }
    }
}
