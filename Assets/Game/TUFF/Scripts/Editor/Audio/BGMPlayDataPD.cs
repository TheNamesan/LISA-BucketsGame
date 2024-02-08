using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(BGMPlayData))]
    public class BGMPlayDataPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
            }
            int lines = 6;
            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                ((EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * lines) +
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var orgX = position.x;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            if(property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                var data = LISAEditorUtility.GetTargetObjectOfProperty(property) as BGMPlayData;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("bgm"), new GUIContent("BGM"));
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                EditorGUI.PropertyField(position, property.FindPropertyRelative("volume"));
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                EditorGUI.PropertyField(position, property.FindPropertyRelative("pitch"));
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                EditorGUI.PropertyField(position, property.FindPropertyRelative("loop"));
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                if(GUI.Button(new Rect(position.x, position.y, position.width, 20f), "Play"))
                {
                    GameObject.FindWithTag("AudioManager")?.GetComponent<AudioManager>()?.PlayBGMAsPreview(data);
                }
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                if (GUI.Button(new Rect(position.x, position.y, position.width, 20f), "Stop"))
                {
                    TUFFWizard.StopPreviewBGM();
                }
                position.width += 15f;
            }
            position.x = orgX;

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}

