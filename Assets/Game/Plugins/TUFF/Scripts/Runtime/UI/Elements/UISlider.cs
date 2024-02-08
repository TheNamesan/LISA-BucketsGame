using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TUFF
{
    [RequireComponent(typeof(BarHandler))]
    public class UISlider : UIElement
    {
        [Header("References")]
        private BarHandler bar;

        [Header("Slider Values")]
        [Tooltip("Minimum fill value of the slider.")]
        public float minValue = 0f;
        [Tooltip("Maximum fill value of the slider.")]
        public float maxValue = 100f;
        [Tooltip("The amount of fill added when pressing left or right.")]
        public float fillIntervals = 1;
        [Tooltip("The amount of fill added when pressing Skip and left or right.")]
        public float skipHoldFillIntervals = 10;
        [Tooltip("The amount of decimals to display in text.")]
        [Range(0, 6)] public int valueDecimals = 0;
        [Tooltip("Current slider fill amount.")]
        [SerializeField] private float m_fillAmount = 0f;
        public UnityEvent<float> onValueChanged;
        public UISlider()
        {
            disableMenuHInput = true;
        }
        public float fillAmount
        {
            get { return m_fillAmount; }
            set { 
                m_fillAmount = value; 
                UpdateBar(); 
            }
        }

        private void Awake()
        {
            bar = GetComponent<BarHandler>();
        }

        public override void OnOpenMenu()
        {
            UpdateBar();
        }

        public override void HorizontalInput(int direction, bool inAutoFire = false)
        {
            if (GameManager.disableUIInput) return;
            if (!m_disabled)
            {
                bool skipHeld = UIController.instance.skipButtonHold;
                int dir = LISAUtility.Sign(direction);
                float amount = ((float)System.Math.Round(fillIntervals, 2)) * dir;
                if (skipHeld) amount = ((float)System.Math.Round(skipHoldFillIntervals, 2)) * LISAUtility.Sign(direction);
                if ((dir < 0 && fillAmount > minValue) ||
                    (dir > 0 && fillAmount < maxValue))
                { 
                    PlaySound(SelectClip());
                    fillAmount = Mathf.Clamp(fillAmount + amount, minValue, maxValue);
                }
            }
            else
            {
                if(!inAutoFire)
                    PlaySound(DisabledClip());
            }
            onHorizontalInput?.Invoke(direction);
        }

        public void UpdateBar()
        {
            if(bar == null) bar = GetComponent<BarHandler>();
            bar.SetValue(fillAmount, maxValue, "F" + valueDecimals.ToString());
            onValueChanged?.Invoke(fillAmount);
        }
    }
}
