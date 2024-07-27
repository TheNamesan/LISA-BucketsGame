using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    public class MemberBioHUD : MonoBehaviour
    {
        public TMP_Text descriptionText;

        [Header("Labels")]
        public TMP_Text fightingArtLabel;
        public TMP_Text pastOccupationLabel;
        public TMP_Text likesLabel;
        public TMP_Text favoriteFoodLabel;
        public TMP_Text mostHatedThingLabel;
        
        [Header("Text")]
        public TMP_Text fightingStyleText;
        public TMP_Text pastOccupationText;
        public TMP_Text likesText;
        public TMP_Text favoriteFoodText;
        public TMP_Text mostHatedThingText;

        public void UpdateLabels()
        {
            if (fightingArtLabel) fightingArtLabel.text = TUFFSettings.fightingArtText;
            if (pastOccupationLabel) pastOccupationLabel.text = TUFFSettings.pastOccupationText;
            if (likesLabel) likesLabel.text = TUFFSettings.likesText;
            if (favoriteFoodLabel) favoriteFoodLabel.text = TUFFSettings.favoriteFoodText;
            if (mostHatedThingLabel) mostHatedThingLabel.text = TUFFSettings.mostHatedThingText;
        }
        public void UpdateInfo(PartyMember member)
        {
            if (member == null) return;
            var bio = member.unitRef.bio;
            if (descriptionText) descriptionText.text = member.GetJob().GetDescription();

            if (fightingStyleText) fightingStyleText.text = bio.GetFightingArt();
            if (pastOccupationText) pastOccupationText.text = bio.GetPastOccupation();
            if (likesText) likesText.text = bio.GetLikes();
            if (favoriteFoodText) favoriteFoodText.text = bio.GetFavoriteFood();
            if (mostHatedThingText) mostHatedThingText.text = bio.GetMostHatedThing();
        }
    }
}

