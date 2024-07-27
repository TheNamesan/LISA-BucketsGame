using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(BranchActionContent))]
    public class BranchActionContentPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 0;
            float height = 0f;
            var conditionList = property.FindPropertyRelative("conditionList");
            if (property.isExpanded)
            {
                lines += 0;
                height += EditorGUI.GetPropertyHeight(conditionList);
            }
            return 20f + (20f * lines) + height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            position.y += 20f;
            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                var conditionList = property.FindPropertyRelative("conditionList");
                EditorGUI.PropertyField(position, conditionList);
                position.y += EditorGUI.GetPropertyHeight(conditionList);

                position.width += 15f;
                position.x -= 15f;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
        public static string GetConditionText(BranchActionContent branchContent)
        {
            string text = "";
            if (branchContent == null) return text;
            var conditions = branchContent.conditionList;
            text += $"({conditions.Count}) ";
            for (int i = 0; i < conditions.Count; i++)
            {
                if (i > 0) text += " AND ";
                var element = conditions[i];
                if (element == null) { text += "null"; continue; }
                var not = element.not;
                if (not) text += "(Not) ";
                switch (element.conditionType)
                {
                    case BranchConditionType.GameVariable:
                        var comparator = element.variableComparator;
                        text += GetVariableText(comparator); break;
                    case BranchConditionType.InteractableSwitch:
                        text += GetInteractableSwitchText(element.targetInteractable, element.targetSwitch); break;
                    case BranchConditionType.Timer:
                        text += "Timer"; break;
                    case BranchConditionType.Unit:
                        text += GetUnitText(element.unitComparator); break;
                    case BranchConditionType.Enemy:
                        text += "Enemy"; break;
                    case BranchConditionType.Character:
                        text += "Character"; break;
                    case BranchConditionType.Mags:
                        text += "Mags is " + GetNumberComparisonText(element.targetMags, element.numberComparison); break;
                    case BranchConditionType.InventoryItem:
                        text += GetInventoryItemText(element.inventoryComparator); break;
                }
            }
            return text;
        }
        private static string GetVariableText(GameVariableComparator comparator)
        {
            var variablesData = GameVariableList.GetList();
            if (comparator.targetVariableIndex >= variablesData.Length) return "";
            string text = $"'{variablesData[comparator.targetVariableIndex].name}' ";
            switch (comparator.targetVariableValueType)
            {
                case GameVariableValueType.BoolValue:
                    text += $"Bool Value is {comparator.variableBool}"; break;
                case GameVariableValueType.NumberValue:
                    text += $"Number Value is {comparator.variableNumber}"; break;
                case GameVariableValueType.StringValue:
                    text += $"String Value is '{comparator.variableString}'"; break;
                case GameVariableValueType.VectorValue:
                    text += $"Vector Value is {comparator.variableVector}"; break;
            }
            return text;
        }
        private static string GetInteractableSwitchText(InteractableObject target, int targetSwitch)
        {
            string text = "";
            string name = "null";
            if (target) name = target.gameObject.name;
            text = $"{name} Switch is {targetSwitch}";
            return text;
        }
        private static string GetInventoryItemText(InventoryComparator comparator)
        {
            string text = "";
            string name = "null";
            DropType inventoryType = comparator.inventoryType;
            switch (inventoryType)
            {
                case DropType.Item:
                    if (comparator.targetItem) name = comparator.targetItem.GetName(); break;
                case DropType.KeyItem:
                    if (comparator.targetKeyItem) name = comparator.targetKeyItem.GetName(); break;
                case DropType.Weapon:
                    if (comparator.targetWeapon) name = comparator.targetWeapon.GetName(); break;
                case DropType.Armor:
                    if (comparator.targetArmor) name = comparator.targetArmor.GetName(); break;
            }
            string amount = GetNumberComparisonText(comparator.targetItemCount, comparator.numberComparison);
            text = $"Has {amount} {name} in Inventory";
            if (inventoryType == DropType.Weapon || inventoryType == DropType.Armor)
                text += (comparator.includeEquipment ? "(Include Equipment)" : "");
            return text;
        }
        private static string GetUnitText(UnitStatusComparator comparator)
        {
            string text = "";
            string name = "null";
            Unit unit = comparator.targetUnit;
            var condition = comparator.unitCondition;
            if (unit) name = unit.GetName();
            string conditionText = "";
            switch (condition)
            {
                case UnitStatusConditionType.IsInParty:
                    conditionText = "is in Party"; break;
                case UnitStatusConditionType.IsInActiveParty:
                    conditionText = "is in Active Party"; break;
                case UnitStatusConditionType.IsNamed:
                    conditionText = $"is named '{comparator.targetName}'"; break;
                case UnitStatusConditionType.HasJob:
                    string jobName = (comparator.targetJob ? comparator.targetJob.GetName() : "null");
                    conditionText = $"has '{jobName}' job"; break;
                case UnitStatusConditionType.KnowsSkill:
                    string skillName = (comparator.targetSkill ? comparator.targetSkill.GetName() : "null");
                    conditionText = $"has learned '{skillName}'"; break;
                case UnitStatusConditionType.HasWeaponEquipped:
                    string weaponName = (comparator.targetWeapon ? comparator.targetWeapon.GetName() : "null");
                    conditionText = $"has '{weaponName}' equipped"; break;
                case UnitStatusConditionType.HasArmorEquipped:
                    string armorName = (comparator.targetArmor ? comparator.targetArmor.GetName() : "null");
                    conditionText = $"has '{armorName}' equipped"; break;
                case UnitStatusConditionType.IsStateInflicted:
                    string stateName = (comparator.targetState ? comparator.targetState.GetName() : "null");
                    conditionText = $"is '{stateName}' inflicted"; break;
            }
            text = $"{name} {conditionText}";
            return text;
        }
        private static string GetNumberComparisonText(int count, NumberComparisonType comparison)
        {
            string text = $"";
            switch (comparison)
            {
                case NumberComparisonType.NotEqual:
                    text += $"not {count}"; break;
                case NumberComparisonType.Less:
                    text += $"less than {count}"; break;
                case NumberComparisonType.LessOrEqual:
                    text += $"{count} or less"; break;
                case NumberComparisonType.Greater:
                    text += $"more than {count}"; break;
                case NumberComparisonType.GreaterOrEqual:
                    text += $"{count} or more"; break;
                default:
                    text += $"{count}"; break;
            }
            return text;
        }
    }
    [CustomPropertyDrawer(typeof(BranchActionContentElement))]
    public class BranchActionContentElementPD : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 1;
            if (property.isExpanded)
            {
                lines += 2;
                var element = LISAEditorUtility.GetTargetObjectOfProperty(property) as BranchActionContentElement;
                if (element.conditionType == BranchConditionType.GameVariable) lines += 2;
                if (element.conditionType == BranchConditionType.InteractableSwitch) lines += 1;
                if (element.conditionType == BranchConditionType.Timer) lines += 1;
                if (element.conditionType == BranchConditionType.Unit) lines += 1;
                if (element.conditionType == BranchConditionType.Enemy) lines += 1;
                if (element.conditionType == BranchConditionType.Character) lines += 1;
                if (element.conditionType == BranchConditionType.Mags) lines += 1;
                if (element.conditionType == BranchConditionType.InventoryItem) lines += 1;
                if (element.conditionType == BranchConditionType.Button) lines += 1;
                //lines += 3;
            }
            return (20f * lines);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 20f;
            var orgX = position.x;
            var orgWidth = position.width;
            var orgLabel = EditorGUIUtility.labelWidth;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            position.y += 20f;
            if (property.isExpanded)
            {
                position.x += 15f;
                position.width -= 15f;

                EditorGUI.PropertyField(position, property.FindPropertyRelative("not"));
                position.y += 20f;

                var conditionType = property.FindPropertyRelative("conditionType");
                EditorGUI.PropertyField(position, conditionType);
                position.y += 20f;

                var type = (BranchConditionType)conditionType.enumValueIndex;
                if (type == BranchConditionType.GameVariable)
                    DrawGameVariable(position, orgWidth, property);
                if (type == BranchConditionType.InteractableSwitch)
                    DrawInteractable(position, orgWidth, property);
                if (type == BranchConditionType.Timer)
                    ;
                if (type == BranchConditionType.Unit)
                    DrawUnit(position, orgWidth, property);
                if (type == BranchConditionType.Enemy)
                    ;
                if (type == BranchConditionType.Character)
                    ;
                if (type == BranchConditionType.Mags)
                    DrawMags(position, orgWidth, property);
                if (type == BranchConditionType.InventoryItem)
                    DrawInventoryItem(position, orgWidth, property);//DrawMags(position, orgWidth, property, nameof(BranchActionContentElement.includeEquipment));

                position.width += 15f;
                position.x -= 15f;
                EditorGUIUtility.labelWidth = orgLabel;
            }
            property.serializedObject.ApplyModifiedProperties();
        }
        private void DrawGameVariable(Rect position, float orgWidth, SerializedProperty property)
        {
            //position.x += position.width + 2;
            position.width = orgWidth - 2;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("variableComparator"), new GUIContent(""));
            position.width = orgWidth;
        }
        private void DrawInteractable(Rect position, float orgWidth, SerializedProperty property)
        {
            position.width = orgWidth * 0.5f - 8;
            position.height = 20;
            float orgLabel = EditorGUIUtility.labelWidth;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("targetInteractable"));

            position.x += position.width + 2;
            EditorGUIUtility.labelWidth = 70;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("targetSwitch"), new GUIContent("switch is"));
            EditorGUIUtility.labelWidth = orgLabel;
            position.width = orgWidth;
        }
        private void DrawUnit(Rect position, float orgWidth, SerializedProperty property)
        {
            position.width = orgWidth - 2;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("unitComparator"), new GUIContent(""));
            position.width = orgWidth;
        }
        private void DrawMags(Rect position, float orgWidth, SerializedProperty property)
        {
            var orgX = position.x;
            var orgLabel = EditorGUIUtility.labelWidth;

            float w = orgWidth * 0.33f - 2;
            position.width = w;

            var comparison = property.FindPropertyRelative(nameof(BranchActionContentElement.numberComparison));
            EditorGUIUtility.labelWidth = 128;
            EditorGUI.PropertyField(position, comparison, new GUIContent("Player Mags is"));
            EditorGUIUtility.labelWidth = orgLabel;
            position.x += position.width + 2;

            var targetMags = property.FindPropertyRelative(nameof(BranchActionContentElement.targetMags));
            EditorGUI.PropertyField(position, targetMags, new GUIContent(""));

            //position.x += position.width + 2;
            //EditorGUIUtility.labelWidth = 16;
            //EditorGUIUtility.labelWidth = orgLabel;
        }
        private void DrawInventoryItem(Rect position, float orgWidth, SerializedProperty property)
        {
            position.width = orgWidth - 2;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("inventoryComparator"), new GUIContent(""));
            position.width = orgWidth;
        }
    }
}

