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
        public StartBattleAction()
        {
            eventName = "Start Battle";
            eventColor = new Color(0.5f, 1f, 0.95f, 1f);
        }
        public override void Invoke()
        {
            if (battle != null) BattleManager.instance.InitiateBattle(battle, this);
            else
            {
                Debug.LogWarning($"No Battle set for {eventName} event.");
                isFinished = true;
            }
        }
    }
}

