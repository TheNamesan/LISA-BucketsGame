using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class UnitHUD : MonoBehaviour
    {
        public GraphicHandler graphicHandler;
        public RectTransform contentParent;
        [SerializeField] Image unitGraphic;
        [SerializeField] TMP_Text nameText;
        [SerializeField] BarHandler HPBar;
        [SerializeField] BarHandler SPBar;
        [SerializeField] BarHandler TPBar;
        [SerializeField] ActiveStatesHUD activeStatesHUD;
        [SerializeField] Image commandIcon;
        [System.NonSerialized] public PartyMember memberRef;

        public void InitializeUnitHUD()
        {
            activeStatesHUD.InitializeHUD();
        }
        public void SetActive(bool setActive)
        {
            gameObject.SetActive(setActive);
        }
        public virtual void UpdateInfo(PartyMember member, bool displayMaxValues = false)
        {
            if (member == null) return;
            memberRef = member;
            UpdateName(member.GetName(), member.isKOd);
            UpdateGraphic(member.GetGraphic(), member.isKOd);
            UpdateHP(member.GetMaxHP(), member.HP, member.isKOd, displayMaxValues);
            UpdateSP(member.GetMaxSP(), member.SP, member.job.usesSP, displayMaxValues);
            UpdateTP(member.GetMaxTP(), member.TP, member.job.usesTP, displayMaxValues);
            UpdateStates(member.states);
        }
        public void UpdateName(string name, bool isKOd)
        {
            nameText.text = name;
            nameText.color = GetHPTextColor(isKOd);
        }

        protected Color GetHPTextColor(bool isKOd)
        {
            return (isKOd ? TUFFSettings.HPKOColor :
                            (memberRef.GetHPPercentage() <= TUFFSettings.HPDangerThreshold ? TUFFSettings.HPDangerColor : TUFFSettings.HPNormalColor));
        }

        public virtual void UpdateGraphic(Sprite sprite, bool isKOd)
        {
            unitGraphic.sprite = sprite;
            unitGraphic.color = (isKOd ? TUFFSettings.KOGraphicColor : TUFFSettings.aliveGraphicColor);
        }
        public Image GetUnitImage()
        {
            return unitGraphic;
        }
        public void UpdateHP(int maxValue, int value, bool isKOd, bool displayMaxValue)
        {
            HPBar.SetValue(value, maxValue, displayMaxValue: displayMaxValue);
            HPBar.valueText.color = GetHPTextColor(isKOd);
        }
        public void UpdateSP(int maxValue, int value, bool usesSP, bool displayMaxValue)
        {
            if (!usesSP) SPBar.gameObject.SetActive(false);
            else SPBar.gameObject.SetActive(true);
            SPBar.SetValue(value, maxValue, displayMaxValue: displayMaxValue);
        }
        public void UpdateTP(int maxValue, int value, bool usesTP, bool displayMaxValue)
        {
            if (!usesTP) TPBar.gameObject.SetActive(false);
            else TPBar.gameObject.SetActive(true);
            TPBar.SetValue(value, maxValue, displayMaxValue: displayMaxValue);
        }
        public void UpdateStates(List<ActiveState> states)
        {
            activeStatesHUD.UpdateStates(states);
        }
        public void UpdateCommandIcon(Sprite sprite)
        {
            if (sprite == null) commandIcon.color = new Color(commandIcon.color.r, commandIcon.color.g, commandIcon.color.b, 0f);
            else commandIcon.color = new Color(commandIcon.color.r, commandIcon.color.g, commandIcon.color.b, 1f);
            commandIcon.sprite = sprite;
        }
    }
}