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
        public GameObject arrowParent;
        public bool hideIfNoChanges = false;

        public void UpdateInfo(int value)
        {
            if (oldValueText) oldValueText.text = LISAUtility.IntToString(value);
            DisplayChanges(value, value);
        }
        public void UpdateInfo(int oldValue, int newValue)
        {
            if (oldValueText) oldValueText.text = LISAUtility.IntToString(oldValue);
            if (newValueText) newValueText.text = LISAUtility.IntToString(newValue);
            DisplayChanges(oldValue, newValue);
        }
        public void UpdateInfo(float oldValue, float newValue, string suffix = "", string format = null)
        {
            if (oldValueText) oldValueText.text = $"{LISAUtility.FloatToString(oldValue, format)}{suffix}";
            if (newValueText) newValueText.text = $"{LISAUtility.FloatToString(newValue, format)}{suffix}";
            DisplayChanges(oldValue, newValue);
        }
        public void UpdateLabel(string label)
        {
            if (labelText != null) labelText.text = label;
        }
        protected void DisplayChanges(float oldValue, float newValue)
        {
            if (oldValue == newValue)
            {
                newValueText.color = Color.white;
                if (hideIfNoChanges)
                {
                    arrowParent?.SetActive(false);
                    if (newValueText) newValueText.gameObject.SetActive(false);
                }
            }
            else
            {
                if (newValueText)
                {
                    if (hideIfNoChanges) newValueText.gameObject.SetActive(true);
                    if (oldValue < newValue) newValueText.color = TUFFSettings.positiveColor;
                    if (oldValue > newValue) newValueText.color = TUFFSettings.negativeColor;
                }
                if (hideIfNoChanges) arrowParent?.SetActive(true);
            }
        }
    }
}

