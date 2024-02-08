using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(FlashData))]
    public class FlashDataPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return 18f;
            return 20 + 20f * 2;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                float oldLabel = EditorGUIUtility.labelWidth;
                var indentRect = EditorGUI.IndentedRect(position);
                indentRect.height = 20f;
                EditorGUIUtility.labelWidth -= 20f;
                indentRect.y += 20f;
                EditorGUI.PropertyField(indentRect, property.FindPropertyRelative("flashColor"));
                indentRect.y += 20f;
                EditorGUI.PropertyField(indentRect, property.FindPropertyRelative("flashDuration"));
                indentRect.y += 40f;
                EditorGUI.indentLevel--;
                EditorGUIUtility.labelWidth = oldLabel;
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
    
}

