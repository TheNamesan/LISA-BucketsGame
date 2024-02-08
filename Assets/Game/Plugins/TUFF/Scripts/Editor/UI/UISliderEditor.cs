using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(UISlider))]
    [CanEditMultipleObjects]
    public class UISliderEditor : Editor
    {
        bool showActions = true;
        bool showOtherActions = false;
        override public void OnInspectorGUI()
        {
            var element = target as UISlider;

            GUI.enabled = false;
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("highlighted"));
            SerializedProperty disabled = serializedObject.FindProperty("m_disabled");
            EditorGUILayout.PropertyField(disabled);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("text"));

            SerializedProperty min = serializedObject.FindProperty("minValue");
            EditorGUILayout.PropertyField(min);
            SerializedProperty max = serializedObject.FindProperty("maxValue");
            EditorGUILayout.PropertyField(max);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fillIntervals"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("skipHoldFillIntervals"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("valueDecimals"));
            SerializedProperty fill = serializedObject.FindProperty("m_fillAmount");
            fill.floatValue = EditorGUILayout.Slider("Fill Amount", fill.floatValue, min.floatValue, max.floatValue);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onValueChanged"));

            showActions = EditorGUILayout.BeginFoldoutHeaderGroup(showActions, "Action Events");
            if (showActions)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onHighlight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onUnhighlight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onSkip"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onHorizontalInput"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onVerticalInput"));
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            showOtherActions = EditorGUILayout.BeginFoldoutHeaderGroup(showOtherActions, "Other Action Events");
            if (showOtherActions)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onSelect"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onSelectCanceled"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onCancel"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onCancelCanceled"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onSkipCanceled"));
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("onEnabled"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onDisabled"));
            if (GUILayout.Button("Debug Invoke Disabled Events"))
            {
                element.disabled = element.disabled;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("highlightDisplayText"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("useCustomSelectSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("customSelectSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useCustomDisabledSFX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("customDisabledSFX"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
