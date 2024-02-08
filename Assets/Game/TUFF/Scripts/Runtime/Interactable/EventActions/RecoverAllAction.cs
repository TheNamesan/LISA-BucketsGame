using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class RecoverAllAction : EventAction
    {
        [Tooltip("Specifies the Unit to recover.")]
        public PartyScope scope = PartyScope.EntireParty;
        [Tooltip("Reference to the Unit.")]
        public Unit unit;
        [Tooltip("If true, the user will recover from permanent states.")]
        public bool curePermanentStates = false;
        public RecoverAllAction()
        {
            eventName = "Recover All";
            eventColor = new Color(0.8f, 0.8f, 1f, 1f);
        }
        public override void Invoke()
        {
            if (scope == PartyScope.EntireParty)
            {
                var playerParty = PlayerData.instance.GetAllPartyMembers();
                for (int i = 0; i < playerParty.Count; i++)
                {
                    playerParty[i].RecoverAll(curePermanentStates);
                }
            }
            else if (scope == PartyScope.OnePartyMember)
            {
                if (unit == null) { isFinished = true; return; }
                var member = PlayerData.instance.GetPartyMember(unit);
                member.RecoverAll(curePermanentStates);
            }
            isFinished = true;
        }
    }
}

