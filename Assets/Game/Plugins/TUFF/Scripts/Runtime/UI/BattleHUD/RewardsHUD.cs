using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    public class RewardsHUD : MonoBehaviour
    {
        public ResultsScreenHUD resultsScreenHUD;
        public QuoteBoxHUD quoteBoxHUD;
        public InventoryItemViewer rewardsItemViewer;
        public void Initiate()
        {
            rewardsItemViewer.LoadItems(BattleManager.instance.rewards);
            SetQuote();
        }

        public void Update()
        {
            if (UIController.instance.actionButtonDown)
            {
                resultsScreenHUD.NextMenu();
            }
        }
        public void SetQuote()
        {
            var member = PlayerData.instance.GetRandomActivePartyMember();
            quoteBoxHUD.DisplayQuote(member.GetGraphic(), member.GetName(), member.GetRandomDropsQuote());
        }
    }
}

