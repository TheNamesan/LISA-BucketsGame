using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    public class CommandSubmenuHUD : MonoBehaviour
    {
        public CommandSubmenuElement submenuElementPrefab;
        public Transform elementsParent;
        public UIMenu uiMenu;
        public ScrollRectForUIMenu scrollRect;
        public int columns = 3;
        public List<CommandSubmenuElement> elements = new List<CommandSubmenuElement>();
        [System.NonSerialized] public PartyMember memberRef;
        [HideInInspector] public Command commandRef;
        [HideInInspector] public BattleHUD battleHUD;

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
            uiMenu.closeMenuWithCancel = true;
            uiMenu.onCancelMenu.AddListener(GoToPrevMenu);
            initialized = true;
        }
        private void InitializeElements()
        {
            uiMenu?.ExpandRows(0);
            foreach (Transform child in elementsParent) // Add existing GameObjects to list
            {
                if (child.TryGetComponent(out CommandSubmenuElement existing))
                {
                    existing.Initialize(this);
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
        public void InitializeSubmenuHUD(BattleHUD battleHUD)
        {
            this.battleHUD = battleHUD;
            if (uiMenu && this.battleHUD)
                uiMenu.descriptionDisplay = battleHUD.descriptionDisplay.text;
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
            //if (columns <= 0) columns = 1;
            //int rows = 0;
            //UIElementContainer[] uiElementContainer = new UIElementContainer[0];
            //if (command.commandType == CommandType.Items)
            //{
            //    var valid = GetValidItemIDs(Inventory.instance.items);
            //    if (valid.Length <= 0) 
            //    { 
            //        uiElementContainer = new UIElementContainer[1]; 
            //        AddEmpty(uiElementContainer); 
            //    }
            //    else
            //    {
            //        rows = LISAUtility.Ceil(valid.Length / ((float)columns));
            //        uiElementContainer = new UIElementContainer[rows];
            //        AddFromItems(valid, uiElementContainer, rows);
            //    }
            //}
            //else 
            //{
            //    var valid = GetValidCommands(member, commandRef.skills);
            //    if (valid.Count <= 0)
            //    {
            //        uiElementContainer = new UIElementContainer[1];
            //        AddEmpty(uiElementContainer);
            //    }
            //    else
            //    {
            //        rows = LISAUtility.Ceil(valid.Count / ((float)columns));
            //        uiElementContainer = new UIElementContainer[rows];
            //        AddFromCommandList(valid, uiElementContainer, rows);
            //    }
            //}
            ////Debug.Log($"command.skills.Count {command.skills.Count}" +
            ////    $"\ncolumns: {columns}" +
            ////    $"\nrows: {rows}");
            ////Debug.Log(uiElementContainer.Length);
            //uiMenu.UIElementContainers = uiElementContainer;
            //uiMenu.SetupUIElements();
            //scrollRect.SetupScroll();
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
            create.Initialize(this);
        
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
                if (!skill.CanBeUsedInBattle()) submenuElement.uiElement.disabled = true;
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
        private void GoToPrevMenu()
        {
            battleHUD.CancelCommandSubmenu();
        }
        //private int[] GetValidItemIDs(int[] inventoryItems)
        //{
        //    var valid = new List<int>();
        //    for (int i = 0; i < inventoryItems.Length; i++)
        //    {
        //        if (inventoryItems[i] > 0)
        //        {
        //            valid.Add(i);
        //        }
        //    }
        //    return valid.ToArray();
        //}

        //private void AddFromCommandList(List<SkillsLearned> skillList, UIElementContainer[] uiElementContainer, int rows)
        //{
        //    int index = -1;
        //    for (int i = 0; i < rows; i++)
        //    {
        //        uiElementContainer[i] = new UIElementContainer();
        //        for (int j = 0; j < columns; j++)
        //        {
        //            index++;
        //            if (index >= skillList.Count) return;

        //            CommandSubmenuElement submenuElement = InstantiateElement(index);
        //            var skill = skillList[index].skill;
        //            submenuElement.SetInvocation(skill);
        //            submenuElement.LoadInvocationInfo(memberRef);
        //            var comUIElement = submenuElement.GetComponent<UIButton>();
        //            comUIElement.highlightDisplayText = skill.GetDescription();
        //            if (!BattleManager.instance.CanUseSkill(skillList[index].skill, memberRef, false)) comUIElement.disabled = true;
        //            var validTargets = BattleManager.instance.GetInvocationValidTargets(memberRef, skill.scopeData);
        //            comUIElement.onHighlight.AddListener(() => 
        //            {
        //                battleHUD.MarkVulnerableTargets(skill, memberRef, validTargets);
        //            });
        //            comUIElement.onSelect.AddListener(() =>
        //            {
        //                battleHUD.TargetSelectionMenu(validTargets, skill, memberRef, true);
        //            });
        //            uiElementContainer[i].UIElements.Add(comUIElement);
        //            elements.Add(submenuElement);
        //        }
        //    }
        //}

        //private CommandSubmenuElement InstantiateElement(int index)
        //{
        //    CommandSubmenuElement skillGO = Instantiate(submenuElementPrefab, elementsParent);
        //    skillGO.name = $"SubmenuCommand{index}";
        //    return skillGO;
        //}

        //private void AddFromItems(int[] validItems, UIElementContainer[] uiElementContainer, int rows)
        //{
        //    int index = -1;
        //    for (int i = 0; i < rows; i++)
        //    {
        //        uiElementContainer[i] = new UIElementContainer();
        //        for (int j = 0; j < columns; j++)
        //        {
        //            index++;
        //            if (index >= validItems.Length) return;

        //            CommandSubmenuElement submenuElement = InstantiateElement(index);
        //            var item = DatabaseLoader.instance.items[validItems[index]];
        //            submenuElement.SetInvocation(item);
        //            submenuElement.LoadInvocationInfo(memberRef);
        //            var comUIElement = submenuElement.GetComponent<UIButton>();
        //            comUIElement.highlightDisplayText = item.GetDescription();
        //            var validTargets = BattleManager.instance.GetInvocationValidTargets(memberRef, item.scopeData);
        //            comUIElement.onSelect.AddListener(() => {
        //                battleHUD.TargetSelectionMenu(validTargets, item, memberRef, true);
        //            });
        //            uiElementContainer[i].UIElements.Add(comUIElement);
        //            elements.Add(submenuElement);
        //        }
        //    }
        //}
        //private void AddEmpty(UIElementContainer[] uiElementContainer)
        //{
        //    uiElementContainer[0] = new UIElementContainer();
        //    CommandSubmenuElement submenuElement = InstantiateElement(0);
        //    var comUIElement = submenuElement.GetComponent<UIButton>();
        //    submenuElement.LoadInvocationInfo(memberRef);
        //    uiElementContainer[0].UIElements.Add(comUIElement);
        //    elements.Add(submenuElement);
        //}
        //public void ResetCommands()
        //{
        //    foreach (Transform child in elementsParent)
        //    {
        //        Destroy(child.gameObject);
        //    }
        //    elements.Clear();
        //}
    }
}
