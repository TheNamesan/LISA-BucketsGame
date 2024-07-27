using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public interface IEquipable
    {
        public int maxHP { get; set; }
        public int maxSP { get; set; }
        public int maxTP { get; set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
        public int SATK { get; set; }
        public int SDEF { get; set; }
        public int AGI { get; set; }
        public int LUK { get; set; }
        public List<Feature> features { get; set; }
        public int GetBonusesOfStat(StatChangeType statType)
        {
            switch (statType)
            {
                case StatChangeType.MaxHP:
                    return maxHP;
                case StatChangeType.MaxSP:
                    return maxSP;
                case StatChangeType.MaxTP:
                    return maxTP;
                case StatChangeType.ATK:
                    return ATK;
                case StatChangeType.DEF:
                    return DEF;
                case StatChangeType.SATK:
                    return SATK;
                case StatChangeType.SDEF:
                    return SDEF;
                case StatChangeType.AGI:
                    return AGI;
                case StatChangeType.LUK:
                    return LUK;
                default:
                    return 0;
            }
        }
        public int GetStatTotal()
        {
            int sum = maxHP + maxSP + maxTP + ATK + DEF + SATK + SDEF + AGI + LUK;
            return sum;
        }
    }
}
