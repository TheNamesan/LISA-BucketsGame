using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(GameVariableComparator))]
    public class GameVariableComparatorPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            DrawGameVariable(position, property, label);
            property.serializedObject.ApplyModifiedProperties();
        }
        private void DrawGameVariable(Rect position, SerializedProperty property, GUIContent label)
        {
            //position.x += position.width + 2;
            var orgX = position.x;
            var orgWidth = position.width;
            var orgLabel = EditorGUIUtility.labelWidth;

            float w = orgWidth * 0.33f - 2;
            position.width = w;

            var variableIndex = property.FindPropertyRelative("targetVariableIndex");
            var variablesData = GameVariableList.GetList();
            GUIContent[] options = new GUIContent[variablesData.Length];
            int[] values = new int[variablesData.Length];
            for (int i = 0; i < variablesData.Length; i++)
            {
                if (options[i] == null)
                    options[i] = new GUIContent(variablesData[i].name);
                options[i].text = variablesData[i].name;
                values[i] = i;
            }
            variableIndex.intValue = EditorGUI.IntPopup(position, label, variableIndex.intValue, options, values);

            var valueType = property.FindPropertyRelative("targetVariableValueType");
            position.x += position.width + 2;

            EditorGUI.PropertyField(position, valueType, new GUIContent(""));

            position.x += position.width + 2;
            EditorGUIUtility.labelWidth = 16;
            DrawOnType(position, property, new GUIContent("is"));
            EditorGUIUtility.labelWidth = orgLabel;
        }
        private void DrawOnType(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueEnum = (GameVariableValueType)property.FindPropertyRelative("targetVariableValueType").enumValueIndex;
            switch (valueEnum)
            {
                case GameVariableValueType.BoolValue:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(GameVariableComparator.variableBool)), label); break;
                case GameVariableValueType.NumberValue:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(GameVariableComparator.variableNumber)), label); break;
                case GameVariableValueType.StringValue:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(GameVariableComparator.variableString)), label); break;
                case GameVariableValueType.VectorValue:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(GameVariableComparator.variableVector)), label); break;
            }
        }
    }
}


