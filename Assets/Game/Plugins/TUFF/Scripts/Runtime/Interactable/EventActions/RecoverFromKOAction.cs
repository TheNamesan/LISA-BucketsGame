using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class RecoverFromKOAction : EventAction
    {
        [Tooltip("Specifies the Unit to recover from KO.")]
        public PartyScope scope = PartyScope.EntireParty;
        [Tooltip("Reference to the Unit.")]
        public Unit unit;
        public RecoverFromKOAction()
        {
            eventName = "Recover From KO";
            eventColor = new Color(0.8f, 0.8f, 1f, 1f);
        }
        public override void Invoke()
        {
            if (scope == PartyScope.EntireParty)
            {
                var playerParty = PlayerData.instance.GetAllPartyMembers();
                for (int i = 0; i < playerParty.Count; i++)
                {
                    playerParty[i].RemoveKO();
                }
            }
            else if (scope == PartyScope.OnePartyMember)
            {
                if (unit == null) { isFinished = true; return; }
                var member = PlayerData.instance.GetPartyMember(unit);
                member.RemoveKO();
            }
            isFinished = true;
        }
    }
}

