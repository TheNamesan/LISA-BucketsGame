using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeMagazinesAction))]
    public class ChangeMagazinesActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("operation"));
            var operand = targetProperty.FindPropertyRelative("operand");
            EditorGUILayout.PropertyField(operand);
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeMagazinesAction;
            string amountText = "";
            int amount = 0;
            var operationType = action.operation;
            if (action.operand.operandType == NumberOperandType.FromConstant)
            {
                amount = (int)action.operand.constant;
                if (operationType == AddSetOperationType.Add)
                    amountText = $"{(amount >= 0 ? $"+{amount}" : amount)}";
                else if (operationType == AddSetOperationType.Set)
                    amountText = $"= {amount}";
            }
            else
            {
                amountText = $"assigned from '{GameVariableList.GetVariableName(action.operand.variableIndex)}'";
            }
            return $"Magazines {amountText}";
        }
    }
}

