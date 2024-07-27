using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(CharacterBio))]
    public class CharacterBioPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return 20f;
            return 20f + 20f * 5;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
            position.height = 20f;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                float oldLabel = EditorGUIUtility.labelWidth;
                var indentRect = EditorGUI.IndentedRect(position);
                indentRect.height = 20f;
                EditorGUIUtility.labelWidth -= 20f;
                indentRect.y += 20f;

                DrawBioContent(indentRect, property);

                EditorGUI.indentLevel--;
                EditorGUIUtility.labelWidth = oldLabel;
            }

            property.serializedObject.ApplyModifiedProperties();
        }
        private void DrawBioContent(Rect position, SerializedProperty property)
        {
            var obj = LISAEditorUtility.GetTargetObjectOfProperty(property) as CharacterBio;
            float orgX = position.x;
            float orgWidth = position.width;
            float orgLabelWidth = EditorGUIUtility.labelWidth;
            //EditorGUIUtility.labelWidth = 100f;
            
            float firstWidth = orgWidth * 0.65f;
            float secondWidth = orgWidth * 0.45f;

            position.width = firstWidth;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("fightingArtKey"));
            position.x += firstWidth;
            position.width = secondWidth;
            EditorGUI.LabelField(position, obj.GetFightingArt());
            position.x = orgX;
            position.y += 20f;

            position.width = firstWidth;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("pastOccupationKey"));
            position.x += firstWidth;
            position.width = secondWidth;
            EditorGUI.LabelField(position, obj.GetPastOccupation());
            position.x = orgX;
            position.y += 20f;

            position.width = firstWidth;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("likesKey"));
            position.x += firstWidth;
            position.width = secondWidth;
            EditorGUI.LabelField(position, obj.GetLikes());
            position.x = orgX;
            position.y += 20f;

            position.width = firstWidth;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("favoriteFoodKey"));
            position.x += firstWidth;
            position.width = secondWidth;
            EditorGUI.LabelField(position, obj.GetFavoriteFood());
            position.x = orgX;
            position.y += 20f;

            position.width = firstWidth;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("mostHatedThingKey"));
            position.x += firstWidth;
            position.width = secondWidth;
            EditorGUI.LabelField(position, obj.GetMostHatedThing());
            EditorGUIUtility.labelWidth = orgLabelWidth;
        }
    }

}
