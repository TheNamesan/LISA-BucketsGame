using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class PauseUnitHUD : UnitHUD
    {
        public Image portraitImage;
        public TMP_Text jobNameText;
        public TMP_Text levelText;
        public override void UpdateInfo(PartyMember member, bool displayMaxValues = false)
        {
            base.UpdateInfo(member, displayMaxValues);
            portraitImage.sprite = member.GetPortraitSprite();
            jobNameText.text = member.job.GetName();
            levelText.text = $"{TUFFSettings.levelShortText} {LISAUtility.IntToString(member.level)}";
        }
    }
}

