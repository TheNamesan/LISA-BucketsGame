using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeLevelAction : EventAction
    {
        [Tooltip("Specifies the Unit's Level to change.")]
        public PartyScope scope = PartyScope.EntireParty;
        [Tooltip("Unit to add level to.")]
        public Unit unit;
        public AddSetOperationType operation = AddSetOperationType.Add;

        [Header("Operand")]
        [Tooltip("Amount of level to change.")]
        public NumberOperand operand = new();

        public ChangeLevelAction()
        {
            eventName = "Change Level";
            eventColor = EventGUIColors.unit;
        }
        public override void Invoke()
        {
            int value = (int)operand.GetNumber();
            if (scope == PartyScope.EntireParty)
            {
                var playerParty = PlayerData.instance.GetAllPartyMembers();
                for (int i = 0; i < playerParty.Count; i++)
                {
                    CalculateValue(playerParty[i], value);
                }
            }
            else if (scope == PartyScope.OnePartyMember)
            {
                if (unit == null) { isFinished = true; return; }
                var member = PlayerData.instance.GetPartyMember(unit);
                CalculateValue(member, value);
            }
            isFinished = true;
        }
        private void CalculateValue(PartyMember member, int value)
        {
            if (member == null) return;
            if (operation == AddSetOperationType.Add)
                member.AddLevel(value);
            if (operation == AddSetOperationType.Set)
                member.SetLevel(value);
        }
    }
}
