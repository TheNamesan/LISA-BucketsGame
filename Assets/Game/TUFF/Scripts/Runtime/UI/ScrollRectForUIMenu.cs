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
        private RectTransform content { get { if (!scroll) return null; return scroll.content; } }
        private float scrollValue { get { return UIElementsHeight + layoutSpaceValue; } } 

        [Header("Debug")]
        [SerializeField] int topVisibleIndex;
        [SerializeField] int bottomVisibleIndex;

        private bool m_initialized = false;
        private void Awake()
        {
            Initialize();
        }
        private void Initialize()
        {
            if (m_initialized) return;
            scroll = GetComponent<ScrollRect>();
            m_initialized = true;
        }
        private void OnEnable()
        {
            UpdateScroll();
        }
        public void UpdateScroll()
        {
            Debug.Log("UpdateScroll", this);
            Initialize();
            int visibleColumns = uiMenu.GetVisibleColumnsCount();
            //Debug.Log("Visible columns: " + visibleColumns);
            int cursorValue = uiMenu.highlightY;
            if (visibleColumns > 0)
            {
                // If first element on menu on highlighted
                if (cursorValue == 0)
                {
                    topVisibleIndex = uiMenu.highlightY;
                    bottomVisibleIndex = maxVisibleElements - 1;
                    SetScroll(0);
                }
                // If last element on menu on highlighted
                else if (cursorValue == Mathf.Max(maxVisibleElements, visibleColumns - 1))
                {
                    topVisibleIndex = uiMenu.highlightY - maxVisibleElements + 1;
                    bottomVisibleIndex = uiMenu.highlightY;
                    SetScroll((visibleColumns - maxVisibleElements) * scrollValue);
                }
                else if (cursorValue > bottomVisibleIndex)
                {
                    int valueDiff = cursorValue - bottomVisibleIndex;
                    topVisibleIndex += valueDiff;
                    bottomVisibleIndex += valueDiff;
                    ScrollDown(valueDiff);
                }
                else if (cursorValue < topVisibleIndex)
                {
                    int valueDiff = Mathf.Abs(cursorValue - topVisibleIndex);
                    topVisibleIndex -= valueDiff;
                    bottomVisibleIndex -= valueDiff;
                    ScrollUp(valueDiff);
                }
            }
            UpdateArrows();
        }
        
        private void ScrollUp(float times = 1)
        {
            content.anchoredPosition -= new Vector2(0, scrollValue) * times;
        }

        private void ScrollDown(float times = 1)
        {
            content.anchoredPosition += new Vector2(0, scrollValue) * times;
        }
        void SetScroll(float value)
        {
            if (!content) { Debug.LogWarning("No content!", this); return; }
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, value);
        }
        public void UpdateArrows()
        {
            if (!UIMenu.CheckIfUIMenuHasElements(uiMenu))
            {
                if (upArrow != null) upArrow?.SetActive(false);
                if (downArrow != null) downArrow?.SetActive(false);
                return; 
            }
            int visibleColumns = uiMenu.GetVisibleColumnsCount();
            if (visibleColumns < maxVisibleElements)
            {
                if (upArrow != null) upArrow?.SetActive(false);
                if (downArrow != null) downArrow?.SetActive(false);
                return;
            }
            if (upArrow != null) upArrow?.SetActive(topVisibleIndex > 0);
            if (downArrow != null) downArrow?.SetActive(bottomVisibleIndex < visibleColumns - 1);
        }
    }
}
