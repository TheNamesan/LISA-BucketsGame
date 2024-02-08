using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    [RequireComponent(typeof(Image))]
    public class UIImageColorControl : MonoBehaviour
    {
        Image img;
        public Color highlightColor;
        public Color unhighlightColor;

        void Awake()
        {
            GetImg();
        }

        public void ChangeRed(float value)
        {
            GetImg();
            img.color = new Color(value, img.color.g, img.color.b, img.color.a);
        }

        public void ChangeGreen(float value)
        {
            GetImg();
            img.color = new Color(img.color.r, value, img.color.b, img.color.a);
        }

        public void ChangeBlue(float value)
        {
            GetImg();
            img.color = new Color(img.color.r, img.color.g, value, img.color.a);
        }

        public void ChangeAlpha(float value)
        {
            GetImg();
            img.color = new Color(img.color.r, img.color.g, img.color.b, value);
        }

        public void UseHighlightColor()
        {
            GetImg();
            img.color = highlightColor; 
        }

        public void UseUnhighlightColor()
        {
            GetImg();
            img.color = unhighlightColor;
        }
    
        void GetImg()
        {
            if(img == null) img = GetComponent<Image>();
        }
    }
}
