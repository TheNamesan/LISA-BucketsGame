using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeSwitchAction))]
    public class ChangeSwitchActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var objTarget = targetProperty.FindPropertyRelative("target");
            objTarget.objectReferenceValue = EditorGUILayout.ObjectField(objTarget.displayName, objTarget.objectReferenceValue, typeof(InteractableObject), true);
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("newSwitch"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeSwitchAction;
            if (action.target == null) return "No target set";
            return $"Set {action.target.gameObject.name} switch to {action.newSwitch}";
        }
    }
}

