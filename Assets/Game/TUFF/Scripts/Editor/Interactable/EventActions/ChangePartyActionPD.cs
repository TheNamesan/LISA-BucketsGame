using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor 
{
    [CustomPropertyDrawer(typeof(ChangePartyAction))]
    public class ChangePartyActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("unit"));
            var op = targetProperty.FindPropertyRelative("operation");
            EditorGUILayout.PropertyField(op);
            if (op.enumValueIndex == (int)OperationType.Add) EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("initialize"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }

        private string GetSummaryText()
        {
            var action = targetObject as ChangePartyAction;
            if (action.unit == null) return "No Unit assigned.";
            return $"{action.operation} {action.unit.GetName()} {(action.initialize && action.operation == OperationType.Add ? "(Initialized) " : "")}" +
                $"{(action.operation == OperationType.Add ? "to" : "from")} party";
        }
    }
}

