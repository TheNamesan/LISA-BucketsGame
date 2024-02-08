using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(ChangeEnemyGraphicAction))]
    public class ChangeEnemyGraphicActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("enemyIndex"));
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("graphic"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var action = targetObject as ChangeEnemyGraphicAction;
            var sprite = action.graphic;
            string name = (sprite != null ? sprite.name : "null");

            return $"Change Enemy Graphic to {name}";
        }
    }
}

