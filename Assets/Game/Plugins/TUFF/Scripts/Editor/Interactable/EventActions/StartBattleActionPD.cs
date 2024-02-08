using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(StartBattleAction))]
    public class StartBattleActionPD : EventActionPD
    {
        int popupValue = 0;
        public override void InspectorGUIContent()
        {
            var battle = targetObject as StartBattleAction;
            var battleTarget = targetProperty.FindPropertyRelative("battle");

            DatabaseDropdownDrawer.DrawBattlesDropdown(ref popupValue, battleTarget);
            GUIContent content = new GUIContent(battleTarget.displayName, battleTarget.tooltip);
            EditorGUILayout.PropertyField(battleTarget);
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as StartBattleAction;
            if (action.battle == null) return "No Battle set";
            return $"Start '{action.battle.name}' Battle";
        }
    }
}

