using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class DetailedUnitHUD : UnitHUD
    {
        public TMP_Text jobNameText;
        public TMP_Text levelText;
        [Header("Stat Displays")]
        public StatDisplayHUD ATKDisplay;
        public StatDisplayHUD DEFDisplay;
        public StatDisplayHUD SATKDisplay;
        public StatDisplayHUD SDEFDisplay;
        public StatDisplayHUD AGIDisplay;
        public StatDisplayHUD LUKDisplay;

        public override void UpdateInfo(PartyMember member, bool displayMaxValues = false)
        {
            base.UpdateInfo(member, displayMaxValues);
            jobNameText.text = member.job.GetName();
            levelText.text = $"{TUFFSettings.levelShortText} {LISAUtility.IntToString(member.level)}";
            ATKDisplay.UpdateInfo(member.GetATK(), Color.white, true, TUFFSettings.ATKShortText);
            DEFDisplay.UpdateInfo(member.GetDEF(), Color.white, true, TUFFSettings.DEFShortText);
            SATKDisplay.UpdateInfo(member.GetSATK(), Color.white, true, TUFFSettings.SATKShortText);
            SDEFDisplay.UpdateInfo(member.GetSDEF(), Color.white, true, TUFFSettings.SDEFShortText);
            AGIDisplay.UpdateInfo(member.GetAGI(), Color.white, true, TUFFSettings.AGIShortText);
            LUKDisplay.UpdateInfo(member.GetLUK(), Color.white, true, TUFFSettings.LUKShortText);
        }
    }
}

