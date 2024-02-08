using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class PartyBattleData
    {
        public bool disableUP { get => m_disableUP; set => value = m_disableUP; }
        [SerializeField] private bool m_disableUP = false;
        public int UP = 0;

        public void RecoverUPPercentage(int percentage)
        {
            int value = LISAUtility.Truncate(GetMaxUP() * percentage * 0.01f);
            RecoverUP(value);
        }
        public void RecoverUP(int value)
        {
            CalculateUP(value);
            BattleManager.instance.ForceUpdateHUD();
        }
        public void CalculateUP(int value)
        {
            UP += value;
            if (UP <= 0) UP = 0;
            int maxUP = GetMaxUP();
            if (UP > maxUP) UP = maxUP;
        }
        public int GetMaxUP()
        {
            return TUFFSettings.baseMaxUP;
        }
        public float GetUPPercentage()
        {
            float value = (float)UP / GetMaxUP() * 100f;
            return value;
        }
    }
}
