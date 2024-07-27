using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TUFF
{
    public class CommandSubmenuElement : GeneralInfoDisplay
    {
        private IBattleInvocation invocation;

        public void Initialize(CommandSubmenuHUD commandSubmenuHUD)
        {
            if (uiElement)
            {
                uiElement.onHighlight.AddListener(() => commandSubmenuHUD.battleHUD.ShowDescriptionDisplay(true));
                uiElement.onHighlight.AddListener(() => OnHighlightMarkVulnerableTargets(commandSubmenuHUD));
                uiElement.onSelect.AddListener(() => GoToTargetSelectionMenu(commandSubmenuHUD));
            }
        }
        private void OnHighlightMarkVulnerableTargets(CommandSubmenuHUD commandSubmenuHUD)
        {
            var validTargets = BattleManager.instance.GetInvocationValidTargets(commandSubmenuHUD.memberRef, invocation.ScopeData);
            commandSubmenuHUD.battleHUD.MarkVulnerableTargets(invocation, commandSubmenuHUD.memberRef, validTargets);
        }
        private void GoToTargetSelectionMenu(CommandSubmenuHUD commandSubmenuHUD)
        {
            var validTargets = BattleManager.instance.GetInvocationValidTargets(commandSubmenuHUD.memberRef, invocation.ScopeData);
            commandSubmenuHUD.battleHUD.TargetSelectionMenu(validTargets, invocation, commandSubmenuHUD.memberRef, true);
        }
        public void SetInvocation(IBattleInvocation setSkill)
        {
            invocation = setSkill;
        }
        public IBattleInvocation GetInvocation()
        {
            return invocation;
        }
        public void LoadInvocationInfo(Targetable user)
        {
            if (invocation == null)
            {
                DisplayInfo(null, "", iconActive: false, textActive: false);
            }
            else if (invocation is Skill)
            {
                var skl = invocation as Skill;
                var SPCost = skl.GetSPCost(user);
                var TPCost = skl.GetTPCost(user);
                bool costsSP = SPCost > 0;
                bool costsTP = TPCost > 0;
                bool costsItem = skl.requiredItem != null;
                DisplayInfo(skl.icon, 
                    skl.GetName(), 
                    (costsSP ? LISAUtility.IntToString(SPCost) : ""), 
                    (costsTP ? LISAUtility.IntToString(TPCost) : ""),
                    (costsItem ? $"x{Inventory.instance.GetItemAmount(skl.requiredItem)}" : ""),
                    SPCostActive: costsSP, 
                    TPCostActive: costsTP,
                    usesTextActive: costsItem);
            }
            else if (invocation is Item)
            {
                DisplayInfo(invocation.icon, invocation.GetName(), usesText: $"x{GameManager.instance.playerData.inventory.items[invocation.databaseElement.id]}",
                    usesTextActive: true);
            }
        }
    }
}

