using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(FlashScreenAction))]
    public class FlashScreenActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("flashData"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("waitForCompletion"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as FlashScreenAction;
            var flashData = action.flashData;
            float seconds = flashData.flashDuration;
            string secText = (seconds == 1 ? "" : "s");
            string wait = (action.waitForCompletion ? " (Wait For Completion)": "");

            return $"Flash Screen to color {flashData.flashColor} for {seconds} second{secText}{wait}";
        }
    }
}

