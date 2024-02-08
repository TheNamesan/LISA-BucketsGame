using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace TUFF
{
    public class BarHandler : MonoBehaviour
    {
        public TMP_Text valueText;
        public Image fill;
        public Gradient gradient;

        public void SetValue(float value, float maxValue, string format = null, bool displayMaxValue = false, string postfix = "")
        {
            fill.fillAmount = Mathf.Clamp(value / maxValue, 0f, 1f);
            if (displayMaxValue) valueText.text = $"{LISAUtility.FloatToString(value, format)}{postfix}/{LISAUtility.FloatToString(maxValue, format)}{postfix}" ;
            else valueText.text = $"{LISAUtility.FloatToString(value, format)}{postfix}";
            UpdateFillColor();
        }
        public void SetValue(float fillAmount, string valueText)
        {
            fill.fillAmount = Mathf.Clamp(fillAmount, 0f, 1f);
            this.valueText.text = valueText;
            UpdateFillColor();
        }
        public void UpdateFillColor()
        {
            fill.color = gradient.Evaluate(fill.fillAmount);
        }
    }
}