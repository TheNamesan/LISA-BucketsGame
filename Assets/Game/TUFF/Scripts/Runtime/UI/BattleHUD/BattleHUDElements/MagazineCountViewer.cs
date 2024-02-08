using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class MagazineCountViewer : MonoBehaviour
    {
        public Image magsIcon;
        public TMP_Text magsText;
        public void ShowMags(string countText, Sprite magsSprite = null)
        {
            if (magsIcon != null)
            {
                if (magsSprite == null) magsIcon.sprite = TUFFSettings.magsIcon;
                else magsIcon.sprite = magsSprite;
            }
            if (magsText != null) magsText.text = countText;
        }
    }
}

