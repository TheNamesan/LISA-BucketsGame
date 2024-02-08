using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TUFF
{
    public class InventoryItemViewer : MonoBehaviour
    {
        public GeneralInfoDisplay generalInfoDisplayPrefab;
        public UIMenu uiMenu;
        public ScrollRectForUIMenu scrollRect;
        public RectTransform contentParent;
        public int columns = 3;
        public string itemPrefix = "x";
        public bool clearContentFirstLoad = true;

        public UnityEvent<GeneralInfoDisplay, UIButton, KeyValuePair<InventoryItem, int>> onElementCreate = new UnityEvent<GeneralInfoDisplay, UIButton, KeyValuePair<InventoryItem, int>>();
        public UnityEvent<GeneralInfoDisplay, UIButton, KeyValuePair<InventoryItem, int>> onElementUpdate = new UnityEvent<GeneralInfoDisplay, UIButton, KeyValuePair<InventoryItem, int>>();

        public Dictionary<InventoryItem, int> currentDictionary = new Dictionary<InventoryItem, int>();
        public Dictionary<UIButton, InventoryItem> infoButtons = new Dictionary<UIButton, InventoryItem>();
        protected List<GeneralInfoDisplay> infoDisplays = new List<GeneralInfoDisplay>();
        protected List<UIButton> m_buttons = new List<UIButton>();
        protected bool elementAdded = false;

        protected bool firstLoadComplete = false;

        public void LoadItems(Dictionary<InventoryItem, int> items)
        {
            LoadItems(items, false);
        }
        public void LoadItems(Dictionary<InventoryItem, int> items, bool includeEmpty)
        {
            if (items == null) return;
            if (clearContentFirstLoad && !firstLoadComplete) ResetContent();
            firstLoadComplete = true;

            currentDictionary = items;

            if (columns <= 0) columns = 1;

            int totalElements = items.Keys.Count; // All items
            UpdateContent(items, totalElements, includeEmpty);

            if (elementAdded) SetupElements();
        }
        
        
        private void SetupElements()
        {
            Debug.Log("Setup Elements");
            uiMenu.SetupUIElements();
            scrollRect.SetupScroll();
            elementAdded = false;
        }
        private void UpdateContent(Dictionary<InventoryItem, int> items, int totalElements, bool includeEmpty)
        {
            int cols = 0;
            int row = 0;
            int index = 0;

            int count = totalElements + (includeEmpty ? 1 : 0);
            if (totalElements <= 0) count = 1; // Add space for empty
            int rows = LISAUtility.Ceil(count / ((float)columns));

            infoButtons.Clear();
            Debug.Log("Rows: " + rows);
            VerifyMenuArrays(rows);
            if (uiMenu.UIElementContainers[row] == null) // Check Index 0
                uiMenu.UIElementContainers[row] = new UIElementContainer();

            if (totalElements > 0) // If has elements
            {
                foreach (KeyValuePair<InventoryItem, int> pair in items)
                {
                    if (cols >= columns)
                    {
                        row++;
                        if (uiMenu.UIElementContainers[row] == null)
                            uiMenu.UIElementContainers[row] = new UIElementContainer();
                        cols = 0;
                    }

                    var invItem = pair.Key;
                    int amount = pair.Value;

                    if (index >= infoDisplays.Count) // Create
                    {
                        CreateNewElement($"{cols}, {row}", row, pair);
                    }
                    else // Only Update
                    {
                        UpdateInfoDisplay(infoDisplays[index], pair);
                        m_buttons[index].highlightDisplayText = invItem.GetDescription();
                    }
                    infoDisplays[index].gameObject.SetActive(true);
                    if (!infoButtons.ContainsKey(m_buttons[index]))
                        infoButtons.Add(m_buttons[index], invItem);

                    onElementUpdate?.Invoke(infoDisplays[index], m_buttons[index], pair);

                    cols++;
                    index++;
                }
                if (includeEmpty)
                {
                    if (cols >= columns)
                    {
                        row++;
                        if (uiMenu.UIElementContainers[row] == null)
                            uiMenu.UIElementContainers[row] = new UIElementContainer();
                        cols = 0;
                    }
                    PutEmpty(cols, row, index, new KeyValuePair<InventoryItem, int>(null, 0));
                    index++;
                }
            }
            else // If empty 
            {
                PutEmpty();
                index++;
            }

            for (int i = index; i < infoDisplays.Count; i++)
                infoDisplays[i].gameObject.SetActive(false);
            CheckIfCurrentHighlightIsValid();
        }

        private void CheckIfCurrentHighlightIsValid()
        {
            if (uiMenu.IsOpen)
            {
                var curHighlight = uiMenu.GetCurrentHighlight();
                if (curHighlight != null)
                {
                    if (!curHighlight.IsActiveInHierarchy())
                    {
                        uiMenu.HighlightLastValidElement();
                    }
                }
            }
        }

        private void VerifyMenuArrays(int rows)
        {
            if (uiMenu.UIElementContainers == null)
            {
                uiMenu.UIElementContainers = new UIElementContainer[rows];
            }
            if (uiMenu.UIElementContainers.Length < rows)
            {
                var newArray = new UIElementContainer[rows];
                System.Array.Copy(uiMenu.UIElementContainers, newArray, uiMenu.UIElementContainers.Length);
                uiMenu.UIElementContainers = newArray;
            }
        }

        private void CreateNewElement(string name, int row, KeyValuePair<InventoryItem, int> pair)
        {
            var infoDisplay = InstantiateElement(name);
            var invItem = pair.Key;
            UpdateInfoDisplay(infoDisplay, pair);
            infoDisplays.Add(infoDisplay);
            var button = infoDisplay.GetComponent<UIButton>();
            if (invItem != null) button.highlightDisplayText = invItem.GetDescription();
            else button.highlightDisplayText = "";
            uiMenu.UIElementContainers[row].UIElements.Add(button);
            m_buttons.Add(button);
            onElementCreate?.Invoke(infoDisplay, button, pair);
            elementAdded = true;
        }
        public void UpdateInfoDisplay(GeneralInfoDisplay infoDisplay, KeyValuePair<InventoryItem, int> pair)
        {
            if (infoDisplay == null) return;
            var invItem = pair.Key;
            int amount = pair.Value;
            if (invItem != null)
                infoDisplay.DisplayInfo(invItem.icon, invItem.GetName(), usesText: $"{TUFFTextParser.ParseText(itemPrefix)}{amount}", usesTextActive: true);
            else infoDisplay.DisplayInfo(null, "", iconActive: false, textActive: false);
        }
        private void PutEmpty()
        {
            PutEmpty(0, 0, 0, new KeyValuePair<InventoryItem, int>(null, 0));
        }
        private void PutEmpty(int col, int row, int index, KeyValuePair<InventoryItem, int> pair)
        {
            if (index >= infoDisplays.Count)
            {
                CreateNewElement($"{col}, {row}", row, pair);
            }
            else // Update
            {
                UpdateInfoDisplay(infoDisplays[index], pair);
                m_buttons[index].highlightDisplayText = "";
            }
            infoDisplays[index].gameObject.SetActive(true);
            onElementUpdate?.Invoke(infoDisplays[index], m_buttons[index], pair);
        }
        private GeneralInfoDisplay InstantiateElement(string suffix)
        {
            var skillGO = Instantiate(generalInfoDisplayPrefab, contentParent);
            skillGO.name = $"Item{suffix}";
            return skillGO;
        }
        public void ResetContent()
        {
            foreach (Transform child in contentParent.transform)
            {
                Destroy(child.gameObject);
            }
            infoDisplays.Clear();
            m_buttons.Clear();
            infoButtons.Clear();
            uiMenu.UIElementContainers = new UIElementContainer[0];
        }
        //public void LoadItems(Dictionary<InventoryItem, int> items, int magsCount)
        //{
        //    if (items == null) return;
        //    ResetContent();

        //    if (columns <= 0) columns = 1;
        //    int rows = 0;
        //    UIElementContainer[] uiElementContainer = new UIElementContainer[0];
        //    int totalElements = items.Keys.Count + 1; // Items + magsCount
        //    Debug.Log("Keys: " + totalElements); 

        //    rows = LISAUtility.Ceil(totalElements / ((float)columns));
        //    Debug.Log("Rows: " + rows);
        //    uiElementContainer = new UIElementContainer[rows];
        //    AddToContent(items, uiElementContainer);
        //    CreateMagsElement(magsCount, uiElementContainer, rows - 1);

        //    SetupElements(uiElementContainer);
        //}
        //private void CreateMagsElement(int mags, UIElementContainer[] uiElementContainer, int lastRow)
        //{
        //    //uiElementContainer[0] = new UIElementContainer();
        //    if (uiElementContainer[lastRow] == null) 
        //        uiElementContainer[lastRow] = new UIElementContainer();
        //    var infoDisplay = InstantiateElement($"Mags");
        //    infoDisplay.DisplayInfo(TUFFSettings.magsIcon, "", usesText: LISAUtility.IntToString(mags), usesTextActive: true);
        //    var comUIElement = infoDisplay.GetComponent<UIButton>();
        //    comUIElement.highlightDisplayText = "";
        //    uiElementContainer[lastRow].UIElements.Add(comUIElement);
        //}
    }
}
