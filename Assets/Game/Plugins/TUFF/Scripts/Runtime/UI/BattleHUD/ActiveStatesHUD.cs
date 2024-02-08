using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    public class ActiveStatesHUD : MonoBehaviour
    {
        [Header("References")]
        public RectTransform viewport;
        public ActiveStatesHUDElement elementPrefab;

        RectTransform rect;
        [SerializeField] protected List<ActiveStatesHUDElement> elements = new List<ActiveStatesHUDElement>();
        [SerializeField] protected List<ActiveStatesHUDElement> elementsClones = new List<ActiveStatesHUDElement>();
        private Vector2 orgPosition;
        public float scrollSpeed = 80f;
        public int maxVisibleStates = 5;
        
        private bool enableScroll = false;
        public bool initialized { get { return m_initialized; } }
        protected bool m_initialized = false;
        void Awake()
        {
            rect = GetComponent<RectTransform>();
            orgPosition = viewport.anchoredPosition;
        }
        public void InitializeHUD()
        {
            ResetChildren();
            ResetElements();
            ResetClones();
            m_initialized = true;
        }

        public void UpdateStates(List<ActiveState> activeStates)
        {
            if (!initialized) InitializeHUD();
            if(rect == null) rect = GetComponent<RectTransform>();
            orgPosition = viewport.anchoredPosition;

            for (int i = 0; i < elements.Count; i++)
            {
                var existElementIdx = FindExistingState(activeStates, elements[i].activeState.state); 
                if (existElementIdx < 0) //if exists in elements but not in activestates: remove
                {
                    RemoveElement(i);
                    i--;
                }
            }
            for (int i = 0; i < activeStates.Count; i++)
            {
                var existingIdx = FindExistingElement(activeStates[i]);
                if (existingIdx >= 0) UpdateElement(activeStates[i], i); //if exists in both: update
                else CreateElement(activeStates, i); //if exists in activestates but not in elements: create
        }

            if (activeStates.Count > maxVisibleStates)
            {
                enableScroll = true;
                for (int i = 0; i < elementsClones.Count; i++)
                {
                    elementsClones[i].gameObject.SetActive(true);
                }
            }
            else
            {
                enableScroll = false;
                for (int i = 0; i < elementsClones.Count; i++)
                {
                    elementsClones[i].gameObject.SetActive(false);
                }
            } 
        }
        protected void CreateElement(List<ActiveState> activeStates, int index)
        {
            var elementGO = Instantiate(elementPrefab.gameObject, transform);
            var element = elementGO.GetComponent<ActiveStatesHUDElement>();
            element.UpdateStateInfo(activeStates[index]);
            elements.Add(element);
            var elementGOCopy = Instantiate(elementGO, transform);
            elementsClones.Add(elementGOCopy.GetComponent<ActiveStatesHUDElement>());
            elementGOCopy.SetActive(false);
            elementGOCopy.transform.SetSiblingIndex(elements.Count - 1);
        }
        protected void UpdateElement(ActiveState activeState, int index)
        {
            elements[index].UpdateStateInfo(activeState);
            elementsClones[index].UpdateStateInfo(activeState);
        }
        protected void RemoveElement(int index)
        {
            Destroy(elements[index].gameObject);
            Destroy(elementsClones[index].gameObject);
            elements.RemoveAt(index);
            elementsClones.RemoveAt(index);
        }
        protected int FindExistingElement(ActiveState activeState)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].activeState.state == activeState.state) return i;
            }
            return -1;
        }
        protected virtual int FindExistingState(List<ActiveState> states, State state)
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].state == state) return i;
            }
            return -1;
        }
        public void ResetChildren()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        protected void ResetElements()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                Destroy(elements[i].gameObject);
            }
            if (elements != null) elements.Clear();
            else elements = new List<ActiveStatesHUDElement>();
        }
        protected void ResetClones()
        {
            for(int i = 0; i < elementsClones.Count; i++)
            {
                Destroy(elementsClones[i].gameObject);
            }
            if(elementsClones != null) elementsClones.Clear();
            else elementsClones = new List<ActiveStatesHUDElement>();
        }
        protected void Update()
        {
            if (enableScroll)
            {
                orgPosition = viewport.anchoredPosition;

                rect.anchoredPosition =
                    new Vector2(rect.anchoredPosition.x - scrollSpeed * Time.deltaTime,
                    rect.anchoredPosition.y);

                float maxScrollX = orgPosition.x - LayoutUtility.GetPreferredWidth(rect) * 0.5f;
                rect.anchoredPosition =
                    new Vector2(
                        Mathf.Clamp(rect.anchoredPosition.x, maxScrollX,
                        orgPosition.x + rect.sizeDelta.x),
                        rect.anchoredPosition.y
                        );

                if (rect.anchoredPosition.x == maxScrollX)
                {
                    rect.anchoredPosition = orgPosition;
                }
            }
            else rect.anchoredPosition = orgPosition;
        }
    }
}
