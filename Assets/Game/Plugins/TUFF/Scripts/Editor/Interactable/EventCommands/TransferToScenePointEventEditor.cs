using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(TransferToScenePointEvent)), CanEditMultipleObjects]
    public class TransferToScenePointEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneToLoad"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("position"));
            var retainFacing = serializedObject.FindProperty("retainFacing");
            EditorGUILayout.PropertyField(retainFacing);
            if(!retainFacing.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("faceDirection"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hideLoadingIcon"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        public string GetSummaryText()
        {
            var eventCommand = target as TransferToScenePointEvent;
            return $"Transfer to Scene '{eventCommand.sceneToLoad}' at Position '{eventCommand.position}', facing '{eventCommand.faceDirection}'";
        }
    }
}
