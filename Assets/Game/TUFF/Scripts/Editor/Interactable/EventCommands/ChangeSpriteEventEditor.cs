using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(ChangeSpriteEvent)), CanEditMultipleObjects]
    public class ChangeSpriteEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            var originType = serializedObject.FindProperty("originType");
            var persistentOrigin = serializedObject.FindProperty("persistentOrigin");
            var renderer = serializedObject.FindProperty("spriteRenderer");

            EditorGUILayout.PropertyField(originType);
            if ((FieldOriginType)originType.enumValueIndex == FieldOriginType.FromScene)
            {
                EditorGUILayout.PropertyField(renderer);
            }
            else if ((FieldOriginType)originType.enumValueIndex == FieldOriginType.FromPersistentInstance)
            {
                EditorGUILayout.PropertyField(persistentOrigin);
            }
            
            if (renderer.objectReferenceValue != null || 
                ((FieldOriginType)originType.enumValueIndex == FieldOriginType.FromPersistentInstance && (PersistentType)persistentOrigin.enumValueIndex != PersistentType.None))
            {
                var keepEnabled = serializedObject.FindProperty("keepEnabled");
                EditorGUILayout.PropertyField(keepEnabled);
                if (!keepEnabled.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("enabled"));

                var keepSprite = serializedObject.FindProperty("keepSprite");
                EditorGUILayout.PropertyField(keepSprite);
                if (!keepSprite.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("sprite"));

                var keepOrderInLayer = serializedObject.FindProperty("keepOrderInLayer");
                EditorGUILayout.PropertyField(keepOrderInLayer);
                if (!keepOrderInLayer.boolValue) EditorGUILayout.PropertyField(serializedObject.FindProperty("orderInLayer"));

                var keepColor = serializedObject.FindProperty("keepColor");
                EditorGUILayout.PropertyField(keepColor);
                if (!keepColor.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("color"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("changeOnlyTransparency"));
                    var fade = serializedObject.FindProperty("colorFadeDuration");
                    EditorGUILayout.PropertyField(fade);
                    if (fade.floatValue > 0) EditorGUILayout.PropertyField(serializedObject.FindProperty("waitForFade"));
                }

                var keepMaterial = serializedObject.FindProperty("keepMaterial");
                EditorGUILayout.PropertyField(keepMaterial);
                if (!keepMaterial.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("material"));
                }
            }
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var eventCommand = target as ChangeSpriteEvent;
            if (eventCommand.spriteRenderer == null && eventCommand.originType == FieldOriginType.FromScene) return "No Sprite Renderer set";
            string renderer = 
                (eventCommand.originType == FieldOriginType.FromScene ? eventCommand.spriteRenderer.gameObject.name : eventCommand.persistentOrigin.ToString());
            string sprite = (eventCommand.keepSprite ? "Keep Sprite" : $"Set {renderer} sprite to {eventCommand.sprite}");
            string order = (eventCommand.keepOrderInLayer ? "Keep Order In Layer" : $"Order In Layer {eventCommand.orderInLayer}");
            string color = (eventCommand.keepColor ? "Keep Color" : $"Color {eventCommand.color}");
            string material = (eventCommand.keepMaterial ? "Keep Material" : $"Material {eventCommand.material}");
            
            return $"{sprite}, {order}, {color}, {material}";
        }
    }
}

