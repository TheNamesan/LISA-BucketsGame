using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(MoveCameraEvent)), CanEditMultipleObjects]
    public class MoveCameraEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetCamera"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraMove"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }

        private string GetSummaryText()
        {
            var eventCommand = target as MoveCameraEvent;
            if (eventCommand.targetCamera == null) return "No target set";
            if (eventCommand.cameraMove == null) return "No Camera Move set";
            string targetText = "";
            if (eventCommand.cameraMove.moveCameraType == MoveCameraType.MoveDelta)
            {
                targetText = $"Move {eventCommand.targetCamera.gameObject.name} Delta {eventCommand.cameraMove.moveDelta}";
            }
            else if (eventCommand.cameraMove.moveCameraType == MoveCameraType.MoveToWorldPosition)
            {
                targetText = $"Move {eventCommand.targetCamera.gameObject.name} to World Position {eventCommand.cameraMove.targetWorldPosition}";
            }
            else if (eventCommand.cameraMove.moveCameraType == MoveCameraType.MoveToTransformPosition)
            {
                targetText = $"Move {eventCommand.targetCamera.gameObject.name} to Transform position ({eventCommand.cameraMove.targetTransform.gameObject.name})";
            }
            else if (eventCommand.cameraMove.moveCameraType == MoveCameraType.ReturnToPlayer)
            {
                targetText = $"Return {eventCommand.targetCamera.gameObject.name} to Player";
            }
            return $"{targetText}. " +
                $"Ease {eventCommand.cameraMove.easeType}, in {eventCommand.cameraMove.timeDuration} second{(eventCommand.cameraMove.timeDuration == 1 ? "" : "s")}";
        }
    }
}
