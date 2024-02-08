using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(EventAction), false)]
    public class EventActionPD : PropertyDrawer
    {
        //public EventAction target { get { return (EventAction)fieldInfo.GetValue(typeof(EventAction)); } }
        public SerializedProperty targetProperty;
        public EventAction targetObject { get { return LISAEditorUtility.GetTargetObjectOfProperty(targetProperty) as EventAction; } }
        public void PanelGUI()
        {
            PanelGUI(targetProperty);
        }
        public void PanelGUI(SerializedProperty property)
        {
            property.serializedObject.Update();
            EditorGUILayout.BeginVertical("box");
            //EditorGUILayout.LabelField(serializedObject.FindProperty("parent").type);
            //EditorGUILayout.PropertyField(property);
            var eventName = property.FindPropertyRelative("eventName");
            EditorGUILayout.PropertyField(eventName);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("eventColor"));
            InspectorGUIContent();
            EditorGUILayout.EndVertical();
            property.serializedObject.ApplyModifiedProperties();
        }
        public virtual float GetSummaryHeight()
        {
            return 20f;
        }

        public virtual void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, "No Summary");
        }

        public virtual void InspectorGUIContent() { }
        public virtual void OnEditorInstantiate() { }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 40f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var obj = targetObject;
            position.height = 20f;
            if (property != null && obj != null)
            {
                
                EditorGUI.PropertyField(position, property?.FindPropertyRelative("eventName"));
                position.y += 20f;
                EditorGUI.PropertyField(position, property?.FindPropertyRelative("eventColor"));
                position.y += 20f;
                FixedGUIContent(ref position, property, label);
                property.serializedObject?.ApplyModifiedProperties();
            }
        }
        public virtual void FixedGUIContent(ref Rect position, SerializedProperty property, GUIContent label)
        {

        }
        public string GetEventName()
        {
            var action = targetObject as EventAction;
            if (action == null) return "";
            return action.eventName;
        }
    }
}

