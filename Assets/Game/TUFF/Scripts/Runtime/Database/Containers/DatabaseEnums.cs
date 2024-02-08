using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    /// <summary>
    /// Center: Pivot is (0, 0).
    /// Top: Pivot is (0, Graphic Height / 2).
    /// Bottom: Pivot is (0, -Graphic Height / 2).
    /// Screen: Pivot is always the middle of the screen.
    /// </summary>
    public enum AnimationPivotType
    {
        Center = 0,
        Top = 1,
        Bottom = 2,
        Screen = 3
    }
    public enum ScopeType
    {
        None = 0,
        TheUser = 1,
        OneEnemy = 2,
        AllEnemies = 3,
        RandomEnemies = 4,
        OneAlly = 5,
        AllAllies = 6,
        RandomAllies = 7,
        Anyone = 8,
        Everyone = 9,
        RandomAnyone = 10
    }

    public enum ConditionScopeType
    {
        OnlyAlive = 0,
        OnlyKO = 1,
        AliveAndKO = 2
    }

    public enum OccasionType
    {
        Always = 0,
        OnlyInBattle = 1,
        OnlyFromMenu = 2,
        Never = 3
    }
    public enum MultihitTimingType
    {
        AllAtOnce = 0,
        FirstToLast = 1,
        LastToFirst = 2
    }
    public enum HitType
    {
        PhysicalAttack = 0,
        SpecialAttack = 1
    }

    public enum DamageType
    {
        None = 0,
        HPDamage = 1,
        SPDamage = 2,
        TPDamage = 3,
        HPRecover = 4,
        SPRecover = 5,
        TPRecover = 6,
        HPDrain = 7,
        SPDrain = 8
    }
    public enum TPRecoveryByDamageType
    {
        PortionOfDamage = 0,
        PortionOfHP = 1
    }
    public enum ActionConditionType
    {
        Always = 0,
        TurnNo = 1,
        HPThreshold = 2,
        SPThreshold = 3,
        TPThreshold = 4,
        HasState = 5
        //Variable = 6
    }
    public enum BranchConditionType
    {
        GameVariable = 0
    }
    public enum SpanType
    {
        OncePerBattle = 0,
        OncePerTurn = 1,
        OncePerAct = 2
    }
    public enum BattleTargetType
    {
        Enemy = 0,
        Ally = 1
    }

    public enum EquipType
    {
        Head = 0,
        Body = 1,
        Accessory = 2
    }

    public enum StateType
    {
        Buff = 0,
        Debuff = 1,
        Permanent = 2
    }
    public enum VulnerabilityType
    {
        Normal = 0,
        Weakpoint = 1,
        Resist = 2,
        Immune = 3
    }

    public enum AutoRemovalTiming
    {
        None = 0,
        ActionEnd = 1,
        TurnEnd = 2
    }

    public enum Restriction
    {
        None = 0,
        ForceSkills = 1,
        CannotMove = 2
    }
    public enum LearnType
    {
        None = 0,
        Level = 1
    }

    public enum CommandType
    {
        Single = 0,
        Group = 1,
        Items = 2
    }

    public enum DropType
    {
        Item = 0,
        KeyItem = 1,
        Weapon = 2,
        Armor = 3
    }
    public enum EquipmentSlotType
    {
        PrimaryWeapon = 0,
        SecondaryWeapon = 1,
        Head = 2,
        Body = 3,
        PrimaryAccessory = 4,
        SecondaryAccessory = 5
    }
    public enum OperationType
    {
        Add = 0,
        Remove = 1
    }
    public enum AddSetOperationType
    {
        Add = 0,
        Set = 1
    }
    public enum LearnOperationType
    {
        Learn = 0,
        Forget = 1
    }
    public enum NumberOperandType
    {
        FromConstant = 0,
        FromGameVariableNumber = 1
    }
    public enum GameVariableAssignType
    {
        Constant = 0,
        GameVariable = 1,
        Random = 2
    }
    public enum PartyScope
    {
        EntireParty = 0,
        OnePartyMember = 1
    }
    public enum PartyStateScope
    {
        OneState = 0,
        AllBuffs = 1,
        AllDebuffs = 2,
        AllPermanents = 3
    }
    public enum UseMessagePrefix
    {
        None = 0,
        UserName = 1
    }
}
