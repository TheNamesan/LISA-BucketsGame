using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TUFF
{
    [System.Serializable]
    public class BattleAnimationEvent
    {
        [HideInInspector] public BattleAnimation animationInstance;
        [HideInInspector] public TargetedSkill skillOrigin;
        [Tooltip("If true, will play Audio clips as SFXs when RunEvent is called.")]
        public bool playSFX = false;
        [Tooltip("If true, audio clips will only be played when target is hit.")]
        public bool playSFXOnHit = false;
        [Tooltip("List of Audio Clips to play when the Event runs.")]
        public List<SFX> SFXList = new List<SFX>();

        //Override Scope
        public bool overrideScopeData = false;
        public ScopeData scopeDataOverride = new ScopeData();

        [Tooltip("If true, will hit the target when the Event runs. Hits are affected by Hit Rate and Evasion Rate.")]
        public bool hit = false;
        //Invocation
        [Tooltip("Multiplier to user's Hit Rate.")]
        [Range(1, 100)] public int successPercent = 100;
        [Tooltip("Certain Hit attacks will always hit regardless of Hit Rate, Evasion Rate and Success Percent.")]
        public bool certainHit = false;
        [Tooltip("TP gained to the user when running this event.")]
        public int TPGain = 5;
        [Tooltip("UP gained to the user when running this event.")]
        public int UPGain = 0;
        [Tooltip("Determines the damage display text color. Certain Hit attacks will always hit regardless of Hit Rate, Evasion Rate and Success Percent.")]
        public HitType hitType = HitType.PhysicalAttack;
        [Tooltip("When the invocation has more than one target, determines the timing of each hit.")]
        public MultihitTimingType multihitTiming = MultihitTimingType.AllAtOnce;
        [Tooltip("Delay between each hit.")]
        public float multihitTimingDelay = 0.075f;

        // Damage
        [Tooltip("The target's stat to damage.")]
        public DamageType damageType = DamageType.None; //if none, disables everything else from Damage Tab
        [Tooltip("The hit's element.")]
        public int element = 0;
        public int ElementIndex { get => (element > 0 ? (element == 1 ? -1 : element - 2) : -1); } // Fix case when element == 1
        public HitFormula formula = new HitFormula();
        [Tooltip("The variability (percentage) of the hit formula's result. The hit formula's result will be affected by a random percentage between +(variance)% and -(variance)%.")]
        [Range(0, 100)] public int variance = 0;
        [Tooltip("If true, hitting the target will have a chance to deal Crit Damage. Crit Chance is affected by the user's Crit Rate and the target's Crit Evasion Rate.")]
        public bool canCrit = true;
        [Tooltip("If true, you can adjust the pause timer for the animation when the attack gets a Crit.")]
        public bool overrideCritPauseTimer = false;
        [Tooltip("The duration of the pause in seconds. Pause is skipped if the player holds the Skip button.")]
        public float critPauseTimer = 0;
        //Effects
        [Tooltip("If true, the effects will have their separate chance of triggering even when the attack misses.")]
        public bool ignoreHitEffects = false;
        public List<Effect> effects = new List<Effect>();

        // Motion
        [Tooltip("If true, user's graphic will do an animation.")]
        public bool userMotion = false;
        public MotionOneTimeType motionType = MotionOneTimeType.None;

        // Flash Target
        [Tooltip("If true, the target's graphic will be tinted.")]
        public bool flashTarget = false;
        [Tooltip("If true, the target will flash even if the attack misses.")]
        public bool ignoreHitFlashTarget = false;
        public FlashData flashTargetData = new FlashData(Color.white, 0);

        // Flash Screen
        [Tooltip("If true, the screen will be tinted.")]
        public bool flashScreen = false;
        public FlashData flashScreenData = new FlashData(Color.white, 0);


        public UnityEvent onEventRun = new UnityEvent();

        public void InitiateAnimationEvent(BattleAnimation battleAnimation, TargetedSkill targetedSkill)
        {
            animationInstance = battleAnimation;
            skillOrigin = targetedSkill;
        }
    }
}
