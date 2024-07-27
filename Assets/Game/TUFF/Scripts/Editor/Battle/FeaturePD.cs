using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(Feature))]
    public class FeaturePD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                (EditorGUIUtility.singleLineHeight * 0f) +
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var orgLabelWidth = EditorGUIUtility.labelWidth;
            float totalWidth = position.width;
            position.width = totalWidth * 0.3f;
            var featureType = property.FindPropertyRelative("featureType");

            EditorGUIUtility.labelWidth = 7f;
            EditorGUI.PropertyField(position, featureType, new GUIContent("?"));
            EditorGUIUtility.labelWidth = orgLabelWidth;

            if (featureType.enumValueIndex == (int)FeatureType.StatChange)
            {
                DoubleField(position, property, orgLabelWidth, totalWidth, "statChange", "statChangeValue", new GUIContent("?"), new GUIContent("x"));
            }
            else if (featureType.enumValueIndex == (int)FeatureType.ExtraRateChange)
            {
                DoubleField(position, property, orgLabelWidth, totalWidth, "extraRateChange", "extraRateChangeValue", new GUIContent("?"), new GUIContent("+"));
            }
            else if (featureType.enumValueIndex == (int)FeatureType.SpecialRateChange)
            {
                DoubleField(position, property, orgLabelWidth, totalWidth, "specialRateChange", "specialRateChangeValue", new GUIContent("?"), new GUIContent("x"));
            }
            else if (featureType.enumValueIndex == (int)FeatureType.ElementPotency)
            {
                ElementField(position, property, orgLabelWidth, totalWidth);
            }
            else if (featureType.enumValueIndex == (int)FeatureType.ElementVulnerability)
            {
                ElementField(position, property, orgLabelWidth, totalWidth);
            }
            else if (featureType.enumValueIndex == (int)FeatureType.StatePotency)
            {
                DoubleField(position, property, orgLabelWidth, totalWidth, "state", "stateValue", new GUIContent("?"), new GUIContent("x"));
            }
            else if (featureType.enumValueIndex == (int)FeatureType.StateVulnerability)
            {
                DoubleField(position, property, orgLabelWidth, totalWidth, "state", "stateValue", new GUIContent("?"), new GUIContent("x"));
            }
            else if (featureType.enumValueIndex == (int)FeatureType.StateImmunity)
            {
                SingleField(position, property, orgLabelWidth, totalWidth, "state", new GUIContent("?"));
            }
            else if (featureType.enumValueIndex == (int)FeatureType.AttackSpeed)
            {
                SingleField(position, property, orgLabelWidth, totalWidth, "attackSpeed", new GUIContent("?"));
            }
            else if (featureType.enumValueIndex == (int)FeatureType.AddCommand || featureType.enumValueIndex == (int)FeatureType.SealCommand)
            {
                SingleField(position, property, orgLabelWidth, totalWidth, "command", new GUIContent("?"));
            }
            else if (featureType.enumValueIndex == (int)FeatureType.AddSkill || featureType.enumValueIndex == (int)FeatureType.SealSkill)
            {
                SingleField(position, property, orgLabelWidth, totalWidth, "skill", new GUIContent("?"));
            }
            else if (featureType.enumValueIndex == (int)FeatureType.AddWeaponEquipType || featureType.enumValueIndex == (int)FeatureType.RemoveWeaponEquipType)
            {
                var types = TUFFSettings.weaponTypes;
                GUIContent[] options = new GUIContent[types.Count];
                int[] values = new int[types.Count];
                for (int i = 0; i < types.Count; i++)
                {
                    options[i] = new GUIContent(types[i].GetName());
                    values[i] = i;
                }
                IntPopupField(position, property, orgLabelWidth, totalWidth, "weaponType", new GUIContent("?"), options, values);
            }
            else if (featureType.enumValueIndex == (int)FeatureType.AddArmorEquipType || featureType.enumValueIndex == (int)FeatureType.RemoveArmorEquipType)
            {
                var types = TUFFSettings.armorTypes;
                GUIContent[] options = new GUIContent[types.Count];
                int[] values = new int[types.Count];
                for (int i = 0; i < types.Count; i++)
                {
                    options[i] = new GUIContent(types[i].GetName());
                    values[i] = i;
                }
                IntPopupField(position, property, orgLabelWidth, totalWidth, "armorType", new GUIContent("?"), options, values);
            }
            else if (featureType.enumValueIndex == (int)FeatureType.AutoState)
            {
                position.x += totalWidth * 0.31f;
                position.width = totalWidth * 0.68f;
                var autoState = property.FindPropertyRelative("autoState");
                EditorGUIUtility.labelWidth = 7f;
                EditorGUI.PropertyField(position, autoState, new GUIContent("?"));
                EditorGUIUtility.labelWidth = orgLabelWidth;
            }
            else if (featureType.enumValueIndex == (int)FeatureType.SpecialFeature)
            {
                position.x += totalWidth * 0.31f; //0.31
                position.width = totalWidth * 0.33f;
                var specialFeature = property.FindPropertyRelative("specialFeature");
                EditorGUIUtility.labelWidth = 7f;
                EditorGUI.PropertyField(position, specialFeature, new GUIContent("?"));
                EditorGUIUtility.labelWidth = orgLabelWidth;
            }
            property.serializedObject.ApplyModifiedProperties();
        }

        private static void ElementField(Rect position, SerializedProperty property, float orgLabelWidth, float totalWidth)
        {
            position.x += totalWidth * 0.31f; //0.31
            position.width = totalWidth * 0.33f;
            var element = property.FindPropertyRelative("element");
            EditorGUIUtility.labelWidth = 7f;
            var elements = TUFFSettings.elements;
            string[] options = new string[elements.Count];
            int[] values = new int[elements.Count];
            for (int i = 0; i < elements.Count; i++)
            {
                options[i] = elements[i].GetName();
                values[i] = i;
            }
            element.intValue = EditorGUI.IntPopup(position, "?", element.intValue, options, values);
            EditorGUIUtility.labelWidth = orgLabelWidth;

            position.x += totalWidth * 0.34f; //0.65
            position.width = totalWidth * 0.35f;
            var elementVulValue = property.FindPropertyRelative("elementValue");
            EditorGUIUtility.labelWidth = 15f;
            EditorGUI.PropertyField(position, elementVulValue, new GUIContent("x"));
            EditorGUIUtility.labelWidth = orgLabelWidth;
        }

        private static void SingleField(Rect position, SerializedProperty property, float orgLabelWidth, float totalWidth, string firstFieldName, GUIContent firstGUIContent)
        {
            position.x += totalWidth * 0.31f; //0.31
            position.width = totalWidth * 0.33f;
            var field = property.FindPropertyRelative(firstFieldName);

            EditorGUIUtility.labelWidth = 7f;
            EditorGUI.PropertyField(position, field, firstGUIContent);
            EditorGUIUtility.labelWidth = orgLabelWidth;
        }
        private static void IntPopupField(Rect position, SerializedProperty property, float orgLabelWidth, float totalWidth, string firstFieldName, GUIContent firstGUIContent, GUIContent[] options, int[] values)
        {
            position.x += totalWidth * 0.31f; //0.31
            position.width = totalWidth * 0.33f;
            var field = property.FindPropertyRelative(firstFieldName);

            EditorGUIUtility.labelWidth = 7f;
            //EditorGUI.PropertyField(position, field, firstGUIContent);
            field.intValue = EditorGUI.IntPopup(position, firstGUIContent, field.intValue, options, values);
            EditorGUIUtility.labelWidth = orgLabelWidth;
        }

        private static void DoubleField(Rect position, SerializedProperty property, float orgLabelWidth, float totalWidth, string firstFieldName, string secondFieldName, GUIContent firstGUIContent, GUIContent secondGUIContent)
        {
            position.x += totalWidth * 0.31f; //0.31
            position.width = totalWidth * 0.33f;
            var firstField = property.FindPropertyRelative(firstFieldName);

            EditorGUIUtility.labelWidth = 7f;
            EditorGUI.PropertyField(position, firstField, firstGUIContent);
            EditorGUIUtility.labelWidth = orgLabelWidth;

            position.x += totalWidth * 0.34f; //0.65
            position.width = totalWidth * 0.35f;
            var secondField = property.FindPropertyRelative(secondFieldName);
            EditorGUIUtility.labelWidth = 15f;
            EditorGUI.PropertyField(position, secondField, secondGUIContent);
            EditorGUIUtility.labelWidth = orgLabelWidth;
        }
    }
}

