using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(ChangePartyEvent)), CanEditMultipleObjects]
    public class ChangePartyEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("unit"));
            var op = serializedObject.FindProperty("operation");
            EditorGUILayout.PropertyField(op);
            if (op.enumValueIndex == (int)OperationType.Add) EditorGUILayout.PropertyField(serializedObject.FindProperty("initialize"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }

        private string GetSummaryText()
        {
            var command = target as ChangePartyEvent;
            if (command.unit == null) return "No Unit assigned.";
            return $"{command.operation} {command.unit.GetName()} {(command.initialize && command.operation == OperationType.Add ? "(Initialized) " : "")}" +
                $"{(command.operation == OperationType.Add ? "to": "from")} party";
        }
    }
}