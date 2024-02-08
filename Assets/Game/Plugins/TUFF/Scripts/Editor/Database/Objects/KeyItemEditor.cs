using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(KeyItem))]
    public class KeyItemEditor : Editor
    {
        private KeyItem keyItem
        {
            get { return (target as KeyItem); }
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
            icon.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Icon", icon.objectReferenceValue, typeof(Sprite), false);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("price"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("notes"));

            serializedObject.ApplyModifiedProperties();
        }
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return LISAEditorUtility.SpriteRenderStaticPreview(keyItem.icon, Color.white, width, height);
        }
    }
}

