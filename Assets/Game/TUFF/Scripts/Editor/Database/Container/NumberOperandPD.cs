using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(NumberOperand))]
    public class NumberOperandPD : PropertyDrawer
    {
        private GUIContent empty = new GUIContent("");
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            position.width *= 0.5f;
            var operandTypeProp = property.FindPropertyRelative("operandType");
            var operandType = (NumberOperandType)operandTypeProp.enumValueIndex;
            EditorGUI.PropertyField(position, operandTypeProp, label);
            position.x += position.width;
            if (operandType == NumberOperandType.FromConstant)
                EditorGUI.PropertyField(position, property.FindPropertyRelative("constant"), empty);
            else LISAEditorUtility.DrawVariableList(position, property.FindPropertyRelative("variableIndex"), empty);
        }
    }
}

