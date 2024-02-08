using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{

    [System.Serializable]
    public class HitFormula
    {
        public List<HitFormulaGroup> formulaGroups = new List<HitFormulaGroup>();
        public int GetFormulaValue(BattleAnimationEvent hitInfo, int targetIndex)
        {
            var totalValue = 0;
            for(int j = 0; j < formulaGroups.Count; j++)
            {
                if (formulaGroups[j] == null) { continue; }
                var formulaGroup = formulaGroups[j].formulaOperations;
                float groupTotal = 0f;
                for (int i = 0; i < formulaGroup.Count; i++)
                {
                    float current = 0;
                    if (formulaGroup[i] == null) { continue; }
                    if (formulaGroup[i].formulaTargetable == HitFormulaOperation.FormulaCasterType.FlatNumber)
                    {
                        current = formulaGroup[i].flatNumber;
                    }
                    else
                    {
                        var caster = GetCaster(hitInfo, formulaGroup[i].formulaTargetable, targetIndex);
                        var stat = GetStatValue(caster, formulaGroup[i].formulaStat);
                        current = stat;
                    }
                    if (i == 0) groupTotal = current;
                    else groupTotal = ApplyOperation(groupTotal, current, formulaGroup[i].formulaOp);
                }
                if (j == 0) totalValue = LISAUtility.Truncate(groupTotal);
                else totalValue = ApplyOperation(totalValue, groupTotal, formulaGroups[j].formulaGroupOp);
            }
            if (totalValue < 0) totalValue = 0;
            if(hitInfo.variance != 0)
            {
                var variance = Random.Range(-hitInfo.variance, hitInfo.variance + 1);
                //Debug.Log($"Value: {totalValue}. Variance: {variance}");
                totalValue = LISAUtility.Truncate(totalValue * (1 + variance * 0.01f));
            }
            return totalValue;
        }
        private Targetable GetCaster(BattleAnimationEvent hitInfo, HitFormulaOperation.FormulaCasterType casterType, int targetIndex)
        {
            if (casterType == HitFormulaOperation.FormulaCasterType.User)
            { return hitInfo.skillOrigin.user; }
            return hitInfo.skillOrigin.targets[targetIndex]; 
        }
        private int GetStatValue(Targetable caster, HitFormulaOperation.FormulaStatType statType)
        {
            switch(statType)
            {
                case HitFormulaOperation.FormulaStatType.Level:
                    return caster.GetLevel();
                case HitFormulaOperation.FormulaStatType.HP:
                    return caster.HP;
                case HitFormulaOperation.FormulaStatType.SP:
                    return caster.SP;
                case HitFormulaOperation.FormulaStatType.TP:
                    return caster.TP;
                case HitFormulaOperation.FormulaStatType.MaxHP:
                    return caster.GetMaxHP();
                case HitFormulaOperation.FormulaStatType.MaxSP:
                    return caster.GetMaxSP();
                case HitFormulaOperation.FormulaStatType.MaxTP:
                    return caster.GetMaxTP();
                case HitFormulaOperation.FormulaStatType.ATK:
                    return caster.GetATK();
                case HitFormulaOperation.FormulaStatType.DEF:
                    return caster.GetDEF();
                case HitFormulaOperation.FormulaStatType.SATK:
                    return caster.GetSATK();
                case HitFormulaOperation.FormulaStatType.SDEF:
                    return caster.GetSDEF();
                case HitFormulaOperation.FormulaStatType.AGI:
                    return caster.GetAGI();
                case HitFormulaOperation.FormulaStatType.LUK:
                    return caster.GetAGI();
                default:
                    return caster.GetLevel();
            }
        }
        private int ApplyOperation(float termA, float termB, HitFormulaOperation.FormulaOperationType formulaOp)
        {
            float total = 0f;
            if (formulaOp == HitFormulaOperation.FormulaOperationType.Addition) total = termA + termB;
            if (formulaOp == HitFormulaOperation.FormulaOperationType.Substraction) total = termA - termB;
            if (formulaOp == HitFormulaOperation.FormulaOperationType.Multiplication) total = termA * termB;
            if (formulaOp == HitFormulaOperation.FormulaOperationType.Division)
            {
                float divider = termB;
                if (divider == 0) divider = 1;
                total = termA / divider;
            }
            return LISAUtility.Truncate(total);
        }
    }
    [System.Serializable]
    public class HitFormulaOperation
    {
        public enum FormulaCasterType
        {
            FlatNumber = 0,
            User = 1,
            Target = 2
        }
        public enum FormulaStatType
        {
            Level = 0,
            HP = 1,
            SP = 2,
            TP = 3,
            MaxHP = 4,
            MaxSP = 5,
            ATK = 6,
            DEF = 7,
            SATK = 8,
            SDEF = 9,
            AGI = 10,
            LUK = 11,
            MaxTP = 12 // Change this
        }
        public enum FormulaOperationType
        {
            None = 0,
            Addition = 1,
            Substraction = 2,
            Multiplication = 3,
            Division = 4
        }
        public FormulaOperationType formulaOp = FormulaOperationType.Addition;
        public FormulaCasterType formulaTargetable = FormulaCasterType.User;
        public FormulaStatType formulaStat = FormulaStatType.ATK;

        public float flatNumber = 250;
    }
    [System.Serializable]
    public class HitFormulaGroup
    {
        public HitFormulaOperation.FormulaOperationType formulaGroupOp = HitFormulaOperation.FormulaOperationType.Addition;
        public List<HitFormulaOperation> formulaOperations = new List<HitFormulaOperation>();
    }
}
