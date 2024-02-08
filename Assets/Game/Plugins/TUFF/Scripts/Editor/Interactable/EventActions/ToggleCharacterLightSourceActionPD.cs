using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ToggleCharacterLightSourceAction))]
    public class ToggleCharacterLightSourceActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var originType = targetProperty.FindPropertyRelative("originType");
            EditorGUILayout.PropertyField(originType);
            if ((FieldOriginType)originType.enumValueIndex == FieldOriginType.FromScene)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("sceneCharacter"));
            }
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("enable"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            
            var action = targetObject as ToggleCharacterLightSourceAction;
            string origin = "null";
            if (action.originType == FieldOriginType.FromPersistentInstance) { origin = "Follower Instance"; }
            if (action.originType == FieldOriginType.FromScene && action.sceneCharacter) 
            { origin = action.sceneCharacter.name; }

            return $"{(action.enable ? "Enable" : "Disable")} {origin} Light Source";
        }
    }
}
