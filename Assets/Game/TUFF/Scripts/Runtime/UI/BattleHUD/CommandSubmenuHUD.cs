using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    public class CommandSubmenuHUD : MonoBehaviour
    {
        public GameObject submenuElementPrefab;
        public UIMenu uiMenu;
        public GridLayoutGroup gridLayoutGroup;
        public ScrollRectForUIMenu scrollRect;
        public RectTransform content;
        public int columns = 3;
        [HideInInspector] public BattleHUD battleHUD;
        public List<CommandSubmenuElement> submenuElements = new List<CommandSubmenuElement>();
        [HideInInspector] public Command commandRef;
        [System.NonSerialized] public PartyMember memberRef;
        public void InitializeSubmenuHUD(BattleHUD battleHUD)
        {
            this.battleHUD = battleHUD;
        }
        public void UpdateSubmenu(Command command, PartyMember member)
        {
            memberRef = member;
            commandRef = command;
            uiMenu.onCancelMenu.RemoveListener(GoToPrevMenu);
            uiMenu.closeMenuWithCancel = true;
            uiMenu.onCancelMenu.AddListener(GoToPrevMenu);
            ResetCommands();
            if (columns <= 0) columns = 1;
            int rows = 0;
            UIElementContainer[] uiElementContainer = new UIElementContainer[0];
            if (command.commandType == CommandType.Items)
            {
                var valid = GetValidItemIDs(GameManager.instance.playerData.inventory.items);
                if (valid.Length <= 0) 
                { 
                    uiElementContainer = new UIElementContainer[1]; 
                    AddEmpty(uiElementContainer); 
                }
                else
                {
                    rows = LISAUtility.Ceil(valid.Length / ((float)columns));
                    uiElementContainer = new UIElementContainer[rows];
                    AddFromItems(valid, uiElementContainer, rows);
                }
            }
            else 
            {
                var valid = GetValidCommands(member, commandRef.skills);
                if (valid.Count <= 0)
                {
                    uiElementContainer = new UIElementContainer[1];
                    AddEmpty(uiElementContainer);
                }
                else
                {
                    rows = LISAUtility.Ceil(valid.Count / ((float)columns));
                    uiElementContainer = new UIElementContainer[rows];
                    AddFromCommandList(valid, uiElementContainer, rows);
                }
            }
            //Debug.Log($"command.skills.Count {command.skills.Count}" +
            //    $"\ncolumns: {columns}" +
            //    $"\nrows: {rows}");
            //Debug.Log(uiElementContainer.Length);
            uiMenu.descriptionDisplay = battleHUD.descriptionDisplay.text;
            uiMenu.UIElementContainers = uiElementContainer;
            uiMenu.SetupUIElements();
            scrollRect.SetupScroll();
        }
        private List<SkillsLearned> GetValidCommands(PartyMember user, List<SkillsLearned> skillList)
        {   
            var valid = new List<SkillsLearned>();
            for(int i = 0; i < skillList.Count; i++)
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
        private int[] GetValidItemIDs(int[] inventoryItems)
        {
            var valid = new List<int>();
            for (int i = 0; i < inventoryItems.Length; i++)
            {
                if (inventoryItems[i] > 0)
                {
                    valid.Add(i);
                }
            }
            return valid.ToArray();
        }
        private void GoToPrevMenu()
        {
            battleHUD.CancelCommandSubmenu();
        }
        
        private void AddFromCommandList(List<SkillsLearned> skillList, UIElementContainer[] uiElementContainer, int rows)
        {
            int index = -1;
            for (int i = 0; i < rows; i++)
            {
                uiElementContainer[i] = new UIElementContainer();
                for (int j = 0; j < columns; j++)
                {
                    index++;
                    if (index >= skillList.Count) return;

                    GameObject skillGO = InstantiateElement(index);
                    var submenuElement = skillGO.GetComponent<CommandSubmenuElement>();
                    var skill = skillList[index].skill;
                    submenuElement.SetInvocation(skill);
                    submenuElement.LoadInvocationInfo(memberRef);
                    var comUIElement = skillGO.GetComponent<UIButton>();
                    comUIElement.highlightDisplayText = skill.GetDescription();
                    if (!BattleManager.instance.CanUseSkill(skillList[index].skill, memberRef, false)) comUIElement.disabled = true;
                    var validTargets = BattleManager.instance.GetInvocationValidTargets(memberRef, skill.scopeData);
                    comUIElement.onHighlight.AddListener(() => 
                    {
                        battleHUD.MarkVulnerableTargets(skill, memberRef, validTargets);
                    });
                    comUIElement.onSelect.AddListener(() =>
                    {
                        battleHUD.TargetSelectionMenu(validTargets, skill, memberRef, true);
                    });
                    uiElementContainer[i].UIElements.Add(comUIElement);
                    submenuElements.Add(submenuElement);
                }
            }
        }

        private GameObject InstantiateElement(int index)
        {
            GameObject skillGO = Instantiate(submenuElementPrefab);
            skillGO.name = $"SubmenuCommand{index}";
            skillGO.transform.SetParent(content.transform, false);
            return skillGO;
        }

        private void AddFromItems(int[] validItems, UIElementContainer[] uiElementContainer, int rows)
        {
            int index = -1;
            for (int i = 0; i < rows; i++)
            {
                uiElementContainer[i] = new UIElementContainer();
                for (int j = 0; j < columns; j++)
                {
                    index++;
                    if (index >= validItems.Length) return;

                    GameObject skillGO = InstantiateElement(index);
                    var submenuElement = skillGO.GetComponent<CommandSubmenuElement>();
                    var item = DatabaseLoader.instance.items[validItems[index]];
                    submenuElement.SetInvocation(item);
                    submenuElement.LoadInvocationInfo(memberRef);
                    var comUIElement = skillGO.GetComponent<UIButton>();
                    comUIElement.highlightDisplayText = item.GetDescription();
                    var validTargets = BattleManager.instance.GetInvocationValidTargets(memberRef, item.scopeData);
                    comUIElement.onSelect.AddListener(() => {
                        battleHUD.TargetSelectionMenu(validTargets, item, memberRef, true);
                    });
                    uiElementContainer[i].UIElements.Add(comUIElement);
                    submenuElements.Add(submenuElement);
                }
            }
        }
        private void AddEmpty(UIElementContainer[] uiElementContainer)
        {
            uiElementContainer[0] = new UIElementContainer();
            GameObject skillGO = InstantiateElement(0);
            var submenuElement = skillGO.GetComponent<CommandSubmenuElement>();
            var comUIElement = skillGO.GetComponent<UIButton>();
            submenuElement.LoadInvocationInfo(memberRef);
            uiElementContainer[0].UIElements.Add(comUIElement);
            submenuElements.Add(submenuElement);
        }
        public void ResetCommands()
        {
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }
            submenuElements.Clear();
        }
    }
}
