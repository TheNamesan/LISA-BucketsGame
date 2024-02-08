using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TUFF
{
    public class CommandListHUD : MonoBehaviour
    {
        public GameObject commandElementPrefab;
        public UIMenu uiMenu;
        public ScrollRectForUIMenu scrollRect;
        public RectTransform content;
        [HideInInspector] public BattleHUD battleHUD;
        public List<Command> defaultCommands;
        public List<CommandElement> commandElements = new List<CommandElement>();
        [HideInInspector] public PartyMember memberRef;
        private int commandListIndex;

        public void InitializeCommandListHUD(BattleHUD battleHUD)
        {
            this.battleHUD = battleHUD;
        }

        public void UpdateCommands(PartyMember partyMember, int index, int firstUnitAlive)
        {
            memberRef = partyMember;
            commandListIndex = index;
            uiMenu.onCancelMenu.RemoveListener(GoToPrevMenu);
            if (commandListIndex > firstUnitAlive) 
            {
                uiMenu.closeMenuWithCancel = true;
                uiMenu.onCancelMenu.AddListener(GoToPrevMenu);
            }
            else uiMenu.closeMenuWithCancel = false;
            var commandList = memberRef.GetCommands();
            ResetCommands();
            UIElementContainer[] uiElementContainer = new UIElementContainer[commandList.Count + defaultCommands.Count];
            AddFromCommandList(commandList, uiElementContainer);
            AddDefaultCommands(commandList, uiElementContainer);
            uiMenu.descriptionDisplay = battleHUD.descriptionDisplay.text;
            uiMenu.UIElementContainers = uiElementContainer;
            uiMenu.SetupUIElements();
            scrollRect.SetupScroll();
        }
        private void GoToPrevMenu()
        {
            battleHUD.CommandCancel();
        }
        private void AddFromCommandList(List<Command> commandList, UIElementContainer[] uiElementContainer)
        {
            for (int i = 0; i < commandList.Count; i++)
            {
                InstantiateCommandElement(commandList[i], uiElementContainer, i);
            }
        }
        void AddDefaultCommands(List<Command> commandList, UIElementContainer[] uiElementContainer)
        {
            for (int i = 0; i < defaultCommands.Count; i++)
            {
                InstantiateCommandElement(defaultCommands[i], uiElementContainer, commandList.Count + i);
            }
        }
        private void InstantiateCommandElement(Command command, UIElementContainer[] uiElementContainer, int containerPosition)
        {
            GameObject commandGO = Instantiate(commandElementPrefab);
            commandGO.transform.SetParent(content.transform, false);
            CommandElement commandElement = commandGO.GetComponent<CommandElement>();
            commandElement.SetCommand(command);
            commandElement.LoadCommandInfo();
            uiElementContainer[containerPosition] = new UIElementContainer();
            UIButton comUIElement = commandGO.GetComponent<UIButton>();
            uiElementContainer[containerPosition].UIElements.Add(comUIElement);
            if (memberRef.HasCommandSeal(command)) comUIElement.disabled = true;
            comUIElement.highlightDisplayText = command.GetName();
            if (command.skills.Count > 0)
            {
                if (command.commandType == CommandType.Single)
                {
                    var skill = command.skills[0].skill;
                    var validTargets = BattleManager.instance.GetInvocationValidTargets(memberRef, skill.scopeData);
                    comUIElement.highlightDisplayText = skill.GetDescription();
                    if (!BattleManager.instance.CanUseSkill(skill, memberRef, false)) comUIElement.disabled = true;
                    comUIElement.onHighlight.AddListener(() => 
                    {
                        battleHUD.MarkVulnerableTargets(skill, memberRef, validTargets);
                    });
                }
            }
            comUIElement.onHighlight.AddListener(() => battleHUD.ShowDescriptionDisplay(true));
            comUIElement.onSelect.AddListener(() => { 
                battleHUD.CommandSelect(memberRef, commandListIndex, command);
            });
            comUIElement.onHorizontalInput.AddListener((input) => 
            {
                battleHUD.SkipCommandMenu(input, commandListIndex, uiMenu);
            });
            commandElements.Add(commandElement);
        }
        public void ResetCommands()
        {
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }
            commandElements.Clear();
        }
        public void SetActive(bool setActive)
        {
            gameObject.SetActive(setActive);
        }
    }
}
