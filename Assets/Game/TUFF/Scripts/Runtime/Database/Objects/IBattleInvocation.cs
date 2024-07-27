using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public interface IBattleInvocation
    {
        public DatabaseElement databaseElement { get; }
        [Tooltip("Invocation's icon.")]
        public Sprite icon { get; set; }

        public ScopeData ScopeData { get; set; }
        [Tooltip("Determines if the invocation can be used in and/or outside of Battles.")]
        public OccasionType occasion { get; set; }
        [Tooltip("Added to the user's AGI to determine the attack order.")]
        public int speed { get; set; }
        public int repeats { get; set; }
        public BattleAnimation animation { get; set; }

        [Tooltip("Battle Animation Events to run when the invocation starts.")]
        public List<BattleAnimationEvent> startEvents { get; set; }
        [Tooltip("Battle Animation Events to run when the invocation ends.")]
        public List<BattleAnimationEvent> endEvents { get; set; }
        [Tooltip("Time to wait in seconds after the animation is finished.")]
        public float endDelay { get; set; }

        public string GetName()
        {
            return "";
        }
        public string GetDescription()
        {
            return "";
        }
        public bool CanBeUsedInMenu()
        {
            return occasion == OccasionType.Always || occasion == OccasionType.OnlyFromMenu;
        }
        public bool CanBeUsedInBattle()
        {
            return occasion == OccasionType.Always || occasion == OccasionType.OnlyInBattle;
        }
        public List<BattleAnimationEvent> GetAllEvents()
        {
            var evts = new List<BattleAnimationEvent>();
            if (animation != null)
            {
                for (int i = 0; i < animation.events.Count; i++)
                    evts.Add(animation.events[i]);
                
            }
            if (startEvents != null)
            {
                for (int i = 0; i < startEvents.Count; i++)
                    evts.Add(startEvents[i]);
            }
            if (endEvents != null)
            {
                for (int i = 0; i < endEvents.Count; i++)
                    evts.Add(endEvents[i]);
            }
            return evts;
        }    
    }
}

