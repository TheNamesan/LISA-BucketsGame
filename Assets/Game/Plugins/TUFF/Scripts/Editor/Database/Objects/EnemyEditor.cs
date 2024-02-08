using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(Enemy))]
    public class EnemyEditor : Editor
    {
        private Enemy enemy
        {
            get { return (target as Enemy); }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var nameKey = serializedObject.FindProperty("nameKey");
            EditorGUILayout.PropertyField(nameKey);
            LISAEditorUtility.DrawDatabaseParsedTextPreview("Name", nameKey.stringValue);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("graphic"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHP"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSP"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxTP"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ATK"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DEF"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SATK"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SDEF"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AGI"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LUK"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetRate"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hitRate"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("evasionRate"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("critRate"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("critEvasionRate"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("features"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("EXP"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mags"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("drops"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("actionPatterns"));

            

            var KOmotion = serializedObject.FindProperty("KOMotion");
            EditorGUILayout.PropertyField(KOmotion);
            /*if(KOmotion.enumValueIndex == (int)MotionKOType.Custom)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("customKOMotion"), new GUIContent("Custom KO Motion"));
            }*/

            EditorGUILayout.PropertyField(serializedObject.FindProperty("notes"));

            serializedObject.ApplyModifiedProperties();
        }
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return LISAEditorUtility.SpriteRenderStaticPreview(enemy.graphic, Color.white, width, height);
        }
    }
}

