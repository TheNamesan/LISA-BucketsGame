using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    public class StatsOverviewHUD : MonoBehaviour
    {
        public RectTransform rectTransform { get => transform as RectTransform; }
        public TMP_Text nameText;
        private Vector2 m_originalSize;
        public float collapsedWidth = 2353;
        [Header("Stats")]
        public StatChangeElement maxHPElement;
        public StatChangeElement maxSPElement;
        public StatChangeElement maxTPElement;
        public StatChangeElement ATKElement;
        public StatChangeElement DEFElement;
        public StatChangeElement SATKElement;
        public StatChangeElement SDEFElement;
        public StatChangeElement AGIElement;
        public StatChangeElement LUKElement;
        [Header("Extra Rates")]
        public StatChangeElement hitRateElement;
        public StatChangeElement evasionRateElement;
        public StatChangeElement critRateElement;
        public StatChangeElement critEvasionRateElement;
        public StatChangeElement targetRateElement;

        private void Awake()
        {
            m_originalSize = rectTransform.sizeDelta;
        }
        private void Update()
        {
            if (UIController.instance.skipButtonHold)
                rectTransform.sizeDelta = new Vector2(collapsedWidth, m_originalSize.y);
            else rectTransform.sizeDelta = m_originalSize;
        }
        public void UpdateInfo(PartyMember member, List<IEquipable> previewEquipment)
        {
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
            ExtraRateUpdateInfo(critEvasionRateElement, member.GetCritEvasionRate() * 100f, member.GetCritEvasionRate(previewEquipment) * 100f);
            ExtraRateUpdateInfo(targetRateElement, member.GetTargetRate() * 100f, member.GetTargetRate(previewEquipment) * 100f);
        }
        public void UpdateLabels()
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

            hitRateElement?.UpdateLabel(TUFFSettings.hitRateShortText);
            evasionRateElement?.UpdateLabel(TUFFSettings.evasionRateShortText);
            critRateElement?.UpdateLabel(TUFFSettings.criticalRateShortText);
            critEvasionRateElement?.UpdateLabel(TUFFSettings.criticalEvasionRateShortText);
            targetRateElement?.UpdateLabel(TUFFSettings.targetRateShortText);
        }
        protected void ExtraRateUpdateInfo(StatChangeElement element, float oldValue, float newValue)
        {
            element.UpdateInfo(oldValue, newValue, "%", "F0");
        }
    }
}
