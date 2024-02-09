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
        public UIMenu uiMenu;

        [Header("References")]
        public TMP_Text nameText;
        public StatChangeElement maxHPElement;
        public StatChangeElement maxSPElement;
        public StatChangeElement maxTPElement;
        public StatChangeElement ATKElement;
        public StatChangeElement DEFElement;
        public StatChangeElement SATKElement;
        public StatChangeElement SDEFElement;
        public StatChangeElement AGIElement;
        public StatChangeElement LUKElement;
        public StatChangeElement hitRateElement;
        public StatChangeElement evasionRateElement;
        public StatChangeElement critRateElement;
        public StatChangeElement targetRateElement;

        [Header("Selection")]
        public InventoryItem selectedEquipment = null;
        public EquipmentSlotType selectedSlot = EquipmentSlotType.PrimaryWeapon;
        [System.NonSerialized] public PartyMember selectedMember;
        public void InitializeMenu()
        {
            detailedUnitsMenu.uiMenu = uiMenu;
            detailedUnitsMenu?.UpdateUnits();
            selectedMember = null;
            selectedSlot = EquipmentSlotType.PrimaryWeapon;
            selectedEquipment = null;
            UpdateEquipment();
            ApplyLabels();
        }
        public void UpdateInfo(List<IEquipable> previewEquipment)
        {
            var member = selectedMember;
            if (member == null) return;
            var job = member.GetJob();
            if (job == null) return;
            Debug.Log(member.GetName());
            nameText.text = member.GetName();
            maxHPElement.UpdateInfo(member.GetMaxHP(), member.GetMaxHP(previewEquipment));
            if (job.usesSP)
            {
                maxSPElement.gameObject.SetActive(true);
                maxSPElement.UpdateInfo(member.GetMaxSP(), member.GetMaxSP(previewEquipment));
            }
            else maxSPElement.gameObject.SetActive(false);
            if (job.usesTP)
            {
                maxTPElement.gameObject.SetActive(true);
                maxTPElement.UpdateInfo(member.GetMaxTP(), member.GetMaxTP(previewEquipment));
            }
            else maxTPElement.gameObject.SetActive(false);
            ATKElement.UpdateInfo(member.GetATK(), member.GetATK(previewEquipment));
            DEFElement.UpdateInfo(member.GetDEF(), member.GetDEF(previewEquipment));
            SATKElement.UpdateInfo(member.GetSATK(), member.GetSATK(previewEquipment));
            SDEFElement.UpdateInfo(member.GetSDEF(), member.GetSDEF(previewEquipment));
            AGIElement.UpdateInfo(member.GetAGI(), member.GetAGI(previewEquipment));
            LUKElement.UpdateInfo(member.GetLUK(), member.GetLUK(previewEquipment));
            ExtraRateUpdateInfo(hitRateElement, member.GetHitRate() * 100f, member.GetHitRate(previewEquipment) * 100f);
            ExtraRateUpdateInfo(evasionRateElement, member.GetEvasionRate() * 100f, member.GetEvasionRate(previewEquipment) * 100f);
            ExtraRateUpdateInfo(critRateElement, member.GetCritRate() * 100f, member.GetCritRate(previewEquipment) * 100f);
            ExtraRateUpdateInfo(targetRateElement, member.GetTargetRate() * 100f, member.GetTargetRate(previewEquipment) * 100f);

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
        public void LoadPrimaryWeapons()
        {
            selectedSlot = EquipmentSlotType.PrimaryWeapon;
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetWeaponsAndAmount(), true);
        }
        public void LoadSecondaryWeapons()
        {
            selectedSlot = EquipmentSlotType.SecondaryWeapon;
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetWeaponsAndAmount(), true);
        }
        public void LoadHeadArmors()
        {
            selectedSlot = EquipmentSlotType.Head;
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetArmorsAndAmount(), true);
        }
        public void LoadBodyArmors()
        {
            selectedSlot = EquipmentSlotType.Body;
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetArmorsAndAmount(), true);
        }
        public void LoadPrimaryAccessoryArmors()
        {
            selectedSlot = EquipmentSlotType.PrimaryAccessory;
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetArmorsAndAmount(), true);
        }
        public void LoadSecondaryAccessoryArmors()
        {
            selectedSlot = EquipmentSlotType.SecondaryAccessory;
            inventoryItemViewer?.LoadItems(PlayerData.instance.GetArmorsAndAmount(), true);
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
        protected void HighlightMember(PartyMember member)
        {
            selectedMember = member;
            UpdateInfo();
        }

        protected void ApplyLabels()
        {
            maxHPElement?.UpdateLabel(TUFFSettings.maxHPShortText);
            maxSPElement?.UpdateLabel(TUFFSettings.maxSPShortText);
            maxTPElement?.UpdateLabel(TUFFSettings.maxTPShortText);
            ATKElement?.UpdateLabel(TUFFSettings.ATKShortText);
            DEFElement?.UpdateLabel(TUFFSettings.DEFShortText);
            SATKElement?.UpdateLabel(TUFFSettings.SATKShortText);
            SDEFElement?.UpdateLabel(TUFFSettings.SDEFShortText);
            AGIElement?.UpdateLabel(TUFFSettings.AGIShortText);
            LUKElement?.UpdateLabel(TUFFSettings.LUKShortText);
        }
    }
}

