using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(BattleConditions))]
    public class BattleConditionsPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 0;
            float height = 0f;
            var conditionList = property.FindPropertyRelative("conditionList");
            if (property.isExpanded) {
                lines += 1;
                height += EditorGUI.GetPropertyHeight(conditionList); 
            }
            return 20f + (20f * lines) + height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GetLabel(property, label), true);
            position.y += 20f;
            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                EditorGUI.PropertyField(position, property.FindPropertyRelative("span"));
                position.y += 20f;
                var conditionList = property.FindPropertyRelative("conditionList");
                EditorGUI.PropertyField(position, conditionList);
                position.y += EditorGUI.GetPropertyHeight(conditionList);

                position.width += 15f;
                position.x -= 15f;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
        private GUIContent GetLabel(SerializedProperty property, GUIContent orgLabel)
        {
            var conditionListProperty = property.FindPropertyRelative("conditionList");
            var arraySize = conditionListProperty.arraySize;
            string text = $"{orgLabel.text} ({arraySize})";
            if (arraySize > 0)
            {
                text += " [";
                for (int i = 0; i < arraySize; i++)
                {
                    if (i >= 3) { text += "..."; break; }
                    if (i > 0) text += ", ";
                    text += BattleConditionElementPD.GetLabelName(conditionListProperty.GetArrayElementAtIndex(0), "0");
                }
                text += "]";
            }

            var label = new GUIContent(text, orgLabel.tooltip);
            return label;
        }
    }

    [CustomPropertyDrawer(typeof(BattleConditionElement))]
    public class BattleConditionElementPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 1;
            if (property.isExpanded)
            { 
                lines += 2;
                var element = LISAEditorUtility.GetTargetObjectOfProperty(property) as BattleConditionElement;
                if (element.conditionType == ActionConditionType.TurnNo)
                    lines += 1;
                if (element.conditionType == ActionConditionType.HPThreshold || element.conditionType == ActionConditionType.SPThreshold || 
                    element.conditionType == ActionConditionType.TPThreshold || element.conditionType == ActionConditionType.HasState)
                    lines += 3;
            }
            return (20f * lines);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            string prefix = LISAEditorUtility.GetIndexOfElementLabel(label.text);//label.text.Substring(label.text.Length - 1);
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GetLabelName(property, prefix), true);
            position.y += 20f;
            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                EditorGUI.PropertyField(position, property.FindPropertyRelative("not"));
                position.y += 20f;
                var condition = property.FindPropertyRelative("conditionType");
                var condEnum = (ActionConditionType)condition.enumValueIndex;
                EditorGUI.PropertyField(position, condition);
                position.y += 20f;
                if (condEnum == ActionConditionType.TurnNo)
                {
                    var equals = property.FindPropertyRelative("equalsTurn");
                    var repeats = property.FindPropertyRelative("turnRepeats");

                    var orgX = position.x;
                    var orgWidth = position.width;
                    var orgLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 65f;
                    position.width = orgWidth * 0.49f;
                    EditorGUI.PropertyField(position, equals, new GUIContent("From Turn"));
                    position.x += orgWidth * 0.5f;
                    position.width = orgWidth * 0.4f;
                    EditorGUIUtility.labelWidth = 35f;
                    EditorGUI.PropertyField(position, repeats, new GUIContent("Every"));
                    position.x += orgWidth * 0.41f;
                    position.width = orgWidth * 0.09f;
                    EditorGUI.LabelField(position, new GUIContent("Turns"));

                    EditorGUIUtility.labelWidth = orgLabelWidth;
                    position.width = orgWidth;
                    position.x = orgX;
                }
                if (condEnum == ActionConditionType.HPThreshold || condEnum == ActionConditionType.SPThreshold || condEnum == ActionConditionType.TPThreshold)
                {
                    DrawUserField(position, property);
                    position.y += 40f;

                    var min = property.FindPropertyRelative("percentThresholdMin");
                    var max = property.FindPropertyRelative("percentThresholdMax");

                    var orgX = position.x;
                    var orgWidth = position.width;
                    var orgLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 45f;
                    position.width = orgWidth * 0.49f;
                    EditorGUI.PropertyField(position, min, new GUIContent("Min%"));
                    position.x += orgWidth * 0.5f;
                    EditorGUI.PropertyField(position, max, new GUIContent("Max%"));
                    EditorGUIUtility.labelWidth = orgLabelWidth;
                    position.width = orgWidth;
                    position.x = orgX;
                }
                if (condEnum == ActionConditionType.HasState)
                {
                    DrawUserField(position, property);
                    position.y += 40f;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("state"));
                }
                position.y += 20f;

                position.width += 15f;
                position.x -= 15f;
            }

            property.serializedObject.ApplyModifiedProperties();
        }
        public static string GetLabelName(SerializedProperty property, string prefix)
        {
            var element = LISAEditorUtility.GetTargetObjectOfProperty(property) as BattleConditionElement;
            string not = (element.not ? "[Not] " : "");
            string condition = ObjectNames.NicifyVariableName(element.conditionType.ToString());
            string details = "";
            var condEnum = element.conditionType;
            if (condEnum == ActionConditionType.TurnNo)
            {
                var repeat = element.turnRepeats;
                details = $"(Turn {element.equalsTurn}{(repeat > 0 ? $" + {element.turnRepeats}*X" : "")})";
            }
            if (condEnum == ActionConditionType.HPThreshold || condEnum == ActionConditionType.SPThreshold || condEnum == ActionConditionType.TPThreshold)
            {
                details = $"({element.percentThresholdMin}% - {element.percentThresholdMax}%)";
            }
            if (condEnum == ActionConditionType.HasState)
                details = $"({(element.state != null ? element.state.GetName() : "null")})";
            return $"{prefix}: {not}{condition} {details}";
        }
        private static void DrawUserField(Rect position, SerializedProperty property)
        {
            var targetTypeProp = property.FindPropertyRelative("targetType");
            EditorGUI.PropertyField(position, targetTypeProp);
            position.y += 20f;
            BattleTargetType targetType = (BattleTargetType)targetTypeProp.enumValueIndex;
            if (targetType == BattleTargetType.Enemy)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("enemyIndex"));
            }
            else if (targetType == BattleTargetType.Ally)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("unit"));
            }
        }
    }
}

