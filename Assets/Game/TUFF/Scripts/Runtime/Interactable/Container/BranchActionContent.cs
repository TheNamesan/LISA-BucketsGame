using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class BranchActionContent
    {
        public List<BranchActionContentElement> conditionList = new List<BranchActionContentElement>();
        public bool condition = false;
        
        public ActionList actionList = new ActionList();

        public bool ValidateCondition()
        {
            for (int i = 0; i < conditionList.Count; i++)
            {
                var condition = conditionList[i];
                if (condition == null) continue;
                if (!condition.ValidateCondition())
                    return false;
            }
            return true;
        }
    }
    [System.Serializable]
    public class BranchActionContentElement
    {
        [Tooltip("If true, this branch will trigger when the condtion is not met.")]
        public bool not = false;
        public BranchConditionType conditionType;

        public GameVariableComparator variableComparator;
        public InteractableObject targetInteractable;
        public int targetSwitch;

        public UnitStatusComparator unitComparator;

        public int targetMags;
        public NumberComparisonType numberComparison;

        public InventoryComparator inventoryComparator;
        public bool ValidateCondition()
        {
            bool valid = false;
            switch (conditionType)
            {
                case BranchConditionType.GameVariable:
                    valid = variableComparator.ValidateGameVariable(); break;
                case BranchConditionType.InteractableSwitch:
                    if (!targetInteractable) valid = true;
                    else valid = targetInteractable.currentSwitch == targetSwitch;
                    break;
                case BranchConditionType.Timer:
                    break;
                case BranchConditionType.Unit:
                    valid = unitComparator.ValidateUnit(); break;
                case BranchConditionType.Enemy:
                    break;
                case BranchConditionType.Character:
                    break;
                case BranchConditionType.Mags:
                    valid = ValidateCount(PlayerData.instance.mags); break;
                case BranchConditionType.InventoryItem:
                    valid = inventoryComparator.ValidateInventory(); break;
                default: break;
            }
            if (not) valid = !valid;
            return valid;
        }
        public bool ValidateCount(int count)
        {
            switch (numberComparison)
            {
                case NumberComparisonType.Equal:
                    return count == targetMags;
                case NumberComparisonType.NotEqual:
                    return count != targetMags;
                case NumberComparisonType.Less:
                    return count < targetMags;
                case NumberComparisonType.LessOrEqual:
                    return count <= targetMags;
                case NumberComparisonType.Greater:
                    return count > targetMags;
                case NumberComparisonType.GreaterOrEqual:
                    return count >= targetMags;
            }
            return true;
        }
    }
}