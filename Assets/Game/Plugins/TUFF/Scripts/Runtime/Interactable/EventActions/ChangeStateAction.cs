using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeStateAction : EventAction
    {
        [Tooltip("Specifies the Unit's State to change.")]
        public PartyScope scope = PartyScope.EntireParty;
        [Tooltip("Reference to the Unit.")]
        public Unit unit;
        public OperationType operation = OperationType.Add;
        [Tooltip("State to apply/remove.")]
        public PartyStateScope stateTarget = PartyStateScope.OneState;
        [Tooltip("Reference to the State.")]
        public State state;
        public ChangeStateAction()
        {
            eventName = "Change State";
            eventColor = new Color(0.8f, 0.8f, 1f, 1f);
        }
        public override void Invoke()
        {
            if (scope == PartyScope.EntireParty)
            {
                var playerParty = PlayerData.instance.GetAllPartyMembers();
                for (int i = 0; i < playerParty.Count; i++)
                {
                    ApplyRemoveState(playerParty[i]);
                }
            }
            else if (scope == PartyScope.OnePartyMember)
            {
                if (unit == null) { isFinished = true; return; }
                var member = PlayerData.instance.GetPartyMember(unit);
                ApplyRemoveState(member);
            }
            isFinished = true;
        }
        private void ApplyRemoveState(PartyMember member)
        {
            if (operation == OperationType.Add)
            {
                switch(stateTarget)
                {
                    case PartyStateScope.OneState:
                        if (state == null) break;
                        member.ApplyState(state); break;
                    case PartyStateScope.AllBuffs:
                        member.ApplyAllStatesOfType(StateType.Buff); break;
                    case PartyStateScope.AllDebuffs:
                        member.ApplyAllStatesOfType(StateType.Debuff); break;
                    case PartyStateScope.AllPermanents:
                        member.ApplyAllStatesOfType(StateType.Permanent); break;
                    default:
                        break;
                }
            }
            else if (operation == OperationType.Remove)
            {
                switch (stateTarget)
                {
                    case PartyStateScope.OneState:
                        if (state == null) break;
                        member.RemoveState(state);
                        break;
                    case PartyStateScope.AllBuffs:
                        member.RemoveAllStatesOfType(StateType.Buff); break;
                    case PartyStateScope.AllDebuffs:
                        member.RemoveAllStatesOfType(StateType.Debuff); break;
                    case PartyStateScope.AllPermanents:
                        member.RemoveAllStatesOfType(StateType.Permanent); break;
                    default:
                        break;
                }
            }
        }
    }
}