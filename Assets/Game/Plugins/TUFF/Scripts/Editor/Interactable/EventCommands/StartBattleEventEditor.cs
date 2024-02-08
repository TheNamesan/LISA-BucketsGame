using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(StartBattleEvent)), CanEditMultipleObjects]
    public class StartBattleEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("battle"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var eventCommand = target as StartBattleEvent;
            if (eventCommand.battle == null) return "No Battle set";
            return $"Start {eventCommand.battle.name} Battle";
        }
    }
}