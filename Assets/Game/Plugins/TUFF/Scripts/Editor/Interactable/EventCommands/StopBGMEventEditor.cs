using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(StopBGMEvent)), CanEditMultipleObjects]
    public class StopBGMEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeOutDuration"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var eventCommand = target as StopBGMEvent;
            float dur = eventCommand.fadeOutDuration;
            return $"Stop BGM" +
                $"{(dur > 0 ? $" with {dur} second{(dur == 1 ? "" : "s")} Fade Out" : "")}";
        }
    }
}
