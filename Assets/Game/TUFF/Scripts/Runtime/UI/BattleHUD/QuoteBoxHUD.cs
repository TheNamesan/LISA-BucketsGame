using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class QuoteBoxHUD : MonoBehaviour
    {
        public Image graphic;
        public TMP_Text nameText;
        public TMP_Text quoteText;
        public void DisplayQuote(Sprite sprite, string name, string text)
        {
            graphic.sprite = sprite;
            nameText.text = name;
            quoteText.text = text;
        }
    }
}

