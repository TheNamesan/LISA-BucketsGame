using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeGameObjectAction))]
    public class ChangeGameObjectActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var gameObject = targetProperty.FindPropertyRelative("gameObject");
            EditorGUILayout.PropertyField(gameObject);

            // Active
            var keepActive = targetProperty.FindPropertyRelative("keepActive");
            EditorGUILayout.PropertyField(keepActive);
            if (!keepActive.boolValue)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("setActive"));
            }
            // Name
            var changeName = targetProperty.FindPropertyRelative("changeName");
            EditorGUILayout.PropertyField(changeName);
            if (changeName.boolValue)
            {
                EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("newName"));
            }
            // Tag
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
            int[] values = new int[tags.Length];
            for (int i = 0; i < tags.Length; i++)
            {
                values[i] = i;
            }
            var keepTag = targetProperty.FindPropertyRelative("keepTag");
            EditorGUILayout.PropertyField(keepTag);
            if (!keepTag.boolValue)
            {
                var newTag = targetProperty.FindPropertyRelative("newTag");

                string tag = "";
                if (gameObject.objectReferenceValue) tag = newTag.stringValue;
                int index = System.Array.IndexOf(tags, tag);
                if (index < 0) index = 0;
                index = EditorGUILayout.IntPopup("Tag", index, tags, values);
                newTag.stringValue = tags[index];

            }
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeGameObjectAction;
            if (action.gameObject == null) return "No Game Object set";
            string name = action.gameObject.name;
            string active = "";
            if (!action.keepActive)
            {
                active = $"[Active: {action.setActive}]";
            }
            string newName = "";
            if (action.changeName)
            {
                newName = $"[Name: {action.newName}]";
            }
            string newTag = "";
            if (!action.keepTag)
            {
                newTag = $"[Tag: {action.newTag}]";
            }
            return $"Set {name}: {active}{newName}{newTag}";
        }
    }
}
