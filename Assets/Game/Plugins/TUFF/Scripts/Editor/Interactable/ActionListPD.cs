using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ActionList))]
    public class ActionListPD : PropertyDrawer
    {
        public static void DrawPreview(ref Rect rect, SerializedProperty prop, GUIContent label)
        {
            label = EditorGUI.BeginProperty(rect, label, prop);
            EditorGUI.LabelField(rect, label);

            rect.height = 20f;
            EditorGUI.indentLevel += 1;
            rect = EditorGUI.IndentedRect(rect);
            rect.y += 20f;

            EditorGUI.indentLevel = 0;
            SerializedProperty index = prop.FindPropertyRelative("index");
            SerializedProperty content = prop.FindPropertyRelative("content");

            
            var comms = LISAEditorUtility.GetTargetObjectOfProperty(content) as List<EventAction>;
            EditorGUI.PropertyField(rect, index);

            rect.y += 20f;
            EditorGUI.LabelField(rect, new GUIContent($"Event Count: {comms.Count}"));

            rect.y += 20f;
            GUIContent eventList = new GUIContent("Content", "Show the event list.");

            if (GUI.Button(rect, eventList))
            {
                EventActionListWindow.ShowWindow(LISAEditorUtility.GetArrayIndexFromPath(prop.propertyPath));
            }
            rect.y += 20f;
            EditorGUI.EndProperty();

            rect = EditorGUI.IndentedRect(rect);
            content.serializedObject.ApplyModifiedProperties();
        }
        public static float GetDrawPreviewHeight()
        {
            return 20f * 4;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20f + (3 * 18f);
        }
        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            label = EditorGUI.BeginProperty(rect, label, prop);
            Rect contentRect = EditorGUI.PrefixLabel(rect, label);
            
            rect.height = 18f;
            EditorGUI.indentLevel += 1;
            contentRect = EditorGUI.IndentedRect(rect);
            contentRect.y += 18f;
            
            EditorGUI.indentLevel = 0;
            SerializedProperty index = prop.FindPropertyRelative("index");
            SerializedProperty content = prop.FindPropertyRelative("content");
            var comms = LISAEditorUtility.GetTargetObjectOfProperty(content) as List<EventAction>;

            EditorGUI.PropertyField(contentRect, index);

            contentRect.y += 18f;
            EditorGUI.LabelField(contentRect, new GUIContent($"Event Count: {comms.Count}"));

            contentRect.y += 18f;
            GUIContent eventList = new GUIContent("Content", "Show the event list.");

            if (GUI.Button(contentRect, eventList))
            {
                EventActionListWindow.ShowWindow(LISAEditorUtility.GetArrayIndexFromPath(prop.propertyPath));
            }

            EditorGUI.EndProperty();
            content.serializedObject.ApplyModifiedProperties();
        }
    }
}