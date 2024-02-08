using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(MoveCameraAction))]
    public class MoveCameraActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("targetCamera"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("cameraMove"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }

        private string GetSummaryText()
        {
            var action = targetObject as MoveCameraAction;
            if (action.targetCamera == null) return "No target set";
            if (action.cameraMove == null) return "No Camera Move set";
            string targetText = "";
            if (action.cameraMove.moveCameraType == MoveCameraType.MoveDelta)
            {
                targetText = $"Move {action.targetCamera.gameObject.name} Delta {action.cameraMove.moveDelta}";
            }
            else if (action.cameraMove.moveCameraType == MoveCameraType.MoveToWorldPosition)
            {
                targetText = $"Move {action.targetCamera.gameObject.name} to World Position {action.cameraMove.targetWorldPosition}";
            }
            else if (action.cameraMove.moveCameraType == MoveCameraType.MoveToTransformPosition)
            {
                targetText = $"Move {action.targetCamera.gameObject.name} to Transform position ({action.cameraMove.targetTransform.gameObject.name})";
            }
            else if (action.cameraMove.moveCameraType == MoveCameraType.ReturnToPlayer)
            {
                targetText = $"Return {action.targetCamera.gameObject.name} to Player";
            }
            return $"{targetText}. " +
                $"Ease {action.cameraMove.easeType}, in {action.cameraMove.timeDuration} second{(action.cameraMove.timeDuration == 1 ? "" : "s")}";
        }
    }
}