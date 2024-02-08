using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(TargetedSkill))]
    public class TargetedSkillPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                ((EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3f) +
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = LISAEditorUtility.GetTargetObjectOfProperty(property) as TargetedSkill;
            position.height = 20f;
            SerializedProperty invocation = property.FindPropertyRelative("user");
            //Debug.Log(invocation.propertyPath);
            //EditorGUI.PropertyField(position, invocation);
            EditorGUI.LabelField(position, $"Skill: {target.skill.GetName()}");
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            LISAUtility.CheckLocaleIsNotNull();
            string targetName = "null";
            if(target.targets != null)
            {
                targetName = "";
                for (int i = 0; i < target.targets.Count; i++)
                {
                    targetName += target.targets[i].GetName();
                    if (i + 1 < target.targets.Count) targetName += ", ";
                }
            }
            EditorGUI.LabelField(position, $"Target: {targetName}");
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            string userName = "null";
            if (target.user != null)
            {
                userName = target.user.GetName();
            }
            EditorGUI.LabelField(position, $"User: {userName}");
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            EditorGUI.LabelField(position, $"Attack Speed: {target.attackSpeed}");
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
