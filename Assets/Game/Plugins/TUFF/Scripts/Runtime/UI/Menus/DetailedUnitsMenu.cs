using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TUFF
{
    public class DetailedUnitsMenu : MonoBehaviour
    {
        public DetailedUnitHUD detailedUnitHUDPrefab;
        public RectTransform contentParent;
        public UIMenu uiMenu;

        public bool clearContentOnAwake = false;
        
        public UnityEvent<UIButton, PartyMember> onButtonCreate = new UnityEvent<UIButton, PartyMember>();
        public UnityEvent<UIButton, PartyMember> onButtonUpdate = new UnityEvent<UIButton, PartyMember>();
        protected List<DetailedUnitHUD> unitHUDs = new List<DetailedUnitHUD>();
        protected List<UIButton> unitButtons = new List<UIButton>();
        protected bool elementAdded = false;

        public void Awake()
        {
            if (clearContentOnAwake) ClearContent();
        }
        public void UpdateUnits()
        {
            CreateNewHUDs(PlayerData.instance.GetAllPartyMembers());
        }
        protected void ClearContent()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
            unitHUDs.Clear();
            unitButtons.Clear();
            uiMenu.UIElementContainers = new UIElementContainer[0];
            Debug.Log("CLEARED CONTENT");
        }
        protected void CreateNewHUDs(List<PartyMember> members)
        {
            if (uiMenu.UIElementContainers == null)
            {
                uiMenu.UIElementContainers = new UIElementContainer[members.Count];
            }
            if (uiMenu.UIElementContainers.Length < members.Count)
            {
                var newArray = new UIElementContainer[members.Count];
                System.Array.Copy(uiMenu.UIElementContainers, newArray, uiMenu.UIElementContainers.Length);
                uiMenu.UIElementContainers = newArray;
            }
            for (int i = 0; i < members.Count; i++)
            {
                if (i >= unitHUDs.Count)
                {
                    CreateNewHUD(members, i);
                }
                else
                {
                    unitHUDs[i].UpdateInfo(members[i], true);
                }
                unitHUDs[i].gameObject.SetActive(true);
                onButtonUpdate?.Invoke(unitButtons[i], members[i]);
            }
            for (int i = members.Count; i < unitHUDs.Count; i++)
            {
                unitHUDs[i].gameObject.SetActive(false);
            }
            if (elementAdded) { 
                uiMenu.SetupUIElements(); 
                elementAdded = false; 
            }
        }

        private void CreateNewHUD(List<PartyMember> members, int i)
        {
            var unitHUD = Instantiate(detailedUnitHUDPrefab, contentParent);
            unitHUD.UpdateInfo(members[i], true);
            unitHUDs.Add(unitHUD);
            UIButton uiButton = unitHUD.GetComponent<UIButton>();
            if (uiButton == null)
            {
                uiButton = unitHUD.gameObject.AddComponent<UIButton>();
            }
            uiButton.onHighlight.AddListener(() => unitHUD.graphicHandler.Highlight(true));
            uiButton.onUnhighlight.AddListener(() => unitHUD.graphicHandler.Highlight(false));
            onButtonCreate?.Invoke(uiButton, members[i]);
            uiMenu.UIElementContainers[i] = new UIElementContainer(); 
            uiMenu.UIElementContainers[i].UIElements.Add(uiButton);
            unitButtons.Add(uiButton);
            elementAdded = true;
        }
    }
}

