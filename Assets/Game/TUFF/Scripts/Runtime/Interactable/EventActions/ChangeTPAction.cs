using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeTPAction : EventAction
    {
        [Tooltip("Specifies the Unit's TP to change.")]
        public PartyScope scope = PartyScope.EntireParty;
        [Tooltip("Reference to the Unit.")]
        public Unit unit;
        public AddSetOperationType operation = AddSetOperationType.Add;

        [Header("Operand")]
        [Tooltip("Constant TP value to change.")]
        public int constant = 0;
        public ChangeTPAction()
        {
            eventName = "Change TP";
            eventColor = new Color(0.8f, 0.8f, 1f, 1f);
        }
        public override void Invoke()
        {
            if (scope == PartyScope.EntireParty)
            {
                var playerParty = PlayerData.instance.GetAllPartyMembers();
                for (int i = 0; i < playerParty.Count; i++)
                {
                    CalculateValue(playerParty[i]);
                }
            }
            else if (scope == PartyScope.OnePartyMember)
            {
                if (unit == null) { isFinished = true; return; }
                var member = PlayerData.instance.GetPartyMember(unit);
                CalculateValue(member);
            }
            isFinished = true;
        }

        private void CalculateValue(PartyMember member)
        {
            if (operation == AddSetOperationType.Add)
                member.CalculateTP(constant);
            else if (operation == AddSetOperationType.Set)
                member.SetTP(constant);
        }
    }
}

