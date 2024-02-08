using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace TUFF
{
    public class UIPicker : UIElement
    {
        [Header("Picker Values")]
        [Tooltip("Options to display from the picker.")]
        public List<string> options = new List<string>();
        [Tooltip("Currently highlighted option.")]
        public int highlightedOption = 0;
        [Tooltip("Returns the option index.")]
        public UnityEvent<int> onValueChanged;

        public UIPicker()
        {
            disableMenuHInput = true;
        }

        protected override void OnEnable()
        {
            UpdateText();
        }

        public override void OnOpenMenu()
        {
            UpdateText();
        }

        public override void HorizontalInput(int direction, bool inAutoFire = false)
        {
            if (GameManager.disableUIInput) return;
            if (options.Count > 1)
            {
                if (!m_disabled)
                {
                    int dir = LISAUtility.Sign(direction);
                    highlightedOption += dir;
                    if (highlightedOption >= options.Count) highlightedOption = 0;
                    else if (highlightedOption < 0) highlightedOption = options.Count - 1;
                    PlaySound(SelectClip());
                    UpdateText();
                    onValueChanged.Invoke(highlightedOption);
                }
            }
            else
            {
                if (!inAutoFire)
                    PlaySound(DisabledClip());
            }
            onHorizontalInput?.Invoke(direction);
        }

        public void UpdateText()
        {
            if(text == null) return;
            if (options.Count <= 0) return;
            text.text = options[highlightedOption];
        }
    }
}
