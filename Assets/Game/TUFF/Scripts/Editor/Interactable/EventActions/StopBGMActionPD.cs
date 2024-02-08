using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(StopBGMAction))]
    public class StopBGMActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("fadeOutDuration"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as StopBGMAction;
            float dur = action.fadeOutDuration;
            return $"Stop BGM" +
                $"{(dur > 0 ? $" with {dur} second{(dur == 1 ? "" : "s")} Fade Out" : "")}";
        }
    }
}

