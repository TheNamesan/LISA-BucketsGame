using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectForUIMenu : MonoBehaviour
    {
        public UIMenu uiMenu;
        public GameObject upArrow;
        public GameObject downArrow;
        public int maxVisibleElements;
        public float UIElementsHeight;
        public float layoutSpaceValue;
        ScrollRect scroll;
        RectTransform content;
        float scrollValue;

        [Header("Debug")]
        [SerializeField] int topVisibleIndex;
        [SerializeField] int bottomVisibleIndex;
        private void Awake()
        {
            scroll = GetComponent<ScrollRect>();
            content = scroll.content;
        }
        private void OnEnable()
        {
            UpdateScroll();
            //int cursorValue = uiMenu.highlightY;
            //SetScroll(cursorValue);
        }
        void Start()
        {
            SetupScroll();
        }

        public void SetupScroll()
        {
            scrollValue = UIElementsHeight + layoutSpaceValue;
            topVisibleIndex = uiMenu.highlightY;
            if(UIMenu.CheckIfUIMenuHasElements(uiMenu))
            {
                if (uiMenu.UIElements.Length < maxVisibleElements) bottomVisibleIndex = uiMenu.UIElements.Length - 1;
                else bottomVisibleIndex = maxVisibleElements - 1;

                for (int i = 0; i < uiMenu.UIElements.Length; i++)
                {
                    for(int j = 0; j < uiMenu.UIElements[i].Length; j++)
                    {
                        uiMenu.UIElements[i][j].onHighlight.AddListener(UpdateScroll);
                    }
                }
            }
        }

        public void UpdateScroll()
        {
            int cursorValue = uiMenu.highlightY;
            if (cursorValue == 0)
            {
                topVisibleIndex = uiMenu.highlightY;
                bottomVisibleIndex = maxVisibleElements - 1;
                SetScroll(0);
            }
            else if (cursorValue == Mathf.Max(maxVisibleElements, uiMenu.UIElements.Length - 1))
            {
                topVisibleIndex = uiMenu.highlightY - maxVisibleElements + 1;
                bottomVisibleIndex = uiMenu.highlightY;
                SetScroll((uiMenu.UIElements.Length - maxVisibleElements) * scrollValue);
            }
            else if (cursorValue > bottomVisibleIndex)
            {
                topVisibleIndex++;
                bottomVisibleIndex++;
                ScrollDown();
            }
            else if (cursorValue < topVisibleIndex)
            {
                topVisibleIndex--;
                bottomVisibleIndex--;
                ScrollUp();
            }
            UpdateArrows();
        }
        
        void ScrollUp()
        {
            content.anchoredPosition -= new Vector2(0, scrollValue);
        }

        void ScrollDown()
        {
            content.anchoredPosition += new Vector2(0, scrollValue);
        }
        void SetScroll(float value)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, value);
        }
        public void UpdateArrows()
        {
            if (!UIMenu.CheckIfUIMenuHasElements(uiMenu)) return;
            if (uiMenu.UIElements.Length < maxVisibleElements)
            {
                if (upArrow != null) upArrow?.SetActive(false);
                if (downArrow != null) downArrow?.SetActive(false);
                return;
            }
            if (upArrow != null) upArrow?.SetActive(topVisibleIndex > 0);
            if (downArrow != null) downArrow?.SetActive(bottomVisibleIndex < uiMenu.UIElements.Length - 1);
        }
    }
}
