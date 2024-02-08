using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ChangeInventoryEvent : EventCommand
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
        [Tooltip("Constant amount of items to change.")]
        public int constant = 0;

        public override void Invoke()
        {
            if (dropType == DropType.Item) GameManager.instance.playerData.AddToInventory(item, constant);
            else if (dropType == DropType.KeyItem) GameManager.instance.playerData.AddToInventory(keyItem, constant);
            else if (dropType == DropType.Weapon) GameManager.instance.playerData.AddToInventory(weapon, constant);
            else if (dropType == DropType.Armor) GameManager.instance.playerData.AddToInventory(armor, constant);
            isFinished = true;
        }
        public override void OnInstantiate()
        {
            eventName = "Change Inventory";
            eventColor = new Color(1f, 0.8f, 0.25f, 1f);
        }
    }
}

