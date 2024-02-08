using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ScopeData))]
    public class ScopeDataPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 0;
            var scope = (ScopeType)property.FindPropertyRelative("scopeType").enumValueIndex;
            if (property.isExpanded)
            {
                lines += 3;
                if (BattleManager.IsRandomScope(scope)) lines += 1;
            }
            
            return 20f + (20f * lines);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GetLabel(property, label), true);
            position.y += 20f;
            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                var scopeProp = property.FindPropertyRelative("scopeType");
                var scope = (ScopeType)scopeProp.enumValueIndex;
                EditorGUI.PropertyField(position, scopeProp);
                position.y += 20f;

                EditorGUI.PropertyField(position, property.FindPropertyRelative("targetCondition"));
                position.y += 20f;

                if (BattleManager.IsRandomScope(scope))
                {
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("randomNumberOfTargets"));
                    position.y += 20f;
                }
                EditorGUI.PropertyField(position, property.FindPropertyRelative("excludeUser"));
                position.y += 20f;

                position.width += 15f;
                position.x -= 15f;
            }

            property.serializedObject.ApplyModifiedProperties();
        }
        private GUIContent GetLabel(SerializedProperty property, GUIContent label)
        {
            var scope = (ScopeType)property.FindPropertyRelative("scopeType").enumValueIndex;
            var scopeText = ObjectNames.NicifyVariableName(scope.ToString());
            label.text += $" ({scopeText})";
            return label;
        }
    }
}

