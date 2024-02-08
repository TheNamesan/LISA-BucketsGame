using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeCharacterSpriteAction))]
    public class ChangeCharacterSpriteActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var originType = targetProperty.FindPropertyRelative("originType");
            EditorGUILayout.PropertyField(originType);
            if ((FieldOriginType)originType.enumValueIndex == FieldOriginType.FromScene)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("targetAnimationHandler"));
            }
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("sprite"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeCharacterSpriteAction;

            string origin = (action.targetAnimationHandler != null && action.originType == FieldOriginType.FromScene ?
                action.targetAnimationHandler.name : "(No Handler Set)");
            if (action.originType == FieldOriginType.FromPersistentInstance) { origin = "Follower Instance"; }
            string clip = (action.sprite != null ? action.sprite.name : "null");

            return $"Change Sprite {clip} in {origin}";
        }
    }
}
