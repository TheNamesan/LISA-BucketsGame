using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    public class StatChangeElement : MonoBehaviour
    {
        public TMP_Text labelText;
        public TMP_Text oldValueText;
        public TMP_Text newValueText;

        public void UpdateInfo(int oldValue, int newValue)
        {
            oldValueText.text = LISAUtility.IntToString(oldValue);
            newValueText.text = LISAUtility.IntToString(newValue);
            if (oldValue == newValue) newValueText.color = Color.white;
            else if (oldValue < newValue) newValueText.color = TUFFSettings.positiveColor;
            else if (oldValue > newValue) newValueText.color = TUFFSettings.negativeColor;
        }
        public void UpdateInfo(float oldValue, float newValue, string suffix = "", string format = null)
        {
            oldValueText.text = $"{LISAUtility.FloatToString(oldValue, format)}{suffix}";
            newValueText.text = $"{LISAUtility.FloatToString(newValue, format)}{suffix}";
            if (oldValue == newValue) newValueText.color = Color.white;
            else if (oldValue < newValue) newValueText.color = TUFFSettings.positiveColor;
            else if (oldValue > newValue) newValueText.color = TUFFSettings.negativeColor;
        }
        public void UpdateLabel(string label)
        {
            if (labelText != null) labelText.text = label;
        }
    }
}

