using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangePartyAction : EventAction
    {
        [Tooltip("Reference to the Unit to add as a party member.")]
        public Unit unit;
        public OperationType operation = OperationType.Add;
        [Tooltip("If true, the party member will join reverted into its initial Unit properties.")]
        public bool initialize = false;
        public ChangePartyAction()
        {
            eventName = "Change Party";
            eventColor = new Color(1f, 0.8f, 0.25f, 1f);
        }
        public override void Invoke()
        {
            if (unit == null) { isFinished = true; return; }
            if (operation == OperationType.Add) GameManager.instance.playerData.AddToParty(unit.id, initialize);
            else if (operation == OperationType.Remove) GameManager.instance.playerData.RemoveFromParty(unit.id);
            isFinished = true;
        }
    }
}

