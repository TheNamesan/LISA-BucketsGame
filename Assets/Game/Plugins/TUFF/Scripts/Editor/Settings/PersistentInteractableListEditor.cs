using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(PersistentInteractableList)), CanEditMultipleObjects]
    public class PersistentInteractableListEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("persistentIDs"), new GUIContent("Persistent IDs"));
            GUI.enabled = true;
            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPropertyDrawer(typeof(PersistentInteractableData))]
    public class PersistentInteractableDataPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property == null) return 0f;
            var data = LISAEditorUtility.GetTargetObjectOfProperty(property) as PersistentInteractableData;
            if (!property.isExpanded || data == null)
            {
                return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
            }
            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                ((EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3f) +
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property == null) return;
            var data = LISAEditorUtility.GetTargetObjectOfProperty(property) as PersistentInteractableData;
            if (data == null) { EditorGUI.LabelField(position, label + ": Null"); return; };
            position.height = 20f;
            var orgX = position.x;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                EditorGUI.PropertyField(position, property.FindPropertyRelative("scene"));
                //EditorGUI.LabelField(position, "Scene: " + data.scene.namepat);
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                EditorGUI.PropertyField(position, property.FindPropertyRelative("localID"), new GUIContent("Local Identifier In File"));
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                EditorGUI.PropertyField(position, property.FindPropertyRelative("name"));
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

                position.width += 15f;
            }
            position.x = orgX;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
