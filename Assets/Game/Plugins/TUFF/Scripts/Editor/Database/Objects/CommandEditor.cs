using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(Command))]
    public class CommandEditor : Editor
    {
        private Command command
        {
            get { return (target as Command); }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var nameKey = serializedObject.FindProperty("nameKey");
            EditorGUILayout.PropertyField(nameKey);
            LISAEditorUtility.DrawDatabaseParsedTextPreview("Name", nameKey.stringValue, true);

            var icon = serializedObject.FindProperty("icon");
            icon.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Icon", icon.objectReferenceValue, typeof(Sprite), false);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("commandType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("skills"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("notes"));

            serializedObject.ApplyModifiedProperties();
        }
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return LISAEditorUtility.SpriteRenderStaticPreview(command.icon, Color.white, width, height);
        }
    }
}

