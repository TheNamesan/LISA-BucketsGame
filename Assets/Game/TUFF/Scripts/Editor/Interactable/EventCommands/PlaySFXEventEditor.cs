using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(PlaySFXEvent)), CanEditMultipleObjects]
    public class PlaySFXEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sfx"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }

        private string GetSummaryText()
        {
            var eventCommand = target as PlaySFXEvent;
            if (eventCommand.sfx == null) return "No SFX set";
            return $"Play '{(eventCommand.sfx.audioClip != null ? eventCommand.sfx.audioClip.name : "null")}' at Volume {eventCommand.sfx.volume}, Pitch {eventCommand.sfx.pitch}";
        }
    }
}
