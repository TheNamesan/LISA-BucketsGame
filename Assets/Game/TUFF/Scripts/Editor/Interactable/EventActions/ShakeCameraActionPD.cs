using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ShakeCameraAction))]
    public class ShakeCameraActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("targetCamera"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("cameraShake"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ShakeCameraAction;
            if (action.targetCamera == null) return "No target set";
            if (action.cameraShake == null) return "No Camera Shake set";
            return $"Shake {action.targetCamera.gameObject.name} in {action.cameraShake.timeDuration} second{(action.cameraShake.timeDuration == 1 ? "" : "s")}. " +
                $"Strength: {action.cameraShake.shakeStrength}. Vibrato: {action.cameraShake.vibrato}. Random: {action.cameraShake.randomness}";
        }
    }
}

