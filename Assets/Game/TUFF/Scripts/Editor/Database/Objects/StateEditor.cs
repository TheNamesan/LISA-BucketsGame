using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(State))]
    public class StateEditor : Editor
    {
        private State state
        {
            get { return (target as State); }
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

            var icon = serializedObject.FindProperty("icon");
            icon.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Icon", icon.objectReferenceValue, typeof(Sprite), false);
            var restriction = serializedObject.FindProperty("restriction");
            EditorGUILayout.PropertyField(restriction);
            if((Restriction)restriction.enumValueIndex == Restriction.ForceSkills)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("forcedActionPatterns"));
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hidden"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stateType"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("removeAtBattleEnd"));
            var autoRemovalTiming = serializedObject.FindProperty("autoRemovalTiming");
            EditorGUILayout.PropertyField(autoRemovalTiming);
            if((AutoRemovalTiming)autoRemovalTiming.enumValueIndex != AutoRemovalTiming.None)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("durationInTurns"));
            }
            var removeByDamage = serializedObject.FindProperty("removeByDamage");
            EditorGUILayout.PropertyField(removeByDamage);
            if(removeByDamage.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("removeByDamageChance"));
            }
            var removeByWalking = serializedObject.FindProperty("removeByWalking");
            EditorGUILayout.PropertyField(removeByWalking);
            if(removeByWalking.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("removeByWalkingSeconds"));
            }

            var progressiveState = serializedObject.FindProperty("progressiveState");
            EditorGUILayout.PropertyField(progressiveState);
            if (progressiveState.objectReferenceValue != null)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("progressiveStateTriggerChance"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("features"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("visual"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("notes"));

            serializedObject.ApplyModifiedProperties();
        }
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return LISAEditorUtility.SpriteRenderStaticPreview(state.icon, Color.white, width, height);
        }
    }
}

