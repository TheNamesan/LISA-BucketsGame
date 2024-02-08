using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class PartyEXPReadout : MonoBehaviour
    {
        public ResultsScreenHUD resultsScreenHUD;
        public TMP_Text descriptionText;
        public QuoteBoxHUD quoteBoxHUD;
        public Image magsIcon;
        public TMP_Text magsText;
        public PartyEXPReadoutElement readoutPrefab;
        public UIMenu uiMenu;
        public RectTransform contentParent;

        [Header("EXP Ticks")]
        [Tooltip("Time needed in seconds for Tick SFX to play.")]
        public float tickTimeBuffer = 0.05f;
        protected float tickTime = 0f;

        protected bool animationsDone = false;

        [HideInInspector] public List<PartyEXPReadoutElement> readoutElements = new List<PartyEXPReadoutElement>();
        
        public void InitiateEXPReadout()
        {
            animationsDone = false;
            ResetElements();
            int activePartySize = GameManager.instance.playerData.GetActivePartySize();
            for (int i = 0; i < activePartySize; i++)
            {
                //if (i >= GameManager.instance.playerData.GetActivePartySize()) { break; }
                var element = Instantiate(readoutPrefab, contentParent);
                readoutElements.Add(element);
                element.InitiateAnimation(GameManager.instance.playerData.GetActivePartyMember(i));
            }
            descriptionText.text = TUFFSettings.victoryMessageText;
            UpdateMags();
            SetQuote();
        }
        public void Update()
        {
            if (UIController.instance.actionButtonDown)
            {
                AdvanceReadout();
            }
            TickTimer();
        }
        protected virtual void AdvanceReadout()
        {
            if(animationsDone) //skip to Level Up Overview or Rewards Menu
            {
                resultsScreenHUD.NextMenu();
            }
        }
        protected void TickTimer()
        {
            if (animationsDone) return;
            if (readoutElements.Count == 0) { animationsDone = true; return; }

            bool queueTick = false;
            foreach(PartyEXPReadoutElement p in readoutElements)
            {
                if (UIController.instance.actionButtonDown) p.StopAnimation();
                if (p.animIsFinished) { animationsDone = true; continue; }
                animationsDone = false;
                if (p.animIsRunning) queueTick = true;
                break;
            }
            if(queueTick)
            {
                tickTime -= Time.deltaTime;
                if (tickTime <= 0)
                {
                    AudioManager.instance.PlaySFX(TUFFSettings.EXPTickSFX);
                    tickTime = tickTimeBuffer;
                }
            }
        }
        public virtual void ResetElements()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
            readoutElements.Clear();
        }
        public void UpdateMags()
        {
            if (magsIcon != null) magsIcon.sprite = TUFFSettings.magsIcon;
            if (magsText != null)
            {
                var mags = BattleManager.instance.magsCollected;
                magsText.text = $"{(mags < 0 ? "" : "+")}{LISAUtility.IntToString(mags)}";
                magsText.color = (mags < 0 ? TUFFSettings.negativeColor : TUFFSettings.positiveColor);
            }
        }
        public void SetQuote()
        {
            var member = PlayerData.instance.GetRandomActivePartyMember();
            quoteBoxHUD.DisplayQuote(member.GetGraphic(), member.GetName(), member.GetRandomWinQuote());
        }
    }
}