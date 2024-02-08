using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class UIElementContainer
    {
        public List<UIElement> UIElements = new List<UIElement>();
        public UIElementContainer()
        {
            UIElements = new List<UIElement>();
        }
    }
}
