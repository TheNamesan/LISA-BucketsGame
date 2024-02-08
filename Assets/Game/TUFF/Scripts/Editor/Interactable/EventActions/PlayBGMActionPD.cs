using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(PlayBGMAction))]
    public class PlayBGMActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("bgmPlayData"), new GUIContent("BGM Play Data"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("fadeInDuration"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }

        private string GetSummaryText()
        {
            var action = targetObject as PlayBGMAction;
            if (action.bgmPlayData == null) return "No BGM set";
            if (action.bgmPlayData.bgm == null) return "No BGM set";
            float dur = action.fadeInDuration;
            return $"Play '{action.bgmPlayData.bgm.songName}' BGM, at Volume {action.bgmPlayData.volume}, Pitch {action.bgmPlayData.pitch}" +
                $"{(dur > 0 ? $" with {dur} second{(dur == 1 ? "" : "s")} Fade In" : "")}";
        }
    }
}

