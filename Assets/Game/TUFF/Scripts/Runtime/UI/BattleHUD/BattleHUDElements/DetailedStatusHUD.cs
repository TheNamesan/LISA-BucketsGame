using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    public class DetailedStatusHUD : MonoBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text hpText;
        public TMP_Text stateText;
        public ActiveStatesHUDElement activeStateHUDElement;
        public TMP_Text detailedText;
        public void UpdateStatus(ActiveState activeState)
        {
            if (activeState == null) { return; }
            if (activeState.state == null) { Debug.LogWarning("ActiveState has no State!"); return; }
            if (activeState.user == null) { Debug.LogWarning("ActiveState has no user!"); return; }
            var user = activeState.user;
            string HP = $"{(user.CanShowStatus() ? user.HP : "???")}";
            string maxHP = $"{(user.CanShowStatus() ? user.GetMaxHP() : "???")}";
            nameText.text = $"{user.GetName()}";
            hpText.text = $"{TUFFSettings.HPShortText}: {HP}/{maxHP}";
            stateText.text = $"{activeState.state.GetName()}";
            activeStateHUDElement?.UpdateStateInfo(activeState);
            detailedText.text = GetDetailedText(activeState.state);
        }
        public string GetDetailedText(State state)
        {
            if (!state) return "ERROR";
            string text = "";
            for (int i = 0; i < state.features.Count; i++)
            {
                bool valid = false;
                var feat = state.features[i];
                string nextText = GetNextText(ref valid, feat);
                text += nextText;
                if (valid && i + 1 < state.features.Count)
                    text += "\n";
            }
            return text;
        }

        private string GetNextText(ref bool valid, Feature feat)
        {
            var featType = feat.featureType;
            string nextText = "";
            float value = 0;
            switch (featType)
            {
                case FeatureType.StatChange:
                    nextText += $"- {TUFFFeatureToText.GetStatChangeText(feat.statChange)} ";
                    value = LISAUtility.PercentageToValue(feat.statChangeValue);
                    nextText += $"x{GetValue(value)}";
                    valid = true;
                    break;
                case FeatureType.ExtraRateChange:
                    nextText += $"- {feat.extraRateChange} "; // Change later
                    value = feat.extraRateChangeValue;
                    nextText += $"{(value >= 0 ? "+" : "")}{GetValue(value)}%";
                    valid = true;
                    break;
                case FeatureType.SpecialRateChange:
                    nextText += $"- {feat.specialRateChange}"; // Change later
                    value = feat.specialRateChangeValue;
                    nextText += $"x{GetValue(value)}%";
                    valid = true;
                    break;
                case FeatureType.ElementPotency:
                    nextText += $"- {feat.featureType}"; // Change later
                    nextText += $" - {TUFFSettings.elements[feat.element].GetName()} ";
                    value = LISAUtility.PercentageToValue(feat.elementValue);
                    nextText += $"x{GetValue(value)}";
                    valid = true;
                    break;
                case FeatureType.ElementVulnerability:
                    nextText += $"- {feat.featureType}"; // Change later
                    nextText += $" - {TUFFSettings.elements[feat.element].GetName()} ";
                    value = LISAUtility.PercentageToValue(feat.elementValue);
                    nextText += $"x{GetValue(value)}";
                    valid = true;
                    break;
                case FeatureType.StatePotency:
                    nextText += $"- {feat.featureType}"; // Change later
                    State targetState = feat.state;
                    if (targetState == null) { nextText = ""; break; }
                    nextText += $" - {targetState.GetName()} ";
                    value = LISAUtility.PercentageToValue(feat.stateValue);
                    nextText += $"x{GetValue(value)}";
                    valid = true;
                    break;
                case FeatureType.StateVulnerability:
                    nextText += $"- {feat.featureType}"; // Change later
                    targetState = feat.state;
                    if (targetState == null) { nextText = ""; break; }
                    nextText += $" - {targetState.GetName()} ";
                    value = LISAUtility.PercentageToValue(feat.stateValue);
                    nextText += $"x{GetValue(value)}";
                    valid = true;
                    break;
                case FeatureType.StateImmunity:
                    nextText += $"- {feat.featureType}"; // Change later
                    targetState = feat.state;
                    if (targetState == null) { nextText = ""; break; }
                    nextText += $" - {targetState.GetName()} ";
                    valid = true;
                    break;
                // Add Hit Element
                // Add State Element
                case FeatureType.AttackSpeed:
                    nextText += $"- {feat.featureType} "; // Change later
                    value = feat.attackSpeed;
                    nextText += $"{(value >= 0 ? "+" : "")}{GetValue(value)}";
                    valid = true;
                    break;
                //case FeatureType.AttackTimes:
                //    valid = true;
                //    break;
            }
            return nextText;
        }

        private string GetValue(float value)
        {
            return LISAUtility.FloatToString(value);
        }
    }
}
