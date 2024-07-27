using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(UnitStatusComparator))]
    public class UnitStatusComparatorPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            DrawUnit(position, property, label);
            property.serializedObject.ApplyModifiedProperties();
        }
        private void DrawUnit(Rect position, SerializedProperty property, GUIContent label)
        {
            //position.x += position.width + 2;
            var orgX = position.x;
            var orgWidth = position.width;
            var orgLabel = EditorGUIUtility.labelWidth;

            float w = orgWidth * 0.33f - 2;
            position.width = w;

            var unitProp = property.FindPropertyRelative(nameof(UnitStatusComparator.targetUnit));
            EditorGUIUtility.labelWidth = 64;
            EditorGUI.PropertyField(position, unitProp, new GUIContent("Unit"));
            EditorGUIUtility.labelWidth = orgLabel;
            position.x += position.width + 2;
            
            var conditionTypeProp = property.FindPropertyRelative(nameof(UnitStatusComparator.unitCondition));
            EditorGUI.PropertyField(position, conditionTypeProp, new GUIContent(""));

            position.x += position.width + 2;
            EditorGUIUtility.labelWidth = 16;
            DrawOnType(position, property, new GUIContent(""));
            EditorGUIUtility.labelWidth = orgLabel;
        }
        private void DrawOnType(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueEnum = (UnitStatusConditionType)property.FindPropertyRelative("unitCondition").enumValueIndex;
            switch (valueEnum)
            {
                case UnitStatusConditionType.IsNamed:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(UnitStatusComparator.targetName)), label); break;
                case UnitStatusConditionType.HasJob:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(UnitStatusComparator.targetJob)), label); break;
                case UnitStatusConditionType.KnowsSkill:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(UnitStatusComparator.targetSkill)), label); break;
                case UnitStatusConditionType.HasWeaponEquipped:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(UnitStatusComparator.targetWeapon)), label); break;
                case UnitStatusConditionType.HasArmorEquipped:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(UnitStatusComparator.targetArmor)), label); break;
                case UnitStatusConditionType.IsStateInflicted:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(UnitStatusComparator.targetState)), label); break; ;
            }
        }
    }
}

