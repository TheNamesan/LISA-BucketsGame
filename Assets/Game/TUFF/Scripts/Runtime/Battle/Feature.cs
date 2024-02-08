using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public enum FeatureType
    {
        StatChange = 0,
        ExtraRateChange = 1,
        SpecialRateChange = 2,
        ElementPotency = 3,
        ElementVulnerability = 4,
        StatePotency = 5,
        StateVulnerability = 6,
        StateImmunity = 7,
        AddHitElement = 8,
        AddStateOnHit = 9,
        AttackSpeed = 10,
        AttackTimes = 11,
        AddCommand = 12,
        SealCommand = 13,
        AddSkill = 14,
        SealSkill = 15,
        AutoState = 16,
        SpecialFeature = 17
    }
    public enum StatChangeType
    {
        MaxHP = 0,
        MaxSP = 1,
        MaxTP = 2,
        ATK = 3,
        DEF = 4,
        SATK = 5,
        SDEF = 6,
        AGI = 7,
        LUK = 8
    }
    public enum ExtraRateChangeType
    {
        HitRate = 0,
        EvasionRate = 1,
        CritRate = 2,
        CritEvasionRate = 3,
        HPRegenRate = 4,
        SPRegenRate = 5,
        TPRegenRate = 6
    }
    public enum SpecialRateChangeType
    {
        TargetRate = 0,
        GuardPotency = 1,
        HealingDealtPotency = 2, // Amplifies HP recovery inflicted by the user
        HealingReceivedPotency = 3, // Amplifies HP recovery received to the user
        ItemDealtPotency = 4, // Amplifies HP recovery inflicted by the user
        ItemReceivedPotency = 5, // Amplifies HP recovery received to the user
        SPCostRate = 6,
        TPCostRate = 7,
        PhysicalDamagePotency = 8,
        PhysicalDamageVulnerability = 9,
        SpecialDamagePotency = 10,
        SpecialDamageVulnerability = 11
    }
    public enum SpecialFeatureType
    {
        Guard = 0,
        ShowStatus = 1,
        HideStatus = 2,
        Death = 3,
        ImmuneToKO = 4,
        Untargetable = 5
    }
    [System.Serializable]
    public class Feature
    {
        [Tooltip("The modifier to apply.\nStat Change: Affects the user's base stats. Stats cannot go below 1. Default: 100%.\n" +
            "Extra Rate Change: Affects the user's base Extra Rates. Default: 0.\n" +
            "Special Rate Change: Affects the user's Special Rates. Default: 100%.\n" +
            "Element Potency: Multiplier to damage dealt by attacks of the specified Element. Default: 100%.\n" +
            "Element Vulnerability: Multiplier to damage received by attacks of the specified Element. Default: 100%.\n" +
            "State Potency: Multiplier to chance for the user to inflict the specified State. Default: 100%.\n" +
            "State Vulnerability: Multiplier to chance for the user to be inflicted with the specified State. Default: 100%.\n" +
            "State Immunity: Grants complete immunity to being inflicted with the specified State and removes it if it's already applied.\n" +
            "Add Hit Element: Adds an Element type to all inflicted attacks.\n" +
            "Add State On Hit: Adds a chance for all hit attacks to inflict a State.\n" +
            "Attack Speed: Adds a value to the user's AGI when calculating the attack order.\n" +
            "Attack Times: Increases the amount of times a skill cast by the user repeats.\n" +
            "Auto State: Applies a state with infinite duration to the user while the feature is active.\nSpecial Feature: Has varied behaviors.")]
        public FeatureType featureType = FeatureType.StatChange;
        public StatChangeType statChange = StatChangeType.MaxHP;
        public ExtraRateChangeType extraRateChange = ExtraRateChangeType.HitRate;
        [Tooltip("Guard Potency: Multiplies the damage reduced while the user's guarding. Formula is [damage / ((1 / (1 - BaseGuardDmgReduction)) * Guard Potency%)].")]
        public SpecialRateChangeType specialRateChange = SpecialRateChangeType.TargetRate;
        [Tooltip("Guard: Reduces HP Damage received by the Base Guard Damage Reduction. Affected by Guard Potency.\nShow HP: Shows the user's statuses.\nHide HP: Hides the user's statuses.\nDeath: Removes the user from the player's party.\nImmune To KO: The user can act even when their HP reaches 0.\nUntargetable: User cannot be targeted by attacks, including attacks that target random people.")]
        public SpecialFeatureType specialFeature = SpecialFeatureType.Guard;
        [Tooltip("Percentage multiplier to stat. Default: 100%.")]
        [Range(0, 1000)] public int statChangeValue = 100;
        [Tooltip("Flat value added to Extra Rate. Default: 0.")]
        [Range(-100, 100)] public int extraRateChangeValue = 0;
        [Tooltip("Percentage multiplier to Special Rate. Default: 100%.")]
        [Range(0, 1000)] public int specialRateChangeValue = 100;
        [Tooltip("Percentage damage multiplier to attacks of Element. Default: 100%.")]
        [Range(0, 1000)] public int elementValue = 100;
        [Tooltip("Percentage multiplier to chance to inflict State. Default: 100%.")]
        [Range(0, 10000)] public int stateValue = 100;
        [Tooltip("Element target.")]
        public int element = 0;
        [Tooltip("State target.")]
        public State state = null;
        [Tooltip("Attack Speed constant to add.")]
        public int attackSpeed = 0;
        [Tooltip("Command target.")]
        public Command command = null;
        [Tooltip("Skill target.")]
        public Skill skill = null;
        [Tooltip("State to apply.")]
        public State autoState = null;
       
    }
}


