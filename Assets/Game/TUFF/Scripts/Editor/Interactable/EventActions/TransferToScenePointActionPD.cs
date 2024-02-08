using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(TransferToScenePointAction))]
    public class TransferToScenePointActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("sceneToLoad"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("position"));
            var retainFacing = targetProperty.FindPropertyRelative("retainFacing");
            EditorGUILayout.PropertyField(retainFacing);
            if (!retainFacing.boolValue) EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("faceDirection"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("hideLoadingIcon"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        public string GetSummaryText()
        {
            var action = targetObject as TransferToScenePointAction;
            return $"Transfer to Scene '{action.sceneToLoad}' at Position '{action.position}', facing '{action.faceDirection}'";
        }
    }
}
