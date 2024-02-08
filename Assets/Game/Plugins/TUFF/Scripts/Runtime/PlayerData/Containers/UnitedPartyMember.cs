using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{ 
    public class UnitedPartyMember : PartyMember
    {
        public override Unit unitRef { get { return userA.unitRef; } set { userA.unitRef = value; } }
        public Unit unitRefA { get { return userA.unitRef; } set { userA.unitRef = value; } }
        public Unit unitRefB { get { return userB.unitRef; } set { userB.unitRef = value; } }
        public PartyMember userA;
        public PartyMember userB;
        public override bool acted { get => base.acted; 
            set { 
                base.acted = value;
                userA.acted = value;
                userB.acted = value;
            }
        }

        public UnitedPartyMember(PartyMember userA, PartyMember userB)
        {
            this.userA = userA;
            this.userB = userB;

            m_job = this.userA.job;
            level = this.userA.level;
            exp = this.userA.exp;
            HP = GetMaxHP();
            prevHP = HP;
            SP = GetMaxSP();
            TP = 0;
        }
        public override string GetName()
        {
            return unitRefA.GetName() + " & " + unitRefB.GetName();
        }
        public override int GetMaxHP()
        {
            return userA.GetMaxHP() + userB.GetMaxHP();
        }
        public override int GetMaxSP()
        {
            return userA.GetMaxSP() + userB.GetMaxSP();
        }
        public override int GetMaxTP()
        {
            return userA.GetMaxTP() + userB.GetMaxTP();
        }
        public override int GetATK()
        {
            return userA.GetATK() + userB.GetATK();
        }
        public override int GetDEF()
        {
            return userA.GetDEF() + userB.GetDEF();
        }
        public override int GetSATK()
        {
            return userA.GetSATK() + userB.GetSATK();
        }
        public override int GetSDEF()
        {
            return userA.GetSATK() + userB.GetSATK();
        }
        public override int GetAGI()
        {
            return userA.GetAGI() + userB.GetAGI();
        }
        public override int GetLUK()
        {
            return userA.GetLUK() + userB.GetLUK();
        }
        public override float GetHitRate()
        {
            return userA.GetHitRate() + userB.GetHitRate();
        }
        public override float GetEvasionRate()
        {
            return userA.GetEvasionRate() + userB.GetEvasionRate();
        }
        public override float GetCritRate()
        {
            return userA.GetCritRate() + userB.GetCritRate();
        }
        public override float GetTargetRate()
        {
            return userA.GetTargetRate() + userB.GetTargetRate();
        }
        //public override int GetBaseMaxHP()
        //{
        //    return userA.GetBaseMaxHP() + userB.GetBaseMaxHP();
        //}
        //public override int GetBaseMaxSP()
        //{
        //    return userA.GetBaseMaxSP() + userB.GetBaseMaxSP();
        //}
        //public override int GetBaseMaxTP()
        //{
        //    return userA.GetBaseMaxTP() + userB.GetBaseMaxTP();
        //}
        //public override int GetBaseATK()
        //{
        //    return userA.GetBaseATK() + userB.GetBaseATK();
        //}
        //public override int GetBaseDEF()
        //{
        //    return userA.GetBaseDEF() + userB.GetBaseDEF();
        //}
        //public override int GetBaseSATK()
        //{
        //    return userA.GetBaseSATK() + userB.GetBaseSATK();
        //}
        //public override int GetBaseSDEF()
        //{
        //    return userA.GetBaseSDEF() + userB.GetBaseSDEF();
        //}
        //public override int GetBaseAGI()
        //{
        //    return userA.GetBaseAGI() + userB.GetBaseAGI();
        //}
        //public override int GetBaseLUK()
        //{
        //    return userA.GetBaseLUK() + userB.GetBaseLUK();
        //}
        //public override int GetBaseHitRate()
        //{
        //    return userA.GetBaseHitRate() + userB.GetBaseHitRate();
        //}
        //public override int GetBaseEvasionRate()
        //{
        //    return userA.GetBaseEvasionRate() + userB.GetBaseEvasionRate();
        //}
        //public override int GetBaseCritRate()
        //{
        //    return userA.GetBaseCritRate() + userB.GetBaseCritRate();
        //}
        //public override int GetBaseCritEvasionRate()
        //{
        //    return job.critEvasionRate;
        //}
        //public override int GetBaseTargetRate()
        //{
        //    return job.targetRate;
        //}
    }
}
