using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(InteractableEvent))]
    public class InteractableEventPD : PropertyDrawer
    {
        float skip = 20f;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
            }
            int lines = 3;
            int spriteLines = 1;
            int colliderLines = 1;
            float switchDataHeight = 20f;
            var conditions = property.FindPropertyRelative("conditions");
            var spriteRef = property.FindPropertyRelative("spriteRef");
            float conditionsHeight = EditorGUI.GetPropertyHeight(conditions);
            if (spriteRef.isExpanded)
            {
                if (spriteRef.objectReferenceValue != null) spriteLines = 5;
                var overrideSpriteTransform = property.FindPropertyRelative("overrideSpriteTransform");
                if (overrideSpriteTransform.boolValue) spriteLines += 2;
                lines += spriteLines;
            }
            var colliderRef = property.FindPropertyRelative("colliderRef");
            if (colliderRef.isExpanded)
            {
                if (colliderRef.objectReferenceValue != null) colliderLines = 3;
                lines += colliderLines;
            }
            var onSwitchDataLoad = property.FindPropertyRelative("onSwitchDataLoad");
            if (onSwitchDataLoad.isExpanded)
            {
                switchDataHeight += EditorGUI.GetPropertyHeight(onSwitchDataLoad);
            }
            float actionListHeight = ActionListPD.GetDrawPreviewHeight();//EditorGUI.GetPropertyHeight(property.FindPropertyRelative("actionList"));

            return (EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing) +
                ((EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * lines) + actionListHeight +
                switchDataHeight + conditionsHeight +
                (EditorGUIUtility.standardVerticalSpacing) + 2f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var orgX = position.x;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            position.y += skip;
            if (property.isExpanded)
            {
                var conditions = property.FindPropertyRelative("conditions");
                string conditionsLabel = $"{conditions.displayName} ({(conditions.FindPropertyRelative("elements").arraySize)})";
                EditorGUI.PropertyField(position, conditions, new GUIContent(conditionsLabel, conditions.tooltip));
                position.y += EditorGUI.GetPropertyHeight(conditions);
                EditorGUI.PropertyField(position, property.FindPropertyRelative("triggerType"));
                position.y += skip;

                var spriteRef = property.FindPropertyRelative("spriteRef");
                spriteRef.isExpanded = EditorGUI.Foldout(position, spriteRef.isExpanded, "Sprite");
                position.y += skip;
                if (spriteRef.isExpanded)
                {
                    position.x += 15f;
                    position.width -= 15f;
                    EditorGUI.PropertyField(position, spriteRef);
                    position.y += skip;
                    if (spriteRef.objectReferenceValue != null)
                    {
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("graphic"));
                        position.y += skip;
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("orderInLayer"));
                        position.y += skip;
                        var overrideSpriteTransform = property.FindPropertyRelative("overrideSpriteTransform");
                        EditorGUI.PropertyField(position, overrideSpriteTransform);
                        position.y += skip;
                        if (overrideSpriteTransform.boolValue)
                        {
                            EditorGUI.PropertyField(position, property.FindPropertyRelative("spriteLocalPosition"));
                            position.y += skip;
                            EditorGUI.PropertyField(position, property.FindPropertyRelative("spriteScale"));
                            position.y += skip;
                        }
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("disableSprite"));
                        position.y += skip;
                    }
                    position.width += 15f;
                }
                position.x = orgX;

                var colliderRef = property.FindPropertyRelative("colliderRef");
                colliderRef.isExpanded = EditorGUI.Foldout(position, colliderRef.isExpanded, "Collider");
                position.y += skip;
                if (colliderRef.isExpanded)
                {
                    position.x += 15f;
                    position.width -= 15f;
                    EditorGUI.PropertyField(position, colliderRef);
                    position.y += skip;
                    if (colliderRef.objectReferenceValue != null)
                    {
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("setIsTrigger"));
                        position.y += skip;
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("disableCollider"));
                        position.y += skip;
                    }
                    position.width += 15f;
                }
                position.x = orgX;
                var onSwitchDataLoad = property.FindPropertyRelative("onSwitchDataLoad");
                onSwitchDataLoad.isExpanded = EditorGUI.Foldout(position, onSwitchDataLoad.isExpanded, "Events");
                position.y += skip;
                if (onSwitchDataLoad.isExpanded)
                {
                    position.x += 15f;
                    position.width -= 15f;
                    EditorGUI.PropertyField(position, onSwitchDataLoad);
                    position.y += EditorGUI.GetPropertyHeight(onSwitchDataLoad);
                    position.width += 15f;
                }

                position.x = orgX;

                //var eventList = property.FindPropertyRelative("actionList");
                //EditorGUI.PropertyField(position, eventList);
                //position.y += EditorGUI.GetPropertyHeight(eventList);
                var actionList = property.FindPropertyRelative("actionList");
                ActionListPD.DrawPreview(ref position, actionList, new GUIContent(actionList.displayName, actionList.tooltip));
                position.y += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPropertyDrawer(typeof(InteractableEventConditions))]
    public class InteractableEventConditionsPD : PropertyDrawer
    {
        float skip = 20f;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return 20f;
            var elements = property.FindPropertyRelative("elements");
            return 20f + EditorGUI.GetPropertyHeight(elements);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var orgX = position.x;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            position.y += skip;
            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                var elements = property.FindPropertyRelative("elements");
                EditorGUI.PropertyField(position, elements);
                position.y += EditorGUI.GetPropertyHeight(elements);

                position.width += 15f;
                position.x = orgX;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPropertyDrawer(typeof( InteractableEventConditionElement))]
    public class InteractableEventConditionElementPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var orgX = position.x;
            var orgWidth = position.width;
            var orgLabel = EditorGUIUtility.labelWidth;

            position.width = orgWidth * 0.05F;
            EditorGUIUtility.labelWidth = 22;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("not"));
            position.x += position.width;

            position.width = orgWidth * 0.2F;
            EditorGUIUtility.labelWidth = orgLabel;
            var conditionType = property.FindPropertyRelative("conditionType");
            EditorGUI.PropertyField(position, conditionType, new GUIContent(""));
            
            var type = (InteractableEventConditionType)conditionType.enumValueIndex;
            if (type == InteractableEventConditionType.SelfSwitch)
                DrawSwitch(position, orgWidth, property);
            else if (type == InteractableEventConditionType.GameVariable)
                DrawGameVariable(position, orgWidth, property);
            else if (type == InteractableEventConditionType.ItemInInventory)
                DrawItem(position, orgWidth, property);
            else if (type == InteractableEventConditionType.UnitInParty)
                DrawUnit(position, orgWidth, property);

            EditorGUIUtility.labelWidth = orgLabel;
            position.x = orgX;
            property.serializedObject.ApplyModifiedProperties();
        }
        private void DrawSwitch(Rect position, float orgWidth, SerializedProperty property)
        {
            position.x += position.width + 2;
            position.width = orgWidth * 0.75f - 2;
            EditorGUIUtility.labelWidth = 16;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("targetSwitch"), new GUIContent("is"));
        }
        private void DrawGameVariable(Rect position, float orgWidth, SerializedProperty property)
        {
            position.x += position.width + 2;
            position.width = orgWidth * 0.75f - 2;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("variableComparator"), new GUIContent(""));
            //position.x += position.width + 2;
            //float w = orgWidth * 0.25f - 2;
            //position.width = w;

            //var variableIndex = property.FindPropertyRelative("targetVariableIndex");
            //var variablesData = GameVariableList.GetList();
            //string[] options = new string[variablesData.Length];
            //int[] values = new int[variablesData.Length];
            //for (int i = 0; i < variablesData.Length; i++)
            //{
            //    options[i] = variablesData[i].name;
            //    values[i] = i;
            //}
            //variableIndex.intValue = EditorGUI.IntPopup(position, "", variableIndex.intValue, options, values);

            //var valueType = property.FindPropertyRelative("targetVariableValueType");
            //position.x += position.width + 2;

            //EditorGUI.PropertyField(position, valueType, new GUIContent(""));

            //position.x += position.width + 2;
            //EditorGUIUtility.labelWidth = 16;
            //DrawOnType(position, property, new GUIContent("is"));
        }
        //private void DrawOnType(Rect position, SerializedProperty property, GUIContent label)
        //{
        //    var valueEnum = (GameVariableValueType)property.FindPropertyRelative("targetVariableValueType").enumValueIndex;
        //    switch (valueEnum)
        //    {
        //        case GameVariableValueType.BoolValue:
        //            EditorGUI.PropertyField(position, property.FindPropertyRelative("variableBool"), label); break;
        //        case GameVariableValueType.NumberValue:
        //            EditorGUI.PropertyField(position, property.FindPropertyRelative("variableNumber"), label); break;
        //        case GameVariableValueType.StringValue:
        //            EditorGUI.PropertyField(position, property.FindPropertyRelative("variableString"), label); break;
        //        case GameVariableValueType.VectorValue:
        //            EditorGUI.PropertyField(position, property.FindPropertyRelative("variableVector"), label); break;
        //    }
        //}
        private void DrawItem(Rect position, float orgWidth, SerializedProperty property)
        {
            position.x += position.width + 2;
            position.width = orgWidth * 0.75f - 2;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("targetItem"), new GUIContent(""));
        }
        private void DrawUnit(Rect position, float orgWidth, SerializedProperty property)
        {
            position.x += position.width + 2;
            position.width = orgWidth * 0.75f - 2;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("targetUnit"), new GUIContent(""));
        }
    }
}

