using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class StatusHUD : MonoBehaviour
    {
        public UIMenu uiMenu;
        public DetailedStatusHUD detailedStatusHUD;
        public StatusHUDElement elementPrefab;
        public StatusHUDElement noneElement;
        public Transform elementsParent;
        public int initialElements = 5;
        public bool hideNone = false;
        
        protected List<StatusHUDElement> elements = new();
        protected bool initialized = false;

        public void Awake()
        {
            Initialize();
        }
        public void OnEnable()
        {
            if (detailedStatusHUD) uiMenu.onCloseMenu.AddListener(HideDetailedHUD);
        }
        public void OnDisable()
        {
            if (detailedStatusHUD) uiMenu.onCloseMenu.RemoveListener(HideDetailedHUD);
        }
        protected void Initialize()
        {
            if (initialized) return;
            VerifyMenuArrays(initialElements + NoneOffset());
            int index = 0;
            if (noneElement)
            {
                AddToMenu(0, noneElement);
                if (hideNone) noneElement.gameObject.SetActive(false);
                //noneElement.uiButton.onHighlight.AddListener(UpdateNoneDetailedHUD); // Update Detailed Status HUD
            }
            foreach (Transform child in elementsParent) // Add existing GameObjects to list
            {
                if (child.TryGetComponent(out StatusHUDElement existing))
                {
                    elements.Add(existing);
                    AddToMenu(index + NoneOffset(), existing);
                }
                index++;
            }

            if (elements.Count < initialElements)
                for (int i = elements.Count; i < initialElements; i++)
                {
                    CreateNewElement(i + NoneOffset()); // Plus 1 to make space for none element
                }
            HideAll();
            SetupUIMenu();
            initialized = true;
        }

        protected void AddToMenu(int index, StatusHUDElement element)
        {
            if (uiMenu == null) return;
            if (index >= uiMenu.UIElementContainers.Length)
            {
                VerifyMenuArrays(index + NoneOffset());
            }
            Debug.Log($"{uiMenu.UIElementContainers.Length} VS {index}");
            if (uiMenu.UIElementContainers[index] == null)
                    uiMenu.UIElementContainers[index] = new UIElementContainer();
            uiMenu.UIElementContainers[index].UIElements.Add(element.uiButton);
            element.uiButton.onHighlight.AddListener(UpdateDetailedHUD);
        }

        protected void HideAll()
        {
            for (int i = 0; i < elements.Count; i++) // Hide leftover elements
            {
                elements[i].gameObject.SetActive(false);
            }
        }

        public void UpdateStatus(Targetable targetable)
        {
            Initialize();
            if (targetable == null) return;
            if (targetable.states.Count <= 0)
            {
                HideAll();
                if (!hideNone) noneElement.gameObject.SetActive(true); 
            }
            else
            {
                noneElement.gameObject.SetActive(false);
                UpdateElements(targetable.states);
            }
        }
        protected void UpdateElements(List<ActiveState> activeStates)
        {
            VerifyMenuArrays(activeStates.Count);
            int i;
            for (i = 0; i < activeStates.Count; i++) // Assign Data to GameObjects
            {
                StatusHUDElement curElement;
                if (i >= elements.Count) // If index is out of bounds
                {
                    CreateNewElement(i + NoneOffset()); // Plus 1 to make space for none element
                }
                curElement = elements[i];
                curElement.AssignData(activeStates[i]);
                curElement.gameObject.SetActive(true);
            }
            for (int j = i; i < elements.Count; i++) // Hide leftover elements
            {
                elements[j].gameObject.SetActive(false);
            }
            SetupUIMenu();
        }

        protected void CreateNewElement(int index)
        {
            // Add new element gameObject
            var newElement = Instantiate(elementPrefab, elementsParent);
            elements.Add(newElement);
            AddToMenu(index, newElement);
            Debug.LogError("Stop", this);
            
        }
        private void VerifyMenuArrays(int rows)
        {
            if (uiMenu == null) return;
            if (uiMenu.UIElementContainers == null)
            {
                uiMenu.UIElementContainers = new UIElementContainer[rows];
            }
            if (rows >= uiMenu.UIElementContainers.Length) // If row is out of row count
            {
                var newArray = new UIElementContainer[rows];
                System.Array.Copy(uiMenu.UIElementContainers, newArray, uiMenu.UIElementContainers.Length);
                uiMenu.UIElementContainers = newArray;
            }
            if (uiMenu.UIElementContainers[0] == null) // Check Index 0
                uiMenu.UIElementContainers[0] = new UIElementContainer();
        }
        protected void SetupUIMenu()
        {
            if (uiMenu == null) return;
            uiMenu.SetupUIElements();
            //scrollRect.SetupScroll();
        }
        protected void UpdateDetailedHUD()
        {
            int index = uiMenu.highlightY - NoneOffset();
            Debug.Log("updating to: " + index);
            if (index < 0) { UpdateNoneDetailedHUD(); return; }
            ToggleDetailedHUD(true);
            detailedStatusHUD.UpdateStatus(elements[index].activeState);
        }
        protected void UpdateNoneDetailedHUD()
        {
            ToggleDetailedHUD(false);
            detailedStatusHUD.UpdateStatus(null);
        }
        protected int NoneOffset()
        {
            if (noneElement != null) return 1;
            return 0;
        }
        protected void ToggleDetailedHUD(bool enable)
        {
            detailedStatusHUD.gameObject.SetActive(enable);
        }
        protected void HideDetailedHUD()
        {
            ToggleDetailedHUD(false);
        }
    }
}
