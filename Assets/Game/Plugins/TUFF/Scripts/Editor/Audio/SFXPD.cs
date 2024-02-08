using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(SFX))]
    public class SFXPD : PropertyDrawer
    {
        private const float lineSkip = 20f;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
            }
            int lines = 3;
            float audioClipsHeight = 0f;
            var audioClipMode = property.FindPropertyRelative("audioClipMode");
            if (audioClipMode.intValue == 0) lines += 1;
            else audioClipsHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("randomClips"));
            return 20f + (20f * lines) + audioClipsHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var orgX = position.x;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                var audioClipMode = property.FindPropertyRelative("audioClipMode");
                var data = LISAEditorUtility.GetTargetObjectOfProperty(property) as SFX;
                GUIContent[] sfxOptions = new GUIContent[] { new("One Audio Clip"), new("Random Audio Clíp") };
                if (audioClipMode.intValue == 0)
                {
                    DrawField(position, property.FindPropertyRelative("audioClip"), sfxOptions, audioClipMode);
                    position.y += lineSkip;
                }
                else
                {
                    var randomClips = property.FindPropertyRelative("randomClips");
                    DrawField(position, randomClips, sfxOptions, audioClipMode);
                    position.y += EditorGUI.GetPropertyHeight(randomClips);
                }
                
                GUIContent[] intOptions = new GUIContent[] { new("Constant"), new("Random Between Two Constants") };
                var volumeMode = property.FindPropertyRelative("volumeMode");
                var volumeProp = property.FindPropertyRelative("volume");
                if (volumeMode.intValue == 0) 
                    DrawField(position, volumeProp, intOptions, volumeMode);
                else
                    DrawMinMaxField(position, property.FindPropertyRelative("minVolume"), volumeProp, intOptions, volumeMode);
                position.y += lineSkip;

                var pitchMode = property.FindPropertyRelative("pitchMode");
                var pitchProp = property.FindPropertyRelative("pitch");
                if (pitchMode.intValue == 0)
                    DrawField(position, pitchProp, intOptions, pitchMode);
                else
                    DrawMinMaxField(position, property.FindPropertyRelative("minPitch"), pitchProp, intOptions, pitchMode);
                position.y += lineSkip;

                if (GUI.Button(new Rect(position.x, position.y, position.width, 20f), "Play"))
                {
                    GameObject.FindWithTag("AudioManager")?.GetComponent<AudioManager>()?.PlaySFX(data);
                }
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                position.width += 15f;
            }
            position.x = orgX;

            property.serializedObject.ApplyModifiedProperties();
        }
        private void DrawField(Rect position, SerializedProperty prop, GUIContent[] options, SerializedProperty selectedIndex)
        {
            position.width -= 20f;
            EditorGUI.PropertyField(position, prop);
            
            position.x += position.width;
            position.width = 20f;
            selectedIndex.intValue = EditorGUI.Popup(position, null, selectedIndex.intValue, options);
        }
        private void DrawMinMaxField(Rect position, SerializedProperty minProp, SerializedProperty maxProp, GUIContent[] options, SerializedProperty selectedIndex)
        {
            position.width -= 20f;
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            EditorGUI.PrefixLabel(labelRect, new GUIContent(maxProp.displayName));

            position.x += EditorGUIUtility.labelWidth + 2;
            position.width -= EditorGUIUtility.labelWidth + 2;
            position.width *= 0.5f;
            EditorGUI.PropertyField(position, minProp, GUIContent.none);

            position.x += position.width;
            EditorGUI.PropertyField(position, maxProp, GUIContent.none);

            position.x += position.width;
            position.width = 20f;
            selectedIndex.intValue = EditorGUI.Popup(position, null, selectedIndex.intValue, options);
        }
    }
}

