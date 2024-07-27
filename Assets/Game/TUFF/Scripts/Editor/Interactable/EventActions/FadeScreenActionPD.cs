using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(FadeScreenAction))]
    public class FadeScreenActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("duration"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("fadeType"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("waitForCompletion"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as FadeScreenAction;
            float seconds = action.duration;
            string secText = (seconds == 1 ? "" : "s");
            string wait = (action.waitForCompletion ? " (Wait For Completion)" : "");
            string fadeType = ObjectNames.NicifyVariableName(action.fadeType.ToString());

            return $"{fadeType} Screen in {seconds} second{secText}{wait}";
        }
    }
}
