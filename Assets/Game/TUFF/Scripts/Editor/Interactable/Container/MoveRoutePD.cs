using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(MoveRouteElement))]
    public class MoveRouteElementPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var orgLabelWidth = EditorGUIUtility.labelWidth;
            float totalWidth = position.width;

            position.width = totalWidth * 0.3f;
            var instruction = property.FindPropertyRelative("instruction");

            EditorGUIUtility.labelWidth = 18f;
            string index = LISAEditorUtility.GetIndexOfElementLabel(label.text);
            EditorGUI.PropertyField(position, instruction, new GUIContent($"{index}: "));
            EditorGUIUtility.labelWidth = orgLabelWidth;
            var ins = (MoveRouteInstruction)instruction.enumValueIndex;
            if (ins == MoveRouteInstruction.WaitForSeconds)
            {
                SingleField(position, property, orgLabelWidth, totalWidth, "duration", new GUIContent("?"));//DoubleField(position, property, orgLabelWidth, totalWidth, "statChange", "statChangeValue", new GUIContent("?"), new GUIContent("x"));
            }
            if (ins == MoveRouteInstruction.MoveHorizontal)
            {
                DoubleField(position, property, orgLabelWidth, totalWidth, "moveDirectionH", "duration", new GUIContent("?"), new GUIContent("?"));//DoubleField(position, property, orgLabelWidth, totalWidth, "statChange", "statChangeValue", new GUIContent("?"), new GUIContent("x"));
            }
            if (ins == MoveRouteInstruction.ChangeFacing)
            {
                SingleField(position, property, orgLabelWidth, totalWidth, "facing", new GUIContent("?"));
            }

            EditorGUIUtility.labelWidth = orgLabelWidth;
            property.serializedObject.ApplyModifiedProperties();
        }
        private static void SingleField(Rect position, SerializedProperty property, float orgLabelWidth, float totalWidth, string firstFieldName, GUIContent firstGUIContent)
        {
            position.x += totalWidth * 0.31f; //0.31
            position.width = totalWidth * 0.33f;
            var field = property.FindPropertyRelative(firstFieldName);

            EditorGUIUtility.labelWidth = 7f;
            EditorGUI.PropertyField(position, field, firstGUIContent);
            EditorGUIUtility.labelWidth = orgLabelWidth;
        }
        private static void DoubleField(Rect position, SerializedProperty property, float orgLabelWidth, float totalWidth, string firstFieldName, string secondFieldName, GUIContent firstGUIContent, GUIContent secondGUIContent)
        {
            position.x += totalWidth * 0.31f; //0.31
            position.width = totalWidth * 0.33f;
            var firstField = property.FindPropertyRelative(firstFieldName);

            EditorGUIUtility.labelWidth = 7f;
            EditorGUI.PropertyField(position, firstField, firstGUIContent);
            EditorGUIUtility.labelWidth = orgLabelWidth;

            position.x += totalWidth * 0.34f; //0.65
            position.width = totalWidth * 0.35f;
            var secondField = property.FindPropertyRelative(secondFieldName);
            EditorGUIUtility.labelWidth = 7f;
            EditorGUI.PropertyField(position, secondField, secondGUIContent);
            EditorGUIUtility.labelWidth = orgLabelWidth;
        }
    }
}