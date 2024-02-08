using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeGameVariableAction))]
    public class ChangeGameVariableActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {

            var variableIndex = targetProperty.FindPropertyRelative("variableIndex");
            LISAEditorUtility.DrawVariableListLayout(variableIndex, new GUIContent(variableIndex.displayName));
            //var variablesData = GameVariableList.GetList();
            //string[] options = new string[variablesData.Length];
            //int[] values = new int[variablesData.Length];
            //for (int i = 0; i < variablesData.Length; i++)
            //{
            //    options[i] = variablesData[i].name;
            //    values[i] = i;
            //}
            //variableIndex.intValue = EditorGUILayout.IntPopup(variableIndex.displayName, variableIndex.intValue, options, values);
            var valueType = targetProperty.FindPropertyRelative("valueType");
            EditorGUILayout.PropertyField(valueType);
            var assignType = targetProperty.FindPropertyRelative("assignType");
            EditorGUILayout.PropertyField(assignType);
            DrawOnType(valueType, assignType);

        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeGameVariableAction;
            int variableIndex = action.variableIndex;
            string name = GameVariableList.GetVariableName(variableIndex);
            var valueType = action.valueType;
            string value = ChangeGameVariableAction.GetValue(action).ToString();
            var assignType = action.assignType;
            string assignTypeText = (assignType != GameVariableAssignType.Constant ? $" ({assignType})" : "");
            if (assignType == GameVariableAssignType.Random)
            {
                value = "Random";
                string onlyInt = (action.onlyIntegers ? " Only Integers" : "");
                if (valueType == GameVariableValueType.NumberValue)
                    value = $"Random [{action.numberValueMin} ~ {action.numberValue}]{onlyInt}";
                if (valueType == GameVariableValueType.StringValue)
                {
                    string elementA = "null";
                    string elementB = "null";
                    if (action.randomStrings != null && action.randomStrings.Count > 0)
                    {
                        elementA = $"'{action.randomStrings[0]}'";
                        elementB = $"'{action.randomStrings[^1]}'";
                    }
                    value = $"Random [{elementA}... {elementB}]";
                }
                if (valueType == GameVariableValueType.VectorValue)
                    value = $"Random [{action.vectorValueMin} ~ {action.vectorValue}]{onlyInt}";
            }
            return $"Change Variable '{name}' {valueType} to '{value}'";
        }
        private void DrawOnType(SerializedProperty valueType, SerializedProperty assignType)
        {
            var valueEnum = (GameVariableValueType)valueType.enumValueIndex;
            switch (valueEnum)
            {
                case GameVariableValueType.BoolValue:
                    if ((GameVariableAssignType)assignType.enumValueIndex == GameVariableAssignType.Constant)
                        EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("boolValue"));
                    break;
                case GameVariableValueType.NumberValue:
                    if ((GameVariableAssignType)assignType.enumValueIndex == GameVariableAssignType.Constant)
                        EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("numberValue"));
                    if ((GameVariableAssignType)assignType.enumValueIndex == GameVariableAssignType.Random)
                    {
                        EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("onlyIntegers"));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("numberValueMin"), new GUIContent("Min"));
                        EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("numberValue"), new GUIContent("Max"));
                        EditorGUILayout.EndHorizontal();
                    }
                    break;
                case GameVariableValueType.StringValue:
                    if ((GameVariableAssignType)assignType.enumValueIndex == GameVariableAssignType.Constant)
                        EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("stringValue"));
                    if ((GameVariableAssignType)assignType.enumValueIndex == GameVariableAssignType.Random)
                        EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("randomStrings"));
                    break;
                case GameVariableValueType.VectorValue:
                    if ((GameVariableAssignType)assignType.enumValueIndex == GameVariableAssignType.Constant)
                        EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("vectorValue"));
                    if ((GameVariableAssignType)assignType.enumValueIndex == GameVariableAssignType.Random)
                    {
                        EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("onlyIntegers"));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("vectorValueMin"), new GUIContent("Min"));
                        EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("vectorValue"), new GUIContent("Max"));
                        EditorGUILayout.EndHorizontal();
                    }
                    break;
            }
        }
    }
}
