using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(EnemyIndex))]
    public class EnemyIndexPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20f;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;

            var enemyIdx = property.FindPropertyRelative("index");
            List<EnemyReference> enemies = new List<EnemyReference>();
            if (Selection.activeObject is GameObject obj)
            {
                var battle = obj.GetComponent<Battle>();
                if (battle != null)
                    enemies = battle.initialEnemies;
            }
            int idx = enemyIdx.intValue;
            if (idx >= 0 && idx < enemies.Count)
            {
                string name = (enemies[idx].enemy != null ? enemies[idx].enemy.GetName() : "");
                label.text += $" ({name})";
            }

            EditorGUI.PropertyField(position, enemyIdx, label);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
