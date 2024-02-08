using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    public class LayoutFix : MonoBehaviour
    {
        private void Start()
        {
            ForceRebuild();
        }
        void OnEnable()
        {
            ForceRebuild();
        }
        void ForceRebuild()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
}
