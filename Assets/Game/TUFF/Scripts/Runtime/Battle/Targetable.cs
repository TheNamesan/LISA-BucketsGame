using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public abstract class Targetable
    {
        public int HP = 0;
        public int prevHP = 0;
        public int SP = 0;
        public int TP = 0;
        public bool isKOd = false;
        public virtual bool acted { get => m_acted; set { m_acted = value; } }
        protected bool m_acted = false;
        public List<ActiveState> states = new List<ActiveState>();
        public GraphicHandler imageReference;

        public virtual void TakeHit(BattleAnimationEvent hitInfo, int targetIndex)
        {
            int value = 0;
            bool isCrit = false;
            VulnerabilityType vulType = VulnerabilityType.Normal;
            if (hitInfo.damageType != DamageType.None)
            {
                value = hitInfo.formula.GetFormulaValue(hitInfo, targetIndex);
            }
            if (hitInfo.flashTarget) BattleManager.TintTarget(hitInfo.skillOrigin.targets[targetIndex], hitInfo.flashTargetData.flashColor, hitInfo.flashTargetData.flashDuration);
            switch (hitInfo.damageType)
            {
                case DamageType.HPDamage:
                    ApplyElementVulnerability(hitInfo, ref value, ref vulType);
                    ApplyHitTypeModifiers(hitInfo, ref value);
                    isCrit = GetAndApplyCrit(hitInfo, ref value, targetIndex);
                    GetAndApplyGuard(ref value);
                    RecoverTPByDamage(value);
                    CalculateHP(-value);
                    RollForStatesRemovalByDamage();
                    if (value > 0) imageReference.Twitch(Mathf.Lerp(10, 100, Mathf.InverseLerp(0, GetMaxHP(), value)));
                    break;
                case DamageType.SPDamage:
                    isCrit = GetAndApplyCrit(hitInfo, ref value, targetIndex);
                    CalculateSP(-value);
                    break;
                case DamageType.TPDamage:
                    isCrit = GetAndApplyCrit(hitInfo, ref value, targetIndex);
                    CalculateTP(-value);
                    break;
                case DamageType.HPRecover:
                    isCrit = GetAndApplyCrit(hitInfo, ref value, targetIndex, true);
                    ApplyHealingModifiers(hitInfo, ref value);
                    CalculateHP(value);
                    break;
                case DamageType.SPRecover:
                    isCrit = GetAndApplyCrit(hitInfo, ref value, targetIndex, true);
                    CalculateSP(value);
                    break;
                case DamageType.TPRecover:
                    isCrit = GetAndApplyCrit(hitInfo, ref value, targetIndex, true);
                    CalculateTP(value);
                    break;

                default:
                    break;
            }
            BattleManager.CheckHitEffects(hitInfo, targetIndex);
            if (vulType != VulnerabilityType.Normal && hitInfo.ElementIndex >= 0) BattleManager.instance.DisplayVulnerability(hitInfo.skillOrigin.targets[targetIndex], hitInfo.ElementIndex, vulType);
            if (hitInfo.damageType != DamageType.None) BattleManager.instance.DisplayHit(hitInfo, value, targetIndex, isCrit);
        }
        public virtual void OnBattleStart() {}

        public void ApplyElementVulnerability(BattleAnimationEvent hitInfo, ref int value, ref VulnerabilityType vulType)
        {
            if (hitInfo.ElementIndex >= 0)
            {
                var element = hitInfo.ElementIndex;
                var user = hitInfo.skillOrigin.user;
                float totalMultiplier = BattleManager.GetTotalElementVulnerability(element, user, this);
                
                vulType = BattleManager.GetVulnerabilityType(totalMultiplier);
                value = LISAUtility.Truncate(value * totalMultiplier);
            }
        }
        public void ApplyHitTypeModifiers(BattleAnimationEvent hitInfo, ref int value)
        {
            if (hitInfo == null) return;
            if (hitInfo.damageType == DamageType.HPDamage || hitInfo.damageType == DamageType.SPDamage || hitInfo.damageType == DamageType.TPDamage)
            {
                var user = hitInfo.skillOrigin.user;
                var hitType = hitInfo.hitType;
                float totalMultiplier = BattleManager.GetTotalHitTypeVulnerability(hitType, user, this);
                
                value = LISAUtility.Truncate(value * totalMultiplier);
            }
        }
        public void ApplyHealingModifiers(BattleAnimationEvent hitInfo, ref int value)
        {
            if (hitInfo == null) return;
            var user = hitInfo.skillOrigin.user;
            float totalMultiplier = 1;
            if (hitInfo.skillOrigin.skill is Skill)
                totalMultiplier = BattleManager.GetTotalHealingPotency(user, this);
            else if (hitInfo.skillOrigin.skill is Item)
                totalMultiplier = BattleManager.GetTotalItemPotency(user, this);
            
            value = LISAUtility.Truncate(value * totalMultiplier);
        }
        public virtual float GetElementPotency(int elementIndex)
        {
            if (elementIndex < 0) return 1f;
            var elementVulFeatures = GetAllFeaturesOfType(FeatureType.ElementPotency);
            var multiplier = BattleManager.GetElementPotency(elementVulFeatures, elementIndex);
            Debug.Log("Pot multiplier: " + multiplier + ", Element: " + elementIndex);
            return multiplier;
        }
        public virtual float GetElementVulnerability(int elementIndex)
        {
            if (elementIndex < 0) return 1f;
            var elementVulFeatures = GetAllFeaturesOfType(FeatureType.ElementVulnerability);
            var multiplier = BattleManager.GetElementVulnerability(elementVulFeatures, elementIndex);
            Debug.Log("Vul multiplier: " + multiplier + ", Element: " + elementIndex);
            return multiplier;
        }
        public virtual float GetStatePotency(State state)
        {
            if (state == null) return 1f;
            var stateVulFeatures = GetAllFeaturesOfType(FeatureType.StatePotency);
            var multiplier = BattleManager.GetStatePotency(stateVulFeatures, state);
            return multiplier;
        }
        public virtual float GetStateVulnerability(State state)
        {
            if (state == null) return 1f;
            var stateVulFeatures = GetAllFeaturesOfType(FeatureType.StateVulnerability);
            var multiplier = BattleManager.GetStateVulnerability(stateVulFeatures, state);
            Debug.Log("Vul multiplier: " + multiplier + ", State: " + state.GetName());
            return multiplier;
        }
        protected int GetAndApplyGuard(ref int value)
        {
            if (IsGuarded())
            {
                var baseGuard = 1 - (TUFFSettings.baseGuardDmgReduction * 0.01f);
                if (baseGuard <= 0) value = 0;
                else
                {
                    Debug.Log("baseGuard: " + baseGuard);
                    float guardDivisor = (1f / baseGuard) * (GetGuardPotency());
                    Debug.Log("applying guard of " + guardDivisor);
                    value = (guardDivisor <= 1 ? value : LISAUtility.Truncate(value / guardDivisor));
                }
            }

            return value;
        }

        protected static bool GetAndApplyCrit(BattleAnimationEvent hitInfo, ref int value, int targetIndex, bool ignoreCritEvade = false)
        {
            bool isCrit = BattleManager.IsCriticalHit(hitInfo, targetIndex, ignoreCritEvade);
            if (isCrit)
            {
                value = LISAUtility.Truncate(value * TUFFSettings.critMultiplier);
                if (value < 0) value = 0;
                hitInfo.animationInstance.QueuePause(hitInfo);
            }
            return isCrit;
        }

        public virtual void RecoverHP(int value, bool hideDisplay = false, bool playSFX = false, bool bypassKO = false)
        {
            if (isKOd && !bypassKO) return;
            CalculateHP(value, bypassKO);
            if (!hideDisplay) BattleManager.instance.DisplayRegen(this, Mathf.Abs(value), (value > 0 ? DamageType.HPRecover : DamageType.HPDamage));
            if (BattleManager.instance.InBattle && playSFX && value > 0) AudioManager.instance.PlaySFX(TUFFSettings.recoverySFX);
            BattleManager.instance.ForceUpdateHUD();
        }
        public virtual void RecoverSP(int value, bool hideDisplay = false, bool playSFX = false)
        {
            CalculateSP(value);
            if (!hideDisplay) BattleManager.instance.DisplayRegen(this, Mathf.Abs(value), (value > 0 ? DamageType.SPRecover : DamageType.SPDamage));
            if (BattleManager.instance.InBattle && playSFX && value > 0) AudioManager.instance.PlaySFX(TUFFSettings.recoverySFX);
            BattleManager.instance.ForceUpdateHUD();
        }
        public virtual void RecoverTP(int value, bool hideDisplay = false, bool playSFX = false)
        {
            CalculateTP(value);
            if (!hideDisplay) BattleManager.instance.DisplayRegen(this, Mathf.Abs(value), (value > 0 ? DamageType.TPRecover : DamageType.TPDamage));
            if (BattleManager.instance.InBattle && playSFX && value > 0) AudioManager.instance.PlaySFX(TUFFSettings.recoverySFX);
            BattleManager.instance.ForceUpdateHUD();
        }
        public virtual void RecoverTPByDamage(int value)
        {
            float percentage = TUFFSettings.TPRecoveryByDamageRatio * 0.01f;
            if (TUFFSettings.TPRecoveryByDamageType == TPRecoveryByDamageType.PortionOfDamage)
            {
                int recover = LISAUtility.Truncate(value * percentage);
                if (recover <= 0) recover = 1;
                RecoverTP(recover, true);
            }
            else if (TUFFSettings.TPRecoveryByDamageType == TPRecoveryByDamageType.PortionOfHP)
            {
                int maxHP = GetMaxHP();
                int maxTP = GetMaxTP();
                float maxTPValue = maxTP * percentage;
                float ratio = Mathf.Clamp01((float)value / maxHP);
                int recover = LISAUtility.Truncate(maxTPValue * ratio);
                RecoverTP(recover, true);
            }
        }
        public virtual void CalculateHP(int value, bool bypassKO = false)
        {
            if (BattleManager.instance.disableStatChanges) return;
            if (isKOd && !bypassKO) return;
            UpdatePrevHP();
            HP += value;
            CapHP();
        }
        public virtual void SetHP(int value, bool bypassKO = false)
        {
            if (BattleManager.instance.disableStatChanges) return;
            if (isKOd && !bypassKO) return;
            UpdatePrevHP();
            HP = value;
            CapHP();
        }
        public virtual void UpdatePrevHP()
        {
            prevHP = HP;
        }
        public virtual void CalculateSP(int value)
        {
            if (BattleManager.instance.disableStatChanges) return;
            SP += value;
            CapSP();
        }
        public virtual void SetSP(int value)
        {
            if (BattleManager.instance.disableStatChanges) return;
            SP = value;
            CapSP();
        }
        public virtual void CalculateTP(int value)
        {
            if (BattleManager.instance.disableStatChanges) return;
            TP += value;
            CapTP();
        }
        public virtual void SetTP(int value)
        {
            if (BattleManager.instance.disableStatChanges) return;
            TP = value;
            CapTP();
        }
        public virtual void CapHP()
        {
            if (HP <= 0)
            {
                if (!IsImmuneToKO())
                {
                    isKOd = true;
                }
                HP = 0;
            }
            if (HP > GetMaxHP())
            {
                HP = GetMaxHP();
            }
        }
        public virtual void CapSP()
        {
            SP = Mathf.Clamp(SP, 0, GetMaxSP());
        }
        public virtual void CapTP()
        {
            TP = Mathf.Clamp(TP, 0, GetMaxTP());
        }
        public virtual void PaySkillCost(Skill skill)
        {
            if (skill == null) return;
            var SPCost = skill.GetSPCost(this);
            var TPCost = skill.GetTPCost(this);
            if (SPCost > 0)
            {
                SP -= SPCost;
                CapSP();
            }
            if (TPCost > 0)
            {
                TP -= TPCost;
                CapTP();
            }
            if (skill.UPCost > 0)
            {
                Debug.Log("Skill UPCost: " + skill.UPCost + ", Name: " + skill.GetName());
                PlayerData.instance.battleData.RecoverUPPercentage(-skill.UPCost);
            }
            if (skill.requiredItem != null)
            {
                if (skill.consumeItem) PlayerData.instance.AddToInventory(skill.requiredItem, -skill.itemAmount);
            }
        }
        public virtual void PayItemCost(Item item)
        {
            if (item.consumable) GameManager.instance.playerData.AddToInventory(item, -1);
        }
        public virtual void ApplyRegens()
        {
            if (isKOd) return;
            var HPRegenRate = GetHPRegenRate();
            if (HPRegenRate != 0) {
                RecoverHP(HPRegenRate);
            }
            if (isKOd) return;
            var SPRegenRate = GetSPRegenRate();
            if (SPRegenRate != 0) {
                RecoverSP(SPRegenRate);
            }
            var TPRegenRate = GetTPRegenRate();
            if (TPRegenRate != 0) {
                RecoverTP(TPRegenRate);
            }
        }
        public virtual List<ActiveState> GetAllAutoStates()
        {
            var autoStates = new List<ActiveState>();
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].isAutoState) autoStates.Add(states[i]);
            }
            return autoStates;
        }
        /// <summary>
        /// Updates Auto States and checks for state immunity.
        /// </summary>
        public virtual void UpdateStates()
        {
            UpdateAutoStates();
            UpdateStateImmunity();
        }

        private void UpdateAutoStates()
        {
            // Auto States
            var featuresOfAutoStates = GetAllFeaturesOfType(FeatureType.AutoState);
            var autoStates = new List<ActiveState>();
            for (int i = 0; i < featuresOfAutoStates.Count; i++)
            {
                var state = featuresOfAutoStates[i].autoState;
                if (state == null) continue;
                var activeState = ApplyState(state, false, true, false);
                autoStates.Add(activeState);
            }

            // Remove AutoStates which are no longer in features.
            var currentAutoStates = GetAllAutoStates();
            for (int i = 0; i < currentAutoStates.Count; i++)
            {
                if (!autoStates.Contains(currentAutoStates[i]))
                    RemoveAutoState(currentAutoStates[i], false);
            }
        }

        public void UpdateStateImmunity(bool updateAutoStates = true)
        {
            var featuresOfStateImmunity = GetAllFeaturesOfType(FeatureType.StateImmunity);
            UpdateStateImmunityInFeatures(featuresOfStateImmunity, updateAutoStates);
        }
        /// <summary>
        /// Checks for State Immunity in the given feature list and removes the state from the user if immune.
        /// </summary>
        /// <param name="featureList"></param>
        public virtual void UpdateStateImmunityInFeatures(List<Feature> featureList, bool updateAutoStates = true)
        {
            if (featureList == null || featureList.Count <= 0) return;
            for (int i = 0; i < featureList.Count; i++)
            {
                if (featureList[i].featureType != FeatureType.StateImmunity) continue;
                if (featureList[i].state == null) continue;
                var state = featureList[i].state;
                RemoveState(state, true, false);
            }
            if (updateAutoStates) UpdateAutoStates();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="showDisplay"></param>
        /// <param name="isAutoState"></param>
        /// <returns>Returns the existing or recently added ActiveState.</returns>
        public virtual ActiveState ApplyState(State state, bool showDisplay = true, bool isAutoState = false, bool updateAutoStates = true)
        {
            if (isKOd && state.stateType != StateType.Permanent) return null;
            ActiveState activeState = null;
            if (!BattleManager.instance.disableStateChanges)
            {
                int existing = FindExistingState(state);
                if (existing >= 0) {
                    states[existing].ResetState(isAutoState);
                    activeState = states[existing];
                }
                else {
                    activeState = new ActiveState(state, this, isAutoState);
                    states.Add(activeState);
                    imageReference?.AddStateVisual(activeState);
                
                    if (state.restriction != Restriction.None)
                        BattleManager.instance.DequeueSkillsFromUser(this);
                }
                UpdateStateImmunityInFeatures(state.features, false);
                UpdatePrevHP();
                if (updateAutoStates) UpdateAutoStates();
            }
            if (showDisplay) BattleManager.instance.DisplayState(this, state);
            return activeState;
        }
        public virtual void ApplyAllStatesOfType(StateType type, bool showDisplay = true)
        {
            var statesOfType = DatabaseLoader.instance.GetAllStatesOfType(type);
            for (int i = 0; i < statesOfType.Count; i++)
            {
                ApplyState(statesOfType[i], showDisplay, false, false);
            }
            UpdateAutoStates();
            BattleManager.instance.ForceUpdateHUD();
        }
        protected virtual void RemoveAutoState(ActiveState activeState, bool displayState = false)
        {
            RemoveState(activeState, displayState, false);
        }
        public virtual void RemoveState(int index, bool displayState = true, bool updateAutoStates = true)
        {
            State state = states[index].state;
            if (!BattleManager.instance.disableStateChanges)
            {
                imageReference?.RemoveStateVisual(states[index]);
                states.RemoveAt(index);
                UpdatePrevHP();
                if (updateAutoStates) UpdateAutoStates();
            }
            if (displayState) BattleManager.instance.DisplayState(this, state, true);
        }
        /// <summary>
        /// Checks if user is inflicted with the state and removes it.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="updateAutoStates"></param>
        public virtual void RemoveState(State state, bool displayState = true, bool updateAutoStates = true)
        {
            int index = FindExistingState(state);
            if (index < 0) return;
            RemoveState(index, displayState, updateAutoStates);
        }
        public virtual void RemoveState(ActiveState activeState, bool displayState = true, bool updateAutoStates = true)
        {
            if (!BattleManager.instance.disableStateChanges)
            {
                imageReference?.RemoveStateVisual(activeState);
                states.Remove(activeState);
                UpdatePrevHP();
                if (updateAutoStates) UpdateAutoStates();
            }
            if (displayState) BattleManager.instance.DisplayState(this, activeState.state, true);
        }
        public virtual void RemoveAllStates(bool showDisplay = false, bool removePermanents = false)
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].state.stateType == StateType.Permanent && !removePermanents) continue;
                RemoveState(i, showDisplay, false);
                i--;
            }
            UpdateStates();
            BattleManager.instance.ForceUpdateHUD();
        }
        public virtual void RemoveAllStatesOfType(StateType type, bool showDisplay = true)
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].state.stateType != type) continue;
                RemoveState(i, showDisplay, false);
                i--;
            }
            UpdateStates();
            BattleManager.instance.ForceUpdateHUD();
        }
        public virtual void RemoveStatesWithAtBattleEndCondition()
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (!states[i].state.removeAtBattleEnd) continue;
                RemoveState(i, false, false);
                i--;
            }
            UpdateStates();
            BattleManager.instance.ForceUpdateHUD();
        }
        public virtual void RollForStatesRemovalByDamage()
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (!states[i].state.removeByDamage) continue;
                if(BattleManager.RollChance(states[i].state.removeByDamageChance))
                {
                    RemoveState(i, true, false);
                    i--;
                }
            }
            UpdateStates();
            BattleManager.instance.ForceUpdateHUD();
        }
        public virtual void RemoveKO()
        {
            isKOd = false;
            if (HP <= 0) RecoverHP(1, true, false, true);
        }
        /// <summary>
        /// Returns the index of the existing ActiveState in the states list
        /// </summary>
        /// <param name="state">The state to find</param>
        /// <returns>The index of the existing ActiveState</returns>
        public virtual int FindExistingState(State state)
        {
            for(int i = 0; i < states.Count; i++)
            {
                if (states[i].state == state) return i;
            }
            return -1;
        }
        public bool HasState(State state)
        {
            return FindExistingState(state) >= 0;
        }
        public virtual bool IsImmuneToState(State state)
        {
            if (state == null) return false;
            var featuresOfStateImmunity = GetAllFeaturesOfType(FeatureType.StateImmunity);
            for (int i = 0; i < featuresOfStateImmunity.Count; i++)
            {
                var feature = featuresOfStateImmunity[i];
                if (feature.featureType != FeatureType.StateImmunity) continue;
                if (feature.state == null) continue;
                if (feature.state == state) { Debug.Log("Immune to: " + state.GetName()); return true; }
            }
            return false;
        }
        public virtual void RecoverAll(bool curePermanents = false)
        {
            RemoveKO();
            RemoveAllStates(removePermanents: curePermanents);
            int maxHP = GetMaxHP();
            int maxSP = GetMaxSP();
            int maxTP = GetMaxTP();
            if (HP < maxHP) SetHP(maxHP);
            if (SP < maxSP) SetSP(maxSP);
            if (TP < maxTP) SetTP(maxTP);
        }
        public virtual string GetName() { return ""; }
        public virtual int GetLevel() { return 0; }
        public virtual Job GetJob() { return null; }
        public virtual List<Command> GetCommands() { return null; }
        public virtual int GetBaseMaxHP() { return 0; }
        public virtual int GetBaseMaxSP() { return 0; }
        public virtual int GetBaseMaxTP() { return 100; } //Change with user's MaxTP later
        public virtual int GetBaseATK() { return 0; }
        public virtual int GetBaseDEF() { return 0; }
        public virtual int GetBaseSATK() { return 0; }
        public virtual int GetBaseSDEF() { return 0; }
        public virtual int GetBaseAGI() { return 0; }
        public virtual int GetBaseLUK() { return 0; }
        public virtual int GetBaseHitRate() { return 0; }
        public virtual int GetBaseEvasionRate() { return 0; }
        public virtual int GetBaseCritRate() { return 0; }
        public virtual int GetBaseCritEvasionRate() { return 0; }
        public virtual int GetBaseTargetRate() { return 0; }
        public virtual float GetHPPercentage()
        {
            return (HP / (float)GetMaxHP()) * 100f;
        }
        public virtual int GetMaxHP() 
        {
            var flatValue = GetBaseMaxHP(); //Insert armor bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange), StatChangeType.MaxHP));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value); 
        }
        public virtual int GetMaxSP()
        {
            var flatValue = GetBaseMaxSP(); //Insert armor bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange), StatChangeType.MaxSP));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public virtual int GetMaxTP()
        {
            var flatValue = GetBaseMaxTP(); //Insert armor bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange), StatChangeType.MaxTP));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public virtual int GetATK() 
        {
            var flatValue = GetBaseATK(); //Insert armor bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange), StatChangeType.ATK));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        } 
        public virtual int GetDEF()
        {
            var flatValue = GetBaseDEF(); //Insert armor bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange), StatChangeType.DEF));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public virtual int GetSATK()
        {
            var flatValue = GetBaseSATK(); //Insert armor bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange), StatChangeType.SATK));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public virtual int GetSDEF()
        {
            var flatValue = GetBaseSDEF(); //Insert armor bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange), StatChangeType.SDEF));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public virtual int GetAGI()
        {
            var flatValue = GetBaseAGI(); //Insert armor bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange), StatChangeType.AGI));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public virtual int GetLUK()
        {
            var flatValue = GetBaseLUK(); //Insert armor bonuses here
            var value = flatValue * (BattleManager.GetStatChange(GetAllFeaturesOfType(FeatureType.StatChange), StatChangeType.LUK));
            if (value < 1) value = 1;
            return LISAUtility.Truncate(value);
        }
        public virtual float GetHitRate() 
        {
            var value = (GetBaseHitRate() * 0.01f) + BattleManager.GetExtraRateChange(GetAllFeaturesOfType(FeatureType.ExtraRateChange), ExtraRateChangeType.HitRate);
            return value; 
        }
        public virtual float GetEvasionRate()
        {
            var value = (GetBaseEvasionRate() * 0.01f) + BattleManager.GetExtraRateChange(GetAllFeaturesOfType(FeatureType.ExtraRateChange), ExtraRateChangeType.EvasionRate);
            return value;
        }
        public virtual float GetCritRate()
        {
            var value = (GetBaseCritRate() * 0.01f) + BattleManager.GetExtraRateChange(GetAllFeaturesOfType(FeatureType.ExtraRateChange), ExtraRateChangeType.CritRate);
            return value;
        }
        public virtual float GetCritEvasionRate()
        {
            var value = (GetBaseCritEvasionRate() * 0.01f) + BattleManager.GetExtraRateChange(GetAllFeaturesOfType(FeatureType.ExtraRateChange), ExtraRateChangeType.CritEvasionRate);
            return value;
        }
        public virtual int GetHPRegenRate() 
        {
            float value = GetMaxHP() * Mathf.Clamp((BattleManager.GetExtraRateChange(GetAllFeaturesOfType(FeatureType.ExtraRateChange), ExtraRateChangeType.HPRegenRate)), -1, 1);
            return LISAUtility.Truncate(value); 
        } 
        public virtual int GetSPRegenRate()
        {
            float value = GetMaxSP() * Mathf.Clamp((BattleManager.GetExtraRateChange(GetAllFeaturesOfType(FeatureType.ExtraRateChange), ExtraRateChangeType.SPRegenRate)), -1, 1);
            return LISAUtility.Truncate(value);
        }
        public virtual int GetTPRegenRate()
        {
            float value = GetMaxTP() * Mathf.Clamp((BattleManager.GetExtraRateChange(GetAllFeaturesOfType(FeatureType.ExtraRateChange), ExtraRateChangeType.TPRegenRate)), -1, 1);
            return LISAUtility.Truncate(value);
        }
        public virtual float GetTargetRate() 
        {
            var value = (GetBaseTargetRate() * 0.01f) * BattleManager.GetSpecialRateChange(GetAllFeaturesOfType(FeatureType.SpecialRateChange), SpecialRateChangeType.TargetRate);
            return value;
        }
        public virtual float GetGuardPotency() {
            var value = BattleManager.GetSpecialRateChange(GetAllFeaturesOfType(FeatureType.SpecialRateChange), SpecialRateChangeType.GuardPotency);
            return value; 
        }
        public virtual float GetSPCostRate() {
            var value = BattleManager.GetSpecialRateChange(GetAllFeaturesOfType(FeatureType.SpecialRateChange), SpecialRateChangeType.SPCostRate);
            return value;
        }
        public virtual float GetTPCostRate()
        {
            var value = BattleManager.GetSpecialRateChange(GetAllFeaturesOfType(FeatureType.SpecialRateChange), SpecialRateChangeType.TPCostRate);
            return value;
        }
        public virtual float GetHealingDealtPotency()
        {
            var value = BattleManager.GetSpecialRateChange(GetAllFeaturesOfType(FeatureType.SpecialRateChange), SpecialRateChangeType.HealingDealtPotency);
            return value;
        }
        public virtual float GetHealingReceivedPotency()
        {
            var value = BattleManager.GetSpecialRateChange(GetAllFeaturesOfType(FeatureType.SpecialRateChange), SpecialRateChangeType.HealingReceivedPotency);
            return value;
        }
        public virtual float GetItemDealtPotency()
        {
            var value = BattleManager.GetSpecialRateChange(GetAllFeaturesOfType(FeatureType.SpecialRateChange), SpecialRateChangeType.ItemDealtPotency);
            return value;
        }
        public virtual float GetItemReceivedPotency()
        {
            var value = BattleManager.GetSpecialRateChange(GetAllFeaturesOfType(FeatureType.SpecialRateChange), SpecialRateChangeType.ItemReceivedPotency);
            return value;
        }
        public virtual float GetPhysicalDamagePotency()
        {
            var value = BattleManager.GetSpecialRateChange(GetAllFeaturesOfType(FeatureType.SpecialRateChange), SpecialRateChangeType.PhysicalDamagePotency);
            return value;
        }
        public virtual float GetPhysicalDamageVulnerability()
        {
            var value = BattleManager.GetSpecialRateChange(GetAllFeaturesOfType(FeatureType.SpecialRateChange), SpecialRateChangeType.PhysicalDamageVulnerability);
            return value;
        }
        public virtual float GetSpecialDamagePotency()
        {
            var value = BattleManager.GetSpecialRateChange(GetAllFeaturesOfType(FeatureType.SpecialRateChange), SpecialRateChangeType.SpecialDamagePotency);
            return value;
        }
        public virtual float GetSpecialDamageVulnerability()
        {
            var value = BattleManager.GetSpecialRateChange(GetAllFeaturesOfType(FeatureType.SpecialRateChange), SpecialRateChangeType.SpecialDamageVulnerability);
            return value;
        }
        public virtual int GetAttackSpeed() {
            var attackSpeed = (BattleManager.GetAttackSpeed(GetAllFeaturesOfType(FeatureType.AttackSpeed)));
            var value = GetAGI() + attackSpeed;
            return value;
        }
        public virtual void ChangeGraphic(Sprite sprite)
        {
            imageReference.ChangeGraphic(sprite);
        }
        public void HighlightImageRef(bool highlight)
        {
            imageReference.Highlight(highlight);
        }
        public void HighlightImageRef(bool highlight, int elementIndex, VulnerabilityType vulType)
        {
            imageReference.Highlight(highlight, elementIndex, vulType);
        }
        public void MarkVulnerability(int elementIndex, VulnerabilityType vulType, bool activateIfNoVulnerability = false)
        {
            imageReference.ActivateOutline(elementIndex, vulType, activateIfNoVulnerability);
        }
        /// <summary>
        /// Checks if the user can act and choose its own actions. 
        /// User can't act on its own if KO'd or under a state with the "Cannot Move" or "Force Skills" restriction.
        /// </summary>
        /// <returns>Returns true if user can act at will.</returns>
        public virtual bool CanControlAct()
        {
            if (isKOd) return false;
            if (HasStateRestriction() != Restriction.None) return false;
            return true;
        }
        /// <summary>
        /// Checks if the user can act at all. 
        /// User can't act if KO'd or under a state with the "Cannot Move" restriction.
        /// </summary>
        /// <returns>Returns true if user can act, even when uncontrollable.</returns>
        public virtual bool CanAct()
        {
            if (isKOd) return false;
            if (HasStateRestriction() == Restriction.CannotMove) return false;
            return true;
        }
        public Restriction HasStateRestriction()
        {
            Restriction restriction = Restriction.None;
            for(int i = 0; i < states.Count; i++)
            {
                if (states[i].state.restriction == Restriction.None) continue;
                restriction = states[i].state.restriction;
                if (restriction == Restriction.CannotMove)
                {
                    return restriction;
                }
            }
            return restriction;
        }
        public bool HasCommandSeal(Command command)
        {
            if (command == null) return false;
            var sealFeats = GetAllFeaturesOfType(FeatureType.SealCommand);
            int idx = sealFeats.FindIndex(f => f.command == command);
            return idx >= 0;
        }
        public bool HasSkillSeal(Skill skill)
        {
            if (skill == null) return false;
            var sealFeats = GetAllFeaturesOfType(FeatureType.SealSkill);
            int idx = sealFeats.FindIndex(f => f.skill == skill);
            return idx >= 0;
        }
        public virtual List<ActionPattern> GetForcedActions()
        {
            var actionPatterns = new List<ActionPattern>();
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i].state;
                if (state.restriction != Restriction.ForceSkills) continue;
                for(int j = 0; j < state.forcedActionPatterns.Count; j++)
                {
                    actionPatterns.Add(state.forcedActionPatterns[j]);
                }
            }
            return actionPatterns;
        }
        private bool IsGuarded()
        {
            return BattleManager.GetSpecialFeatureIndex(GetAllFeaturesOfType(FeatureType.SpecialFeature), SpecialFeatureType.Guard) >= 0;
        }
        private bool IsImmuneToKO()
        {
            return BattleManager.GetSpecialFeatureIndex(GetAllFeaturesOfType(FeatureType.SpecialFeature), SpecialFeatureType.ImmuneToKO) >= 0;
        }
        public virtual bool CanShowStatus()
        {
            return true;
        }
        public virtual List<Feature> GetAllFeaturesOfType(FeatureType featureType)
        {
            var features = new List<Feature>();
            GetStatesFeaturesOfType(states, featureType, features);
            return features;
        }
        public static void AddFeaturesOfTypeFrom(List<Feature> fromFeatureList, FeatureType featureType, List<Feature> featuresRef)
        {
            if (fromFeatureList == null) return;
            List<Feature> features = featuresRef;
            if (features == null) features = new List<Feature>();
            for (int i = 0; i < fromFeatureList.Count; i++)
            {
                var feature = fromFeatureList[i];
                if (feature == null) continue;
                if (feature.featureType != featureType) continue;
                features.Add(feature);
            }
        }
        public static List<Feature> GetStatesFeatures(List<ActiveState> activeStates)
        {
            var features = new List<Feature>();
            for (int i = 0; i < activeStates.Count; i++)
            {
                var tmp = activeStates[i].state.features;
                if (tmp == null) continue;
                for (int j = 0; j < tmp.Count; j++)
                {
                    features.Add(tmp[j]);
                }
            }
            return features;
        }
        public static List<Feature> GetStatesFeaturesOfType(List<ActiveState> activeStates, FeatureType featureType, List<Feature> featuresRef = null)
        {
            List<Feature> features = featuresRef;
            if (features == null) features = new List<Feature>();
            for(int i = 0; i < activeStates.Count; i++)
            {
                var stateFeatures = activeStates[i].state.features;
                AddFeaturesOfTypeFrom(stateFeatures, featureType, features);
            }
            return features;
        }
    }
}

