using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class PauseMenuHUD : MonoBehaviour
    {
        [Header("Menus")]
        [SerializeField] protected UIMenu mainPauseMenu;
        [SerializeField] protected UIMenu itemsMenu;
        [SerializeField] protected UIMenu equipMenu;

        [Header("Main Menu")]
        public RectTransform partyReadoutParent;
        public GameObject readoutUnitDisplayPrefab;
        public MagazineCountViewer magazineCountViewer;
        public List<PauseUnitHUD> pauseUnitHUDs = new List<PauseUnitHUD>();

        public SFX pauseSFX = new SFX();

        public virtual void Start()
        {
            SetupMainMenu();
            mainPauseMenu.onCancelMenu.AddListener(ClosePauseMenu);
            mainPauseMenu.onSkipMenu.AddListener(ClosePauseMenu);
        }
        public void InvokePauseMenu()
        {
            GameManager.instance.DisablePlayerInput(true);
            AudioManager.instance.PlaySFX(pauseSFX);
            UpdateMainMenuData();
            mainPauseMenu.OpenMenu();
        }
        public virtual void ClosePauseMenu()
        {
            //Close All Open Menus here (use UIController.skipButtonDown)
            GameManager.instance.DisablePlayerInput(false);
        }
        public void SetupMainMenu()
        {
            foreach(Transform child in partyReadoutParent) { Destroy(child.gameObject); }
            pauseUnitHUDs.Clear();

            for(int i = 0; i < PlayerData.activePartyMaxSize; i++)
            {
                GameObject go = Instantiate(readoutUnitDisplayPrefab, partyReadoutParent);
                var puh = go.GetComponentInChildren<PauseUnitHUD>(true);
                if(puh != null) pauseUnitHUDs.Add(puh);
            }
            UpdateMainMenuData();
        }
        public virtual void UpdateMainMenuData()
        {
            for(int i = 0; i < pauseUnitHUDs.Count; i++)
            {
                if(i >= GameManager.instance.playerData.GetActivePartySize())
                {
                    pauseUnitHUDs[i].gameObject.SetActive(false);
                    continue;
                }
                pauseUnitHUDs[i].gameObject.SetActive(true);
                pauseUnitHUDs[i].UpdateInfo(GameManager.instance.playerData.GetActivePartyMember(i), true);
            }
            if (magazineCountViewer != null)
                magazineCountViewer.ShowMags(LISAUtility.IntToString(PlayerData.instance.mags));
        }
    }
}
