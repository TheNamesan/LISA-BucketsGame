using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace TUFF
{
    public class KeybindsMenuManager : MonoBehaviour
    {
        public GameObject rebindOverlayObject;
        public TMP_Text rebindOverlayText;
        public InputActionAsset inputActionAsset;
        private List<KeybindUIHandler> m_handlers = new();
        public float cancelMaxSeconds = 5f;
        private float m_cancelTime = 0;
        //private string m_partName = "";
        private string m_keybindName = "";
        private InputActionRebindingExtensions.RebindingOperation m_targetOperation;

        private void Update()
        {
            Timer();
            UpdateText();
        }
        public void SetOverlayActive(bool active)
        {
            rebindOverlayObject?.SetActive(active);
        }
        public void SetOverlayText(InputActionRebindingExtensions.RebindingOperation targetOperation, string expectedControlType)
        {
            m_cancelTime = cancelMaxSeconds;
            //m_partName = partName;
            m_keybindName = expectedControlType;
            m_targetOperation = targetOperation;
            UpdateText();
        }
        private void UpdateText()
        {
            if (rebindOverlayText)
            {
                var text = !string.IsNullOrEmpty(m_keybindName)
                    ? $"Waiting for {m_keybindName} input..."
                    : $"Waiting for input...";
                text += $"\nAborting in {Mathf.Ceil(m_cancelTime).ToString("F0")}...";
                rebindOverlayText.text = text;
            }
        }

        public void ResetAll()
        {
            if (!inputActionAsset) return;
            foreach(KeybindUIHandler handler in m_handlers)
            {
                handler.ResetToDefault(false);
            }
            SaveConfig();
            Debug.Log("All bindings reset!");
        }
        private void Timer()
        {
            if (m_cancelTime > 0)
            {
                m_cancelTime -= Time.unscaledDeltaTime;
                if (m_cancelTime <= 0)
                {
                    if (m_targetOperation != null)
                        m_targetOperation.Cancel();
                    m_cancelTime = 0;
                }
            }
        }
        public void AddHandler(KeybindUIHandler handler)
        {
            if (m_handlers.Contains(handler)) return;
            m_handlers.Add(handler);
        }
        public void SaveConfig()
        {
            GameManager.instance.configData.keybinds = GameManager.instance.inputActionAsset.SaveBindingOverridesAsJson();
            GameManager.instance.configData.SaveData();
        }
    }
}
