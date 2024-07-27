using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    /// <summary>
    /// UI Data of a set Command
    /// </summary>
    public class CommandElement : MonoBehaviour
    { 
        public Image icon;
        public TMP_Text text;
        public UIButton uiButton;
        [SerializeField] protected Command command;
        
        public void Initialize(CommandListHUD commandListHUD)
        {
            if (uiButton)
            {
                uiButton.onHighlight.AddListener(() => commandListHUD.battleHUD.ShowDescriptionDisplay(true));
                uiButton.onHighlight.AddListener(() => OnHighlightMarkVulnerableTargets(commandListHUD));
                uiButton.onSelect.AddListener(() => {
                    commandListHUD.battleHUD.CommandSelect(commandListHUD.memberRef, commandListHUD.commandListIndex, command);
                });
                uiButton.onHorizontalInput.AddListener((input) =>
                {
                    commandListHUD.battleHUD.SkipCommandMenu(input, commandListHUD.commandListIndex, commandListHUD.uiMenu);
                });
            }
        }
        private void OnHighlightMarkVulnerableTargets(CommandListHUD commandListHUD)
        {
            if (command)
            {
                if (command.IsValidSingleCommand())
                {
                    var skill = command.skills[0].skill;
                    var validTargets = BattleManager.instance.GetInvocationValidTargets(commandListHUD.memberRef, skill.scopeData);
                    commandListHUD.battleHUD.MarkVulnerableTargets(skill, commandListHUD.memberRef, validTargets);
                }
            }
            
        }
        public void SetCommand(Command setCommand)
        {
            command = setCommand;
        }

        public Command GetCommand()
        {
            return command;
        }

        public void LoadCommandInfo()
        {
            icon.sprite = command.icon;
            text.text = command.GetName();
        }
    }
}
