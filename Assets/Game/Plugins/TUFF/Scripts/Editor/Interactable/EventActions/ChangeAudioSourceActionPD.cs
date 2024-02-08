using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeAudioSourceAction))]
    public class ChangeAudioSourceActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var au = targetProperty.FindPropertyRelative("audioSource");
            EditorGUILayout.PropertyField(au);
            if (au.objectReferenceValue != null)
            {
                var keepClip = targetProperty.FindPropertyRelative("keepClip");
                EditorGUILayout.PropertyField(keepClip);
                if (!keepClip.boolValue) EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("clip"));

                var keepVolume = targetProperty.FindPropertyRelative("keepVolume");
                EditorGUILayout.PropertyField(keepVolume);
                if (!keepVolume.boolValue)
                {
                    EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("volume"));
                    EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("volumeFadeDuration"));
                }
            }
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeAudioSourceAction;
            if (action.audioSource == null) return "No Audio Source set";
            string clip = (action.keepClip ? "Keep Clip" : $"Set {action.audioSource.gameObject.name} clip to {action.clip}");
            string volume = (action.keepVolume ? "Keep Volume" : $"Volume {action.volume}");

            return $"{clip}, {volume}";
        }
    }
}

