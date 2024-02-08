using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeSkillsAction))]
    public class ChangeSkillsActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var scope = targetProperty.FindPropertyRelative("scope");
            EditorGUILayout.PropertyField(scope);

            if (scope.enumValueIndex == (int)PartyScope.OnePartyMember)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("unit"));
            }
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("operation"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("skill"));
        }

        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeSkillsAction;
            string unitName = (action.unit == null ? "null" : action.unit.GetName());
            string skillName = (action.skill == null ? "null" : action.skill.GetName());
            string scope = (action.scope == PartyScope.EntireParty ? "Entire Party" : unitName);
            var operation = action.operation;

            return $"{scope}: {operation} {skillName}";
        }
    }
}

