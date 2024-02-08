using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(SkillsLearned))]
    public class SkillsLearnedPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 1;
            var learnType = property.FindPropertyRelative("learnType");
            if (learnType.boolValue) lines = 2;
            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                (EditorGUIUtility.singleLineHeight * (lines)) +
                (EditorGUIUtility.standardVerticalSpacing) + 4f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var levelLearnedAt = property.FindPropertyRelative("levelLearnedAt");
            var skill = property.FindPropertyRelative("skill");
            var learnType = property.FindPropertyRelative("learnType");

            position.height = 20f;

            EditorGUI.PropertyField(position, skill);
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            EditorGUI.PropertyField(position, learnType);
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            if ((LearnType)learnType.enumValueIndex == LearnType.Level)
            {
                EditorGUI.PropertyField(position, levelLearnedAt);
            }
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
