using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(BattleType))]
    public class BattleTypePD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return 18f;
            return 20 + 20f * 2;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var obj = LISAEditorUtility.GetTargetObjectOfProperty(property) as BattleType;
            position.height = 20f;
            GUIContent guiContent = (obj.GetName() != null ? new GUIContent(obj.GetName()) : label);
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, guiContent);
            LISAEditorUtility.DrawSprite(new Rect(position.x + EditorGUIUtility.labelWidth, position.y, 20f, 20f), obj.icon);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                float oldLabel = EditorGUIUtility.labelWidth;
                var indentRect = EditorGUI.IndentedRect(position);
                indentRect.height = 20f;
                EditorGUIUtility.labelWidth -= 20f;
                indentRect.y += 20f;
                EditorGUI.PropertyField(indentRect, property.FindPropertyRelative("nameKey"));
                indentRect.y += 20f;
                var icon = property.FindPropertyRelative("icon");
                EditorGUI.PropertyField(indentRect, icon);
                indentRect.y += 40f;
                EditorGUI.indentLevel--;
                EditorGUIUtility.labelWidth = oldLabel;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}