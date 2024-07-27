using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    public class EquipMenu : MonoBehaviour
    {
        public enum EquipmentCategoryType
        {
            Weapons = 0,
            Equipment = 1
        }
        public DetailedUnitsMenu detailedUnitsMenu;
        public InventoryItemViewer inventoryItemViewer;
        public MemberEquipmentMenu memberEquipmentMenu;
        public StatsOverviewHUD statsOverviewHUD;
        public UIMenu uiMenu;

        [Header("References")]
        
        public UIButton optimizeButton;
        public UIButton clearButton;

        [Header("Selection")]
        public InventoryItem selectedEquipment = null;
        public EquipmentSlotType selectedSlot = EquipmentSlotType.PrimaryWeapon;
        [System.NonSerialized] public PartyMember selectedMember;
        public void Awake()
        {
            if (optimizeButton)
            {
                optimizeButton.useCustomSelectSFX = true;
                optimizeButton.customSelectSFX = TUFFSettings.equipSFX;
                optimizeButton.onSelect.AddListener(OptimizeEquipment);
                optimizeButton.onHighlight.AddListener(ClearItemsBox);
            }
            if (clearButton)
            {
                clearButton.useCustomSelectSFX = true;
                clearButton.customSelectSFX = TUFFSettings.equipSFX;
                clearButton.onSelect.AddListener(ClearEquipment);
                clearButton.onHighlight.AddListener(ClearItemsBox);
            }
            if (memberEquipmentMenu)
            {
                if (memberEquipmentMenu.uiMenu)
                    memberEquipmentMenu.uiMenu.onCloseMenu.AddListener(OnEquipmentBoxClose);
            }
        }
        public void OnOpenMenu()
        {
            detailedUnitsMenu.uiMenu = uiMenu;
            detailedUnitsMenu?.UpdateUnits();
            selectedMember = null;
            selectedSlot = EquipmentSlotType.PrimaryWeapon;
            selectedEquipment = null;

            ClearItemsBox();
            UpdateEquipment();
            statsOverviewHUD?.UpdateLabels();
        }
        public void OnEquipmentBoxClose()
        {
            ClearItemsBox();
        }
        public void UpdateInfo(List<IEquipable> previewEquipment)
        {
            var member = selectedMember;
            if (member == null) return;
            if (statsOverviewHUD)
                statsOverviewHUD.UpdateInfo(member, previewEquipment);
            memberEquipmentMenu?.UpdateInfo(member);
        }
        protected void ExtraRateUpdateInfo(StatChangeElement element, float oldValue, float newValue)
        {
            element.UpdateInfo(oldValue, newValue, "%", "F0");
        }
        public void UpdateInfo()
        {
            UpdateInfo(selectedMember?.GetEquipmentAsList());
        }
        public void UpdateEquipment()
        {
            if (selectedSlot == EquipmentSlotType.PrimaryWeapon)
                LoadPrimaryWeapons();
            else if (selectedSlot == EquipmentSlotType.SecondaryWeapon)
                LoadSecondaryWeapons();
            else if (selectedSlot == EquipmentSlotType.Head)
                LoadHeadArmors();
            else if (selectedSlot == EquipmentSlotType.Body)
                LoadBodyArmors();
            else if (selectedSlot == EquipmentSlotType.PrimaryAccessory)
                LoadPrimaryAccessoryArmors();
            else if (selectedSlot == EquipmentSlotType.SecondaryAccessory)
                LoadSecondaryAccessoryArmors();
        }
        public void ClearItemsBox()
        {
            inventoryItemViewer?.LoadItems(new(), false);
        }
        public void LoadPrimaryWeapons()
        {
            selectedSlot = EquipmentSlotType.PrimaryWeapon;
            var types = new List<int>();
            if (selectedMember != null) types = selectedMember.GetWeaponEquipTypes();
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetWeaponsAndAmountOfType(WeaponWieldType.PrimarySlotOnly, types), true);
        }
        public void LoadSecondaryWeapons()
        {
            selectedSlot = EquipmentSlotType.SecondaryWeapon;
            var types = new List<int>();
            if (selectedMember != null) types = selectedMember.GetWeaponEquipTypes();
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetWeaponsAndAmountOfType(WeaponWieldType.SecondarySlotOnly, types), true);
        }
        public void LoadHeadArmors()
        {
            selectedSlot = EquipmentSlotType.Head;
            var types = new List<int>();
            if (selectedMember != null) types = selectedMember.GetArmorEquipTypes();
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetArmorsAndAmountOfType(EquipType.Head, types), true);
        }
        public void LoadBodyArmors()
        {
            selectedSlot = EquipmentSlotType.Body;
            var types = new List<int>();
            if (selectedMember != null) types = selectedMember.GetArmorEquipTypes();
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetArmorsAndAmountOfType(EquipType.Body, types), true);
        }
        public void LoadPrimaryAccessoryArmors()
        {
            selectedSlot = EquipmentSlotType.PrimaryAccessory;
            var types = new List<int>();
            if (selectedMember != null) types = selectedMember.GetArmorEquipTypes();
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetArmorsAndAmountOfType(EquipType.Accessory, types), true);
        }
        public void LoadSecondaryAccessoryArmors()
        {
            selectedSlot = EquipmentSlotType.SecondaryAccessory;
            var types = new List<int>();
            if (selectedMember != null) types = selectedMember.GetArmorEquipTypes();
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetArmorsAndAmountOfType(EquipType.Accessory, types), true);
        }
        public void DetailedUnitsOnCreate(UIButton button, PartyMember member)
        {
            button.onHighlight.AddListener(() => { HighlightMember(member); });
            button.menusToOpen.Add(memberEquipmentMenu.uiMenu);
        }
        public void InventoryViewerCreate(GeneralInfoDisplay infoDisplay, UIButton uiButton, KeyValuePair<InventoryItem, int> keyValuePair)
        {
            uiButton.useCustomSelectSFX = true;
            uiButton.customSelectSFX = TUFFSettings.equipSFX;
            uiButton.onHighlight.AddListener(InventoryViewerButtonHighlight);
            uiButton.onSelect.AddListener(EquipToUser);
            uiButton.menusToClose.Add(inventoryItemViewer.uiMenu);
            //uiButton.onHighlight.AddListener(() => selectedInfoDisplay = infoDisplay);
        }
        protected void InventoryViewerButtonHighlight()
        {
            var button = (UIButton)inventoryItemViewer.uiMenu.GetCurrentHighlight();
            InventoryItem item = null;
            if (inventoryItemViewer.infoButtons.ContainsKey(button))
                item = inventoryItemViewer.infoButtons[button];
            selectedEquipment = item;
            UpdateInfo(selectedMember?.GetPreviewEquipment((IEquipable)selectedEquipment, selectedSlot));
            //detailedUnitsMenu?.UpdateUnits();
        }
        protected void EquipToUser()
        {
            selectedMember?.Equip((IEquipable)selectedEquipment, selectedSlot);
            detailedUnitsMenu?.UpdateUnits();
            UpdateEquipment();
            UpdateInfo();
        }
        public void OptimizeEquipment()
        {
            selectedMember?.OptimizeEquipment();
            detailedUnitsMenu?.UpdateUnits();
            UpdateEquipment();
            UpdateInfo();
        }
        public void ClearEquipment()
        {
            selectedMember?.ClearEquipment();
            detailedUnitsMenu?.UpdateUnits();
            UpdateEquipment();
            UpdateInfo();
        }
        protected void HighlightMember(PartyMember member)
        {
            selectedMember = member;
            UpdateInfo();
        }
    }
}

