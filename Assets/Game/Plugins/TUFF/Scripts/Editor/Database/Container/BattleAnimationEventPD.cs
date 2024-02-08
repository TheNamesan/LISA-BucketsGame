using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(BattleAnimationEvent))]
    public class BattleAnimationEventPD : PropertyDrawer
    {
        public const float separation = 6f;
        protected static float lineSkip = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) + (EditorGUIUtility.standardVerticalSpacing) + 2f;
            }
            float lines = 12f;
            float scopeHeight = 0f;
            float sfxLines = 0f;
            float hitLines = 0f;
            float effectLines = 0f;
            float motionLines = 0f;
            float flashTargetLines = 0f;
            float sfxListHeight = 0f;
            float eventHeight = 0f;
            float formulaHeight = 0f;
            float effectsHeight = 0f;
            float flashTargetHeight = 0f;
            float flashScreenHeight = 0f;
            var ovrScope = property.FindPropertyRelative("scopeDataOverride").isExpanded;
            var hit = property.FindPropertyRelative("hit").boolValue;
            var userMotion = property.FindPropertyRelative("userMotion").isExpanded;
            var flashTarget = property.FindPropertyRelative("flashTargetData").isExpanded;
            var flashScreen = property.FindPropertyRelative("flashScreenData").isExpanded;
            var onEventRun = property.FindPropertyRelative("onEventRun");

            if (property.FindPropertyRelative("playSFX").boolValue)
            {
                sfxLines += 1;
                sfxListHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("SFXList"));
            }

            scopeHeight = 20f + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("scopeDataOverride"));

            if (hit)
            {
                hitLines += 7f;
                if (property.FindPropertyRelative("damageType").enumValueIndex != 0)
                {
                    hitLines += 6f;
                    formulaHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("formula"));
                }
                effectLines = 2f;
                effectsHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("effects"));
            }
            if (userMotion)
            {
                motionLines = 1f;
            }
            if (hit) flashTargetLines = 1f;
            flashTargetHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("flashTargetData"));
            if (flashTarget)
            {
                
            }
            flashScreenHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("flashScreenData"));
            if (flashScreen)
            {
                
            }
            if (onEventRun.isExpanded)
            {
                eventHeight += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("onEventRun"));
            }

            float totalLines = (lines + sfxLines + hitLines + effectLines + motionLines + flashTargetLines);
            float totalHeight = (sfxListHeight + scopeHeight + eventHeight + formulaHeight + effectsHeight + flashTargetHeight + flashScreenHeight);
            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
            (20f * totalLines) + totalHeight
            + separation * 2 + (EditorGUIUtility.standardVerticalSpacing) + 2f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label/*$"Animation Event {LISAEditorUtility.GetArrayIndexFromPath(property.propertyPath)}"*/, true);
            if(property.isExpanded)
            {
                //EditorGUI.LabelField(position, $"Animation Event {LISAEditorUtility.GetArrayIndexFromPath(property.propertyPath)}", EditorStyles.boldLabel);
                position.y += lineSkip;

                var SFXList = property.FindPropertyRelative("SFXList");
                float unextendedHeight = lineSkip * 2;
                float sfxHeight = lineSkip * 1;
                float extendedHeight = unextendedHeight + sfxHeight + EditorGUI.GetPropertyHeight(SFXList);
                var playSFX = property.FindPropertyRelative("playSFX");
                var playSFXOnHit = property.FindPropertyRelative("playSFXOnHit");

                var orgColor = GUI.color;
                GUI.color = Color.grey;
                GUI.Box(new Rect(position.x - 12, position.y, position.width + 12, playSFX.boolValue ? extendedHeight + 4 : unextendedHeight + 4), "");
                GUI.color = orgColor;

                EditorGUI.LabelField(position, "SFX", EditorStyles.boldLabel);
                position.y += lineSkip;
                EditorGUI.PropertyField(position, playSFX);
                position.y += lineSkip;
                if (playSFX.boolValue)
                {
                    EditorGUI.PropertyField(position, playSFXOnHit);
                    position.y += lineSkip;
                    EditorGUI.PropertyField(position, SFXList);
                    position.y += EditorGUI.GetPropertyHeight(SFXList);
                }
                position.y += separation;

                // Override Scope
                EditorGUI.LabelField(position, "Override Scope Data", EditorStyles.boldLabel);
                position.y += lineSkip;
                var ovrScope = property.FindPropertyRelative("overrideScopeData");
                var scopeData = property.FindPropertyRelative("scopeDataOverride");
                DrawBoolFoldoutOnClassProperty(position, scopeData, ovrScope);
                //EditorGUI.PropertyField(position, scopeData);
                position.y += EditorGUI.GetPropertyHeight(scopeData);
                position.y += separation;

                // Hit
                var hit = property.FindPropertyRelative("hit");
                var damageType = property.FindPropertyRelative("damageType");
                var formula = property.FindPropertyRelative("formula");
                var effects = property.FindPropertyRelative("effects");
                extendedHeight = (lineSkip * 9) + (EditorGUI.GetPropertyHeight(effects) + 46f)
                    + (damageType.enumValueIndex != 0 ? (lineSkip * 4) + EditorGUI.GetPropertyHeight(formula)  : 0);

                orgColor = GUI.color;
                GUI.color = Color.grey;
                GUI.Box(new Rect(position.x - 12, position.y, position.width + 12, hit.boolValue ? extendedHeight + 4 : unextendedHeight + 4), "");
                GUI.color = orgColor;

                EditorGUI.LabelField(position, "Hit", EditorStyles.boldLabel);
                position.y += lineSkip;

                EditorGUI.PropertyField(position, hit);
                position.y += lineSkip;
                if (hit.boolValue)
                {
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("successPercent"));
                    position.y += lineSkip;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("certainHit"));
                    position.y += lineSkip;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("TPGain"));
                    position.y += lineSkip;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("UPGain"));
                    position.y += lineSkip;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("hitType"));
                    position.y += lineSkip;

                    
                    var multihitTiming = property.FindPropertyRelative("multihitTiming");
                    float orgLabelWidth = EditorGUIUtility.labelWidth;
                    var oldWidth = position.width;
                    var oldPos = position.x;
                    //
                    position.width = oldWidth * 0.75f;
                    EditorGUI.PropertyField(position, multihitTiming);
                    position.x += position.width;
                    position.width = oldWidth * 0.25f;
                    EditorGUIUtility.labelWidth = 35f;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("multihitTimingDelay"), new GUIContent("Delay"));
                    position.width = oldWidth;
                    //
                    EditorGUIUtility.labelWidth = orgLabelWidth;
                    position.x = oldPos;
                    position.y += lineSkip;

                    EditorGUI.PropertyField(position, damageType);
                    position.y += lineSkip;
                    if (damageType.enumValueIndex != 0)
                    {
                        var element = property.FindPropertyRelative("element");
                        var elements = TUFFSettings.elements;
                        string[] options = new string[elements.Count + 2];
                        int[] values = new int[elements.Count + 2];
                        options[0] = "None"; values[0] = 0;
                        options[1] = "Use trait element"; values[1] = 1;
                        for (int i = 0; i < elements.Count; i++)
                        {
                            int index = i + 2;
                            options[index] = elements[i].GetName();
                            values[index] = index;
                        }
                        element.intValue = EditorGUI.IntPopup(position, "Element", element.intValue, options, values);
                        position.y += lineSkip;
                        EditorGUI.PropertyField(position, formula);
                        position.y += EditorGUI.GetPropertyHeight(formula);
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("variance"));
                        position.y += lineSkip;
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("canCrit"));
                        position.y += lineSkip;

                        var totalWidth = position.width;
                        
                        position.width = totalWidth * 0.5f;
                        var overrideTime = property.FindPropertyRelative("overrideCritPauseTimer");
                        EditorGUI.PropertyField(position, overrideTime);
                        position.x += totalWidth * 0.45f;
                        position.width = totalWidth * 0.55f;
                        EditorGUIUtility.labelWidth = position.width * 0.33f;
                        if (overrideTime.boolValue) EditorGUI.PropertyField(position, property.FindPropertyRelative("critPauseTimer"));
                        position.x -= totalWidth * 0.45f;
                        position.width = totalWidth;
                        EditorGUIUtility.labelWidth = orgLabelWidth;
                        position.y += lineSkip;
                    }
                    position.y += separation;

                    
                    //unextendedHeight = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * ((damageType.enumValueIndex != 0 && hit.boolValue) ? 2f : 1f);

                    //orgColor = GUI.color;
                    //GUI.color = Color.grey;
                    //GUI.Box(new Rect(position.x - 12, position.y, position.width + 12, unextendedHeight + EditorGUI.GetPropertyHeight(effects) + 4), "");
                    //GUI.color = orgColor;

                    EditorGUI.LabelField(position, "Effects", EditorStyles.boldLabel);
                    position.y += lineSkip;

                    EditorGUI.PropertyField(position, property.FindPropertyRelative("ignoreHitEffects"), new GUIContent("Ignore Hit"));
                    position.y += lineSkip;
                    
                    EditorGUI.PropertyField(position, effects);
                    position.y += EditorGUI.GetPropertyHeight(effects);
                }
                position.y += separation;

                // Motion
                EditorGUI.LabelField(position, "Motion", EditorStyles.boldLabel);
                position.y += lineSkip;
                var userMotion = property.FindPropertyRelative("userMotion");
                DrawBoolFoldout(position, userMotion);
                position.y += lineSkip;
                if (userMotion.isExpanded)
                {
                    bool guiEnabled = GUI.enabled;
                    if (!userMotion.boolValue) GUI.enabled = false;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("motionType"));
                    position.y += lineSkip;
                    if (!userMotion.boolValue) GUI.enabled = guiEnabled;
                }
                position.y += separation;

                // Flash Target
                EditorGUI.LabelField(position, "Flash Target", EditorStyles.boldLabel);
                position.y += lineSkip;
                if (hit.boolValue)
                {
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("ignoreHitFlashTarget"));
                    position.y += lineSkip;
                }
                var flashTarget = property.FindPropertyRelative("flashTarget");
                var flashTargetData = property.FindPropertyRelative("flashTargetData");
                DrawBoolFoldoutOnClassProperty(position, flashTargetData, flashTarget);
                position.y += EditorGUI.GetPropertyHeight(flashTargetData);
                position.y += separation;

                // Flash Screen
                EditorGUI.LabelField(position, "Flash Screen", EditorStyles.boldLabel);
                position.y += lineSkip;
                var flashScreen = property.FindPropertyRelative("flashScreen");
                var flashScreenData = property.FindPropertyRelative("flashScreenData");
                DrawBoolFoldoutOnClassProperty(position, flashScreenData, flashScreen);
                position.y += EditorGUI.GetPropertyHeight(flashScreenData);
                position.y += separation;


                EditorGUI.LabelField(position, "Unity Events", EditorStyles.boldLabel);
                position.y += lineSkip;
                var onEventRun = property.FindPropertyRelative("onEventRun");
                onEventRun.isExpanded = EditorGUI.Foldout(FoldoutPosition(position), onEventRun.isExpanded, new GUIContent("Unity Events"), true);
                position.y += lineSkip;
                if (onEventRun.isExpanded)
                {
                    EditorGUI.PropertyField(position, onEventRun);
                    position.y += EditorGUI.GetPropertyHeight(onEventRun);
                }
                position.y += separation;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
        public void DrawBoolFoldout(Rect position, SerializedProperty targetProperty)
        {
            targetProperty.isExpanded = EditorGUI.Foldout(FoldoutPosition(position), targetProperty.isExpanded, new GUIContent(), true);
            EditorGUI.PropertyField(position, targetProperty);
        }
        public void DrawBoolFoldoutOnClassProperty(Rect position, SerializedProperty targetProperty, SerializedProperty boolProperty)
        {
            EditorGUI.PropertyField(position, boolProperty, new GUIContent(" "));
            bool guiEnabled = GUI.enabled;
            if (!boolProperty.boolValue) GUI.enabled = false;
            EditorGUI.PropertyField(position, targetProperty, new GUIContent(boolProperty.displayName));
            if (!boolProperty.boolValue) GUI.enabled = guiEnabled;
        }
        public Rect FoldoutPosition(Rect position)
        {
            return new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
        }
    }
}

