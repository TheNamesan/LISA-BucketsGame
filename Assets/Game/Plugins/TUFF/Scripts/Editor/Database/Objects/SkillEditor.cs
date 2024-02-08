using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(Skill)), CanEditMultipleObjects]
    public class SkillEditor : Editor
    {
        int popupValue = 0;
        private Skill skill
        {
            get { return (target as Skill); }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            GUI.enabled = true;

            var nameKey = serializedObject.FindProperty("nameKey");
            EditorGUILayout.PropertyField(nameKey);
            LISAEditorUtility.DrawDatabaseParsedTextPreview("Name", nameKey.stringValue);
            var descriptionKey = serializedObject.FindProperty("descriptionKey");
            EditorGUILayout.PropertyField(descriptionKey);
            LISAEditorUtility.DrawDatabaseParsedTextPreview("Description", descriptionKey.stringValue, true);

            var icon = serializedObject.FindProperty("m_icon");
            if (Selection.count <= 1) icon.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Icon", icon.objectReferenceValue, typeof(Sprite), false);
            else EditorGUILayout.PropertyField(icon);

            SerializedProperty sp = serializedObject.FindProperty("SPCost");
            EditorGUILayout.PropertyField(sp);
            SerializedProperty tp = serializedObject.FindProperty("TPCost");
            EditorGUILayout.PropertyField(tp);
            SerializedProperty requiredItem = serializedObject.FindProperty("requiredItem");
            EditorGUILayout.PropertyField(requiredItem);
            if (requiredItem.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("itemAmount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("consumeItem"));
            }

            SerializedProperty sco = serializedObject.FindProperty("scopeData");
            EditorGUILayout.PropertyField(sco);

            SerializedProperty occ = serializedObject.FindProperty("m_occasion");
            EditorGUILayout.PropertyField(occ);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_speed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_repeats"));

            var animation = serializedObject.FindProperty("m_animation");
            DatabaseDropdownDrawer.DrawAnimationsDropdown(ref popupValue, animation);
            EditorGUILayout.PropertyField(animation);

            var prefix = serializedObject.FindProperty("prefix");
            EditorGUILayout.PropertyField(prefix);
            var useKey = serializedObject.FindProperty("useMessageKey");
            EditorGUILayout.PropertyField(useKey);
            LISAEditorUtility.DrawDatabaseParsedTextPreview("Use Message", useKey.stringValue, prefix: $"{(prefix.enumValueIndex == (int)UseMessagePrefix.UserName ? "<User>" : "")}");

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_startEvents"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_endEvents"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_endDelay"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("comboDialMove"));

            
            var isUnitedSkill = serializedObject.FindProperty("isUnitedSkill");
            EditorGUILayout.PropertyField(isUnitedSkill);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UPCost"), new GUIContent("UP% Cost"));
            if (isUnitedSkill.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("unitedUserA"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("unitedUserB"));
            }
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("notes"));

            serializedObject.ApplyModifiedProperties();
        }
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return LISAEditorUtility.SpriteRenderStaticPreview(skill.icon, Color.white, width, height);
        }
    }
}

