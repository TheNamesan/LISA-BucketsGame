using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class ShopMenu : MonoBehaviour
    {
        public DetailedUnitsMenu detailedUnitsMenu;
        public InventoryItemViewer inventoryItemViewer;
        public MagazineCountViewer magazineCountViewer;
        public StatDisplayHUD possessionCount;
        public UIMenu uiMenu;

        [Header("Quanitity")]
        public UIMenu quantityMenu;
        public GeneralInfoDisplay quantityInfoDisplay;
        public StatDisplayHUD magsQuantityDisplay;
        public UIButton quantityButton;

        [Tooltip("Item/Cost")]
        public Dictionary<InventoryItem, int> itemsDictionary = new Dictionary<InventoryItem, int>();

        [Header("Texts")]
        public TMP_Text buyText;
        public TMP_Text sellText;
        public TMP_Text cancelText;
        public TMP_Text quantityLabel;

        [HideInInspector] public ShopData shopData;
        [HideInInspector] public EventAction actionCallback;

        [Header("Selection")]
        public InventoryItem selectedItem = null;
        public int selectedPrice = 0;
        public int selectedQuantity = 0;

        public void OpenShop(ShopData shopData, EventAction actionCallback = null)
        {
            SetSelection(null, 0);
            selectedQuantity = 0;
            this.shopData = shopData;
            itemsDictionary = CreateDictionary(shopData.shopItems);
            UpdateItems();
            this.actionCallback = actionCallback;
            uiMenu.OpenMenu();
            quantityMenu.CloseMenu();
            UpdatePossessionCount(0);
            detailedUnitsMenu?.UpdateUnits();
            UpdateTexts();
            UpdatePlayerMags();
        }
        protected Dictionary<InventoryItem, int> CreateDictionary(List<PurchasableItem> itemList)
        {
            var dictionary = new Dictionary<InventoryItem, int>();
            for(int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i] == null) continue;
                if (itemList[i].invItem == null) continue;
                var invItem = itemList[i].invItem;
                int price = itemList[i].Price;
                if (!dictionary.ContainsKey(invItem))
                {
                    dictionary.Add(invItem, price);
                }
            }
            return dictionary;
        }
        public void UpdateTexts()
        {
            if (buyText != null) buyText.text = TUFFSettings.buyText;
            if (sellText != null) sellText.text = TUFFSettings.sellText;
            if (cancelText != null) cancelText.text = TUFFSettings.cancelText;
            if (quantityLabel != null) quantityLabel.text = TUFFSettings.quantityText;
        }
        public void UpdatePossessionCount(int value)
        {
            possessionCount?.UpdateInfo(value, Color.white, true, TUFFSettings.possessionText);
        }
        protected void SetSelection(InventoryItem item, int price)
        {
            selectedItem = item;
            selectedPrice = price;
        }
        public void UpdatePlayerMags()
        {
            magazineCountViewer?.ShowMags(LISAUtility.IntToString(PlayerData.instance.mags));
        }
        public void OnItemCreation(GeneralInfoDisplay infoDisplay, UIButton uiButton, KeyValuePair<InventoryItem, int> keyValuePair)
        {
            var invItem = keyValuePair.Key;
            var price = keyValuePair.Value;

            uiButton.onHighlight.AddListener(InventoryViewerButtonHighlight);
            uiButton.onSelect.AddListener(OpenQuantityMenu);

            //uiButton.useCustomSelectSFX = true;
            //uiButton.customSelectSFX = TUFFSettings.shopSFX;
        }
        public void InventoryViewerUpdate(GeneralInfoDisplay infoDisplay, UIButton uiButton, KeyValuePair<InventoryItem, int> keyValuePair)
        {
            var invItem = keyValuePair.Key;
            var price = keyValuePair.Value;
            if (PlayerData.instance.mags < price || 
                PlayerData.instance.inventory.GetItemAmount(invItem) >= Inventory.INVENTORY_CAP) 
                uiButton.disabled = true;
            else uiButton.disabled = false;
        }
        protected void InventoryViewerButtonHighlight()
        {
            var button = (UIButton)inventoryItemViewer.uiMenu.GetCurrentHighlight();
            InventoryItem item = null;
            if (inventoryItemViewer.infoButtons.ContainsKey(button))
                item = inventoryItemViewer.infoButtons[button];
            if (item != null)
            {
                int price = inventoryItemViewer.currentDictionary[item];
                SetSelection(item, price);
            }
            UpdatePossessionCount(PlayerData.instance.inventory.GetItemAmount(selectedItem));
            detailedUnitsMenu?.UpdateUnits();
        }
        public void OpenQuantityMenu()
        {
            selectedQuantity = 1;
            quantityButton.disabled = false;
            quantityMenu.OpenMenu();
            UpdateQuantityInfo();
        }
        protected void UpdateQuantityInfo()
        {
            quantityButton.text.text = LISAUtility.IntToString(selectedQuantity);
            quantityInfoDisplay.DisplayInfo(selectedItem.icon, selectedItem.GetName());
            magsQuantityDisplay.UpdateInfo(selectedQuantity * selectedPrice, Color.white, true, TUFFSettings.currencyText);
        }
        public void QuantityPickerHorizontal(int direction)
        {
            int dir = LISAUtility.Sign(direction);
            ChangeQuantity(dir);
            quantityButton.ForcePlaySelectClip();
            UpdateQuantityInfo();
        }
        public void QuantityPickerVertical(int direction)
        {
            int dir = LISAUtility.Sign(direction);
            ChangeQuantity(dir * 10);
            quantityButton.ForcePlaySelectClip();
            UpdateQuantityInfo();
        }
        private void ChangeQuantity(int value)
        {
            int possibleMax = Inventory.INVENTORY_CAP - PlayerData.instance.inventory.GetItemAmount(selectedItem);
            int magsMax = possibleMax;
            if (selectedPrice > 0) magsMax = LISAUtility.Truncate(PlayerData.instance.mags / (float)selectedPrice);
            int max = Mathf.Min(magsMax, possibleMax);
            if (max <= 0) max = 1;

            if (value < 0)
            {
                if (selectedQuantity == 1) selectedQuantity = max;
                else
                { 
                    selectedQuantity += value;
                    if (selectedQuantity < 1) selectedQuantity = 1;
                }
            }
            else if (value > 0)
            {
                if (selectedQuantity == max) selectedQuantity = 1;
                else
                {
                    selectedQuantity += value;
                    if (selectedQuantity > max) selectedQuantity = max;
                }
            }
        }
        public void BuyCurrentItem()
        {
            BuyItem(selectedItem, selectedPrice, selectedQuantity);
            AudioManager.instance.PlaySFX(TUFFSettings.shopSFX);

            quantityMenu.CloseMenu();
            UpdatePossessionCount(PlayerData.instance.inventory.GetItemAmount(selectedItem));
            UpdateItems();
            detailedUnitsMenu?.UpdateUnits();
        }

        protected void UpdateItems()
        {
            inventoryItemViewer?.LoadItems(itemsDictionary);
        }

        public void BuyItem(InventoryItem invItem, int price, int quantity)
        {
            if (invItem == null) return;
            PlayerData.instance.AddToInventory(invItem, quantity);
            PlayerData.instance.AddMags(-price * quantity);
            UpdatePlayerMags();
        }
        public void CloseShop()
        {
            SetSelection(null, 0);
            if (actionCallback != null) actionCallback.isFinished = true;
        }
    }
}

