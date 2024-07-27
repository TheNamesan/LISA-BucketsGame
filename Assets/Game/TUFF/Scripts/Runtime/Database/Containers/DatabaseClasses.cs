using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class SkillsLearned
    {
        [Tooltip("Determines when the Skill should be unlocked.\nNone: Always unlocked.\nLevel: Unlocks after reaching the specified level.")]
        public LearnType learnType = LearnType.None;
        [Tooltip("The skill is unlocked after the user's level is this or higher.")]
        [Range(1, 100)] public int levelLearnedAt = 1;
        [Tooltip("Reference to the Skill.")]
        public Skill skill;
        public bool CanBeUsed(Targetable user)
        {
            if (user == null) return false;

            if (learnType == LearnType.Level && user.GetLevel() >= levelLearnedAt) return true;
            if (learnType == LearnType.None) return true;
            return false;
        }
    }
    [System.Serializable]
    public class BattleType
    {
        public string nameKey = "";
        public Sprite icon;
        public BattleType() { }
        public BattleType(string nameKey, Sprite sprite) { this.nameKey = nameKey; this.icon = sprite; }
        public string GetName() => TUFFTextParser.ParseText(nameKey);
    }
    [System.Serializable]
    public class CharacterQuotes
    {
        public List<CharacterQuoteElement> characterQuotes = new List<CharacterQuoteElement>();
        public List<CharacterQuoteElement> GetValidQuotes()
        {
            var quotes = new List<CharacterQuoteElement>();
            for (int i = 0; i < characterQuotes.Count; i++)
            {
                if (!characterQuotes[i].ValidateQuote()) continue;
                quotes.Add(characterQuotes[i]);
            }
            return quotes;
        }
    }
    [System.Serializable]
    public class CharacterQuoteElement
    {
        public string quoteKey = "";
        //public Condition condition;
        public bool ValidateQuote()
        {
            return true; //Replace this with condition check;
        }
        public string GetQuote()
        {
            return TUFFTextParser.ParseText(quoteKey);
        }
    }
    [System.Serializable]
    public class CharacterBio
    {
        public string fightingArtKey = "";
        public string pastOccupationKey = "";
        public string likesKey = "";
        public string favoriteFoodKey = "";
        public string mostHatedThingKey = "";
        public string GetFightingArt() => TUFFTextParser.ParseText(fightingArtKey);
        public string GetPastOccupation() => TUFFTextParser.ParseText(pastOccupationKey);
        public string GetLikes() => TUFFTextParser.ParseText(likesKey);
        public string GetFavoriteFood() => TUFFTextParser.ParseText(favoriteFoodKey);
        public string GetMostHatedThing() => TUFFTextParser.ParseText(mostHatedThingKey);
    }
    [System.Serializable]
    public class CombatGraphics
    {
        public Sprite defaultFaceGraphic;
        public Sprite KOFaceGraphic;
        public List<Sprite> stateFaces = new List<Sprite>();
    }
    [System.Serializable]
    public struct FlashData
    {
        [Tooltip("The color to tint.")]
        public Color flashColor;
        [Tooltip("Flash duration in seconds.")]
        public float flashDuration;
        public FlashData(Color color, float duration)
        {
            flashColor = color;
            flashDuration = 0f;
        }
    }
    [System.Serializable]
    public class ShopData
    {
        public List<PurchasableItem> shopItems = new List<PurchasableItem>();
        public bool buyOnly = false;
    }
    [System.Serializable]
    public class PurchasableItem
    {
        public InventoryItem invItem;
        public bool customPrice = false;
        public int customPriceValue = 0;
        public int Price { get { return (customPrice ? customPriceValue : invItem.price); } } // CHANGE THIS
    }
    [System.Serializable]
    public class ScopeData
    {
        [Tooltip("Invocation's Scope")]
        public ScopeType scopeType = ScopeType.None;
        [Tooltip("Invocation's Target Condition")]
        public ConditionScopeType targetCondition = ConditionScopeType.OnlyAlive;
        [Tooltip("The amount of targets being hit at a time.")]
        public int randomNumberOfTargets = 1;
        [Tooltip("If true, the user will be excluded from this invocation's possible targets.")]
        public bool excludeUser = false;
    }
    [System.Serializable]
    public class ActionPattern
    {
        [Tooltip("Reference to the Skill to use.")]
        public Skill skill;
        public ActionConditions conditions = new ActionConditions();
        [Tooltip("Chance for the user to use this skill. If Attack Rate is 0, the skill will never be chosen. Default: 100.")]
        [Range(0, 1000)] public int attackRate = 100;
    }
    [System.Serializable]
    public class ActionConditions
    {
        public List<ActionConditionElement> conditionList = new List<ActionConditionElement>();
        public bool ValidateConditions(Targetable user, int turn)
        {
            for (int i = 0; i < conditionList.Count; i++)
            {
                var condition = conditionList[i];
                if (condition == null) continue;
                if (!condition.ValidateCondition(user, turn))
                    return false;
            }
            return true;
        }
    }
    [System.Serializable]
    public class ActionConditionElement
    {
        [Tooltip("If true, the action will trigger when the user is not under this condition.")]
        public bool not = false;
        public ActionConditionType conditionType = ActionConditionType.Always;

        public int equalsTurn = 0;
        public uint turnRepeats = 0;
        [Tooltip("Minimum % needed to trigger this skill.")]
        [Range(0f, 100f)] public float percentThresholdMin = 0f;
        [Tooltip("Maximum % needed to trigger this skill.")]
        [Range(0f, 100f)] public float percentThresholdMax = 100f;
        [Tooltip("Reference to the state.")]
        public State state = null;
        public bool ValidateCondition(Targetable user, int turn)
        {
            bool valid = false;
            switch (conditionType)
            {
                case ActionConditionType.Always:
                    valid = true; break;
                case ActionConditionType.TurnNo:
                    valid = IsWithinTurns(turn); break;
                case ActionConditionType.HPThreshold:
                    valid = IsWithinThreshold(user.HP, user.GetMaxHP()); break;
                case ActionConditionType.SPThreshold:
                    valid = IsWithinThreshold(user.SP, user.GetMaxSP()); break;
                case ActionConditionType.TPThreshold:
                    valid = IsWithinThreshold(user.TP, user.GetMaxTP()); break;
                case ActionConditionType.HasState:
                    valid = user.HasState(state); break;
                default:
                    break;
            }
            if (not) valid = !valid;
            return valid;
        }
        public bool IsWithinTurns(int turn)
        {
            int left = equalsTurn - turn;
            if (turn == equalsTurn) return true;
            if (turn > equalsTurn && turnRepeats > 0)
            {
                if (left % turnRepeats == 0) return true;
            }
            return false;
        }
        public bool IsWithinThreshold(float value, float maxValue)
        {
            float percent = Mathf.Clamp(value / maxValue, 0f, 1f);
            percent *= 100f;
            if (percent >= percentThresholdMin && percent <= percentThresholdMax)
                return true;
            return false;
        }
    }
    [System.Serializable]
    public class BattleConditions
    {
        [Tooltip("Determines how many times the condition check is repeated throughout the battle.\n" +
            "Battle: Runs check only once per battle.\n" +
            "Turn: Runs check at the start of every turn.")]
        public SpanType span = SpanType.OncePerBattle;
        public List<BattleConditionElement> conditionList = new List<BattleConditionElement>();
        private bool m_played = false;
        public bool ValidateConditions()
        {
            if (m_played) return false;
            for (int i = 0; i < conditionList.Count; i++)
            {
                var condition = conditionList[i];
                if (condition == null) continue;
                if (!condition.ValidateCondition())
                {
                    return false;
                }
            }
            m_played = true;
            return true;
        }
        public void SetPlayed(bool played)
        {
            m_played = played;
        }
    }
    [System.Serializable]
    public class BattleConditionElement
    {
        [Tooltip("If true, the action will trigger when the user is not under this condition.")]
        public bool not = false;
        public ActionConditionType conditionType = ActionConditionType.Always;

        public int equalsTurn = 0;
        public uint turnRepeats = 0;

        public BattleTargetType targetType = BattleTargetType.Enemy;
        public Unit unit = null;
        public EnemyIndex enemyIndex;

        [Tooltip("Minimum % needed to trigger this skill.")]
        [Range(0f, 100f)] public float percentThresholdMin = 0f;
        [Tooltip("Maximum % needed to trigger this skill.")]
        [Range(0f, 100f)] public float percentThresholdMax = 100f;
        [Tooltip("Reference to the state.")]
        public State state = null;
        public bool ValidateCondition()
        {
            Targetable user = null;
            if (targetType == BattleTargetType.Enemy) user = enemyIndex.GetEnemyInstance();
            if (targetType == BattleTargetType.Ally) user = PlayerData.instance.GetPartyMember(unit);
            bool valid = false;
            switch (conditionType)
            {
                case ActionConditionType.Always:
                    valid = true; break;
                case ActionConditionType.TurnNo:
                    valid = IsWithinTurns(); break;
                case ActionConditionType.HPThreshold:
                    if (user == null) break;
                    valid = IsWithinThreshold(user.HP, user.GetMaxHP()); break;
                case ActionConditionType.SPThreshold:
                    if (user == null) break;
                    valid = IsWithinThreshold(user.SP, user.GetMaxSP()); break;
                case ActionConditionType.TPThreshold:
                    if (user == null) break;
                    valid = IsWithinThreshold(user.TP, user.GetMaxTP()); break;
                case ActionConditionType.HasState:
                    valid = user.HasState(state); break;
                default:
                    break;
            }
            if (not) valid = !valid;
            return valid;
        }
        public bool IsWithinTurns()
        {
            int turn = BattleManager.instance.turn;
            int left = equalsTurn - turn;
            if (turn == equalsTurn) return true;
            if (turn > equalsTurn && turnRepeats > 0)
            {
                if (left % turnRepeats == 0) return true;
            }
            return false;
        }
        public bool IsWithinThreshold(float value, float maxValue)
        {
            float percent = Mathf.Clamp(value / maxValue, 0f, 1f);
            percent *= 100f;
            if (percent >= percentThresholdMin && percent <= percentThresholdMax)
                return true;
            return false;
            }
    }
    [System.Serializable]
    public struct EnemyIndex
    {
        public int index;
        public EnemyInstance GetEnemyInstance()
        {
            if (index < 0 && index >= BattleManager.instance.enemies.Count) return null;
            return BattleManager.instance.enemies[index];
        }
    }
    [System.Serializable]
    public class DropItem
    {
        [Tooltip("Item/Weapon/Armor to drop on defeat and Battle End.")]
        public DropType dropType = DropType.Item;
        [Tooltip("Reference to the item to drop.")]
        public Item item = null;
        [Tooltip("Reference to the item to drop.")]
        public KeyItem keyItem = null;
        [Tooltip("Reference to the item to drop.")]
        public Weapon weapon = null;
        [Tooltip("Reference to the item to drop.")]
        public Armor armor = null;
        [Tooltip("% probability of dropping the item.")]
        [Range(0, 100)] public int dropChance = 0;

        public InventoryItem GetItem()
        {
            switch(dropType)
            {
                case DropType.Item:
                    return item;
                case DropType.KeyItem:
                    return keyItem;
                case DropType.Weapon:
                    return weapon;
                case DropType.Armor:
                    return armor;

                default: return null;
            }
        }
    }

    [System.Serializable]
    public struct NumberOperand
    {
        public NumberOperandType operandType;
        [Tooltip("Assign a constant value.")]
        public float constant;
        public int variableIndex;

        public float GetNumber()
        {
            float value = 0;
            if (operandType == NumberOperandType.FromConstant) value = constant;
            else {
                if (!PlayerData.instance.IsValidGameVariableIndex(variableIndex)) { Debug.LogWarning("Invalid Variable Index"); return value; }
                value = PlayerData.instance.gameVariables[variableIndex].numberValue;
            }
            return value;
        }
    }

    [System.Serializable]
    public struct WeaponTypeList
    {
        public List<int> weaponTypes;
    }
    [System.Serializable]
    public struct ArmorTypeList
    {
        public List<int> armorTypes;
    }

    [System.Serializable]
    public struct GameVariableComparator
    {
        public int targetVariableIndex;
        public GameVariableValueType targetVariableValueType;
        public bool variableBool;
        public float variableNumber;
        public string variableString;
        public Vector2 variableVector;
        public bool ValidateGameVariable()
        {
            if (PlayerData.instance == null) return false;
            if (!PlayerData.instance.IsValidGameVariableIndex(targetVariableIndex)) { Debug.LogWarning("Invalid Variable Index"); return false; }
            var variable = PlayerData.instance.gameVariables[targetVariableIndex];
            object obj = null;
            switch (targetVariableValueType)
            {
                case GameVariableValueType.BoolValue:
                    obj = variableBool; break;
                case GameVariableValueType.NumberValue:
                    obj = variableNumber; break;
                case GameVariableValueType.StringValue:
                    obj = variableString; break;
                case GameVariableValueType.VectorValue:
                    obj = variableVector; break;
            }
            return variable.EqualsValue(obj);
        }
    }
    [System.Serializable]
    public struct UnitStatusComparator
    {
        public Unit targetUnit;
        public UnitStatusConditionType unitCondition;
        public string targetName;
        public Job targetJob;
        public Skill targetSkill;
        public Weapon targetWeapon;
        public Armor targetArmor;
        public State targetState;
        public bool ValidateUnit()
        {
            if (PlayerData.instance == null) return false;
            if (!targetUnit) return true;
            PartyMember member = PlayerData.instance.GetPartyMember(targetUnit);
            switch (unitCondition)
            {
                case UnitStatusConditionType.IsInParty:
                    return PlayerData.instance.IsInParty(targetUnit);
                case UnitStatusConditionType.IsInActiveParty:
                    return PlayerData.instance.IsInActiveParty(targetUnit);
                case UnitStatusConditionType.IsNamed:
                    return targetName == member.GetName();
                case UnitStatusConditionType.HasJob:
                    return targetJob == member.GetJob();
                case UnitStatusConditionType.KnowsSkill:
                    return member.KnowsSkill(targetSkill);
                case UnitStatusConditionType.HasWeaponEquipped:
                    return member.HasWeaponEquipped(targetWeapon);
                case UnitStatusConditionType.HasArmorEquipped:
                    return member.HasArmorEquipped(targetArmor);
                case UnitStatusConditionType.IsStateInflicted:
                    return member.HasState(targetState);
                default: return false;
            }
        }
    }
    [System.Serializable]
    public struct InventoryComparator
    {
        public DropType inventoryType;
        public NumberComparisonType numberComparison;
        public int targetItemCount;
        
        public Item targetItem;
        public KeyItem targetKeyItem;
        public Weapon targetWeapon;
        public Armor targetArmor;

        public bool includeEquipment;
        public bool ValidateInventory()
        {
            if (Inventory.instance == null) return false;
            switch (inventoryType)
            {
                case DropType.Item:
                    if (!targetItem) return false;
                    return ValidateCount(Inventory.instance.GetItemAmount(targetItem));
                case DropType.KeyItem:
                    if (!targetKeyItem) return false;
                    return ValidateCount(Inventory.instance.GetItemAmount(targetKeyItem));
                case DropType.Weapon:
                    if (!targetWeapon) return false;
                    return ValidateCount(Inventory.instance.GetItemAmount(targetWeapon, includeEquipment));
                case DropType.Armor:
                    if (!targetArmor) return false;
                    return ValidateCount(Inventory.instance.GetItemAmount(targetArmor, includeEquipment));
                default: return false;
            }
        }
        private bool ValidateCount(int count)
        {
            switch (numberComparison)
            {
                case NumberComparisonType.Equal:
                    return count == targetItemCount;
                case NumberComparisonType.NotEqual:
                    return count != targetItemCount;
                case NumberComparisonType.Less:
                    return count < targetItemCount;
                case NumberComparisonType.LessOrEqual:
                    return count <= targetItemCount;
                case NumberComparisonType.Greater:
                    return count > targetItemCount;
                case NumberComparisonType.GreaterOrEqual:
                    return count >= targetItemCount;
                
            }
            return true;
        }
    }
}
