using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(InteractableEventList))]
    public class InteractableEventListEditor : PropertyDrawer
    {
        int lines = 0;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 16 + (lines * 18) + 2;
        }
        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            lines = 0;
            label = EditorGUI.BeginProperty(rect, label, prop);
            Rect contentRect = EditorGUI.PrefixLabel(rect, label);
            if(rect.height > 16f)
            {
                rect.height = 16f;
                EditorGUI.indentLevel += 1;
                contentRect = EditorGUI.IndentedRect(rect);
                contentRect.y += 18f;
            }
            EditorGUI.indentLevel = 0;
            SerializedProperty index = prop.FindPropertyRelative("currentEventIndex");
            SerializedProperty content = prop.FindPropertyRelative("content");
            var comms = LISAEditorUtility.GetTargetObjectOfProperty(content) as List<EventCommand>;

            lines++;
            EditorGUI.PropertyField(contentRect, index);

            lines++;
            contentRect.y += 18f;
            EditorGUI.LabelField(contentRect, new GUIContent($"Event Count: {comms.Count}"));

            lines++;
            contentRect.y += 18f;
            GUIContent eventList = new GUIContent("Content", "Show the event list.");
        
            if(GUI.Button(contentRect, eventList))
            {
                EventListWindow.ShowWindow(LISAEditorUtility.GetArrayIndexFromPath(prop.propertyPath));
            }

            EditorGUI.EndProperty();
            content.serializedObject.ApplyModifiedProperties();
        }
    }
}
