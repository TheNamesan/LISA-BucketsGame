using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(GreatTest))]
    public class GreatTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var greatTest = target as GreatTest;
            serializedObject.Update();
            SerializedProperty prop = serializedObject.FindProperty("op");
            //EditorGUILayout.PropertyField(prop.GetArrayElementAtIndex(1));
            LISAEditorUtility.DrawThing(prop.GetArrayElementAtIndex(1));
            //EditorGUILayout.PropertyField();

            serializedObject.ApplyModifiedProperties();
        }

    }
}

