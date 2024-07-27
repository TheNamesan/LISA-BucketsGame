using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class PreviewCommandSubmenuHUD : MonoBehaviour
    {
        public CommandSubmenuElement submenuElementPrefab;
        public Transform elementsParent;
        public UIMenu uiMenu;
        public ScrollRectForUIMenu scrollRect;
        public int columns = 3;
        public List<CommandSubmenuElement> elements = new();
        [System.NonSerialized] public PartyMember memberRef;
        [HideInInspector] public Command commandRef;

        protected bool initialized = false;
        protected bool elementAdded = false;

        private void Awake()
        {
            Initialize();
        }
        private void Initialize()
        {
            if (initialized) return;
            if (uiMenu == null) uiMenu = GetComponent<UIMenu>();
            InitializeElements();
            if (columns <= 0) columns = 1;
            initialized = true;
        }
        private void InitializeElements()
        {
            uiMenu?.ExpandRows(0);
            foreach (Transform child in elementsParent) // Add existing GameObjects to list
            {
                if (child.TryGetComponent(out CommandSubmenuElement existing))
                {
                    elements.Add(existing);
                }
            }
            for (int i = 0; i < elements.Count; i++)
            {
                AddToMenu(i, elements[i].uiElement);
                elements[i].gameObject.SetActive(true);
            }
            if (uiMenu.UIElements == null) SetupElements();
        }
        protected void AddToMenu(int index, UIElement element)
        {
            if (uiMenu == null) return;
            
            int currentRow = LISAUtility.Truncate(index / ((float)columns));
            int currentColumn = index % columns;
            // Check for rows first
            if (currentRow >= uiMenu.UIElementContainers.Length)
            {
                int rowsDelta = (currentRow + 1) - uiMenu.UIElementContainers.Length;
                uiMenu?.ExpandRows(uiMenu.UIElementContainers.Length + rowsDelta);
            }
            if (uiMenu.UIElementContainers[currentRow] == null)
                uiMenu.UIElementContainers[currentRow] = new UIElementContainer();
            // Check for columns second
            if (currentColumn >= uiMenu.UIElementContainers[currentRow].UIElements.Count)
            {
                int columnsDelta = (currentColumn + 1) - uiMenu.UIElementContainers[currentRow].UIElements.Count;
                int newColumnCount = uiMenu.UIElementContainers[currentRow].UIElements.Count + columnsDelta;
                uiMenu?.ExpandColumnsAtRow(newColumnCount, currentRow);
            }
            uiMenu.UIElementContainers[currentRow].UIElements[currentColumn] = element;
        }
        private void SetupElements()
        {
            uiMenu.SetupUIElements();
            if (scrollRect)
            {
                scrollRect.uiMenu = uiMenu;
            }
            elementAdded = false;
        }
        public void UpdateSubmenu(Command command, PartyMember member)
        {
            Initialize();
            memberRef = member;
            commandRef = command;
            if (memberRef == null) { Debug.LogWarning("No Party Member Reference"); return; }
            if (commandRef == null) { Debug.LogWarning("No Command Reference"); return; }
            UpdateContent();

            if (elementAdded) SetupElements();
            if (scrollRect) scrollRect.UpdateScroll();
        }
        public void ClearSubmenu()
        {
            Initialize();
            AddEmpty();
            if (elementAdded) SetupElements();
        }
        protected void UpdateContent()
        {
            if (commandRef.commandType == CommandType.Items)
            {
                var valid = Inventory.instance.GetItemsAndAmount();
                if (valid.Count <= 0)
                {
                    AddEmpty();
                }
                else
                {
                    int rows = LISAUtility.Ceil(valid.Count / ((float)columns));
                    AddFromItems(valid, rows);
                }
            }
            else
            {
                var valid = GetValidCommands(memberRef, commandRef.skills);
                if (valid.Count <= 0)
                {
                    AddEmpty();
                }
                else
                {
                    int rows = LISAUtility.Ceil(valid.Count / ((float)columns));
                    AddFromCommandList(valid, rows);
                }
            }
        }
        private void AddEmpty()
        {
            int index = 0;
            uiMenu?.ExpandRows(1);
            if (index >= elements.Count)
            {
                InstantiateElement(index);
            }
            else
            {
                UpdateEmpty(elements[index]);
            }
            elements[index].gameObject.SetActive(true);
            index++;
            for (int i = index; i < elements.Count; i++)
                elements[i].gameObject.SetActive(false);
        }
        private void AddFromCommandList(List<SkillsLearned> skillList, int rows)
        {
            int index = 0;
            if (skillList == null) return;
            uiMenu?.ExpandRows(rows);

            for (; index < skillList.Count; index++)
            {
                if (index >= elements.Count)
                {
                    InstantiateElement(skillList[index].skill, index);
                }
                else
                {
                    UpdateElement(elements[index], skillList[index].skill);
                }
                elements[index].gameObject.SetActive(true);
            }
            for (int i = index; i < elements.Count; i++)
                elements[i].gameObject.SetActive(false);
        }
        private void AddFromItems(Dictionary<InventoryItem, int> items, int rows)
        {
            int index = 0;
            if (items == null) return;
            uiMenu?.ExpandRows(rows);
            foreach (KeyValuePair<InventoryItem, int> pair in items)
            {
                Item item = (Item)pair.Key;
                if (index >= elements.Count)
                {
                    InstantiateElement(item, index);
                }
                else
                {
                    UpdateElement(elements[index], item);
                }
                elements[index].gameObject.SetActive(true);
                index++;
            }
            for (int i = index; i < elements.Count; i++)
                elements[i].gameObject.SetActive(false);
        }
        private void InstantiateElement(IBattleInvocation skill, int index)
        {
            if (!submenuElementPrefab) return;
            CommandSubmenuElement create = Instantiate(submenuElementPrefab, elementsParent);

            UpdateElement(create, skill);

            elements.Add(create);
            AddToMenu(index, create.uiElement);
            elementAdded = true;
        }
        private void InstantiateElement(int index)
        {
            if (!submenuElementPrefab) return;
            CommandSubmenuElement create = Instantiate(submenuElementPrefab, elementsParent);
            //skillGO.name = $"SubmenuCommand{index}";
            UpdateEmpty(create);
            elements.Add(create);
            AddToMenu(index, create.uiElement);
            elementAdded = true;
        }

        protected void UpdateElement(CommandSubmenuElement submenuElement, IBattleInvocation skill)
        {
            if (!submenuElement) return;
            if (skill == null) return;
            submenuElement.SetInvocation(skill);
            submenuElement.LoadInvocationInfo(memberRef);
            if (submenuElement.uiElement)
            {
                submenuElement.uiElement.highlightDisplayText = skill.GetDescription();
                submenuElement.uiElement.disabled = false;
                if (!skill.CanBeUsedInMenu()) submenuElement.uiElement.disabled = true; 
                if (!BattleManager.instance.CanUseSkill(skill, memberRef, false)) submenuElement.uiElement.disabled = true;
            }
        }
        protected void UpdateEmpty(CommandSubmenuElement submenuElement)
        {
            if (!submenuElement) return;
            submenuElement.DisplayEmpty();
            if (submenuElement.uiElement)
            {
                submenuElement.uiElement.highlightDisplayText = "";
                submenuElement.uiElement.disabled = true;
            }
        }
        protected List<SkillsLearned> GetValidCommands(PartyMember user, List<SkillsLearned> skillList)
        {
            var valid = new List<SkillsLearned>();
            for (int i = 0; i < skillList.Count; i++)
            {
                Debug.Log(skillList[i].skill.GetName());
                if (skillList[i].learnType == LearnType.Level && memberRef.level >= skillList[i].levelLearnedAt && user.KnowsSkill(skillList[i].skill))
                {
                    if (skillList[i].skill.isUnitedSkill)
                    {
                        if (BattleManager.instance.HasUnitedSkillUsersInActiveParty(skillList[i].skill))
                        {
                            valid.Add(skillList[i]);
                        }
                    }
                    else valid.Add(skillList[i]);
                }
            }
            return valid;
        }
    }

}
