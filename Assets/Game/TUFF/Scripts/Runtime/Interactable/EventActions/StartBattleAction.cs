using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class StartBattleAction : EventAction
    {
        [Tooltip("Battle prefab to trigger.")]
        public Battle battle;

        [Tooltip("Allows the user to use Escape commands and adds a branch If Escape.")]
        public bool canEscape = false;
        [Tooltip("Adds a branch when the party is wiped.")]
        public bool continueOnLose = false;

        public ActionList winActionList = new();
        public ActionList escapeActionList = new();
        public ActionList loseActionList = new();
        public StartBattleAction()
        {
            eventName = "Start Battle";
            eventColor = EventGUIColors.scene;
        }
        public override void Invoke()
        {
            if (battle != null) BattleManager.instance.InitiateBattle(battle, canEscape, this);
            else
            {
                Debug.LogWarning($"No Battle set for {eventName} event.");
                isFinished = true;
            }
        }
        public void OnBattleEnd(BattleState endBattleState)
        {
            if (!UsesBranches()) { isFinished = true; return; }
            if (endBattleState == BattleState.ESCAPED && canEscape)
            {
                CommonEventManager.instance.TriggerEventActionBranch(this, escapeActionList);
            }
            else if (endBattleState == BattleState.LOST  && continueOnLose)
            {
                CommonEventManager.instance.TriggerEventActionBranch(this, loseActionList);
            }
            else
                CommonEventManager.instance.TriggerEventActionBranch(this, winActionList);
        }
        public bool UsesBranches()
        {
            return canEscape || continueOnLose;
        }
    }
}

