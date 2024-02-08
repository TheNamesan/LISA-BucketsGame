using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class GeneralInfoDisplay : MonoBehaviour
    {
        public Image icon;
        public TMP_Text text;
        public TMP_Text SPCostText;
        public TMP_Text TPCostText;
        public TMP_Text usesText;
        public UIElement uiElement;
        public virtual void DisplayInfo(Sprite iconImage, string text, string SPText = "", string TPText = "", string usesText = "", 
            bool iconActive = true, bool textActive = true, bool SPCostActive = false, bool TPCostActive = false, bool usesTextActive = false)
        {
            icon.sprite = iconImage;
            this.text.text = text;
            SPCostText.text = SPText;
            TPCostText.text = TPText;
            this.usesText.text = usesText;
            icon.gameObject.SetActive(iconActive);
            this.text.gameObject.SetActive(textActive);
            SPCostText.gameObject.SetActive(SPCostActive);
            TPCostText.gameObject.SetActive(TPCostActive);
            this.usesText.gameObject.SetActive(usesTextActive);
            SPCostText.color = TUFFSettings.SPColor;
            TPCostText.color = TUFFSettings.TPColor;
        }
        public void DisplayEmpty()
        {
            DisplayInfo(null, "", iconActive: false, textActive: false);
        }
    }
}

