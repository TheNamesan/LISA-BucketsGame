using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(HitFormula))]
    public class HitFormulaPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float separation = 1f;
            float formulaElemHeight = 0f;
            if (property.isExpanded)
            {
                formulaElemHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("formulaGroups")); 
                separation = 6f;
            } 
            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                (EditorGUIUtility.singleLineHeight * 0f) + separation + formulaElemHeight +
                (EditorGUIUtility.standardVerticalSpacing) + 2f ;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            if (property.isExpanded)
            {
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                EditorGUI.indentLevel++;
                Rect rect = EditorGUI.IndentedRect(position);
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("formulaGroups"));
                EditorGUI.indentLevel--;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPropertyDrawer(typeof(HitFormulaOperation))]
    public class HitFormulaOperationPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                (EditorGUIUtility.singleLineHeight * 0f) +
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            float totalWidth = position.width;

            if (LISAEditorUtility.GetArrayIndexFromPath(property.propertyPath) != 0)
            {
                position.width = totalWidth * 0.23f;
                var formulaAddProp = property.FindPropertyRelative("formulaOp");
                string[] options = { "None", "+", "-", "x", @"\" };
                int[] values = { 0, 1, 2, 3, 4 };
                formulaAddProp.enumValueIndex = EditorGUI.IntPopup(position, formulaAddProp.intValue, options, values);

            }

            position.x += totalWidth * 0.23f; //0.23
            position.width = totalWidth * 0.34f;
            var formulaCast = property.FindPropertyRelative("formulaTargetable");
            formulaCast.enumValueIndex = (int)(HitFormulaOperation.FormulaCasterType)EditorGUI.EnumPopup(position, (HitFormulaOperation.FormulaCasterType)formulaCast.enumValueIndex);

            position.x += totalWidth * 0.34f; //0.57
            position.width = totalWidth * 0.43f;
            if ((HitFormulaOperation.FormulaCasterType)formulaCast.enumValueIndex == HitFormulaOperation.FormulaCasterType.FlatNumber)
            {
                var flatNumber = property.FindPropertyRelative("flatNumber");
                flatNumber.floatValue = EditorGUI.FloatField(position, flatNumber.floatValue);
            }
            else
            {
                var formulaStat = property.FindPropertyRelative("formulaStat");
                formulaStat.enumValueIndex = (int)(HitFormulaOperation.FormulaStatType)EditorGUI.EnumPopup(position, (HitFormulaOperation.FormulaStatType)formulaStat.enumValueIndex);
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
    [CustomPropertyDrawer(typeof(HitFormulaGroup))]
    public class HitFormulaGroupPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                (EditorGUIUtility.singleLineHeight * 0f) + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("formulaOperations")) +
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            float totalWidth = position.width;

            var formulaOps = property.FindPropertyRelative("formulaOperations");
            var formulaGroupOp = property.FindPropertyRelative("formulaGroupOp");

            if(LISAEditorUtility.GetArrayIndexFromPath(property.propertyPath) != 0)
            {
                position.width = totalWidth * 0.1f;
                string[] options = { "None", "+", "-", "x", @"\" };
                int[] values = { 0, 1, 2, 3, 4 };
                formulaGroupOp.enumValueIndex = EditorGUI.IntPopup(position, formulaGroupOp.intValue, options, values);
            }

            position.x += totalWidth * 0.125f; //0.1
            position.width = totalWidth * 0.875f;
            EditorGUI.PropertyField(position, formulaOps);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}

