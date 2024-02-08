using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(AnimationSequenceElement))]
    public class AnimationSequenceElementPD : PropertyDrawer
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
            var elementType = property.FindPropertyRelative("elementType");
            EditorGUI.PropertyField(position, elementType, new GUIContent(""));
            if(elementType.enumValueIndex == (int)AnimSequenceElementType.PlayAnimation)
            {
                position.x += totalWidth * 0.31f; //0.31
                position.width = totalWidth * 0.69f;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("animation"), new GUIContent(""));
            }
            if (elementType.enumValueIndex == (int)AnimSequenceElementType.WaitForSeconds)
            {
                position.x += totalWidth * 0.31f; //0.31
                position.width = totalWidth * 0.69f;
                var orgLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 65f;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("waitSeconds"), new GUIContent("Seconds"));
                EditorGUIUtility.labelWidth = orgLabelWidth;
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}

