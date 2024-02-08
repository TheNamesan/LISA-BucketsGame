using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace TUFF
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Database/Enemy", order = 8)]
    public class Enemy : DatabaseElement
    {
        [Header("General Settings")]
        [Tooltip("Name localization key from the Enemy Table Collection.")]
        public string nameKey = "name_key";
        [Tooltip("Enemy's default sprite when spawned.")]
        public Sprite graphic;

        [Header("Stats")]
        public int maxHP = 300;
        public int maxSP = 30;
        public int maxTP = 100;
        public int ATK = 32;
        public int DEF = 15;
        public int SATK = 25;
        public int SDEF = 25;
        public int AGI = 32;
        public int LUK = 32;

        [Header("Base Rates")]
        [Tooltip("Chance of being targeted by Enemies. Ignored when Skill targets random Enemies. Default: 100")]
        [Range(0, 1000)] public int targetRate = 100;
        [Tooltip("Chance for attacks to hit its target. Attacks marked as Certain Hit will always hit regardless of Hit Rate. Default: 95")]
        [Range(-100, 100)] public int hitRate = 95;
        [Tooltip("Chance to evade an incoming attack. Attacks marked as Certain Hit will ignore this value. Default: 0")]
        [Range(-100, 100)] public int evasionRate = 0;
        [Tooltip("Chance to inflict Critical damage. If a crit is triggered, damage is affected by the Crit Damage Multiplier. Only affects hits that can crit. Default: 0")]
        [Range(-100, 100)] public int critRate = 0;
        [Tooltip("Reduces the chance for the incoming damage to be dealt as Critical damage. Default: 0")]
        [Range(-100, 100)] public int critEvasionRate = 0;

        [Header("Features")]
        [Tooltip("The changes applied to the affected user.")]
        public List<Feature> features = new List<Feature>();

        [Header("Rewards")]
        [Tooltip("EXP awarded to Units when the Enemy is defeated and the Battle ends.")]
        public int EXP = 0;
        [Tooltip("Mags awarded to the player when the Enemy is defeated and the Battle ends.")]
        public int mags = 0;

        [Header("Drop Items")]
        [Tooltip("Items dropped on defeat and Battle End.")]
        public List<DropItem> drops; //Class/Struct with Item field and number of items.

        [Header("Action Patterns")]
        [Tooltip("Enemy's Skills to use in Battle.")]
        public List<ActionPattern> actionPatterns;

        [Header("Motions")]
        [Tooltip("The transition to play when the enemy is KOd. Played after the KO Animation")]
        public MotionKOType KOMotion = MotionKOType.FadeOut;
        //public UnityEvent<EnemyInstance> customKOMotion = new UnityEvent<EnemyInstance>();
        public override string GetName()
        {
            return TUFFTextParser.ParseText(nameKey);
        }
    }
}
