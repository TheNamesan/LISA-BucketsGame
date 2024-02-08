using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(Item)), CanEditMultipleObjects]
    public class ItemEditor : Editor
    {
        int popupValue = 0;
        private Item item
        {
            get { return (target as Item); }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var nameKey = serializedObject.FindProperty("nameKey");
            EditorGUILayout.PropertyField(nameKey);
            LISAEditorUtility.DrawDatabaseParsedTextPreview("Name", nameKey.stringValue);
            var descriptionKey = serializedObject.FindProperty("descriptionKey");
            EditorGUILayout.PropertyField(descriptionKey);
            LISAEditorUtility.DrawDatabaseParsedTextPreview("Description", descriptionKey.stringValue, true);

            var icon = serializedObject.FindProperty("m_icon");
            if (Selection.count <= 1) icon.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Icon", icon.objectReferenceValue, typeof(Sprite), false);
            else EditorGUILayout.PropertyField(icon);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("price"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("consumable"));

            SerializedProperty sco = serializedObject.FindProperty("scopeData");
            EditorGUILayout.PropertyField(sco);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_occasion"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_speed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_repeats"));

            var animation = serializedObject.FindProperty("m_animation");
            DatabaseDropdownDrawer.DrawAnimationsDropdown(ref popupValue, animation);
            EditorGUILayout.PropertyField(animation);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_startEvents"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_endEvents"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_endDelay"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("notes"));

            serializedObject.ApplyModifiedProperties();
        }
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return LISAEditorUtility.SpriteRenderStaticPreview(item.icon, Color.white, width, height);
        }
    }
}

