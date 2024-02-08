using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public static class TUFFFeatureToText
    {
        public static string GetStatChangeText(StatChangeType statChangeType)
        {
            switch (statChangeType)
            {
                case StatChangeType.MaxHP: return TUFFSettings.maxHPShortText;
                case StatChangeType.MaxSP: return TUFFSettings.maxSPShortText;
                case StatChangeType.MaxTP: return TUFFSettings.maxTPShortText;
                case StatChangeType.ATK: return TUFFSettings.ATKShortText;
                case StatChangeType.DEF: return TUFFSettings.DEFShortText;
                case StatChangeType.SATK: return TUFFSettings.SATKShortText;
                case StatChangeType.SDEF: return TUFFSettings.SDEFShortText;
                case StatChangeType.AGI: return TUFFSettings.AGIShortText;
                case StatChangeType.LUK: return TUFFSettings.LUKShortText;
            }
            return "";
        }
        public static string GetExtraRateChangeText(ExtraRateChangeType extraRateChangeType)
        {
            switch (extraRateChangeType)
            {
                case ExtraRateChangeType.HitRate: return "hit rate";
            }
            return "";
        }
    }
}
