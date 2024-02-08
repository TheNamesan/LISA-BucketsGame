using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ActionConditions))]
    public class ActionConditionsPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 0;
            float height = 0f;
            var conditionList = property.FindPropertyRelative("conditionList");
            if (property.isExpanded) height += EditorGUI.GetPropertyHeight(conditionList);
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
            var actionConditions = LISAEditorUtility.GetTargetObjectOfProperty(property) as ActionConditions;
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
                    text += ActionConditionElementPD.GetLabelName(conditionListProperty.GetArrayElementAtIndex(0), "0");
                }
                text += "]";
            }
                
            var label = new GUIContent(text, orgLabel.tooltip);
            return label;
        }
    }

    [CustomPropertyDrawer(typeof(ActionConditionElement))]
    public class ActionConditionElementPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 1;
            if (property.isExpanded) lines += 2;
            var action = LISAEditorUtility.GetTargetObjectOfProperty(property) as ActionConditionElement;
            if (action.conditionType == ActionConditionType.Always)
                lines -= 1;
            return 20f + (20f * lines);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
        {
            position.height = 20f;
            string prefix = label.text.Substring(label.text.Length - 1);
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
            var action = LISAEditorUtility.GetTargetObjectOfProperty(property) as ActionConditionElement;
            string not = (action.not ? "[Not] " : "");
            string condition = ObjectNames.NicifyVariableName(action.conditionType.ToString());
            string details = "";
            var condEnum = action.conditionType;
            if (condEnum == ActionConditionType.TurnNo)
            {
                var repeat = action.turnRepeats;
                details = $"(Turn {action.equalsTurn}{(repeat > 0 ? $" + {action.turnRepeats}*X" : "")})";
            }
            if (condEnum == ActionConditionType.HPThreshold || condEnum == ActionConditionType.SPThreshold || condEnum == ActionConditionType.TPThreshold)
            {
                details = $"({action.percentThresholdMin}% - {action.percentThresholdMax}%)";
            }
            if (condEnum == ActionConditionType.HasState)
                details = $"({(action.state != null ? action.state.GetName() : "null")})";
            return $"{prefix}: {not}{condition} {details}";
        }
    }
}
