using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class MemberEquipmentMenu : MonoBehaviour
    {
        public InventoryItemViewer inventoryItemViewer;
        [Header("References")]
        public GeneralInfoDisplay primaryWeaponInfo;
        public GeneralInfoDisplay secondaryWeaponInfo;
        public GeneralInfoDisplay headInfo;
        public GeneralInfoDisplay bodyInfo;
        public GeneralInfoDisplay primaryAccessoryInfo;
        public GeneralInfoDisplay secondaryAccessoryInfo;
        public UIMenu uiMenu;

        [HideInInspector] public PartyMember memberRef;

        public virtual void UpdateInfo(PartyMember member)
        {
            memberRef = member;
            UpdateEquipInfo(primaryWeaponInfo, member.primaryWeapon);
            UpdateEquipInfo(secondaryWeaponInfo, member.secondaryWeapon);
            UpdateEquipInfo(headInfo, member.head);
            UpdateEquipInfo(bodyInfo, member.body);
            UpdateEquipInfo(primaryAccessoryInfo, member.primaryAccessory);
            UpdateEquipInfo(secondaryAccessoryInfo, member.secondaryAccessory);
        }
        protected virtual void UpdateEquipInfo(GeneralInfoDisplay infoDisplay, InventoryItem invItem)
        {
            if (infoDisplay == null) return;
            if (invItem == null) { infoDisplay.DisplayEmpty(); UpdateSlotDescription(infoDisplay, ""); return; }
            infoDisplay.DisplayInfo(invItem.icon, invItem.GetName());
            UpdateSlotDescription(infoDisplay, invItem.GetDescription());
        }
        protected virtual void UpdateSlotDescription(GeneralInfoDisplay infoDisplay, string description)
        {
            if (infoDisplay.uiElement == null) return;
            infoDisplay.uiElement.highlightDisplayText = description;
        }
    }
}

