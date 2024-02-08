using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(SetMoveRouteAction))]
    public class SetMoveRouteActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var originType = targetProperty.FindPropertyRelative("originType");
            EditorGUILayout.PropertyField(originType);
            if ((FieldOriginType)originType.enumValueIndex == FieldOriginType.FromScene)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("sceneMoveRouteHandler"));
            }
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("moveRoute"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("waitForCompletion"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as SetMoveRouteAction;
            int count = -1;
            if (action.moveRoute != null && action.moveRoute.elements != null)
                count = action.moveRoute.elements.Count;
            string wait = (action.waitForCompletion ? " (Wait)" : "");
            return $"Set Move Route ({count}){wait}";
        }
    }
}
