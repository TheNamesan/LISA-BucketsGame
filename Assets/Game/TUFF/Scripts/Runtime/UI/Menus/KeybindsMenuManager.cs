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
        public void SetOverlayActive(bool active)
        {
            rebindOverlayObject?.SetActive(active);
        }
        public void SetOverlayText(string partName, string expectedControlType)
        {
            if (rebindOverlayText)
            {
                var text = !string.IsNullOrEmpty(expectedControlType)
                    ? $"{partName}Waiting for {expectedControlType} input..."
                    : $"{partName}Waiting for input...";
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
