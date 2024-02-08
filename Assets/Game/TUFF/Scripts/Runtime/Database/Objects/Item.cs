using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "Item", menuName = "Database/Item", order = 4)]
    public class Item : InventoryItem, IBattleInvocation
    {
        public DatabaseElement databaseElement { get { return this; } }
        [Header("General Settings")]
        [Tooltip("Name localization key from the Item Table Collection.")]
        public string nameKey = "name_key";
        [Tooltip("Description localization key from the Item Table Collection."), TextArea(2, 2)]
        public string descriptionKey = "description_key";
        [Tooltip("If true, item disappears after use.")]
        public bool consumable = true;

        [Header("Scope")]
        [Tooltip("Item's target data.")]
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
        [Tooltip("The amount of times the item is invoked in a row.")]
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

        public override string GetName()
        {
            return TUFFTextParser.ParseText(nameKey);
        }
        public override string GetDescription()
        {
            return TUFFTextParser.ParseText(descriptionKey);
        }
        public override void ConsumeItemFromMenu(Targetable user)
        {
            if (startEvents == null && endEvents == null) return;
            if (startEvents.Count <= 0 && endEvents.Count <= 0) return;
            var tmpSkill = new TargetedSkill(this, new List<Targetable>() { user }, user);
            if (!BattleManager.instance.CanUseSkill(tmpSkill)) return;
            tmpSkill.RunStartEvents();
            tmpSkill.RunEndEvents();
        }
    }
}
