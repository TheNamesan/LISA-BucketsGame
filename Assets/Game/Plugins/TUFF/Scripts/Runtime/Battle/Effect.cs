using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public enum EffectType
    {
        RecoverHP = 0,
        RecoverSP = 1,
        RecoverTP = 2,
        AddState = 3,
        RemoveState = 4,
        SpecialEffect = 5,
        BonusStats = 6,
        LearnSkill = 7,
        ForgetSkill = 8,
        CommonEvent = 9,
        QueueSkill = 10 // Make this work with Combo Dial and others
    }
    public enum SpecialEffectType
    { 
        RemoveKO = 0,
        EscapeBattle = 1
    }
    [System.Serializable]
    public class Effect
    {
        [Tooltip("The effect to apply on the target.\n" +
            "Recover HP/SP/TP: Recovers a % of the user’s Max HP/SP/TP.\n" +
            "Add/Remove State: Adds a chance for the hit to apply/remove a state. Applying chance is affected by state resistance.\n" +
            "Special Effect: Has varied behaviors.\n" +
            "Queue Skill: Adds a skill to the battle queue after this skill with the same user and targets.\n")]
        public EffectType effectType = EffectType.RecoverHP;
        [Tooltip("The % to recover.")]
        [Range(0, 100)] public int recoverPercent = 0;
        [Tooltip("Flat value to recover.")]
        public int recoverFlat = 0;
        [Tooltip("Reference to the state to apply.")]
        public State state;
        [Tooltip("The probability % of applying/removing the state.")]
        [Range(0, 100)] public int stateTriggerChance = 0;
        [Tooltip("Remove KO: Recovers the target from KO. Target also recovers 1 HP.\nEscape Battle: Battle ends with a Escape. No EXP or Rewards are given.")]
        public SpecialEffectType specialEffect = SpecialEffectType.RemoveKO;
        [Tooltip("Reference to skill.")]
        public Skill skill;
        [Tooltip("Common Event to trigger.")]
        public CommonEvent commonEvent;
        [Tooltip("Skill to queue during battle.")]
        public Skill skillToQueue; // Make this work with Combo Dial and others
    }
}

