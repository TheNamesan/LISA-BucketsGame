using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class Inventory
    {
        public const int INVENTORY_CAP = 99; //Move this to TUFF Settings
        public int[] items = new int[0];
        public int[] keyItems = new int[0];
        public int[] weapons = new int[0];
        public int[] armors = new int[0];

        public static Inventory instance
        {
            get
            {
                return (PlayerData.instance != null ?
                   PlayerData.instance.inventory : null);
            }
        }
        public void Initialize()
        {
            items = new int[DatabaseLoader.instance.items.Length];
            keyItems = new int[DatabaseLoader.instance.keyItems.Length];
            weapons = new int[DatabaseLoader.instance.weapons.Length];
            armors = new int[DatabaseLoader.instance.armors.Length];

            if (TUFFSettings.DebugStartWithMaxItems())
                SetAmountToAll(INVENTORY_CAP);
        }
        

        public Dictionary<InventoryItem, int> GetItemsAndAmount(Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            for (int i = 0; i < items.Length; i++) {
                var item = DatabaseLoader.instance.items[i];
                var amount = GetItemAmount(item);
                if (!includeZero && amount <= 0) continue;
                baseDirectory.Add(item, amount);
            }
            return baseDirectory;
        }
        public Dictionary<InventoryItem, int> GetKeyItemsAndAmount(Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            for (int i = 0; i < keyItems.Length; i++) {
                var item = DatabaseLoader.instance.keyItems[i];
                var amount = GetItemAmount(item);
                if (!includeZero && amount <= 0) continue;
                baseDirectory.Add(item, amount);
            }
            return baseDirectory;
        }
        public Dictionary<InventoryItem, int> GetWeaponsAndAmount(Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            for (int i = 0; i < weapons.Length; i++) {
                var item = DatabaseLoader.instance.weapons[i];
                var amount = GetItemAmount(item);
                if (!includeZero && amount <= 0) continue;
                baseDirectory.Add(item, amount);
            }
            return baseDirectory;
        }
        public Dictionary<InventoryItem, int> GetArmorsAndAmount(Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            for (int i = 0; i < armors.Length; i++) {
                var item = DatabaseLoader.instance.armors[i];
                var amount = GetItemAmount(item);
                if (!includeZero && amount <= 0) continue;
                baseDirectory.Add(item, amount);
            }
            return baseDirectory;
        }
        public Dictionary<InventoryItem, int> GetEntireInventoryAndAmount(Dictionary<InventoryItem, int> baseDirectory = null, bool includeZero = false)
        {
            if (baseDirectory == null) baseDirectory = new Dictionary<InventoryItem, int>();
            GetItemsAndAmount(baseDirectory, includeZero);
            GetKeyItemsAndAmount(baseDirectory, includeZero);
            GetWeaponsAndAmount(baseDirectory, includeZero);
            GetArmorsAndAmount(baseDirectory, includeZero);
            return baseDirectory;
        }
        public int GetItemAmount(InventoryItem item)
        {
            if (item is Item) return items[item.id];
            if (item is KeyItem) return keyItems[item.id];
            if (item is Weapon) return weapons[item.id];
            if (item is Armor) return armors[item.id];
            return -1;
        }
        public bool HasItem(InventoryItem item)
        {
            return GetItemAmount(item) >= 0;
        }
        public void AddToInventory(Item item, int amount)
        {
            if (item == null) return;
            items[item.id] = Mathf.Clamp(items[item.id] + amount, 0, Inventory.INVENTORY_CAP);
        }
        public void AddToInventory(KeyItem keyItem, int amount)
        {
            if (keyItem == null) return;
            keyItems[keyItem.id] = Mathf.Clamp(keyItems[keyItem.id] + amount, 0, Inventory.INVENTORY_CAP);
        }
        public void AddToInventory(Weapon weapon, int amount)
        {
            if (weapon == null) return;
            weapons[weapon.id] = Mathf.Clamp(weapons[weapon.id] + amount, 0, Inventory.INVENTORY_CAP);
        }
        public void AddToInventory(Armor armor, int amount)
        {
            if (armor == null) return;
            armors[armor.id] = Mathf.Clamp(armors[armor.id] + amount, 0, Inventory.INVENTORY_CAP);
        }
        /// <summary>
        /// Sets the amount of Items, Key Items, Weapons and Armors to the specified value.
        /// </summary>
        /// <param name="amount">Amount of items.</param>
        public void SetAmountToAll(int amount)
        {
            int value = Mathf.Clamp(amount, 0, INVENTORY_CAP);

            System.Array.Fill(items, value, 0, items.Length);
            System.Array.Fill(keyItems, value, 0, keyItems.Length);
            System.Array.Fill(weapons, value, 0, weapons.Length);
            System.Array.Fill(armors, value, 0, armors.Length);
        }
    }
}

