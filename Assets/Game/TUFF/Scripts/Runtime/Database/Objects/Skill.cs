using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "Skill", menuName = "Database/Skill", order = 2)]
    public class Skill : DatabaseElement, IBattleInvocation
    {
        public DatabaseElement databaseElement { get { return this; } }
        [Header("General Settings")]
        [Tooltip("Name localization key from the Skill Table Collection.")]
        public string nameKey = "name_key";
        [Tooltip("Description localization key from the Skill Table Collection."), TextArea(2, 2)]
        public string descriptionKey = "description_key";
        [SerializeField] protected Sprite m_icon;
        public Sprite icon
        {
            get { return m_icon; }
            set { m_icon = value; }
        }

        [Tooltip("SP cost of the Skill. If the user's SP is lower than this value it cannot be used.")]
        public int SPCost = 0;
        [Tooltip("TP cost of the Skill. If the user's TP is lower than this value it cannot be used.")]
        public int TPCost = 0;
        [Tooltip("Item needed in the player's inventory to use the skill. Leave empty if no item is needed.")]
        public InventoryItem requiredItem = null;
        [Tooltip("The quantity of the item in the player's inventory needed to use this skill.")]
        public int itemAmount = 1;
        [Tooltip("If true, the amount of items will be consumed when the skill is invoked.")]
        public bool consumeItem = false;

        [Header("Scope")]
        [Tooltip("Skill's target data.")]
        public ScopeData scopeData = new ScopeData();
        public ScopeData ScopeData
        {
            get { return scopeData; }
            set { scopeData = value; }
        }
        [Tooltip("Determines if the Item can be used in and/or outside of Battles.")]
        [SerializeField] protected OccasionType m_occasion = OccasionType.Always;
        public OccasionType occasion
        {
            get { return m_occasion; }
            set { m_occasion = value; }
        }
        [Header("Invocation")]
        [Tooltip("Added to the user's AGI to determine the attack order.")]
        [SerializeField] protected int m_speed = 0;
        public int speed
        {
            get { return m_speed; }
            set { m_speed = value; }
        }
        [Tooltip("The amount of times the skill is invoked in a row.")]
        [SerializeField] protected int m_repeats = 1;
        public int repeats
        {
            get { return m_repeats; }
            set { m_repeats = value; }
        }
        [Tooltip("Item's animation prefab.")]
        [SerializeField] protected BattleAnimation m_animation = null;
        public BattleAnimation animation
        {
            get { return m_animation; }
            set { m_animation = value; }
        }
        [SerializeField] protected List<BattleAnimationEvent> m_startEvents = new List<BattleAnimationEvent>();
        public List<BattleAnimationEvent> startEvents
        {
            get { return m_startEvents; }
            set { m_startEvents = value; }
        }
        [SerializeField] protected List<BattleAnimationEvent> m_endEvents = new List<BattleAnimationEvent>();
        public List<BattleAnimationEvent> endEvents
        {
            get { return m_endEvents; }
            set { m_endEvents = value; }
        }
        [SerializeField] protected float m_endDelay = 0f;
        public float endDelay
        {
            get { return m_endDelay; }
            set { m_endDelay = value; }
        }

        [Header("Use Message")]
        [Tooltip("None: Only the message will be displayed.\nUser Name: The name of the user will be included at the start ")]
        public UseMessagePrefix prefix = UseMessagePrefix.UserName; //Class/Struct with enum for message type (message with user name, or normal message), and if it shows the user's icon or not.
        [Tooltip("Usage Message localization key from the Skill Table Collection.")]
        public string useMessageKey = "use_key";

        [Header("Combo Dial")]
        [Tooltip("Input used to invoke this skill when in Combo Dial. Caps only. If none or input is invalid, this skill won't be used in a Combo Dial. Example: WWS")]
        public string comboDialMove = "";

        [Header("United")]
        [Tooltip("If true, the skill needs two users that can act to be able to be invoked.")]
        public bool isUnitedSkill = false;
        [Tooltip("UP percentage cost of the Skill. If the party's UP% is lower than this value it cannot be used.")]
        public int UPCost = 0;
        public Unit unitedUserA = null;
        public Unit unitedUserB = null;

        public override string GetName()
        {
            return TUFFTextParser.ParseText(nameKey);
        }
        public override string GetDescription()
        {
            return TUFFTextParser.ParseText(descriptionKey);
        }
        public string GetUseMessage(string userName)
        {
            return $"{(prefix == UseMessagePrefix.UserName ? userName : "")}{TUFFTextParser.ParseText(useMessageKey)}" ;
        }
        public int GetSPCost(Targetable user)
        {
            if (user == null) return SPCost;
            return LISAUtility.Truncate(SPCost * user.GetSPCostRate());
        }
        public int GetTPCost(Targetable user)
        {
            if (user == null) return TPCost;
            return LISAUtility.Truncate(TPCost * user.GetTPCostRate());
        }
        public bool CanAffordSP(Targetable user)
        {
            if (user == null) return true;
            return user.SP >= GetSPCost(user);
        }
        public bool CanAffordTP(Targetable user)
        {
            if (user == null) return true;
            return user.TP >= GetTPCost(user);
        }
        public bool CanAffordUP()
        {
            return PlayerData.instance.battleData.GetUPPercentage() >= UPCost;
        }
        public bool CanAffordItem(Targetable user)
        {
            if (user == null) return true;
            if (requiredItem == null) return true;
            int curAmount = Inventory.instance.GetItemAmount(requiredItem);
            int neededAmount = itemAmount;
            return curAmount >= neededAmount;
        }
        public bool UnitedUsersCanAct()
        {
            if (!isUnitedSkill) return true;
            if (unitedUserA == null || unitedUserB == null) return true;
            var userA = PlayerData.instance.GetPartyMember(unitedUserA);
            var userB = PlayerData.instance.GetPartyMember(unitedUserB);
            return userA.CanControlAct() && userB.CanControlAct();
        }
        
    }
}