using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(DropItem))]
    public class DropItemPD : PropertyDrawer
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

            position.width = totalWidth * 0.3f;
            var dropType = property.FindPropertyRelative("dropType");
            EditorGUI.PropertyField(position, dropType, GUIContent.none);

            position.x += totalWidth * 0.31f; //0.31
            position.width = totalWidth * 0.33f;
            if(dropType.enumValueIndex == (int)DropType.Item)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("item"), GUIContent.none);
            }
            if (dropType.enumValueIndex == (int)DropType.KeyItem)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("keyItem"), GUIContent.none);
            }
            if (dropType.enumValueIndex == (int)DropType.Weapon)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("weapon"), GUIContent.none);
            }
            if(dropType.enumValueIndex == (int)DropType.Armor)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("armor"), GUIContent.none);
            }

            position.x += totalWidth * 0.34f; //0.65
            position.width = totalWidth * 0.35f;
            var orgLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 65f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("dropChance"), new GUIContent("Chance%"));
            EditorGUIUtility.labelWidth = orgLabelWidth;

            property.serializedObject.ApplyModifiedProperties();
        }
    }

}
