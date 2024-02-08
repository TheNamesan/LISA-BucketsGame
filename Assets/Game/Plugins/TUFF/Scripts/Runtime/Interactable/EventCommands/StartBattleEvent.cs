using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "EVTStartBattle", menuName = "TUFF/Events/Start Battle Event")]
    public class StartBattleEvent : EventCommand
    {
        [Tooltip("Battle prefab to trigger.")]
        public Battle battle;
        public override void Invoke()
        {
            if (battle != null) ;//BattleManager.instance.InitiateBattle(battle, this);
            else
            {
                Debug.LogWarning($"No Battle set for {eventName} event.");
                isFinished = true;
            }
        }

        public override void OnInstantiate()
        {
            eventName = "Start Battle";
            eventColor = new Color(0.5f, 1f, 0.95f, 1f);
        }
    }
}

