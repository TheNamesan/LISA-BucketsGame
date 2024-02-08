using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomEditor(typeof(ChangeInventoryEvent)), CanEditMultipleObjects]
    public class ChangeInventoryEventEditor : EventCommandEditor
    {
        public override void InspectorGUIContent()
        {
            var dropType = serializedObject.FindProperty("dropType");
            EditorGUILayout.PropertyField(dropType);

            if (dropType.enumValueIndex == (int)DropType.Item)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("item"));
            if (dropType.enumValueIndex == (int)DropType.KeyItem)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("keyItem"));
            if (dropType.enumValueIndex == (int)DropType.Weapon)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("weapon"));
            if (dropType.enumValueIndex == (int)DropType.Armor)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("armor"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("constant"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }
        private string GetSummaryText()
        {
            var command = target as ChangeInventoryEvent;
            string drop = "";

            if (command.dropType == DropType.Item) drop = command.item?.GetName();
            if (command.dropType == DropType.KeyItem) drop = command.keyItem?.GetName();
            if (command.dropType == DropType.Weapon) drop = command.weapon?.GetName();
            if (command.dropType == DropType.Armor) drop = command.armor?.GetName();

            int amount = command.constant;
            return $"{drop} ({command.dropType}) {(amount > 0 ? $"+{amount}" : amount)}";
        }
    }
}

