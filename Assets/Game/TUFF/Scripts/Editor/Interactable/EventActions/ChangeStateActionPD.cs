using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeStateAction))]
    public class ChangeStateActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var scope = targetProperty.FindPropertyRelative("scope");
            EditorGUILayout.PropertyField(scope);
            if (scope.enumValueIndex == (int)PartyScope.OnePartyMember)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("unit"));
            }
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("operation"));
            var stateTarget = targetProperty.FindPropertyRelative("stateTarget");
            EditorGUILayout.PropertyField(stateTarget);
            if (stateTarget.enumValueIndex == (int)PartyStateScope.OneState)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("state"));
            }
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeStateAction;
            string unitName = (action.unit == null ? "null" : action.unit.GetName());
            string scope = (action.scope == PartyScope.EntireParty ? "Entire Party" : unitName);
            string operation = (action.operation == OperationType.Add ? "Apply" : "Remove");
            string stateName = (action.state == null ? "null" : action.state.GetName());
            string stateTarget = (action.stateTarget == PartyStateScope.OneState ? stateName : ObjectNames.NicifyVariableName(action.stateTarget.ToString()));

            return $"{operation} {stateTarget} to {scope}";
        }
    }
}
