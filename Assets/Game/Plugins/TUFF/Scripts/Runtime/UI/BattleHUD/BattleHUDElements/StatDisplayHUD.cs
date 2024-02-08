using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    public class StatDisplayHUD : MonoBehaviour
    {
        public TMP_Text labelText;
        public TMP_Text valueText;

        public void UpdateInfo(int value, Color valueColor, bool updateLabel = false, string label = "")
        {
            if (valueText != null) {
                valueText.text = LISAUtility.IntToString(value); 
                valueText.color = valueColor;
            }
            if (labelText != null && updateLabel)
            {
                labelText.text = label;
            }
        }
    }
}

