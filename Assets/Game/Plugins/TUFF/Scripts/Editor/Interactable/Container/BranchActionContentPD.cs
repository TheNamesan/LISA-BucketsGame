using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(BranchActionContent))]
    public class BranchActionContentPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 0;
            float height = 0f;
            var conditionList = property.FindPropertyRelative("conditionList");
            if (property.isExpanded)
            {
                lines += 0;
                height += EditorGUI.GetPropertyHeight(conditionList);
            }
            return 20f + (20f * lines) + height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            position.y += 20f;
            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                var conditionList = property.FindPropertyRelative("conditionList");
                EditorGUI.PropertyField(position, conditionList);
                position.y += EditorGUI.GetPropertyHeight(conditionList);

                position.width += 15f;
                position.x -= 15f;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
        public static string GetConditionText(BranchActionContent branchContent)
        {
            string text = "";
            if (branchContent == null) return text;
            var conditions = branchContent.conditionList;
            text += $"({conditions.Count}) ";
            for (int i = 0; i < conditions.Count; i++)
            {
                if (i > 0) text += " AND ";
                var element = conditions[i];
                if (element == null) { text += "null"; continue; }
                var not = element.not;
                if (not) text += "(Not) ";
                switch (element.conditionType)
                {
                    case BranchConditionType.GameVariable:
                        var comparator = element.variableComparator;
                        text += GetVariableText(comparator);
                        break;
                }
            }
            return text;
        }
        private static string GetVariableText(GameVariableComparator comparator)
        {
            var variablesData = GameVariableList.GetList();
            if (comparator.targetVariableIndex >= variablesData.Length) return "";
            string text = $"'{variablesData[comparator.targetVariableIndex].name}' ";
            switch (comparator.targetVariableValueType)
            {
                case GameVariableValueType.BoolValue:
                    text += $"Bool Value is {comparator.variableBool}"; break;
                case GameVariableValueType.NumberValue:
                    text += $"Number Value is {comparator.variableNumber}"; break;
                case GameVariableValueType.StringValue:
                    text += $"String Value is '{comparator.variableString}'"; break;
                case GameVariableValueType.VectorValue:
                    text += $"Vector Value is {comparator.variableVector}"; break;
            }
            return text;
        }
    }
    [CustomPropertyDrawer(typeof(BranchActionContentElement))]
    public class BranchActionContentElementPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 1;
            if (property.isExpanded)
            {
                lines += 2;
                var element = LISAEditorUtility.GetTargetObjectOfProperty(property) as BranchActionContentElement;
                if (element.conditionType == BranchConditionType.GameVariable)
                    lines += 2;
                    //lines += 3;
            }
            return (20f * lines);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var orgX = position.x;
            var orgWidth = position.width;
            var orgLabel = EditorGUIUtility.labelWidth;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            position.y += 20f;
            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                EditorGUI.PropertyField(position, property.FindPropertyRelative("not"));
                position.y += 20f;

                var conditionType = property.FindPropertyRelative("conditionType");
                EditorGUI.PropertyField(position, conditionType);
                position.y += 20f;

                var type = (BranchConditionType)conditionType.enumValueIndex;
                if (type == BranchConditionType.GameVariable)
                    DrawGameVariable(position, orgWidth, property);

                position.width += 15f;
                position.x -= 15f;
                EditorGUIUtility.labelWidth = orgLabel;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
        private void DrawGameVariable(Rect position, float orgWidth, SerializedProperty property)
        {
            //position.x += position.width + 2;
            position.width = orgWidth - 2;
            position.height = 100;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("variableComparator"), new GUIContent(""));
            position.width = orgWidth;
        }
    }
}

