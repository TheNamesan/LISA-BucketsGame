using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeInventoryAction : EventAction
    {
        public DropType dropType = DropType.Item;
        [Tooltip("Reference to the item to drop.")]
        public Item item = null;
        [Tooltip("Reference to the item to drop.")]
        public KeyItem keyItem = null;
        [Tooltip("Reference to the item to drop.")]
        public Weapon weapon = null;
        [Tooltip("Reference to the item to drop.")]
        public Armor armor = null;
        [Header("Operand")]
        [Tooltip("Amount of items to change.")]
        public NumberOperand operand = new();
        public ChangeInventoryAction()
        {
            eventName = "Change Inventory";
            eventColor = EventGUIColors.party;
        }
        public override void Invoke()
        {
            int value = (int)operand.GetNumber();
            if (dropType == DropType.Item) GameManager.instance.playerData.AddToInventory(item, value);
            else if (dropType == DropType.KeyItem) GameManager.instance.playerData.AddToInventory(keyItem, value);
            else if (dropType == DropType.Weapon) GameManager.instance.playerData.AddToInventory(weapon, value);
            else if (dropType == DropType.Armor) GameManager.instance.playerData.AddToInventory(armor, value);
            isFinished = true;
        }
    }
}

