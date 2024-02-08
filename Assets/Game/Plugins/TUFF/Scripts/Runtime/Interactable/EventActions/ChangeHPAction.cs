using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeHPAction : EventAction
    {
        [Tooltip("Specifies the Unit's HP to change.")]
        public PartyScope scope = PartyScope.EntireParty;
        [Tooltip("Reference to the Unit.")]
        public Unit unit;
        public AddSetOperationType operation = AddSetOperationType.Add;

        [Header("Operand")]
        [Tooltip("Constant HP value to change.")]
        public int constant = 0;
        public ChangeHPAction()
        {
            eventName = "Change HP";
            eventColor = EventGUIColors.unit;
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
                member.RecoverHP(constant, false, true);
            else if (operation == AddSetOperationType.Set)
                member.SetHP(constant);
        }
    }
}

