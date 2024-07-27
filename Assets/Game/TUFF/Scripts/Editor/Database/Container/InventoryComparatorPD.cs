using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(InventoryComparator))]
    public class InventoryComparatorPD : PropertyDrawer
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

            float w = orgWidth * 0.20f - 2;
            position.width = w;

            var typeProp = property.FindPropertyRelative(nameof(InventoryComparator.inventoryType));
            EditorGUI.PropertyField(position, typeProp, new GUIContent(""));
            position.x += position.width + 2;
            var type = (DropType)typeProp.enumValueIndex;

            DrawFieldOnType(position, property, new GUIContent(""), type);
            position.x += position.width + 2;

            var comparisonProp = property.FindPropertyRelative(nameof(InventoryComparator.numberComparison));
            EditorGUI.PropertyField(position, comparisonProp, new GUIContent(""));
            position.x += position.width + 2;

            var countProp = property.FindPropertyRelative(nameof(InventoryComparator.targetItemCount));
            EditorGUI.PropertyField(position, countProp, new GUIContent(""));
            position.x += position.width + 2;

            if (type == DropType.Weapon || type == DropType.Armor)
            {
                EditorGUIUtility.labelWidth = 120f;
                var includeProp = property.FindPropertyRelative(nameof(InventoryComparator.includeEquipment));
                EditorGUI.PropertyField(position, includeProp);
                position.x += position.width + 2;
                EditorGUIUtility.labelWidth = orgLabel;
            }
        }
        private void DrawFieldOnType(Rect position, SerializedProperty property, GUIContent label, DropType type)
        {
            switch (type)
            {
                case DropType.Item:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(InventoryComparator.targetItem)), label); break;
                case DropType.KeyItem:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(InventoryComparator.targetKeyItem)), label); break;
                case DropType.Weapon:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(InventoryComparator.targetWeapon)), label); break;
                case DropType.Armor:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(InventoryComparator.targetArmor)), label); break;
            }
        }
    }
}
