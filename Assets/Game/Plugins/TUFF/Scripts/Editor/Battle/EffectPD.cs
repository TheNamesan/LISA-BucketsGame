using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(Effect))]
    public class EffectPD : PropertyDrawer
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
            var effectType = property.FindPropertyRelative("effectType");
            var value = effectType.enumValueIndex;

            EditorGUIUtility.labelWidth = 7f;
            EditorGUI.PropertyField(position, effectType, new GUIContent("?"));
            EditorGUIUtility.labelWidth = orgLabelWidth;
            if (value == (int)EffectType.RecoverHP ||
                value == (int)EffectType.RecoverSP ||
                value == (int)EffectType.RecoverTP)
            {
                var recoverPercent = property.FindPropertyRelative("recoverPercent");
                string percentLabel = "";
                if (value == (int)EffectType.RecoverHP) percentLabel = "MaxHP%";
                if (value == (int)EffectType.RecoverSP) percentLabel = "MaxSP%";
                if (value == (int)EffectType.RecoverTP) percentLabel = "MaxTP%";

                position.x += totalWidth * 0.31f; //0.31
                position.width = totalWidth * 0.33f;
                EditorGUIUtility.labelWidth = 65f;
                EditorGUI.PropertyField(position, recoverPercent, new GUIContent(percentLabel));
                EditorGUIUtility.labelWidth = orgLabelWidth;

                position.x += totalWidth * 0.34f; //0.65
                position.width = totalWidth * 0.35f;
                var recoverFlat = property.FindPropertyRelative("recoverFlat");
                EditorGUIUtility.labelWidth = 15f;
                EditorGUI.PropertyField(position, recoverFlat, new GUIContent("+"));
                EditorGUIUtility.labelWidth = orgLabelWidth;
            }
            else if (value == (int)EffectType.AddState || value == (int)EffectType.RemoveState)
            {
                position.x += totalWidth * 0.31f; //0.31
                position.width = totalWidth * 0.33f;
                var state = property.FindPropertyRelative("state");
                EditorGUIUtility.labelWidth = 7f;
                EditorGUI.PropertyField(position, state, new GUIContent("?"));
                EditorGUIUtility.labelWidth = orgLabelWidth;

                position.x += totalWidth * 0.34f; //0.65
                position.width = totalWidth * 0.35f;
                var triggerChance = property.FindPropertyRelative("stateTriggerChance");
                EditorGUIUtility.labelWidth = 65f;
                EditorGUI.PropertyField(position, triggerChance, new GUIContent("Trigger%"));
                EditorGUIUtility.labelWidth = orgLabelWidth;
            }
            else if (value == (int)EffectType.SpecialEffect)
            {
                SingleField(position, property, orgLabelWidth, totalWidth, "specialEffect", new GUIContent("?"));
                //position.x += totalWidth * 0.31f; //0.31
                //position.width = totalWidth * 0.33f;
                //var specialEffect = property.FindPropertyRelative("specialEffect");
                //EditorGUIUtility.labelWidth = 7f;
                //EditorGUI.PropertyField(position, specialEffect, new GUIContent("?"));
                //EditorGUIUtility.labelWidth = orgLabelWidth;
            }
            else if (value == (int)EffectType.LearnSkill || value == (int)EffectType.ForgetSkill)
            {
                SingleField(position, property, orgLabelWidth, totalWidth, "skill", new GUIContent("?"));
                //position.x += totalWidth * 0.31f; //0.31
                //position.width = totalWidth * 0.33f;
                //var state = property.FindPropertyRelative("skill");
                //EditorGUIUtility.labelWidth = 7f;
                //EditorGUI.PropertyField(position, state, new GUIContent("?"));
                //EditorGUIUtility.labelWidth = orgLabelWidth;
            }
            else if (effectType.enumValueIndex == (int)EffectType.CommonEvent)
            {
                SingleField(position, property, orgLabelWidth, totalWidth, "commonEvent", new GUIContent("?"));
                //position.x += totalWidth * 0.31f; //0.31
                //position.width = totalWidth * 0.33f;
                //var state = property.FindPropertyRelative("commonEvent");
                //EditorGUIUtility.labelWidth = 7f;
                //EditorGUI.PropertyField(position, state, new GUIContent("?"));
                //EditorGUIUtility.labelWidth = orgLabelWidth;
            }
            else if (effectType.enumValueIndex == (int)EffectType.QueueSkill)
            {
                SingleField(position, property, orgLabelWidth, totalWidth, "skillToQueue", new GUIContent("?"));
                //position.x += totalWidth * 0.31f; //0.31
                //position.width = totalWidth * 0.33f;
                //var state = property.FindPropertyRelative("skillToQueue");
                //EditorGUIUtility.labelWidth = 7f;
                //EditorGUI.PropertyField(position, state, new GUIContent("?"));
                //EditorGUIUtility.labelWidth = orgLabelWidth;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
        private void SingleField(Rect position, SerializedProperty property, float orgLabelWidth, float totalWidth, string firstFieldName, GUIContent firstGUIContent)
        {
            position.x += totalWidth * 0.31f; //0.31
            position.width = totalWidth * 0.33f;
            var field = property.FindPropertyRelative(firstFieldName);
            EditorGUIUtility.labelWidth = 7f;
            EditorGUI.PropertyField(position, field, new GUIContent("?"));
            EditorGUIUtility.labelWidth = orgLabelWidth;
        }
    }
}

