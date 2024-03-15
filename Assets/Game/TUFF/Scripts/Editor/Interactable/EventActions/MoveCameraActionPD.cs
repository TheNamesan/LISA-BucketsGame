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
            string cameraName = action.targetCamera.gameObject.name;
            if (action.cameraMove.moveCameraType == MoveCameraType.MoveDelta)
            {
                targetText = $"Move {cameraName} Delta {action.cameraMove.moveDelta}";
            }
            else if (action.cameraMove.moveCameraType == MoveCameraType.MoveToWorldPosition)
            {
                targetText = $"Move {cameraName} to World Position {action.cameraMove.targetWorldPosition}";
            }
            else if (action.cameraMove.moveCameraType == MoveCameraType.MoveToTransformPosition)
            {
                string transformName = "null";
                if (action.cameraMove.targetTransform) transformName = action.cameraMove.targetTransform.gameObject.name;
                targetText = $"Move {cameraName} to Transform position ({transformName})";
            }
            else if (action.cameraMove.moveCameraType == MoveCameraType.ReturnToPlayer)
            {
                targetText = $"Return {cameraName} to Player";
            }
            return $"{targetText}. " +
                $"Ease {action.cameraMove.easeType}, in {action.cameraMove.timeDuration} second{(action.cameraMove.timeDuration == 1 ? "" : "s")}";
        }
    }
}