using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    public class DescriptionDisplayHUD : MonoBehaviour
    {
        public TMP_Text text;
        public void DisplayText(string text)
        {
            this.text.text = text;
        }
    }
}
