using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class PartyMember : Targetable
    {
        public virtual Unit unitRef { get; set; }
        public Job job { get => m_job; }
        [SerializeField] protected Job m_job;
        public int prevExp = 0;
        public int exp = 0;
        public int prevLevel = 1;
        public int level = 1;

        // Equipment
        public Weapon primaryWeapon;
        public Weapon secondaryWeapon;
        public Armor head;
        public Armor body;
        public Armor primaryAccessory;
        public Armor secondaryAccessory;

        // Skills
        public bool[] learnedSkills = new bool[0];

        public PartyMember()
        {
            if (DatabaseLoader.instance != null)
                learnedSkills = new bool[DatabaseLoader.instance.skills.Length];
        }
        public override void TakeHit(BattleAnimationEvent hitInfo, int targetIndex)
        {
            base.TakeHit(hitInfo, targetIndex);
        }
        public override void OnBattleStart()
        {
            base.OnBattleStart();
            if (job != null && job.resetTPOnBattleStart)
            {
                int random = Random.Range(job.startTPMin, job.startTPMax);
                float percentage = random * 0.01f;
                Debug.Log(percentage);
                int value = Mathf.RoundToInt(GetMaxTP() * percentage);
                Debug.Log(value);
                SetTP(value);
            }
        }
        public override void CapHP()
        {
            bool prevIsKOd = isKOd;
            base.CapHP();
            if (isKOd) RemoveAllStates();
            if (BattleManager.instance.InBattle && HP <= 0 && !prevIsKOd) AudioManager.instance.PlaySFX(TUFFSettings.unitKOSFX);
        }
        public override string GetName()
        {
            return unitRef.GetName();
        }
        public override int GetLevel()
        {
            return level;
        }
        public void SetLevel(int newLevel)
        {
            newLevel = Mathf.Min(newLevel, 100); // Cap Max, Replace with level cap
            newLevel = Mathf.Max(0, newLevel); // Cap Min
            level = newLevel;
            exp = job.LevelToStat(level, LevelToStatType.EXP);
            var skills = m_job?.GetSkillsToLearnAtLevel(level, 1);
            foreach (Skill skl in skills) LearnSkill(skl);
        }
        public void AddLevel(int levelAdd)
        {
            SetLevel(level + levelAdd);
        }
        public void AddEXP(int expAdd)
        {
            prevExp = exp;
            exp += expAdd;
            prevLevel = level;
            float levelProgress = 1f;
            while (levelProgress >= 1f)
            {
                if (level == 100) break; // Replace with level cap
                levelProgress = GetNextLevelProgress(level, job, exp);
                if (levelProgress >= 1f) { 
                    level++;
                    Debug.Log("LEVEL UP! " + level); 
                }
            }
            var skills = m_job?.GetSkillsToLearnAtLevel(level, 1);
            foreach (Skill skl in skills) LearnSkill(skl);
        }
        public void SetEXP(int expSet)
        {
            expSet = Mathf.Max(0, expSet);
            int expDifference = expSet - exp;
            AddEXP(expDifference);
        }
        public void Equip(IEquipable equipable, EquipmentSlotType slot)
        {
            PlayerData.instance.EquipToUser(this, equipable, slot);
        }
        public void Unequip(EquipmentSlotType slot)
        {
            PlayerData.instance.UnequipFromUser(this, slot);
        }
        public static float GetNextLevelProgress(int level, Job job, int exp)
        {
            if (level == 100) return 0f; //Replace with level cap
            int currentLevelExp = job.LevelToStat(level, LevelToStatType.EXP);
            int nextLevelExp = job.LevelToStat(level + 1, LevelToStatType.EXP);
            return Mathf.Lerp(0f, 1f, Mathf.InverseLerp(currentLevelExp, nextLevelExp, exp));
        }
        public override Job GetJob() {
            Debug.Log(job);
            return job;
        }
        public Sprite GetGraphic()
        {
            if (isKOd)
            {
                Sprite sprite = job.faceGraphics.KOFaceGraphic;
                if (sprite == null) sprite = job.faceGraphics.defaultFaceGraphic;
                return sprite;
            }
            // Find graphic sprite for latest state
            for (int i = states.Count - 1; i >= 0; i--)
            {
                int id = states[i].state.id;
                // Safety check for out of bounds sprite
                if (id >= job.faceGraphics.stateFaces.Count)
                {
                    job.faceGraphics.stateFaces.AddRange(new Sprite[(id - job.faceGraphics.stateFaces.Count + 1) + 1]);
                }
                Sprite sprite = job.faceGraphics.stateFaces[id];
                if (sprite != null) return sprite;
            }
            // If no states or no sprite was found, use the default one.
            return job.faceGraphics.defaultFaceGraphic;
        }
        public Sprite GetPortraitSprite()
        {
            if (job.menuPortrait != null) return job.menuPortrait;
            return unitRef.defaultMenuPortrait;
        }
        public override List<Command> GetCommands()
        {
            return job.commands;
        }
        public List<CharacterQuoteElement> GetAllWinQuotes()
        {
            return unitRef.winQuotes.GetValidQuotes();
        }
        public string GetRandomWinQuote()
        {
            return RollRandomQuote(GetAllWinQuotes());
        }
        public List<CharacterQuoteElement> GetAllLevelUpQuotes()
        {
            return unitRef.levelUpQuotes.GetValidQuotes();
        }
        public string GetRandomLevelUpQuote()
        {
            return RollRandomQuote(GetAllLevelUpQuotes());
        }
        public List<CharacterQuoteElement> GetAllDropsQuotes()
        {
            return unitRef.dropsQuotes.GetValidQuotes();
        }
        public string GetRandomDropsQuote()
        {
            return RollRandomQuote(GetAllDropsQuotes());
        }
        protected string RollRandomQuote(List<CharacterQuoteElement> quotes)
        {
            if (quotes == null) return null;
            if (quotes.Count <= 0) return "";
            int randomIdx = Random.Range(0, quotes.Count);
            return quotes[randomIdx].GetQuote();
        }
        public List<IEquipable> GetEquipmentAsList()
        {
            var equipmentList = new List<IEquipable>();
            equipmentList.Add(primaryWeapon);
            equipmentList.Add(secondaryWeapon);
            equipmentList.Add(head);
            equipmentList.Add(body);
            equipmentList.Add(primaryAccessory);
            equipmentList.Add(secondaryAccessory);
            return equipmentList;
        }
        public List<IEquipable> GetPreviewEquipment(IEquipable equipable, EquipmentSlotType slot)
        {
            var list = GetEquipmentAsList();
            list[(int)slot] = equipable;
            return list;
        }
        public int GetEquipmentStats(StatChangeType stat)
        {
            IEquipable primaryWeapon = this.primaryWeapon;
            IEquipable secondaryWeapon = this.secondaryWeapon;
            IEquipable head = this.head;
            IEquipable body = this.body;
            IEquipable primaryAccessory = this.primaryAccessory;
            IEquipable secondaryAccessory = this.secondaryAccessory;
            int pWeaponValue = (primaryWeapon != null ? primaryWeapon.GetBonusesOfStat(stat) : 0);
            int sWeaponValue = (secondaryWeapon != null ? secondaryWeapon.GetBonusesOfStat(stat) : 0);
            int headValue = (head != null ? head.GetBonusesOfStat(stat) : 0);
            int bodyValue = (body != null ? body.GetBonusesOfStat(stat) : 0);
            int pAccessoryValue = (primaryAccessory != null ? primaryAccessory.GetBonusesOfStat(stat) : 0);
            int sAccessoryValue = (secondaryAccessory != null ? secondaryAccessory.GetBonusesOfStat(stat) : 0);
            return pWeaponValue + sWeaponValue + headValue + bodyValue + pAccessoryValue + sAccessoryValue;
        }
        public int GetEquipmentStats(StatChangeType stat, List<IEquipable> equipables)
        {
            int value = 0;
            for (int i = 0; i < equipables.Count; i++)
            {
                if (equipables[i] == null) continue;
                value += equipables[i].GetBonusesOfStat(stat);
            }
            return value;
        }
        public override int GetBaseMaxHP()
        {
            return job.LevelToStat(level, LevelToStatType.MaxHP);
        }
        public override int GetBaseMaxSP()
        {
            return job.LevelToStat(level, LevelToStatType.MaxSP);
        }
        public override int GetBaseMaxTP()
        {
            return job.LevelToStat(level, LevelToStatType.MaxTP);
        }
        public override int GetBaseATK()
        {
            return job.LevelToStat(level, LevelToStatType.ATK);
        }
        public override int GetBaseDEF()
        {
            return job.LevelToStat(level, LevelToStatType.DEF);
        }
        public override int GetBaseSATK()
        {
            return job.LevelToStat(level, LevelToStatType.SATK);
        }
        public override int GetBaseSDEF()
        {
            return job.LevelToStat(level, LevelToStatType.SDEF);
        }
        public override int GetBaseAGI()
        {
            return job.LevelToStat(level, LevelToStatType.AGI);
        }
        public override int GetBaseLUK()
        {
            return job.LevelToStat(level, LevelToStatType.LUK);
        }
        public override int GetBaseHitRate()
        {
            return job.hitRate;
        }
        public override int GetBaseEvasionRate()
        {
            return job.evasionRate;
        }
        public override int GetBaseCritRate()
        {
            return job.critRate;
        }
        public override int GetBaseCritEvasionRate()
        {
            return job.critEvasionRate;
        }
        public override int GetBaseTargetRate()
        {
            return job.targetRate;
        }
        public override int GetMaxHP()
        {
            return GetMaxHP(GetEquipmentAsList());
        }
        public int GetMaxHP(List<IEquipable> equipables)
        {
            var flatValue = GetBaseMaxHP() + GetEquipmentStats(StatChangeType.MaxHP, equipables); // Insert armor stats + bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange, equipables), StatChangeType.MaxHP));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public override int GetMaxSP()
        {
            return GetMaxSP(GetEquipmentAsList());
        }
        public int GetMaxSP(List<IEquipable> equipables)
        {
            var flatValue = GetBaseMaxSP() + GetEquipmentStats(StatChangeType.MaxSP, equipables); // Insert armor stats + bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange, equipables), StatChangeType.MaxSP));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public override int GetMaxTP()
        {
            return GetMaxTP(GetEquipmentAsList());
        }
        public int GetMaxTP(List<IEquipable> equipables)
        {
            var flatValue = GetBaseMaxTP() + GetEquipmentStats(StatChangeType.MaxTP, equipables); // Insert armor stats + bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange, equipables), StatChangeType.MaxTP));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public override int GetATK()
        {
            return GetATK(GetEquipmentAsList());
        }
        public int GetATK(List<IEquipable> equipables)
        {
            var flatValue = GetBaseATK() + GetEquipmentStats(StatChangeType.ATK, equipables); // Insert bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange, equipables), StatChangeType.ATK));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public override int GetDEF()
        {
            return GetDEF(GetEquipmentAsList());
        }
        public int GetDEF(List<IEquipable> equipables)
        {
            var flatValue = GetBaseDEF() + GetEquipmentStats(StatChangeType.DEF, equipables); // Insert armor stats + bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange, equipables), StatChangeType.DEF));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public override int GetSATK()
        {
            return GetSATK(GetEquipmentAsList());
        }
        public int GetSATK(List<IEquipable> equipables)
        {
            var flatValue = GetBaseSATK() + GetEquipmentStats(StatChangeType.SATK, equipables); // Insert armor stats + bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange, equipables), StatChangeType.SATK));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public override int GetSDEF()
        {
            return GetSDEF(GetEquipmentAsList());
        }
        public int GetSDEF(List<IEquipable> equipables)
        {
            var flatValue = GetBaseSDEF() + GetEquipmentStats(StatChangeType.SDEF, equipables); // Insert armor stats + bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange, equipables), StatChangeType.SDEF));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public override int GetAGI()
        {
            return GetAGI(GetEquipmentAsList());
        }
        public int GetAGI(List<IEquipable> equipables)
        {
            var flatValue = GetBaseAGI() + GetEquipmentStats(StatChangeType.AGI, equipables); // Insert armor stats + bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange, equipables), StatChangeType.AGI));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public override int GetLUK()
        {
            return GetLUK(GetEquipmentAsList());
        }
        public int GetLUK(List<IEquipable> equipables)
        {
            var flatValue = GetBaseLUK() + GetEquipmentStats(StatChangeType.LUK, equipables); // Insert armor stats + bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange, equipables), StatChangeType.LUK));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public virtual float GetHitRate(List<IEquipable> equipables)
        {
            var value = (GetBaseHitRate() * 0.01f) + BattleManager.GetExtraRateChange(GetAllFeaturesOfType(FeatureType.ExtraRateChange, equipables), ExtraRateChangeType.HitRate);
            return value;
        }
        public virtual float GetEvasionRate(List<IEquipable> equipables)
        {
            var value = (GetBaseEvasionRate() * 0.01f) + BattleManager.GetExtraRateChange(GetAllFeaturesOfType(FeatureType.ExtraRateChange, equipables), ExtraRateChangeType.EvasionRate);
            return value;
        }
        public virtual float GetCritRate(List<IEquipable> equipables)
        {
            var value = (GetBaseCritRate() * 0.01f) + BattleManager.GetExtraRateChange(GetAllFeaturesOfType(FeatureType.ExtraRateChange, equipables), ExtraRateChangeType.CritRate);
            return value;
        }
        public virtual float GetTargetRate(List<IEquipable> equipables)
        {
            var value = (GetBaseTargetRate() * 0.01f) * BattleManager.GetSpecialRateChange(GetAllFeaturesOfType(FeatureType.SpecialRateChange, equipables), SpecialRateChangeType.TargetRate);
            return value;
        }
        public override List<Feature> GetAllFeaturesOfType(FeatureType featureType)
        {
            var features = new List<Feature>();
            GetUnitFeaturesOfType(featureType, features);
            GetJobFeaturesOfType(featureType, features);
            GetStatesFeaturesOfType(states, featureType, features);
            GetEquipmentFeaturesOfType(featureType, features);
            return features;
        }
        public List<Feature> GetAllFeaturesOfType(FeatureType featureType, List<IEquipable> equipables)
        {
            var features = new List<Feature>();
            GetUnitFeaturesOfType(featureType, features);
            GetJobFeaturesOfType(featureType, features);
            GetStatesFeaturesOfType(states, featureType, features);
            GetEquipmentFeaturesOfType(featureType, equipables, features);
            return features;
        }
        public List<Feature> GetEquipmentFeaturesOfType(FeatureType featureType, List<Feature> featuresRef = null)
        {
            List<Feature> features = featuresRef;
            if (features == null) features = new List<Feature>();
            List<IEquipable> equipables = new List<IEquipable>{ primaryWeapon, secondaryWeapon, head, body, primaryAccessory, secondaryAccessory };
            for (int i = 0; i < equipables.Count; i++)
            {
                if (equipables[i] == null) continue;
                var equipableFeatures = equipables[i].features;
                AddFeaturesOfTypeFrom(equipableFeatures, featureType, features);
            }
            return features;
        }
        public List<Feature> GetEquipmentFeaturesOfType(FeatureType featureType, List<IEquipable> equipables, List<Feature> featuresRef = null)
        {
            List<Feature> features = featuresRef;
            if (features == null) features = new List<Feature>();
            if (equipables == null) equipables = new List<IEquipable> { primaryWeapon, secondaryWeapon, head, body, primaryAccessory, secondaryAccessory };
            for (int i = 0; i < equipables.Count; i++)
            {
                if (equipables[i] == null) continue;
                var equipableFeatures = equipables[i].features;
                AddFeaturesOfTypeFrom(equipableFeatures, featureType, features);
            }
            return features;
        }
        public List<Feature> GetUnitFeaturesOfType(FeatureType featureType, List<Feature> featuresRef = null)
        {
            List<Feature> features = featuresRef;
            if (features == null) features = new List<Feature>();
            var unitFeatures = unitRef.features;
            AddFeaturesOfTypeFrom(unitFeatures, featureType, features);
            return features;
        }
        public List<Feature> GetJobFeaturesOfType(FeatureType featureType, List<Feature> featuresRef = null)
        {
            List<Feature> features = featuresRef;
            if (features == null) features = new List<Feature>();
            if (job == null) return features;
            var jobFeatures = job.features;
            AddFeaturesOfTypeFrom(jobFeatures, featureType, features);
            return features;
        }
        public virtual void AssignJob(Job job)
        {
            m_job = job;
            if (job == null) return;
            var skills = job?.GetSkillsToLearnAtLevel(level, 1);
            foreach (Skill skl in skills) LearnSkill(skl);
        }
        
        public virtual void LearnSkill(Skill skill, bool learn = true)
        {
            if (skill == null) return;
            LearnSkill(skill.id, learn);
        }
        public virtual void LearnSkill(int id, bool learn = true)
        {
            if (id < 0 || id >= learnedSkills.Length) return;
            learnedSkills[id] = learn;
        }
        public virtual bool KnowsSkill(Skill skill)
        {
            if (skill == null) return false;
            if (TUFFSettings.DebugIgnoreLearnedSkills()) return true;
            return KnowsSkill(skill.id);
        }
        public virtual bool KnowsSkill(int id)
        {
            if (id < 0 || id >= learnedSkills.Length) return false;
            if (TUFFSettings.DebugIgnoreLearnedSkills()) return true;
            return learnedSkills[id];
        }
    }
}