using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TUFF
{
    public enum LevelToStatType
    {
        EXP = 0,
        MaxHP = 1,
        MaxSP = 2,
        MaxTP = 3,
        ATK = 4,
        DEF = 5,
        SATK = 6,
        SDEF = 7,
        AGI = 8,
        LUK = 9
    }

    [CreateAssetMenu(fileName = "Job", menuName = "Database/Job", order = 1)]
    public class Job : DatabaseElement
    {
        [Header("General Settings")]
        [Tooltip("Name localization key from the Job Table Collection.")]
        public string nameKey = "name_key";
        [Tooltip("Description localization key from the Job Table Collection."), TextArea(2,2)]
        public string descriptionKey = "description_key";

        [Header("Resources")]
        [Tooltip("Unit is able to use Skills that use SP. Displays the SP Bar for the Unit.")]
        public bool usesSP = true;
        [Tooltip("Unit is able to use Skills that use TP. Displays the TP Bar for the Unit.")]
        public bool usesTP = false;
        [Tooltip("If true, user's TP will be reset to a random % of their Max TP at the start of battles.")]
        public bool resetTPOnBattleStart = false;
        [Tooltip("Random minimum percentage of starting TP.")]
        [Range(0, 100)] public int startTPMin = 10;
        [Tooltip("Random maximum percentage of starting TP.")]
        [Range(0, 100)] public int startTPMax = 10;

        [Tooltip("Amount of EXP gained from Enemies needed to Level Up. Recommended to leave these values at default.")]
        public AnimationCurve expCurve = GetDefaultEXPCurve();
        public int startEXP = 0;
        public int endEXP = 2547133;

        //Stat Curves
        [Tooltip("Maximum Hit Points. The maximum amount of damage the Unit can resist. Unit is knocked out if HP reaches 0.")]
        public AnimationCurve maxHP = GetDefaultCurve();
        public int startMaxHP;
        public int endMaxHP;

        [Tooltip("Maximum Skill Points. Used to invoke certain Skills.")]
        public AnimationCurve maxSP = GetDefaultCurve();
        public int startMaxSP;
        public int endMaxSP;

        [Tooltip("Maximum Tech Points. Used to invoke certain Skills. Can be gained from using Skills or receiving damage.")]
        public AnimationCurve maxTP = GetDefaultCurve();
        public int startMaxTP = 100;
        public int endMaxTP = 100;

        [Tooltip("Attack Points. Affects damage done by Physical Damage attacks.")]
        public AnimationCurve ATK = GetDefaultCurve();
        public int startATK;
        public int endATK;

        [Tooltip("Defense Points. Reduces damage received by Physical Damage attacks.")]
        public AnimationCurve DEF = GetDefaultCurve();
        public int startDEF;
        public int endDEF;

        [Tooltip("Special Attack Points. Affects damage done by Special Damage attacks.")]
        public AnimationCurve SATK = GetDefaultCurve();
        public int startSATK;
        public int endSATK;

        [Tooltip("Special Defense Points. Reduces damage received by Special Damage attacks.")]
        public AnimationCurve SDEF = GetDefaultCurve();
        public int startSDEF;
        public int endSDEF;

        [Tooltip("Agility Points. Affects the attack order.")]
        public AnimationCurve AGI = GetDefaultCurve();
        public int startAGI;
        public int endAGI;

        [Tooltip("Luck Points. Slightly affects the trigger chance of States.")]
        public AnimationCurve LUK = GetDefaultCurve();
        public int startLUK;
        public int endLUK;

        [Header("Base Rates")]
        [Tooltip("Chance of being targeted by Enemies. Ignored when Skill targets random Enemies. Default: 100")]
        [Range(0, 1000)] public int targetRate = 100;
        [Tooltip("Chance for attacks to hit its target. Attacks marked as Certain Hit will always hit regardless of Hit Rate. Default: 95")]
        [Range(-100, 100)] public int hitRate = 95;
        [Tooltip("Chance to evade an incoming attack. Attacks marked as Certain Hit will ignore this value. Default: 5")]
        [Range(-100, 100)] public int evasionRate = 5;
        [Tooltip("Chance to inflict Critical damage. If a crit is triggered, damage is affected by the Crit Damage Multiplier. Only affects hits that can crit. Default: 5")]
        [Range(-100, 100)] public int critRate = 4;
        [Tooltip("Reduces the chance for the incoming damage to be dealt as Critical damage. Default: 0")]
        [Range(-100, 100)] public int critEvasionRate = 0;

        [Header("Equip Types")]
        [Tooltip("The weapons of the specified type the Unit can use by default.")]
        public WeaponTypeList weaponTypes = new();
        [Tooltip("The armors of the specified type the Unit can use by default.")]
        public ArmorTypeList armorTypes = new();

        [Header("Features")]
        [Tooltip("The changes applied to the affected user.")]
        public List<Feature> features = new List<Feature>();

        [Header("Commands")]
        public List<Command> commands;

        [Header("Skills")]
        public List<SkillsLearned> skills = new List<SkillsLearned>();

        [Header("Graphics")]
        [SerializeField] public Sprite menuPortrait;
        [Header("Combat Graphics")]
        public CombatGraphics faceGraphics; //Class with Sprite field and name of the status effect, plus a Sprite field for no statuses.
        public CombatGraphics zoomedFaceGraphics; //Same as above, this one's used for flavor text icons.

        public override string GetName()
        {
            return TUFFTextParser.ParseText(nameKey);
        }
        public override string GetDescription()
        {
            return TUFFTextParser.ParseText(descriptionKey);
        }
        /// <summary>
        /// Returns the base value of a stat based on a given Unit's level.
        /// </summary>
        /// <param name="level">The level the Unit is at (Range: 1 to 100)</param>
        /// <param name="type">The stat's value to return</param>
        /// <returns></returns>
        public int LevelToStat(int level, LevelToStatType type)
        {
            switch (type)
            {
                case LevelToStatType.EXP:
                    return LevelToStatValue(level, expCurve, startEXP, endEXP);
                case LevelToStatType.MaxHP:
                    return LevelToStatValue(level, maxHP, startMaxHP, endMaxHP);
                case LevelToStatType.MaxSP:
                    return LevelToStatValue(level, maxSP, startMaxSP, endMaxSP);
                case LevelToStatType.MaxTP:
                    return LevelToStatValue(level, maxTP, startMaxTP, endMaxTP);
                case LevelToStatType.ATK:
                    return LevelToStatValue(level, ATK, startATK, endATK);
                case LevelToStatType.DEF:
                    return LevelToStatValue(level, DEF, startDEF, endDEF);
                case LevelToStatType.SATK:
                    return LevelToStatValue(level, SATK, startSATK, endSATK);
                case LevelToStatType.SDEF:
                    return LevelToStatValue(level, SDEF, startSDEF, endSDEF);
                case LevelToStatType.AGI:
                    return LevelToStatValue(level, AGI, startAGI, endAGI);
                case LevelToStatType.LUK:
                    return LevelToStatValue(level, LUK, startLUK, endLUK);
                default:
                    return -1;
            }
        }

        private int LevelToStatValue(int level, AnimationCurve curve, int startValue, int endValue)
        {
            if (curve == null) return -1;
            float evaluatedCurve = curve.Evaluate(Mathf.InverseLerp(1, 100, level));
            return LISAUtility.Truncate(Mathf.Lerp(startValue, endValue, evaluatedCurve));
        }

        private static AnimationCurve GetDefaultCurve()
        {
            return AnimationCurve.Linear(0, 0, 1, 1);
        }
        private static AnimationCurve GetDefaultEXPCurve()
        {
            Keyframe[] keys = new Keyframe[2];
            keys[0] = new Keyframe(0, 0);
            keys[0].weightedMode = WeightedMode.None;
            keys[0].inWeight = 0;

            keys[1] = new Keyframe(1, 1);
            keys[1].weightedMode = WeightedMode.None;
            keys[1].inTangent = 2.8141944f;
            keys[1].outTangent = 2.8141944f;
            keys[1].inWeight = 0.355524116f;
            var curve = new AnimationCurve(keys);
            return curve;
        }
        public List<Skill> GetSkillsToLearnAtLevel(int level) => GetSkillsToLearnAtLevel(level, level);
        public List<Skill> GetSkillsToLearnAtLevel(int level, int fromLevel = 0)
        {
            var skillsLearned = skills.FindAll(q => q.learnType == LearnType.Level && (q.levelLearnedAt >= fromLevel && q.levelLearnedAt <= level));
            var skillList = new List<Skill>();
            foreach (var lrn in skillsLearned) skillList.Add(lrn.skill);
            return skillList;
        }
    }
}