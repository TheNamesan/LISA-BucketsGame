using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(Unit))]
    public class UnitEditor : Editor
    {
        private Unit unit
        {
            get { return (target as Unit); }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var nameKey = serializedObject.FindProperty("nameKey");
            EditorGUILayout.PropertyField(nameKey);
            LISAEditorUtility.DrawDatabaseParsedTextPreview("Name", nameKey.stringValue);
            var fullNameKey = serializedObject.FindProperty("fullNameKey");
            EditorGUILayout.PropertyField(fullNameKey);
            LISAEditorUtility.DrawDatabaseParsedTextPreview("Full Name", fullNameKey.stringValue);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("initialJob"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("initialLevel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultMenuPortrait"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultFaceGraphic"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("primaryWeapon"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("secondaryWeapon"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("head"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("body"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("primaryAccessory"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("secondaryAccessory"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponTypes"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("armorTypes"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("features"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("winQuotes"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("levelUpQuotes"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dropsQuotes"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("bio"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("notes"));

            serializedObject.ApplyModifiedProperties();
        }
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return LISAEditorUtility.SpriteRenderStaticPreview(unit.defaultMenuPortrait, Color.white, width, height);
        }
    }
}

