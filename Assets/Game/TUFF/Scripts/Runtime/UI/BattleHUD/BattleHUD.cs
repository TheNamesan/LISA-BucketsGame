using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TUFF
{
    public class BattleHUD : MonoBehaviour
    {
        public int commandMenuIndex = 0;
        public GameObject unitHUDPrefab;
        public GameObject commandMenuPrefab;
        public GameObject commandSubmenuPrefab;
        public Transform battleStage;
        public Transform overlayInfo;
        public BoxTransitionHandler primaryWindow;
        public BoxTransitionHandler secondaryWindow;
        public Transform unitsInfoContent;
        public Transform commandSubmenuContent;

        public Transform commandsMenusContent;
        public Transform selectedUnitInfoContent;

        public FlashImageHandler tintScreenFlash;
        public DescriptionDisplayHUD descriptionDisplay;
        public UIMenu targetSelectionMenu;
        public ResultsScreenHUD resultsScreenHUD;

        public BarHandler UPBar;
        public StatusHUD allyStatusHUD;
        public StatusHUD enemyStatusHUD;
        public DetailedStatusHUD detailedStatusHUD;
        public UnitHUD[] unitHUD = new UnitHUD[PlayerData.activePartyMaxSize];
        public CommandListHUD commandList = null;
        public CommandSubmenuHUD commandSubmenuHUD;
        public CommandListHUD[] commandListHUD = new CommandListHUD[PlayerData.activePartyMaxSize];
        public UnitHUD selectedUnitHUD;
        public List<HitDisplayGroup> hitDisplayGroups = new List<HitDisplayGroup>();
        public List<EnemyBarHandler> enemyHPBars = new List<EnemyBarHandler>();

        [Header("Terms Name Keys")]
        public string allEnemiesScopeKey = "scope_AllEnemies";
        public string allAlliesScopeKey = "scope_AllAllies";
        public enum PrimaryWindowContent
        {
            All = 0,
            UnitsInfo = 1,
            CommandSubmenu = 2
        };
        public enum SecondWindowContent {
            All = 0,
            CommandsMenus = 1,
            SelectedUnit = 2
        };

        public void InitializeHUD()
        {
            UnloadHUD();
            commandMenuIndex = 0;
            commandListHUD = new CommandListHUD[PlayerData.activePartyMaxSize];
            unitHUD = new UnitHUD[PlayerData.activePartyMaxSize];

            for (int i = 0; i < PlayerData.activePartyMaxSize; i++)
            {
                GameObject menuGO = Instantiate(unitHUDPrefab, parent: unitsInfoContent);
                menuGO.name = $"UnitHUD{i}";
                UnitHUD unit = menuGO.GetComponent<UnitHUD>();
                unit.InitializeUnitHUD();
                if (unit == null) continue;
                unitHUD[i] = unit;
            }

            //Submenu
            var submenuGO = Instantiate(commandSubmenuPrefab, commandSubmenuContent);
            submenuGO.name = "CommandSubmenu";
            var submenu = submenuGO.GetComponent<CommandSubmenuHUD>();
            commandSubmenuHUD = submenu;
            submenu.InitializeSubmenuHUD(this);

            commandList.AssignBattleHUD(this);
            for (int i = 0; i < PlayerData.activePartyMaxSize; i++)
            {
                break;
                GameObject menuGO = Instantiate(commandMenuPrefab, parent: commandsMenusContent);
                menuGO.name = $"CommandsList{i}";
                CommandListHUD listHUD = menuGO.GetComponent<CommandListHUD>();
                if (listHUD == null) continue;
                listHUD.AssignBattleHUD(this);
                commandListHUD[i] = listHUD;
            }
            var selectUnitGO = Instantiate(unitHUDPrefab, selectedUnitInfoContent);
            selectUnitGO.name = "SelectedUnit";
            selectedUnitHUD = selectUnitGO.GetComponent<UnitHUD>();
            selectedUnitHUD.InitializeUnitHUD();

            UpdateCommandsList();
            UpdateSelectedStatusHUD(BattleManager.instance.activeParty[commandMenuIndex]);
            ShowDescriptionDisplay(false);
            HidePrimaryWindowContentExcept(PrimaryWindowContent.UnitsInfo);
            HideSecondWindowContentExcept(SecondWindowContent.CommandsMenus);
            InitializeWindows();
        }
        public void Update()
        {
            OpenStatusHUD();
        }
        public void ResetPrimaryWindow()
        {
            foreach (Transform child in unitsInfoContent)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in commandSubmenuContent)
            {
                Destroy(child.gameObject);
            }
        }
        public void ResetSecondaryWindow()
        {
            foreach (Transform child in selectedUnitInfoContent)
            {
                Destroy(child.gameObject);
            }
        }
        public void ResetOverlayInfo()
        {
            for (int i = 0; i < hitDisplayGroups.Count; i++) //Clean HitDisplayGroups
            {
                Destroy(hitDisplayGroups[i].gameObject);
            }
            hitDisplayGroups = new List<HitDisplayGroup>();
            for (int i = 0; i < enemyHPBars.Count; i++) //Clean EnemyHPBars
            {
                Destroy(enemyHPBars[i].gameObject);
            }
            enemyHPBars = new List<EnemyBarHandler>();

            foreach (Transform child in overlayInfo) //Clean anything else
            {
                Destroy(child.gameObject);
            }
        }
        public void UnloadHUD()
        {
            ResetPrimaryWindow();
            ResetSecondaryWindow();
            ResetOverlayInfo();
            detailedStatusHUD?.gameObject.SetActive(false);
            resultsScreenHUD.gameObject.SetActive(false);
            //HidePrimaryWindowContentExcept(PrimaryWindowContent.UnitsInfo);
            //HideSecondWindowContentExcept(SecondWindowContent.CommandsMenus);
        }
        public void InitializeWindows()
        {
            if (primaryWindow.canvasGroup != null)
                primaryWindow.canvasGroup.alpha = 0;
            if (secondaryWindow.canvasGroup != null)
                secondaryWindow.canvasGroup.alpha = 0;
            HideAll();
        }
        public void DisplayWindow(BoxTransitionHandler window, bool show)
        {
            if (window == null) return;
            if (show && window.state != BoxTransitionState.Visible) window.Appear();
            if (!show && window.state != BoxTransitionState.Hidden) window.Dissapear();
        }
        public void ShowAll()
        {
            DisplayWindow(primaryWindow, true);
            DisplayWindow(secondaryWindow, true);
        }
        public void HideAll()
        {
            DisplayWindow(primaryWindow, false);
            DisplayWindow(secondaryWindow, false);
        }
        public void ShowWindowsDynamic(bool show)
        {
            var state = BattleManager.instance.battleState;
            if (show)
            {
                if (BattleManager.instance.CheckBattleEnd()) return;
                DisplayWindow(primaryWindow, true);
                if (state == BattleState.PLAYERACTIONS)
                    DisplayWindow(secondaryWindow, true);
            }
            else
            {
                DisplayWindow(primaryWindow, false);
                if (state == BattleState.PLAYERACTIONS)
                    DisplayWindow(secondaryWindow, false);
            }
        }
        public void HidePrimaryWindowContentExcept(PrimaryWindowContent contentToShow)
        {
            switch (contentToShow)
            {
                case PrimaryWindowContent.All:
                    unitsInfoContent.gameObject.SetActive(false);
                    commandSubmenuContent.gameObject.SetActive(false);
                    break;

                case PrimaryWindowContent.UnitsInfo:
                    unitsInfoContent.gameObject.SetActive(true);
                    commandSubmenuContent.gameObject.SetActive(false);
                    break;

                case PrimaryWindowContent.CommandSubmenu:
                    unitsInfoContent.gameObject.SetActive(false);
                    commandSubmenuContent.gameObject.SetActive(true);
                    break;
            }
        }
        public void HideSecondWindowContentExcept(SecondWindowContent contentToShow)
        {
            switch (contentToShow)
            {
                case SecondWindowContent.All:
                    commandsMenusContent.gameObject.SetActive(false);
                    selectedUnitInfoContent.gameObject.SetActive(false);
                    break;

                case SecondWindowContent.CommandsMenus:
                    commandsMenusContent.gameObject.SetActive(true);
                    selectedUnitInfoContent.gameObject.SetActive(false);
                    break;

                case SecondWindowContent.SelectedUnit:
                    commandsMenusContent.gameObject.SetActive(false);
                    selectedUnitInfoContent.gameObject.SetActive(true);
                    break;
            }
        }
        public void AddToOverlayInfo(Transform transform)
        {
            transform.SetParent(overlayInfo, true);
        }
        public IEnumerator ShowResults()
        {
            HideAll();
            detailedStatusHUD?.gameObject.SetActive(false);
            yield return resultsScreenHUD.ShowResults();
        }
        public void UpdateUnitsHUD(PartyMember[] activeParty, bool resetCommandIcons = false)
        {
            if (!BattleManager.instance.InBattle) return;
            for (int i = 0; i < PlayerData.activePartyMaxSize; i++)
            {
                if (i >= GameManager.instance.playerData.GetActivePartySize())
                {
                    unitHUD[i].SetActive(false);
                }
                else
                {
                    var member = activeParty[i];
                    unitHUD[i].SetActive(true);
                    unitHUD[i].UpdateInfo(member);
                    if (resetCommandIcons) unitHUD[i].UpdateCommandIcon(null);
                    activeParty[i].imageReference = unitHUD[i].graphicHandler;
                }
            }
        }
        public void UpdateEnemiesHUD()
        {
            if (!BattleManager.instance.InBattle) return;
            for (int i = 0; i < enemyHPBars.Count; i++)
            {
                enemyHPBars[i].QuickUpdate();
            }
        }
        public void UpdateUPBar()
        {
            if (UPBar != null)
            {
                if (PlayerData.instance.battleData.disableUP) UPBar.gameObject.SetActive(false);
                else UPBar.gameObject.SetActive(true);
                float percentage = PlayerData.instance.battleData.GetUPPercentage();
                UPBar.SetValue(percentage, 100, format: "F0", postfix: "%");
            }
        }
        public void UpdateSelectedUnitHUD(PartyMember member)
        {
            selectedUnitHUD.UpdateInfo(member);
            selectedUnitHUD.UpdateCommandIcon(null);
        }
        public void ToggleStatusHUD(bool active)
        {
            allyStatusHUD.gameObject.SetActive(active);
            if (!active)
            {
                allyStatusHUD.uiMenu.CloseMenu();
                allyStatusHUD.gameObject.SetActive(active);
                enemyStatusHUD.uiMenu.CloseMenu();
                enemyStatusHUD.gameObject.SetActive(active);
            }
        }
        public void OpenStatusHUD()
        {
            if (UIController.instance.skipButtonDown)
            {
                if (allyStatusHUD)
                {
                    if (allyStatusHUD.isActiveAndEnabled && !allyStatusHUD.uiMenu.IsOpen)
                    {
                        allyStatusHUD.uiMenu.OpenMenu();
                    }
                }
                if (enemyStatusHUD)
                {
                    if (enemyStatusHUD.isActiveAndEnabled && !enemyStatusHUD.uiMenu.IsOpen)
                    {
                        enemyStatusHUD.uiMenu.OpenMenu();
                    }
                }
            }
        }
        public void UpdateSelectedStatusHUD(Targetable targetable)
        {
            if (targetable is PartyMember)
            {
                allyStatusHUD.gameObject.SetActive(true);
                enemyStatusHUD.gameObject.SetActive(false);
                allyStatusHUD.UpdateStatus(targetable);
            }
            else
            {
                allyStatusHUD.gameObject.SetActive(false);
                enemyStatusHUD.gameObject.SetActive(true);
                enemyStatusHUD.UpdateStatus(targetable);
            }
        }
        public void UpdateCommandsList()
        {
            int firstUnitAlive = GetFirstControllableUnitIndex();
            var member = BattleManager.instance.activeParty[commandMenuIndex];
            commandList.UpdateCommands(member, commandMenuIndex, firstUnitAlive);
        }
        public void StartPlayerActions()
        {
            ShowAll();
            ToggleStatusHUD(true);
            HideSecondWindowContentExcept(SecondWindowContent.CommandsMenus);
            SetNextActableUnitIndex(0);
            if (commandMenuIndex >= GameManager.instance.playerData.GetActivePartySize())
            {
                EndPlayerActions();
            }
            else SetCommandListMenu(commandMenuIndex);
        }
        private int GetNextActableUnitIndex(int startingIndex)
        {
            for (int i = startingIndex; i < GameManager.instance.playerData.GetActivePartySize(); i++)
            {
                var user = BattleManager.instance.activeParty[i];
                if (user.CanControlAct() && !BattleManager.instance.IsPartOfQueuedUnitedSkill(user))
                    return i;
                
            }
            return GameManager.instance.playerData.GetActivePartySize();
        }
        private void SetNextActableUnitIndex(int startingIndex)
        {
            commandMenuIndex = GetNextActableUnitIndex(startingIndex);
        }
        private int GetPrevActableUnitIndex(int startingIndex)
        {
            for (int i = startingIndex; i >= 0; i--)
            {
                var user = BattleManager.instance.activeParty[i];
                if (user.CanControlAct() && !BattleManager.instance.IsPartOfQueuedUnitedSkill(user))
                    return i;
            }
            return -1;
        }
        private void SetPrevActableUnitIndex(int startingIndex)
        {
            commandMenuIndex = GetPrevActableUnitIndex(startingIndex);
        }
        private int GetFirstControllableUnitIndex()
        {
            for (int i = 0; i < PlayerData.instance.GetActivePartySize(); i++)
            {
                var user = BattleManager.instance.activeParty[i];
                if (user.CanControlAct() && !BattleManager.instance.IsPartOfQueuedUnitedSkill(user))
                {
                    return i;
                }
            }
            return -1;
        }
        public void ShowDescriptionDisplay(bool show)
        {
            descriptionDisplay.gameObject.SetActive(show);
        }
        void UnhighlightMembersExcept(int index)
        {
            for (int i = 0; i < PlayerData.instance.GetActivePartySize(); i++)
            {
                var member = BattleManager.instance.activeParty[i];
                if (i == index) member.HighlightImageRef(true);
                else member.HighlightImageRef(false);
            }
        }
        public void TargetSelectionMenu(List<Targetable> validTargets, IBattleInvocation skill, PartyMember user, bool fromSubmenu = false, bool fromCommandList = false)
        {
            if (validTargets.Count == 0) return;
            if (fromSubmenu) HidePrimaryWindowContentExcept(PrimaryWindowContent.UnitsInfo);
            ResetTargetSelectionMenu();
            ToggleStatusHUD(true);
            ShowDescriptionDisplay(true);
            commandList.memberRef.HighlightImageRef(false);
            //commandListHUD[commandMenuIndex].memberRef.HighlightImageRef(false);
            UIElementContainer[] uiElementContainer = new UIElementContainer[1]; //Horizontal navigation setup
            uiElementContainer[0] = new UIElementContainer();
            // If Selection is group or random
            if (BattleManager.IsGroupScope(skill.ScopeData.scopeType) || BattleManager.IsRandomScope(skill.ScopeData.scopeType))
            {
                ToggleStatusHUD(false); // Hide Status HUD
                UIButton button = CreateTargetMenuButton(0);
                button.highlightDisplayText = GetLocalizedScopeName(skill.ScopeData.scopeType);
                int menuIndex = commandMenuIndex;
                button.onHighlight.AddListener(() =>
                {
                    foreach (Targetable target in validTargets)
                    {
                        HighlightVulnerableTarget(skill, user, target);
                        if (target is EnemyInstance)
                            ShowEnemyHPBar(target as EnemyInstance, true);
                    }
                });
                button.onUnhighlight.AddListener(() =>
                {
                    foreach (Targetable target in validTargets)
                    {
                        target.HighlightImageRef(false);
                        if (target is EnemyInstance)
                            RemoveEnemyHPBar(target as EnemyInstance);
                    }
                });
                button.onSelect.AddListener(() =>
                {
                    foreach (Targetable target in validTargets)
                    {
                        target.HighlightImageRef(false);
                    }
                    if (fromSubmenu) CloseCommandSubmenu();
                    SelectTarget(skill, user, validTargets, menuIndex);
                });
                uiElementContainer[0].UIElements.Add(button);
            }
            else // If single target
            {
                for (int i = 0; i < validTargets.Count; i++)
                {
                    var target = validTargets[i];
                    UIButton button = CreateTargetMenuButton(i);
                    button.highlightDisplayText = target.GetName();
                    int menuIndex = commandMenuIndex;
                    button.onHighlight = new UnityEvent();
                    button.onHighlight.AddListener(() =>
                    {
                        HighlightVulnerableTarget(skill, user, target);
                        if (target is EnemyInstance)
                            ShowEnemyHPBar(target as EnemyInstance, true);
                        UpdateSelectedStatusHUD(target);
                    });
                    button.onUnhighlight = new UnityEvent();
                    button.onUnhighlight.AddListener(() =>
                    {
                        target.HighlightImageRef(false);
                        if (target is EnemyInstance)
                            RemoveEnemyHPBar(target as EnemyInstance);
                    });
                    button.onSelect = new UnityEvent();
                    button.onSelect.AddListener(() =>
                    {
                        target.HighlightImageRef(false);
                        if (fromSubmenu) CloseCommandSubmenu();
                        SelectTarget(skill, user, new List<Targetable>() { target }, menuIndex);
                    });
                    uiElementContainer[0].UIElements.Add(button);
                }
            }
            targetSelectionMenu.onCancelMenu.AddListener(CancelTargetSelectionMenu);
            if (fromCommandList) targetSelectionMenu.onCancelMenu.AddListener(TargetSelectionReturnToCommandList);
            if (fromSubmenu) targetSelectionMenu.onCancelMenu.AddListener(TargetSelectionReturnToSubmenu);
            targetSelectionMenu.descriptionDisplay = descriptionDisplay.text;
            targetSelectionMenu.UIElementContainers = uiElementContainer;
            targetSelectionMenu.SetupUIElements();
            targetSelectionMenu.OpenMenu();
        }

        public void UnhighlightAll()
        {
            foreach (Targetable enemy in BattleManager.instance.enemies) enemy.HighlightImageRef(false);
            for (int i = 0; i < PlayerData.instance.GetActivePartySize(); i++)
            {
                BattleManager.instance.activeParty[i].HighlightImageRef(false);
            }
        }

        /// <summary>
        /// Highlights the target and marks them with Weakness, Resist or Immune (or normal highlight if no vulnerability).
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <returns>Returns the type of vulnerability of the target</returns>
        public VulnerabilityType HighlightVulnerableTarget(IBattleInvocation skill, Targetable user, Targetable target, bool unhighlightIfNoVulnerability = false, bool unhighlightUser = false)
        {
            int elementIndex = -1;
            var vulType = BattleManager.GetVulnerabilityTypeFromSkill(skill, user, target, ref elementIndex);

            if (unhighlightIfNoVulnerability)
            {
                if (vulType != VulnerabilityType.Normal) target.HighlightImageRef(true, elementIndex, vulType);
                else {
                    if (target != user) target.HighlightImageRef(false);
                    else if (unhighlightUser) target.HighlightImageRef(false);
                }
            }
            else target.HighlightImageRef(true, elementIndex, vulType);
            return vulType;
        }
        public void MarkVulnerableTargets(IBattleInvocation skill, Targetable user, List<Targetable> targets)
        {
            if (targets == null) return;
            UnhighlightAll();
            commandList.memberRef.HighlightImageRef(true);
            //commandListHUD[commandMenuIndex].memberRef.HighlightImageRef(true);
            for (int i = 0; i < targets.Count; i++)
            {
                int elementIndex = -1;
                var vulType = BattleManager.GetVulnerabilityTypeFromSkill(skill, user, targets[i], ref elementIndex);
                bool isUser = targets[i] == user;
                targets[i]?.MarkVulnerability(elementIndex, vulType, isUser);
            }
        }
        public void HighlightOnlyVulnerableTargets(IBattleInvocation skill, Targetable user, List<Targetable> targets)
        {
            if (targets == null) return;
            for (int i = 0; i < targets.Count; i++)
            {
                HighlightVulnerableTarget(skill, user, targets[i], true);
            }
        }
        public string GetLocalizedScopeName(ScopeType scope) // Delete all of this please
        {
            switch(scope)
            {
                case ScopeType.AllEnemies: return LISAUtility.GetLocalizedText(TUFFSettings.termsTable, allEnemiesScopeKey);
                case ScopeType.AllAllies: return LISAUtility.GetLocalizedText(TUFFSettings.termsTable, allAlliesScopeKey);
                case ScopeType.RandomEnemies: return "Random Enemies";
                case ScopeType.RandomAllies: return "Random Allies";
                default: return "";
            }
        }
        private UIButton CreateTargetMenuButton(int i)
        {
            var buttonGO = new GameObject($"Selection {i}");
            buttonGO.transform.SetParent(targetSelectionMenu.transform, false);
            var button = buttonGO.AddComponent<UIButton>();
            return button;
        }

        private void SelectTarget(IBattleInvocation skill, PartyMember user, List<Targetable> targets, int commandMenuIndex)
        {
            ShowDescriptionDisplay(false);
            foreach(Targetable target in targets) target.HighlightImageRef(false);
            BattleManager.instance.QueueOrReplaceCommand(skill, targets, user, unitHUD[commandMenuIndex]);
            ResetTargetSelectionMenu();
            targetSelectionMenu.CloseMenu();
            unitHUD[commandMenuIndex].UpdateCommandIcon(skill.icon);
            GoToNextCommandMenu();
        }

        private void CancelTargetSelectionMenu()
        {
            ShowDescriptionDisplay(false);
            commandList.memberRef.HighlightImageRef(true);
            //commandListHUD[commandMenuIndex].memberRef.HighlightImageRef(true);
            unitHUD[commandMenuIndex].UpdateCommandIcon(null);
            ResetTargetSelectionMenu();
        }
        private void TargetSelectionReturnToSubmenu()
        {
            ShowDescriptionDisplay(true);
            ToggleStatusHUD(false);
            commandSubmenuHUD.uiMenu.ForceDisplayTextFromHighlight();
            HidePrimaryWindowContentExcept(PrimaryWindowContent.CommandSubmenu);
        }
        private void TargetSelectionReturnToCommandList()
        {
            ShowDescriptionDisplay(true);
            ToggleStatusHUD(true);
            UpdateSelectedStatusHUD(BattleManager.instance.activeParty[commandMenuIndex]);
            commandList.uiMenu.ForceDisplayTextFromHighlight();
            //commandListHUD[commandMenuIndex].uiMenu.ForceDisplayTextFromHighlight();
        }
        private void ResetTargetSelectionMenu()
        {
            targetSelectionMenu.closeMenuWithCancel = true;
            targetSelectionMenu.onCancelMenu.RemoveListener(CancelTargetSelectionMenu);
            targetSelectionMenu.onCancelMenu.RemoveListener(TargetSelectionReturnToSubmenu);
            targetSelectionMenu.onCancelMenu.RemoveListener(TargetSelectionReturnToCommandList);
            foreach (Transform child in targetSelectionMenu.transform) //Wipe existing buttons
            { 
                Destroy(child.gameObject);
            }
        }
        public void CommandSelect(PartyMember user, int index, Command command)
        {
            Skill skill; //If command is Single, go to next List, or, goto pick target menu and then goto next List. If group, open that command group.
            if (command.commandType == CommandType.Single) skill = command.skills[0].skill;
            else if (command.commandType == CommandType.Escape)
            {
                EscapeBattle();
                return;
            }
            else if (command.IsSubmenuType())
            {
                OpenCommandSubmenu(command, user);
                return;
            }
            //else if (command.commandType == CommandType.Group)
            //{
            //    OpenCommandSubmenu(command, user);
            //    return;
            //}
            //else if (command.commandType == CommandType.Items)
            //{
            //    OpenCommandSubmenu(command, user);
            //    return;
            //}
            else return;
            var validTargets = BattleManager.instance.GetInvocationValidTargets(user, skill.scopeData);
            unitHUD[commandMenuIndex].UpdateCommandIcon(skill.icon); //if command is group, dont update yet
            if (validTargets.Count != 0) //if there's valid targets, go to TargetSelectionMenu
            {
                TargetSelectionMenu(validTargets, skill, user, false, command.commandType == CommandType.Single);
            }
            else
            {
                BattleManager.instance.QueueOrReplaceCommand(skill, null, user, unitHUD[commandMenuIndex]);
                ShowDescriptionDisplay(false);
                GoToNextCommandMenu();
            } 
        }
        public void SkipCommandMenu(int direction, int commandListIndex, UIMenu uiMenu)
        {
            int dir = LISAUtility.Sign(direction);
            if (dir > 0)
            {
                uiMenu.ForcePlayHighlightClip();
                GoToNextCommandMenu();
            }
            if(dir < 0)
            {
                if (GetPrevActableUnitIndex(commandListIndex - 1) >= 0) uiMenu.ForcePlayHighlightClip();
                GoToPrevCommandMenu();
            }
        }
        public void OpenCommandSubmenu(Command command, PartyMember user)
        {
            ShowDescriptionDisplay(true);
            HidePrimaryWindowContentExcept(PrimaryWindowContent.CommandSubmenu);
            HideSecondWindowContentExcept(SecondWindowContent.SelectedUnit);
            UpdateSelectedUnitHUD(user);
            ToggleStatusHUD(false);
            commandSubmenuHUD.UpdateSubmenu(command, user);
            commandSubmenuHUD.uiMenu.OpenMenu();
        }
        public void CloseCommandSubmenu()
        {
            ChangeWindowsOnSubmenuClosing();
            ToggleStatusHUD(true);
            commandSubmenuHUD.uiMenu.CloseMenu();
        }
        public void CancelCommandSubmenu()
        {
            //ShowDescriptionDisplay(false);
            ChangeWindowsOnSubmenuClosing();
            ToggleStatusHUD(true);
            UnhighlightAll();
            UnhighlightMembersExcept(commandMenuIndex);
        }
        private void ChangeWindowsOnSubmenuClosing()
        {
            HidePrimaryWindowContentExcept(PrimaryWindowContent.UnitsInfo);
            HideSecondWindowContentExcept(SecondWindowContent.CommandsMenus);
        }
        public void CommandCancel()
        {
            //unitHUD[commandMenuIndex].UpdateCommandIcon(null);
            //BattleManager.instance.RemoveLastCommand();
            GoToPrevCommandMenu();
        }
        private void GoToNextCommandMenu()
        {
            NextCommandList();
            if (commandMenuIndex >= PlayerData.instance.GetActivePartySize())
            {
                Debug.Log("No Commands After");
                EndPlayerActions();
            }
        }
        public void NextCommandList()
        {
            commandList.memberRef.HighlightImageRef(false);
            //commandListHUD[commandMenuIndex].memberRef.HighlightImageRef(false);
            commandList.uiMenu.CloseMenu();
            SetNextActableUnitIndex(commandMenuIndex + 1);
            if(commandMenuIndex < GameManager.instance.playerData.GetActivePartySize()) 
                SetCommandListMenu(commandMenuIndex);
        }
        private void EscapeBattle()
        {
            ResetCommandListUI();
            HideAll();
            BattleManager.instance.EscapeBattle();
        }
        private void EndPlayerActions()
        {
            ResetCommandListUI();
            BattleManager.instance.RunBattleActions();
        }

        private void ResetCommandListUI()
        {
            commandList.uiMenu.CloseMenu();
            commandList.memberRef.HighlightImageRef(false);
            DisplayWindow(secondaryWindow, false);
            ToggleStatusHUD(false);
            ShowDescriptionDisplay(false);
        }

        private void GoToPrevCommandMenu()
        {
            PreviousCommandList();
        }
        public void PreviousCommandList()
        {
            int prev = GetPrevActableUnitIndex(commandMenuIndex - 1);
            if (prev < 0)
            {
                return;
            }
            commandList.uiMenu.CloseMenu();
            commandMenuIndex = prev;
            //SetPrevActableUnitIndex(commandMenuIndex - 1);
            SetCommandListMenu(commandMenuIndex);
        }
        void SetCommandListMenu(int index)
        {
            commandMenuIndex = index;
            UpdateCommandsList();
            UpdateSelectedStatusHUD(BattleManager.instance.activeParty[commandMenuIndex]);
            commandList.uiMenu.OpenMenu();
            //commandListHUD[commandMenuIndex].uiMenu.OpenMenu();
            UnhighlightMembersExcept(commandMenuIndex);
            //HideCommandsMenusExcept(commandMenuIndex);
        }
        public void TintScreen(Color color, float duration)
        {
            tintScreenFlash?.Flash(color, duration);
        }
        public void UpdateCommandIcon(UnitHUD unitHUD, Sprite icon)
        {
            if (unitHUD == null) return;
            unitHUD.UpdateCommandIcon(icon);
        }
        public void DisplayHit(BattleAnimationEvent hitInfo, int value, int targetIndex, bool isCrit = false)
        {
            if (!BattleManager.instance.InBattle) return;
            AddToHitDisplayGroup(hitInfo, value, targetIndex, isCrit);
            if (hitInfo.skillOrigin.targets[targetIndex] is EnemyInstance)
                ShowEnemyHPBar(hitInfo.skillOrigin.targets[targetIndex] as EnemyInstance);
        }
        public void DisplayRegen(Targetable target, int value, DamageType damageType)
        {
            if (!BattleManager.instance.InBattle) return;
            AddRegenToHitDisplayGroup(target, value, damageType);
            if (target is EnemyInstance)
                ShowEnemyHPBar(target as EnemyInstance, false, true);
        }
        public void DisplayState(Targetable target, State state, bool removal = false)
        {
            if (!BattleManager.instance.InBattle) return;
            AddStateToHitDisplayGroup(target, state, removal);
            if (target is EnemyInstance)
                ShowEnemyHPBar(target as EnemyInstance);
        }
        public void DisplayVulnerability(Targetable target, int elementIndex, VulnerabilityType vulType)
        {
            if (!BattleManager.instance.InBattle) return;
            AddVulnerabilityToHitDisplayGroup(target, elementIndex, vulType);
        }
        public void ShowEnemyHPBar(EnemyInstance target, bool infDisplayTime = false, bool forceShow = false)
        {
            if (!BattleManager.instance.InBattle) return;
            if (target == null) return;
            if ((target.prevHP <= 0 && !forceShow) && !infDisplayTime) return;
            if (!target.CanShowStatus()) return;
            for (int i = 0; i < enemyHPBars.Count; i++)
            {
                if (enemyHPBars[i].target == target)
                {
                    enemyHPBars[i].ShowBar(target, this, infDisplayTime);
                    return;
                }
            }
            
            var enemyHPBarGO = Instantiate(TUFFSettings.enemyHPBar, overlayInfo);
            var enemyHPBar = enemyHPBarGO.GetComponent<EnemyBarHandler>();
            enemyHPBars.Add(enemyHPBar);
            enemyHPBar.ShowBar(target, this, infDisplayTime);
        }
        public void RemoveEnemyHPBar(EnemyBarHandler enemyHPBar)
        {
            for (int i = 0; i < enemyHPBars.Count; i++)
            {
                if (enemyHPBars[i] == enemyHPBar)
                {
                    Destroy(enemyHPBar.gameObject);
                    enemyHPBars.RemoveAt(i);
                    return;
                }
            }
        }
        public void RemoveEnemyHPBar(EnemyInstance enemy)
        {
            for (int i = 0; i < enemyHPBars.Count; i++)
            {
                if (enemyHPBars[i].target == enemy)
                {
                    Destroy(enemyHPBars[i].gameObject);
                    enemyHPBars.RemoveAt(i);
                    return;
                }
            }
        }
        private void AddToHitDisplayGroup(BattleAnimationEvent hitInfo, int value, int targetIndex, bool isCrit = false)
        {
            //Search for existing group
            for (int i = 0; i < hitDisplayGroups.Count; i++)
            {
                if (hitDisplayGroups[i].target == hitInfo.skillOrigin.targets[targetIndex])
                {
                    hitDisplayGroups[i].AddHitDisplay(hitInfo, value, isCrit); //Add new hitDisplay with value
                    return;
                }
            }
            //If not, add new group and add hitDisplay with value
            HitDisplayGroup hitDisplayGroup = CreateNewHitDisplayGroup(hitInfo.skillOrigin.targets[targetIndex]);
            hitDisplayGroup.AddHitDisplay(hitInfo, value, isCrit);
        }
        public void AddRegenToHitDisplayGroup(Targetable target, int value, DamageType damageType)
        {
            for (int i = 0; i < hitDisplayGroups.Count; i++)
            {
                if (hitDisplayGroups[i].target == target)
                {
                    hitDisplayGroups[i].AddRegenDisplay(value, damageType); //Add new hitDisplay with value
                    return;
                }
            }
            //If not, add new group and add hitDisplay with value
            HitDisplayGroup hitDisplayGroup = CreateNewHitDisplayGroup(target);
            hitDisplayGroup.AddRegenDisplay(value, damageType);
        }
        public void AddMissToHitDisplayGroup(Targetable target)
        {
            //Search for existing group
            for (int i = 0; i < hitDisplayGroups.Count; i++)
            {
                if (hitDisplayGroups[i].target == target)
                {
                    hitDisplayGroups[i].AddMissDisplay(); //Add new hitDisplay with value
                    return;
                }
            }
            //If not, add new group and add hitDisplay with value
            HitDisplayGroup hitDisplayGroup = CreateNewHitDisplayGroup(target);
            hitDisplayGroup.AddMissDisplay();
        }
        public void AddStateToHitDisplayGroup(Targetable target, State state, bool removal = false)
        {
            //Search for existing group
            for (int i = 0; i < hitDisplayGroups.Count; i++)
            {
                if (hitDisplayGroups[i].target == target)
                {
                    hitDisplayGroups[i].AddStateDisplay(state, removal); //Add new hitDisplay with value
                    return;
                }
            }
            //If not, add new group and add hitDisplay with value
            HitDisplayGroup hitDisplayGroup = CreateNewHitDisplayGroup(target);
            hitDisplayGroup.AddStateDisplay(state, removal);
        }
        public void AddVulnerabilityToHitDisplayGroup(Targetable target, int elementIndex, VulnerabilityType vulType)
        {
            //Search for existing group
            for (int i = 0; i < hitDisplayGroups.Count; i++)
            {
                if (hitDisplayGroups[i].target == target)
                {
                    hitDisplayGroups[i].AddVulnerabilityDisplay(elementIndex, vulType); //Add new hitDisplay with value
                    return;
                }
            }
            //If not, add new group and add hitDisplay with value
            HitDisplayGroup hitDisplayGroup = CreateNewHitDisplayGroup(target);
            hitDisplayGroup.AddVulnerabilityDisplay(elementIndex, vulType);
        }
        private HitDisplayGroup CreateNewHitDisplayGroup(Targetable target)
        {
            var hitDisplayGroupGO = Instantiate(TUFFSettings.hitDisplayGroup, overlayInfo);
            var hitDisplayGroup = hitDisplayGroupGO.GetComponent<HitDisplayGroup>();
            hitDisplayGroup.CreateGroup(target, this);
            hitDisplayGroups.Add(hitDisplayGroup);
            return hitDisplayGroup;
        }

        public void RemoveHitDisplayGroup(HitDisplayGroup group)
        {
            for (int i = 0; i < hitDisplayGroups.Count; i++)
            {
                if (hitDisplayGroups[i] == group)
                {
                    hitDisplayGroups.RemoveAt(i);
                    return;
                }
            }
        }
        public static Color GetVulnerabilityColor(VulnerabilityType vulType)
        {
            switch (vulType)
            {
                case VulnerabilityType.Weakpoint:
                    return TUFFSettings.weakpointTextColor;
                case VulnerabilityType.Resist:
                    return TUFFSettings.resistTextColor;
                case VulnerabilityType.Immune:
                    return TUFFSettings.immuneTextColor;
            }
            return Color.white;
        }
        public static string GetVulnerabilityText(VulnerabilityType vulType)
        {
            switch (vulType)
            {
                case VulnerabilityType.Weakpoint:
                    return TUFFSettings.weakpointText;
                case VulnerabilityType.Resist:
                    return TUFFSettings.resistText;
                case VulnerabilityType.Immune:
                    return TUFFSettings.immuneText;
            }
            return "";
        }
    }
}