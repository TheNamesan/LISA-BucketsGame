using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class SaveFileHUD : MonoBehaviour
    {
        public UIButton fileSelectButton;
        public TMP_Text fileText;
        public List<Image> partyGraphics = new();
        public TMP_Text timeText;
        public int index = -1;

        public void Initialize(int index)
        {
            this.index = index;
        }
        public void UpdateHUD()
        {
            if (index < 0) return;
            if (fileText) fileText.text = $"File {index + 1}";
            var load = PlayerData.LoadData(index);
            UpdateGraphics(load);
            if (timeText)
            { 
                timeText.gameObject.SetActive(load != null);
                if (load != null) timeText.text = load.GetPlaytimeText();
            }
        }

        private void UpdateGraphics(PlayerData load)
        {
            for (int i = 0; i < partyGraphics.Count; i++)
            {
                var graphic = partyGraphics[i];
                if (!graphic) continue;
                Sprite sprite = null;
                if (load != null)
                {
                    var partyMember = load.GetActivePartyMember(i);
                    if (partyMember != null) sprite = partyMember.GetGraphic();
                }
                graphic.sprite = sprite;
                graphic.gameObject.SetActive(sprite != null);
            }
        }
    }

}
