using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TUFF
{
    public class CommandListHUD : MonoBehaviour
    {
        public CommandElement commandElementPrefab;
        public Transform elementsParent;
        public ScrollRectForUIMenu scrollRect;
        public UIMenu uiMenu;
        
        public List<Command> defaultCommands = new();
        public List<CommandElement> elements = new List<CommandElement>();

        [System.NonSerialized] public PartyMember memberRef;
        [HideInInspector] public BattleHUD battleHUD;
        public int commandListIndex { get => m_commandListIndex; }
        [Tooltip("Index of memberRef in Active Party Members list")]
        private int m_commandListIndex;

        protected bool initialized = false;
        protected bool elementAdded = false;

        public void AssignBattleHUD(BattleHUD battleHUD)
        {
            this.battleHUD = battleHUD;
            if (uiMenu && this.battleHUD)
                uiMenu.descriptionDisplay = battleHUD.descriptionDisplay.text;
        }
        private void Awake()
        {
            Initialize();
        }
        private void Initialize()
        {
            if (initialized) return;
            if (uiMenu == null) uiMenu = GetComponent<UIMenu>();
            InitializeElements();
            initialized = true;
        }
        private void InitializeElements()
        {
            uiMenu?.ExpandRows(0);
            foreach (Transform child in elementsParent) // Add existing GameObjects to list
            {
                if (child.TryGetComponent(out CommandElement existing))
                {
                    existing.Initialize(this);
                    elements.Add(existing);
                }
            }
            for (int i = 0; i < elements.Count; i++)
            {
                AddToMenu(i, elements[i].uiButton);
                elements[i].gameObject.SetActive(true);
            }
            if (uiMenu.UIElements == null) SetupElements();
        }
        protected void AddToMenu(int index, UIButton element)
        {
            if (uiMenu == null) return;
            if (index >= uiMenu.UIElementContainers.Length)
            {
                int rowsDelta = (index + 1) - uiMenu.UIElementContainers.Length;
                uiMenu?.ExpandRows(uiMenu.UIElementContainers.Length + rowsDelta);
            }
            if (uiMenu.UIElementContainers[index] == null)
                uiMenu.UIElementContainers[index] = new UIElementContainer();
            uiMenu.UIElementContainers[index].UIElements.Add(element);
        }
        protected void SetupElements()
        {
            uiMenu.SetupUIElements();
            if (scrollRect)
            {
                scrollRect.uiMenu = uiMenu;
            }
            elementAdded = false;
        }
        public void UpdateCommands(PartyMember partyMember, int index, int firstUnitAlive)
        {
            Initialize();
            memberRef = partyMember;
            m_commandListIndex = index;
            uiMenu.onCancelMenu.RemoveListener(GoToPrevMenu);
            if (m_commandListIndex > firstUnitAlive) 
            {
                uiMenu.closeMenuWithCancel = true;
                uiMenu.onCancelMenu.AddListener(GoToPrevMenu);
            }
            else uiMenu.closeMenuWithCancel = false;
            List<Command> commandList = new();
            List<Command> memberCommands = memberRef.GetCommands();
            for (int i = 0; i < memberCommands.Count; i++)
            {
                commandList.Add(memberCommands[i]);
            }
            for (int i = 0; i < defaultCommands.Count; i++)
            {
                commandList.Add(defaultCommands[i]);
            }
            //ResetCommands();
            //UIElementContainer[] uiElementContainer = new UIElementContainer[commandList.Count + defaultCommands.Count];

            UpdateContent(commandList);

            if (elementAdded) SetupElements();
            if (scrollRect) scrollRect.UpdateScroll();
        }
        private void UpdateContent(List<Command> commandList)
        {
            int row = 0;
            if (commandList == null) return;
            uiMenu?.ExpandRows(commandList.Count);

            for (; row < commandList.Count; row++)
            {
                if (row >= elements.Count)
                {
                    InstantiateCommandElement(commandList[row], row);
                }
                else
                {
                    UpdateCommand(elements[row], commandList[row]);
                }
                elements[row].gameObject.SetActive(true);
            }
            for (int i = row; i < elements.Count; i++)
                elements[i].gameObject.SetActive(false);
            //CheckIfCurrentHighlightIsValid();
        }
        private void GoToPrevMenu()
        {
            battleHUD.CommandCancel();
        }
        //private void AddFromCommandList(List<Command> commandList, UIElementContainer[] uiElementContainer)
        //{
        //    for (int i = 0; i < commandList.Count; i++)
        //    {
        //        InstantiateCommandElement(commandList[i], uiElementContainer, i);
        //    }
        //}
        //void AddDefaultCommands(List<Command> commandList, UIElementContainer[] uiElementContainer)
        //{
        //    for (int i = 0; i < defaultCommands.Count; i++)
        //    {
        //        InstantiateCommandElement(defaultCommands[i], uiElementContainer, commandList.Count + i);
        //    }
        //}
        private void InstantiateCommandElement(Command command, int row)
        {
            if (!commandElementPrefab) return;
            CommandElement create = Instantiate(commandElementPrefab, elementsParent);
            create.Initialize(this);

            UpdateCommand(create, command);
            
            elements.Add(create);
            AddToMenu(row, create.uiButton);
            elementAdded = true;
        }
        private void UpdateCommand(CommandElement create, Command command)
        {
            if (!command) return;
            if (!create) return;
            create.SetCommand(command);
            create.LoadCommandInfo();

            UIButton button = create.uiButton;
            if (button)
            {
                button.disabled = false;
                if (memberRef.HasCommandSeal(command)) button.disabled = true;
                
                if (command.IsValidSingleCommand())
                {
                    var skill = command.skills[0].skill;
                    if (!BattleManager.instance.CanUseSkill(skill, memberRef, false)) button.disabled = true;
                }
                else if (command.commandType == CommandType.Escape)
                {
                    button.disabled = !BattleManager.instance.CanEscape;
                }
                button.highlightDisplayText = command.GetDescription();
            }
        }
        //private void InstantiateCommandElement(Command command, UIElementContainer[] uiElementContainer, int containerPosition)
        //{
        //    CommandElement commandElement = Instantiate(commandElementPrefab, content.transform);
        //    commandElement.SetCommand(command);
        //    commandElement.LoadCommandInfo();
        //    uiElementContainer[containerPosition] = new UIElementContainer();
        //    UIButton uiButton = commandElement.uiButton;
        //    uiElementContainer[containerPosition].UIElements.Add(uiButton);
        //    if (memberRef.HasCommandSeal(command)) uiButton.disabled = true;
        //    uiButton.highlightDisplayText = command.GetDescription();
        //    if (command.skills.Count > 0)
        //    {
        //        if (command.commandType == CommandType.Single)
        //        {
        //            var skill = command.skills[0].skill;
        //            var validTargets = BattleManager.instance.GetInvocationValidTargets(memberRef, skill.scopeData);
        //            if (!BattleManager.instance.CanUseSkill(skill, memberRef, false)) uiButton.disabled = true;
        //            uiButton.onHighlight.AddListener(() => 
        //            {
        //                battleHUD.MarkVulnerableTargets(skill, memberRef, validTargets);
        //            });
        //        }
        //    }
        //    uiButton.onHighlight.AddListener(() => battleHUD.ShowDescriptionDisplay(true));
        //    uiButton.onSelect.AddListener(() => { 
        //        battleHUD.CommandSelect(memberRef, m_commandListIndex, command);
        //    });
        //    uiButton.onHorizontalInput.AddListener((input) => 
        //    {
        //        battleHUD.SkipCommandMenu(input, m_commandListIndex, uiMenu);
        //    });
        //    elements.Add(commandElement);
        //}
        //public void ResetCommands()
        //{
        //    foreach (Transform child in content.transform)
        //    {
        //        Destroy(child.gameObject);
        //    }
        //    elements.Clear();
        //}
        public void SetActive(bool setActive)
        {
            gameObject.SetActive(setActive);
        }
    }
}
