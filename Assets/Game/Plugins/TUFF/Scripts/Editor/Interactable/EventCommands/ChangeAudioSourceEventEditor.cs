using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(ChangeAudioSourceEvent)), CanEditMultipleObjects]
    public class ChangeAudioSourceEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            var au = serializedObject.FindProperty("audioSource");
            EditorGUILayout.PropertyField(au);
            if(au.objectReferenceValue != null)
            {
                var keepClip = serializedObject.FindProperty("keepClip");
                EditorGUILayout.PropertyField(keepClip);
                if (!keepClip.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("clip"));

                var keepVolume = serializedObject.FindProperty("keepVolume");
                EditorGUILayout.PropertyField(keepVolume);
                if (!keepVolume.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("volumeFadeDuration"));
                } 
            }
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var eventCommand = target as ChangeAudioSourceEvent;
            if (eventCommand.audioSource == null) return "No Audio Source set";
            string clip = (eventCommand.keepClip ? "Keep Clip" : $"Set {eventCommand.audioSource.gameObject.name} clip to {eventCommand.clip}");
            string volume = (eventCommand.keepVolume ? "Keep Volume" : $"Volume {eventCommand.volume}");

            return $"{clip}, {volume}";
        }
    }
}
