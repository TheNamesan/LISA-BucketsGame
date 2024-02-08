using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(ShakeCameraEvent)), CanEditMultipleObjects]
    public class ShakeCameraEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetCamera"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraShake"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var eventCommand = target as ShakeCameraEvent;
            if (eventCommand.targetCamera == null) return "No target set";
            if (eventCommand.cameraShake == null) return "No Camera Shake set";
            return $"Shake {eventCommand.targetCamera.gameObject.name} in {eventCommand.cameraShake.timeDuration} second{(eventCommand.cameraShake.timeDuration == 1 ? "" : "s")}. " +
                $"Strength: {eventCommand.cameraShake.shakeStrength}. Vibrato: {eventCommand.cameraShake.vibrato}. Random: {eventCommand.cameraShake.randomness}";
        }
    }
}
