using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(PlayBGMEvent)), CanEditMultipleObjects]
    public class PlayBGMEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bgmPlayData"), new GUIContent("BGM Play Data"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeInDuration"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }

        private string GetSummaryText()
        {
            var eventCommand = target as PlayBGMEvent;
            if (eventCommand.bgmPlayData == null) return "No BGM set";
            if (eventCommand.bgmPlayData.bgm == null) return "No BGM set";
            float dur = eventCommand.fadeInDuration;
            return $"Play '{eventCommand.bgmPlayData.bgm.songName}' BGM, at Volume {eventCommand.bgmPlayData.volume}, Pitch {eventCommand.bgmPlayData.pitch}" +
                $"{(dur > 0 ? $" with {dur} second{(dur == 1 ? "":"s")} Fade In" : "")}";
        }
    }
}
