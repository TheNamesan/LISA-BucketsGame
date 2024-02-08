using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeEXPAction))]
    public class ChangeEXPActionPD : EventActionPD
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
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("operand"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeEXPAction;
            string unitName = (action.unit == null ? "null" : action.unit.GetName());
            string scope = (action.scope == PartyScope.EntireParty ? "Entire Party" : unitName);
            var operationType = action.operation;
            string amountText = "";

            if (action.operand.operandType == NumberOperandType.FromConstant)
            {
                int amount = (int)action.operand.constant;
                if (operationType == AddSetOperationType.Add)
                    amountText = $"{(amount >= 0 ? $"+{amount}" : amount)}";
                else if (operationType == AddSetOperationType.Set)
                    amountText = $"= {amount}";
            }
            else
            {
                amountText = $"assigned from '{GameVariableList.GetVariableName(action.operand.variableIndex)}'";
            }

            return $"{scope}'s EXP {amountText}";
        }
    }
}

