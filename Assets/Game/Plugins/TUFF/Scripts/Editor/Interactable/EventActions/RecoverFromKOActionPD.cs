using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(RecoverFromKOAction))]
    public class RecoverFromKOActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var scope = targetProperty.FindPropertyRelative("scope");
            EditorGUILayout.PropertyField(scope);

            if (scope.enumValueIndex == (int)PartyScope.OnePartyMember)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("unit"));
            }
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as RecoverFromKOAction;
            string unitName = (action.unit == null ? "null" : action.unit.GetName());
            string scope = (action.scope == PartyScope.EntireParty ? "Entire Party" : unitName);

            return $"Recover {scope} from KO";
        }
    }
}