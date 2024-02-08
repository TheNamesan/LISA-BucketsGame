using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(BattleEvent))]
    public class BattleEventPD : PropertyDrawer
    {
        const float lineSkip = 20f;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var conditionsHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("conditions"));
            if (!property.isExpanded) return 20f;
            else return 60f + conditionsHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var orgX = position.x;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            position.y += lineSkip;
            if (property.isExpanded)
            {
                var content = property.FindPropertyRelative("actionList.content");
                EditorGUI.LabelField(position, $"Event Count: {content.arraySize}");
                position.y += lineSkip;
                var conditions = property.FindPropertyRelative("conditions");
                EditorGUI.PropertyField(position, conditions);
                position.y += EditorGUI.GetPropertyHeight(conditions);
                if (GUI.Button(position, new GUIContent("Content", "Show the event list.")))
                {
                    EventActionListWindow.ShowWindow();
                }
                position.y += lineSkip;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
