using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    public class PlayerItemsMenuManager : MonoBehaviour
    {
        public enum InventoryCategoryType
        {
            Items = 0,
            Weapons = 1,
            Armors = 2,
            KeyItems = 3
        }

        public DetailedUnitsMenu detailedUnitsMenu;
        public InventoryItemViewer inventoryItemViewer;
        public TMP_Text itemsText;
        public TMP_Text weaponsText;
        public TMP_Text armorsText;
        public TMP_Text keyItemsText;

        public InventoryItem selectedItem = null;
        public GeneralInfoDisplay selectedInfoDisplay = null;
        public InventoryCategoryType inventoryCategoryType = InventoryCategoryType.Items;
        public void InitializeMenu()
        {
            selectedItem = null;
            selectedInfoDisplay = null;
            inventoryCategoryType = InventoryCategoryType.Items;
            UpdateTexts();
            detailedUnitsMenu?.UpdateUnits();
        }
        public void InventoryViewerCreate(GeneralInfoDisplay infoDisplay, UIButton uiButton, KeyValuePair<InventoryItem, int> keyValuePair)
        {
            uiButton.menusToOpen.Add(detailedUnitsMenu.uiMenu);
            uiButton.onHighlight.AddListener(InventoryViewerButtonHighlight);
            uiButton.onHighlight.AddListener(() => selectedInfoDisplay = infoDisplay);
        }
        public void InventoryViewerUpdate(GeneralInfoDisplay infoDisplay, UIButton uiButton, KeyValuePair<InventoryItem, int> keyValuePair)
        {
            var invItem = keyValuePair.Key;

            uiButton.disabled = false;
            if (invItem == null) // If no item assigned
                uiButton.disabled = true;
            else if (invItem is Item)
            {
                var item = invItem as Item;
                if (!BattleManager.IsAllyScope(item.scopeData.scopeType))
                {
                    uiButton.disabled = true;
                }
                else
                {
                    uiButton.disabled = false;
                }
            }
        }
        protected void InventoryViewerButtonHighlight()
        {
            var button = (UIButton)inventoryItemViewer.uiMenu.GetCurrentHighlight();
            InventoryItem item = null;
            if (inventoryItemViewer.infoButtons.ContainsKey(button)) 
                item = inventoryItemViewer.infoButtons[button];
            if (item != null)
            {
                selectedItem = item;
            }
            detailedUnitsMenu?.UpdateUnits();
        }
        public void ButtonCreate(UIButton button, PartyMember member)
        {
            button.onSelect.AddListener(() => { GiveItemToUser(member); });

            button.useCustomSelectSFX = true;
            button.customSelectSFX = TUFFSettings.useItemSFX; // Change this to work with weapons / armor
        }
        public void CheckTargetIsValidForCurrentItem(UIButton button, PartyMember member)
        {
            if (selectedItem == null) {
                button.disabled = true;
                return;
            }
            if (selectedItem is Item)
            {
                var item = selectedItem as Item;
                var isValid = BattleManager.IsValidTargetFromConditionScope(item.scopeData.targetCondition, member);
                button.disabled = !isValid;
            }
            else button.disabled = true; // Change this to respective weapon / armor
        }
        public void GiveItemToUser(PartyMember user)
        {
            if (selectedItem == null) return;
            if (selectedItem is Item)
            {
                var item = selectedItem as Item;
                var isValid = BattleManager.IsValidTargetFromConditionScope(item.scopeData.targetCondition, user);
                if (!isValid) return;
            }
            selectedItem.ConsumeItemFromMenu(user);
            detailedUnitsMenu?.UpdateUnits();
            inventoryItemViewer.UpdateInfoDisplay(selectedInfoDisplay, 
                new KeyValuePair<InventoryItem, int>(selectedItem, PlayerData.instance.inventory.GetItemAmount(selectedItem)));
        }
        public void UpdateTexts()
        {
            if (itemsText != null) itemsText.text = TUFFSettings.itemsText;
            if (weaponsText != null) weaponsText.text = TUFFSettings.weaponsText;
            if (armorsText != null) armorsText.text = TUFFSettings.armorsText;
            if (keyItemsText != null) keyItemsText.text = TUFFSettings.keyItemsText;
        }
        public void UpdateCurrentMenu()
        {
            switch(inventoryCategoryType)
            {
                case InventoryCategoryType.Items:
                    LoadItems();
                    break;
                case InventoryCategoryType.Weapons:
                    LoadWeapons();
                    break;
                case InventoryCategoryType.Armors:
                    LoadArmors();
                    break;
                case InventoryCategoryType.KeyItems:
                    LoadKeyItems();
                    break;
                default:
                    break;
            }
        }
        public void LoadItems()
        {
            inventoryCategoryType = InventoryCategoryType.Items;
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetItemsAndAmount());
        }
        public void LoadWeapons()
        {
            inventoryCategoryType = InventoryCategoryType.Weapons;
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetWeaponsAndAmount());
        }
        public void LoadArmors()
        {
            inventoryCategoryType = InventoryCategoryType.Armors;
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetArmorsAndAmount());
        }
        public void LoadKeyItems()
        {
            inventoryCategoryType = InventoryCategoryType.KeyItems;
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetKeyItemsAndAmount());
        }
        public void OnItemsBoxMenuClose()
        {
            selectedItem = null;
            selectedInfoDisplay = null;
            detailedUnitsMenu?.UpdateUnits();
        }
    }
}

