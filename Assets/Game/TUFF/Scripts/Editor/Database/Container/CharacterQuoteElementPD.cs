using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(CharacterQuoteElement))]
    public class CharacterQuoteElementPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return 18f;
            return 20 + 20f * 1;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var obj = LISAEditorUtility.GetTargetObjectOfProperty(property) as CharacterQuoteElement;
            position.height = 20f;
            GUIContent guiContent = (obj.GetQuote() != null ? new GUIContent(obj.GetQuote()) : label);
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, guiContent);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                float oldLabel = EditorGUIUtility.labelWidth;
                var indentRect = EditorGUI.IndentedRect(position);
                indentRect.height = 20f;
                EditorGUIUtility.labelWidth -= 20f;
                indentRect.y += 20f;
                EditorGUI.PropertyField(indentRect, property.FindPropertyRelative("quoteKey"));
                EditorGUI.indentLevel--;
                EditorGUIUtility.labelWidth = oldLabel;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}

