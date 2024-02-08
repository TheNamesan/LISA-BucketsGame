using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(Armor))]
    public class ArmorEditor : Editor
    {
        private Armor armor
        {
            get { return (target as Armor); }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var arm = target as Armor;
            //GUI.enabled = false;
            //SerializedProperty prop = serializedObject.FindProperty("m_Script");
            //EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            //GUI.enabled = true;

            var nameKey = serializedObject.FindProperty("nameKey");
            EditorGUILayout.PropertyField(nameKey);
            LISAEditorUtility.DrawDatabaseParsedTextPreview("Name", nameKey.stringValue);
            var descriptionKey = serializedObject.FindProperty("descriptionKey");
            EditorGUILayout.PropertyField(descriptionKey);
            LISAEditorUtility.DrawDatabaseParsedTextPreview("Description", descriptionKey.stringValue, true);

            var icon = serializedObject.FindProperty("m_icon");
            icon.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Icon", icon.objectReferenceValue, typeof(Sprite), false);
            SerializedProperty art = serializedObject.FindProperty("armorType");

            var armorTypes = TUFFSettings.armorTypes;
            string[] options = new string[armorTypes.Count];
            int[] values = new int[armorTypes.Count];
            for (int i = 0; i < armorTypes.Count; i++)
            {
                options[i] = armorTypes[i].GetName();
                values[i] = i;
            }
            art.intValue = EditorGUILayout.IntPopup("Armor Type", art.intValue, options, values);

            SerializedProperty eqt = serializedObject.FindProperty("equipType");
            EditorGUILayout.PropertyField(eqt);
            SerializedProperty pri = serializedObject.FindProperty("price");
            EditorGUILayout.PropertyField(pri);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("Stat Bonuses", "Flat value bonuses for the user’s stats. The values can also be negative."), EditorStyles.boldLabel);
            SerializedProperty mHP = serializedObject.FindProperty("m_maxHP");
            EditorGUILayout.PropertyField(mHP);
            SerializedProperty mSP = serializedObject.FindProperty("m_maxSP");
            EditorGUILayout.PropertyField(mSP);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxTP"));
            SerializedProperty ATK = serializedObject.FindProperty("m_ATK");
            EditorGUILayout.PropertyField(ATK);
            SerializedProperty DEF = serializedObject.FindProperty("m_DEF");
            EditorGUILayout.PropertyField(DEF);
            SerializedProperty SATK = serializedObject.FindProperty("m_SATK");
            EditorGUILayout.PropertyField(SATK);
            SerializedProperty SDEF = serializedObject.FindProperty("m_SDEF");
            EditorGUILayout.PropertyField(SDEF);
            SerializedProperty AGI = serializedObject.FindProperty("m_AGI");
            EditorGUILayout.PropertyField(AGI);
            SerializedProperty LUK = serializedObject.FindProperty("m_LUK");
            EditorGUILayout.PropertyField(LUK);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_features"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("notes"));

            serializedObject.ApplyModifiedProperties();
        }
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return LISAEditorUtility.SpriteRenderStaticPreview(armor.icon, Color.white, width, height);
        }
    }
}