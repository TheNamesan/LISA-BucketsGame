using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeSpriteAction))]
    public class ChangeSpriteActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            var originType = targetProperty.FindPropertyRelative("originType");
            var persistentOrigin = targetProperty.FindPropertyRelative("persistentOrigin");
            var renderer = targetProperty.FindPropertyRelative("spriteRenderer");

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
                var keepEnabled = targetProperty.FindPropertyRelative("keepEnabled");
                EditorGUILayout.PropertyField(keepEnabled);
                if (!keepEnabled.boolValue) EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("enabled"));

                var keepSprite = targetProperty.FindPropertyRelative("keepSprite");
                EditorGUILayout.PropertyField(keepSprite);
                if (!keepSprite.boolValue) EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("sprite"));

                var keepOrderInLayer = targetProperty.FindPropertyRelative("keepOrderInLayer");
                EditorGUILayout.PropertyField(keepOrderInLayer);
                if (!keepOrderInLayer.boolValue) EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("orderInLayer"));

                var keepColor = targetProperty.FindPropertyRelative("keepColor");
                EditorGUILayout.PropertyField(keepColor);
                if (!keepColor.boolValue)
                {
                    EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("color"));
                    EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("changeOnlyTransparency"));
                    var fade = targetProperty.FindPropertyRelative("colorFadeDuration");
                    EditorGUILayout.PropertyField(fade);
                    if (fade.floatValue > 0) EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("waitForFade"));
                }

                var keepMaterial = targetProperty.FindPropertyRelative("keepMaterial");
                EditorGUILayout.PropertyField(keepMaterial);
                if (!keepMaterial.boolValue)
                {
                    EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("material"));
                }
            }
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeSpriteAction;
            if (action.spriteRenderer == null && action.originType == FieldOriginType.FromScene) return "No Sprite Renderer set";
            string renderer =
                (action.originType == FieldOriginType.FromScene ? action.spriteRenderer.gameObject.name : action.persistentOrigin.ToString());
            string sprite = (action.keepSprite ? "Keep Sprite" : $"Set {renderer} sprite to {action.sprite}");
            string order = (action.keepOrderInLayer ? "Keep Order In Layer" : $"Order In Layer {action.orderInLayer}");
            string color = (action.keepColor ? "Keep Color" : $"Color {action.color}");
            string material = (action.keepMaterial ? "Keep Material" : $"Material {action.material}");

            return $"{sprite}, {order}, {color}, {material}";
        }
    }
}

