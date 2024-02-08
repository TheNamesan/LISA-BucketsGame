using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    [ExecuteInEditMode]
    public class AdjustToPreferredTextSize : MonoBehaviour
    {
        public TMP_Text tmpText;
        public RectTransform rt;

        public float minWidth = 0;
        public float minHeight = 0;

        public float widthPadding = 0;
        public float heightPadding = 0;

        private void OnEnable()
        {
            Adjust();
        }
        private void Update()
        {
            Adjust();   
        }

        public void Adjust()
        {
            if (tmpText == null || rt == null) return;
            if (!tmpText.enabled) return;
            if (tmpText.text.Length <= 0) return;
            Vector2 prefferedSize = tmpText.GetPreferredValues(tmpText.text, float.PositiveInfinity, float.PositiveInfinity);
            float sizeX = Mathf.Max(minWidth, prefferedSize.x) + widthPadding;
            float sizeY = Mathf.Max(minHeight, prefferedSize.y) + heightPadding;
            rt.sizeDelta = new Vector2(sizeX, sizeY);
        }
    }
}

