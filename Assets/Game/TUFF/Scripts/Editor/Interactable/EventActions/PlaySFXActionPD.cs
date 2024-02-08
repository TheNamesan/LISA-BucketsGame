using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(PlaySFXAction))]
    public class PlaySFXActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("sfxs"), new GUIContent("SFXs"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }

        private string GetSummaryText()
        {
            var action = targetObject as PlaySFXAction;
            if (action.sfxs == null) return "No SFX List";
            if (action.sfxs.Count <= 0) return "No SFXs";
            return $"({action.sfxs.Count})Play '{(action.sfxs[0].audioClip != null ? action.sfxs[0].audioClip.name : "null")}' at Volume {action.sfxs[0].volume}, Pitch {action.sfxs[0].pitch}";
        }
    }
}

