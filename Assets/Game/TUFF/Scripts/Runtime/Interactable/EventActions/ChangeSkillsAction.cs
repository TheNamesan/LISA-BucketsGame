using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ChangeSkillsAction : EventAction
    {
        [Tooltip("Specifies the Unit's skill to learn/forget.")]
        public PartyScope scope = PartyScope.EntireParty;
        [Tooltip("Reference to the Unit.")]
        public Unit unit;
        [Tooltip("Reference to the Skill.")]
        public Skill skill;
        public LearnOperationType operation = LearnOperationType.Learn;
        public ChangeSkillsAction()
        {
            eventName = "Change Skills";
            eventColor = new Color(0.8f, 0.8f, 1f, 1f);
        }
        public override void Invoke()
        {
            bool learn = (operation == LearnOperationType.Learn ? true : false);
            if (scope == PartyScope.EntireParty)
            {
                var playerParty = PlayerData.instance.GetAllPartyMembers();
                for (int i = 0; i < playerParty.Count; i++)
                {
                    playerParty[i].LearnSkill(skill, learn);
                }
            }
            else if (scope == PartyScope.OnePartyMember)
            {
                if (unit == null) { isFinished = true; return; }
                var member = PlayerData.instance.GetPartyMember(unit);
                member.LearnSkill(skill, learn);
            }
            isFinished = true;
        }
    }
}

