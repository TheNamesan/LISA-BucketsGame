using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeSPAction))]
    public class ChangeSPActionPD : EventActionPD
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
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("constant"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeSPAction;
            string unitName = (action.unit == null ? "null" : action.unit.GetName());
            string scope = (action.scope == PartyScope.EntireParty ? "Entire Party" : unitName);
            int value = action.constant;

            string operation = (action.operation == AddSetOperationType.Add ? (value >= 0 ? "+" : "") : "= ");

            return $"{scope}'s SP {operation}{value}";
        }
    }
}

