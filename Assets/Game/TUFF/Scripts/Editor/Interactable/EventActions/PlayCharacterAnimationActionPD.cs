using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(PlayCharacterAnimationAction))]
    public class PlayCharacterAnimationActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var originType = targetProperty.FindPropertyRelative("originType");
            EditorGUILayout.PropertyField(originType);
            if ((FieldOriginType)originType.enumValueIndex == FieldOriginType.FromScene)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("targetSceneCharacter"));
            }
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("clip"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as PlayCharacterAnimationAction;

            string origin = (action.targetSceneCharacter != null && action.originType == FieldOriginType.FromScene ?
                action.targetSceneCharacter.name : "(No Handler Set)");
            if (action.originType == FieldOriginType.FromPersistentInstance) { origin = "Follower Instance"; }
            string clip = (action.clip != null ? action.clip.name : "null");

            return $"Play Animation {clip} in {origin}";
        }
    }
}
