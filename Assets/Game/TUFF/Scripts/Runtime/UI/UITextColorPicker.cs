using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    [RequireComponent(typeof(TMP_Text))]
    public class UITextColorPicker : MonoBehaviour
    {
        public List<Color> colors = new List<Color>();
        public TMP_Text text;

        public void SetTextColor(int index)
        {
            if (colors.Count <= 0) return;
            if (index < 0 || index >= colors.Count) return;
            if(text == null) text = GetComponent<TMP_Text>();
            if(text != null) text.color = colors[index];
        }
    }
}
