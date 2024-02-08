using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "State", menuName = "Database/State", order = 10)]
    public class State : DatabaseElement
    {
        [Header("General Settings")]
        [Tooltip("Name localization key from the State Table Collection.")]
        public string nameKey = "name_key";
        [Tooltip("Description localization key from the State Table Collection."), TextArea(2, 2)]
        public string descriptionKey = "description_key";
        [Tooltip("State's icon.")]
        public Sprite icon;
        [Tooltip("Actions restricted to the affected user.")]
        public Restriction restriction = Restriction.None;
        [Tooltip("Forced Skills to use in Battle.")]
        public List<ActionPattern> forcedActionPatterns = new List<ActionPattern>();
        [Tooltip("If true, will hide the icon and the apply and removal popups from the user's HUD.")]
        public bool hidden = false;
        [Tooltip("The state's benefit to the user. Permanent states will not be removed when KOd.")]
        public StateType stateType = StateType.Buff; //enum

        [Header("Remove Conditions")]
        [Tooltip("If true, the state will be removed from the user when the battle ends.")]
        public bool removeAtBattleEnd = false;
        [Tooltip("Determines if the state should be removed with passing turns/acts.\nAction End: Counts down every time the user does an action like a Skill or Item.\nTurn End: Counts down after all entities have acted.")]
        public AutoRemovalTiming autoRemovalTiming = AutoRemovalTiming.None;
        [Tooltip("The minimum/maximum amount of acts/turns the user needs to perform for the state to be removed. The amount is randomized between the minimum and maximum.")]
        public Vector2Int durationInTurns; //Hide if autoRemovalTiming is None
        [Tooltip("If true, the state will have a chance to remove itself when the user takes damage.")]
        public bool removeByDamage = false; 
        [Tooltip("Probability % of removing the state when taking damage.")]
        [Range(0, 100)] public int removeByDamageChance = 0; //Hide if remove is false
        [Tooltip("If true, the state will be removed by walking in the overworld.")]
        public bool removeByWalking = false;
        [Tooltip("The amount of seconds to walk for the state to be removed.")]
        public float removeByWalkingSeconds = 0; //Hide if remove is false

        [Header("Progressive State")]
        [Tooltip("State to apply to the user if this state's duration expires.")]
        public State progressiveState = null;
        [Tooltip("Probability % of applying the progressive state when this state expires.")]
        [Range(0, 100)] public int progressiveStateTriggerChance = 100;

        [Header("Features")]
        [Tooltip("The changes applied to the affected user.")]
        public List<Feature> features = new List<Feature>();

        [Header("Visuals")]
        public StateVisual visual = null;

        public override string GetName()
        {
            return TUFFTextParser.ParseText(nameKey);
        }
        public override string GetDescription()
        {
            return TUFFTextParser.ParseText(descriptionKey);
        }
    }
}
