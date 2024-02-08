using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ChangePartyEvent : EventCommand
    {
        [Tooltip("Reference to the Unit to add as party member.")]
        public Unit unit;
        public OperationType operation = OperationType.Add;
        [Tooltip("If true, the party member will join reverted into its initial Unit properties.")]
        public bool initialize = false;
        public override void Invoke()
        {
            if (unit == null) { isFinished = true; return; }
            if (operation == OperationType.Add) GameManager.instance.playerData.AddToParty(unit.id, initialize);
            else if (operation == OperationType.Remove) GameManager.instance.playerData.RemoveFromParty(unit.id);
            isFinished = true;
        }
        public override void OnInstantiate()
        {
            eventName = "Change Party";
            eventColor = new Color(1f, 0.8f, 0.25f, 1f);
        }
    }
}

