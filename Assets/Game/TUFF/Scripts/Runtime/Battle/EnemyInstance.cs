using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TUFF
{
    [System.Serializable]
    public class EnemyInstance : Targetable
    {
        public Enemy enemyRef;
        public bool playedKOAnimation = false;

        public EnemyInstance(EnemyReference enemy)
        {
            enemyRef = enemy.enemy;
            imageReference = enemy.imageReference;
            HP = enemy.enemy.maxHP;
            prevHP = HP;
            SP = enemy.enemy.maxSP;
        }

        public override void TakeHit(BattleAnimationEvent hitInfo, int targetIndex)
        {
            base.TakeHit(hitInfo, targetIndex);
        }
        public override void CapHP()
        {
            base.CapHP();
        }
        public override void RemoveKO()
        {
            base.RemoveKO();
            playedKOAnimation = false;
            imageReference.userImage.color = imageReference.originalUserColor;
        }
        public override string GetName()
        {
            return enemyRef.GetName();
        }
        public override int GetBaseMaxHP()
        {
            return enemyRef.maxHP;
        }
        public override int GetBaseMaxSP()
        {
            return enemyRef.maxSP;
        }
        public override int GetBaseMaxTP()
        {
            return enemyRef.maxTP;
        }
        public override int GetBaseATK()
        {
            return enemyRef.ATK;
        }
        public override int GetBaseDEF()
        {
            return enemyRef.DEF;
        }
        public override int GetBaseSATK()
        {
            return enemyRef.SATK;
        }
        public override int GetBaseSDEF()
        {
            return enemyRef.SDEF;
        }
        public override int GetBaseAGI()
        {
            return enemyRef.AGI;
        }
        public override int GetBaseLUK()
        {
            return enemyRef.LUK;
        }
        public override int GetBaseHitRate() 
        { 
            return enemyRef.hitRate;
        }
        public override int GetBaseEvasionRate()
        {
            return enemyRef.evasionRate;
        }
        public override int GetBaseCritRate()
        {
            return enemyRef.critRate;
        }
        public override int GetBaseCritEvasionRate()
        {
            return enemyRef.critEvasionRate;
        }
        public override int GetBaseTargetRate()
        {
            return enemyRef.targetRate;
        }
        public override bool CanShowStatus()
        {
            bool show = BattleManager.GetSpecialFeatureIndex(GetAllFeaturesOfType(FeatureType.SpecialFeature), SpecialFeatureType.ShowStatus) >= 0 || TUFFSettings.showEnemyStatsByDefault;
            bool hide = BattleManager.GetSpecialFeatureIndex(GetAllFeaturesOfType(FeatureType.SpecialFeature), SpecialFeatureType.HideStatus) >= 0 || (!show && !TUFFSettings.showEnemyStatsByDefault);
            if (show == hide) return TUFFSettings.showEnemyStatsByDefault;
            return show && !hide;
        }
        public IEnumerator PlayKOAnimation()
        {
            var animKO = BattleManager.instance.PlayAnimation(TUFFSettings.enemyKOAnimation, imageReference.transform.position);
            yield return new WaitUntil(() => animKO.isFinished);
        }
        public IEnumerator PlayKOMotion()
        {
            var motion = new TUFFMotion();
            motion.PlayKOMotion(this);
            yield return new WaitUntil(() => motion.isFinished);
        }
        public override List<Feature> GetAllFeaturesOfType(FeatureType featureType)
        {
            var features = new List<Feature>();
            GetEnemyFeaturesOfType(featureType, features);
            GetStatesFeaturesOfType(states, featureType, features);
            return features;
        }
        public List<Feature> GetEnemyFeaturesOfType(FeatureType featureType, List<Feature> featuresRef = null)
        {
            List<Feature> features = featuresRef;
            if (features == null) features = new List<Feature>();
            var unitFeatures = enemyRef.features;
            AddFeaturesOfTypeFrom(unitFeatures, featureType, features);
            return features;
        }
    }
}

