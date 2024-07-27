using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class PlayerData
    {
        public const int activePartyMaxSize = 4; // Move to TUFF Settings

        public double playtime = 0;
        [Tooltip("Data list of all Units")]
        public PartyMember[] party; //Save 
        public List<sbyte> partyOrder = new List<sbyte>(); //Save
        public Inventory inventory = new Inventory();
        public int mags = 0;
        public PartyBattleData battleData = new PartyBattleData();
        public SceneSaveData sceneData = new SceneSaveData();
        public CharacterProperties charProperties = new CharacterProperties();
        public GameVariable[] gameVariables = new GameVariable[0];
        public int[] persistentInteractableIDs = new int[0];


        public static PlayerData instance
        {
            get
            {
                return (GameManager.instance != null ?
                    GameManager.instance.playerData : null);
            }
        }
        public void SaveData(int file)
        {
            SaveDataConverter.SavePlayerData(this, file);
        }
        public static PlayerData LoadData(int file)
        {
            return SaveDataConverter.LoadPlayerData(file);
        }
        public static bool CheckAnySaveFileExists()
        {
            for (int i = 0; i < TUFFSettings.maxSaveFileSlots; i++)
            {
                if (SaveDataConverter.CheckSaveExistsAtIndex(i)) return true;
            }
            return false;
        }

        public void StartPlayerData()
        {
            InitPartyMembers(); // Always call when starting the game
            UpdateLoadedScene(SceneLoaderManager.currentScene.name); // Set here to avoid issues when debugging scenes without going to the title screen first
            inventory.Initialize();
            InitGameVariables();
            persistentInteractableIDs = new int[PersistentInteractableList.GetPersistentIDLength()];
        }
        public void UpdateLoadedScene(string name)
        {
            sceneData.loadedSceneName = name;
        }
        public void AssignNewGameData()
        {
            UpdateLoadedScene(TUFFSettings.startingSceneName);
            charProperties.playerPosition = TUFFSettings.startingScenePosition;
            charProperties.playerFacing = TUFFSettings.startingSceneFacing;
        }
        private void InitGameVariables()
        {
            var variableData = GameVariableList.GetList();
            gameVariables = new GameVariable[variableData.Length];
            for (int i = 0; i < gameVariables.Length; i++)
            {
                gameVariables[i] = new GameVariable(variableData[i].name, variableData[i].variableType);
            }
        }

        public void InitPartyMembers()
        {
            party = new PartyMember[DatabaseLoader.instance.units.Length];
            partyOrder = new List<sbyte>();
            for (int i = 0; i < party.Length; i++)
            {
                party[i] = InitiatePartyMember(i);
            }
            SetPartyOrderEmpty();
        }
        public void CheckListSizes()
        {
            // If game interactable list length is larger than this save file, resize
            int gameInteractableSize = PersistentInteractableList.GetPersistentIDLength();
            if (gameInteractableSize > persistentInteractableIDs.Length)
            {
                Debug.Log("Resizing Persistent Interactable List");
                System.Array.Resize(ref persistentInteractableIDs, gameInteractableSize);
            }
            // If game variable list length is larger than this save file, resize
            var variableData = GameVariableList.GetList();
            int variablesSize = variableData.Length;
            if (variablesSize > gameVariables.Length)
            {
                Debug.Log("Resizing Game Variable List");
                System.Array.Resize(ref gameVariables, variablesSize);
            }
            for (int i = 0; i < gameVariables.Length; i++)
            {
                gameVariables[i].name = variableData[i].name;
                gameVariables[i].variableType = variableData[i].variableType;
            }
        }
        public void CheckUnitRefs()
        {
            for (int i = 0; i < party.Length; i++)
            {
                party[i].unitRef = DatabaseLoader.instance.units[i];
            }
        }
        public void Update()
        {
            var player = FollowerInstance.player;
            if (player != null)
            {
                charProperties.playerPosition = player.controller.transform.position;
                charProperties.playerFacing = player.controller.faceDirection;
            }
            if (!GameManager.instance.stopPlaytime)
                playtime += Time.unscaledDeltaTime;
        }
        public string GetPlaytimeText()
        {
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(playtime);
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            string hours = (timeSpan.Hours.ToString("00", culture));
            string minutes = (timeSpan.Minutes.ToString("00", culture));
            string seconds = (timeSpan.Seconds.ToString("00", culture));

            return $"{hours}:{minutes}:{seconds}";
        }
        public void GetSceneData(out string sceneName, out Vector3 playerPosition, out FaceDirections playerFacing)
        {
            sceneName = sceneData.loadedSceneName;
            playerPosition = charProperties.playerPosition;
            playerFacing = charProperties.playerFacing;
        }
        private void SetPartyOrderEmpty()
        {
            partyOrder.Clear();
        }

        public void AddToParty(int memberIndex, bool initialize = false)
        {
            if (initialize) InitiatePartyMember(memberIndex);
            if (IsInParty(memberIndex)) return;
            partyOrder.Add((sbyte)memberIndex);
        }

        public void InsertToParty(int memberIndex, int position)
        {
            partyOrder.Insert(position, (sbyte)memberIndex);
        }

        public void RemoveFromParty(int memberIndex)
        {
            partyOrder.Remove((sbyte)memberIndex);
        }
        public bool IsInParty(int memberIndex)
        {
            return partyOrder.IndexOf((sbyte)memberIndex) >= 0;
        }
        public bool IsInParty(Unit unit)
        {
            if (!unit) return false;
            return IsInParty(unit.id);
        }
        public void AddEXPToParty(int exp)
        {
            foreach (sbyte id in partyOrder)
            {
                party[id].AddEXP(exp);
            }
        }
        public void AddMags(int mags)
        {
            this.mags += mags;
            if (this.mags < 0) this.mags = 0;
        }
        public void SetMags(int mags)
        {
            this.mags = mags;
            if (this.mags < 0) this.mags = 0;
        }
        public void RecoverAllFromKO()
        {
            var playerParty = GetAllPartyMembers();
            for (int i = 0; i < playerParty.Count; i++)
            {
                playerParty[i].RemoveKO();
            }
        }
        public void AddToInventory(InventoryItem invItem, int amount)
        {
            if (invItem is Item) AddToInventory(invItem as Item, amount);
            else if (invItem is KeyItem) AddToInventory(invItem as KeyItem, amount);
            else if (invItem is Weapon) AddToInventory(invItem as Weapon, amount);
            else if (invItem is Armor) AddToInventory(invItem as Armor, amount);
        }
        public void AddToInventory(Item item, int amount) => inventory.AddToInventory(item, amount);
        public void AddToInventory(KeyItem keyItem, int amount) => inventory.AddToInventory(keyItem, amount);
        public void AddToInventory(Weapon weapon, int amount) => inventory.AddToInventory(weapon, amount);
        public void AddToInventory(Armor armor, int amount) => inventory.AddToInventory(armor, amount);
        public void EquipToUser(PartyMember user, IEquipable equipable, EquipmentSlotType slot)
        {
            if (user == null) return;
            UnequipFromUser(user, slot);
            EquipInUserSlot(user, equipable, slot);
            user.OnEquipChange();
            if (equipable is Weapon) AddToInventory((Weapon)equipable, -1);
            else if (equipable is Armor) AddToInventory((Armor)equipable, -1);
        }
        public void UnequipFromUser(PartyMember user, EquipmentSlotType slot)
        {
            if (user == null) return;
            var equipable = GetEquipmentFromUserSlot(user, slot);
            EquipInUserSlot(user, null, slot);
            user.OnEquipChange();
            if (equipable is Weapon) AddToInventory((Weapon)equipable, +1);
            else if (equipable is Armor) AddToInventory((Armor)equipable, +1);
        }
        public void OptimizeEquipmentFromUser(PartyMember user)
        {
            // TODO: Add Check for Sealed Equipment so it doesn't get removed.
            if (user == null) return;
            int totalEquipmentSlots = 6;
            for (int i = 0; i < totalEquipmentSlots; i++)
            {
                OptimizeSlot(user, (EquipmentSlotType)i);
            }
        }
        public void ClearEquipmentFromUser(PartyMember user)
        {
            // TODO: Add Check for Sealed Equipment so it doesn't get removed.
            if (user == null) return;
            int totalEquipmentSlots = 6;
            for (int i = 0; i < totalEquipmentSlots; i++)
            {
                UnequipFromUser(user, (EquipmentSlotType)i);
            }
        }
        protected void EquipInUserSlot(PartyMember user, IEquipable equipable, EquipmentSlotType slot)
        {
            if (user == null) return;
            Weapon weapon = (equipable is Weapon ? (Weapon)equipable : null);
            Armor armor = (equipable is Armor ? (Armor)equipable : null); ;
            switch (slot)
            {
                case EquipmentSlotType.PrimaryWeapon:
                    user.primaryWeapon = weapon; return;
                case EquipmentSlotType.SecondaryWeapon:
                    user.secondaryWeapon = weapon; return;
                case EquipmentSlotType.Head:
                    user.head = armor; return;
                case EquipmentSlotType.Body:
                    user.body = armor; return;
                case EquipmentSlotType.PrimaryAccessory:
                    user.primaryAccessory = armor; return;
                case EquipmentSlotType.SecondaryAccessory:
                    user.secondaryAccessory = armor; return;
            }
        }
        protected void OptimizeSlot(PartyMember user, EquipmentSlotType slot)
        {
            if (user == null) return;
            bool isWeapon = (slot == EquipmentSlotType.PrimaryWeapon || slot == EquipmentSlotType.SecondaryWeapon);
            user.GetArmorEquipTypes();
            IEquipable currentEquipable = GetEquipmentFromUserSlot(user, slot);
            IEquipable nextEquipable;
            if (isWeapon) {
                WeaponWieldType wieldType = WeaponWieldType.AnyWeaponSlot;
                if (slot == EquipmentSlotType.PrimaryWeapon) wieldType = WeaponWieldType.PrimarySlotOnly;
                if (slot == EquipmentSlotType.SecondaryWeapon) wieldType = WeaponWieldType.SecondarySlotOnly;
                nextEquipable = GetHighestStatsWeapon(wieldType, user.GetWeaponEquipTypes());
            }
            else {
                EquipType equipType = EquipType.Head;
                if (slot == EquipmentSlotType.Head) equipType = EquipType.Head;
                if (slot == EquipmentSlotType.Body) equipType = EquipType.Body;
                if (slot == EquipmentSlotType.PrimaryAccessory || slot == EquipmentSlotType.SecondaryAccessory) 
                    equipType = EquipType.Accessory;
                nextEquipable = GetHighestStatsArmor(equipType, user.GetArmorEquipTypes());
            }
            if (nextEquipable != null)
            {
                if (currentEquipable != null)
                {
                    if (nextEquipable.GetStatTotal() > currentEquipable.GetStatTotal())
                        EquipToUser(user, nextEquipable, slot);
                }
                else EquipToUser(user, nextEquipable, slot);
            }
        }
        protected IEquipable GetEquipmentFromUserSlot(PartyMember user, EquipmentSlotType slot)
        {
            if (user == null) return null;
            switch (slot)
            {
                case EquipmentSlotType.PrimaryWeapon:
                    return user.primaryWeapon;
                case EquipmentSlotType.SecondaryWeapon:
                    return user.secondaryWeapon;;
                case EquipmentSlotType.Head:
                    return user.head;
                case EquipmentSlotType.Body:
                    return user.body;
                case EquipmentSlotType.PrimaryAccessory:
                    return user.primaryAccessory;
                case EquipmentSlotType.SecondaryAccessory:
                    return user.secondaryAccessory;
            }
            return null;
        }
        public Dictionary<InventoryItem, int> GetItemsAndAmount() => inventory.GetItemsAndAmount();
        public Dictionary<InventoryItem, int> GetKeyItemsAndAmount() => inventory.GetKeyItemsAndAmount();
        public Dictionary<InventoryItem, int> GetAllWeaponsAndAmount() => inventory.GetAllWeaponsAndAmount();
        public Dictionary<InventoryItem, int> GetWeaponsAndAmountOfType(WeaponWieldType wieldType)
           => inventory.GetWeaponsAndAmountOfType(wieldType);
        public Dictionary<InventoryItem, int> GetWeaponsAndAmountOfType(WeaponWieldType wieldType, List<int> weaponTypes) 
            => inventory.GetWeaponsAndAmountOfType(wieldType, weaponTypes);
        
        public Dictionary<InventoryItem, int> GetAllArmorsAndAmount() => inventory.GetAllArmorsAndAmount();
        public Dictionary<InventoryItem, int> GetArmorsAndAmountOfType(EquipType equipType)
            => inventory.GetArmorsAndAmountOfType(equipType);
        public Dictionary<InventoryItem, int> GetArmorsAndAmountOfType(EquipType equipType, List<int> armorTypes) 
            => inventory.GetArmorsAndAmountOfType(equipType, armorTypes);
        public Dictionary<InventoryItem, int> GetEntireInventoryAndAmount() => inventory.GetEntireInventoryAndAmount();
        public IEquipable GetHighestStatsWeapon(WeaponWieldType wieldType, List<int> weaponTypes) => inventory.GetHighestStatsWeapon(wieldType, weaponTypes);
        public IEquipable GetHighestStatsArmor(EquipType equipType, List<int> armorTypes) => inventory.GetHighestStatsArmor(equipType, armorTypes);
        public int GetItemAmountFromPartyEquipment(IEquipable equipable)
        {
            return 0;
        }
        public bool IsValidGameVariableIndex(int index)
        {
            return index >= 0 && index < gameVariables.Length;
        }
        public void AssignGameVariableValue(int index, object value)
        {
            if (!IsValidGameVariableIndex(index)) return;
            gameVariables[index].AssignValue(value);
        }
        public void AssignGameVariableBoolValue(int index, bool value)
        {
            if (!IsValidGameVariableIndex(index)) return;
            gameVariables[index].boolValue = value;
        }
        public void AssignGameVariableFloatValue(int index, float value)
        {
            if (!IsValidGameVariableIndex(index)) return;
            gameVariables[index].numberValue = value;
        }
        public void AssignGameVariableStringValue(int index, string value)
        {
            if (!IsValidGameVariableIndex(index)) return;
            gameVariables[index].stringValue = value;
        }
        public void AssignGameVariableVectorValue(int index, Vector2 value)
        {
            if (!IsValidGameVariableIndex(index)) return;
            gameVariables[index].vectorValue = value;
        }
        public void AssignSwitchToPersistentID(int interactableID, int switchValue)
        {
            if (interactableID < 0 || interactableID >= persistentInteractableIDs.Length) return;
            persistentInteractableIDs[interactableID] = switchValue;
        }
        public int GetSwitchFromPersistentID(int interactableID)
        {
            if (interactableID < 0 || interactableID >= persistentInteractableIDs.Length) return -1;
            return persistentInteractableIDs[interactableID];
        }
        public PartyMember InitiatePartyMember(int index)
        {
            Unit unit = DatabaseLoader.instance.units[index];
            PartyMember initMember = new PartyMember();
            initMember.AllocateLearnedSkillsSize();
            initMember.unitRef = unit;
            initMember.AssignJob(unit.initialJob);
            initMember.level = unit.initialLevel;
            if (TUFFSettings.DebugOverrideUnitInitLevel()) initMember.level = TUFFSettings.overrideUnitInitLevelValue;
            initMember.primaryWeapon = unit.primaryWeapon;
            initMember.secondaryWeapon = unit.secondaryWeapon;
            initMember.head = unit.head;
            initMember.body = unit.body;
            initMember.primaryAccessory = unit.primaryAccessory;
            initMember.secondaryAccessory = unit.secondaryAccessory;

            initMember.exp = unit.initialJob.LevelToStat(initMember.level, LevelToStatType.EXP);
            initMember.HP = initMember.GetMaxHP();//unit.initialJob.LevelToStat(initMember.level, LevelToStatType.MaxHP);
            initMember.prevHP = initMember.HP;
            initMember.SP = initMember.GetMaxSP();//unit.initialJob.LevelToStat(initMember.level, LevelToStatType.MaxSP);
            initMember.TP = 0;

            initMember.UpdateStates();

            return initMember;
        }
        /// <summary>
        /// An active Party Member is a Unit that can participate in battle.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Returns the active party member on the specified slot.</returns>
        public PartyMember GetActivePartyMember(int index)
        {
            if (index >= activePartyMaxSize) return party[partyOrder[activePartyMaxSize - 1]];
            if (index >= partyOrder.Count || index < 0) return null;
            if (index >= party.Length) return null;
            return party[partyOrder[index]];
        }
        public PartyMember GetRandomActivePartyMember()
        {
            int activePartySize = GameManager.instance.playerData.GetActivePartySize();
            int randomIdx = Random.Range(0, activePartySize);
            return GameManager.instance.playerData.GetActivePartyMember(randomIdx);
        }
        /// <summary>
        /// An active Party Member is a Unit that can participate in battle.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns>Returns the slot index of the specified Unit. 
        /// Returns -1 if the Unit is not in the active party.</returns>
        public int GetActivePartyMemberIndex(Unit unit)
        {
            int activePartySize = GameManager.instance.playerData.GetActivePartySize();
            for (int i = 0; i < activePartySize; i++)
            {
                PartyMember target = GameManager.instance.playerData.GetActivePartyMember(i);
                if (target.unitRef == unit) return i;
            }
            return -1;
        }
        public bool IsInActiveParty(Unit unit)
        {
            return GetActivePartyMemberIndex(unit) >= 0;
        }
        public int GetPartySize()
        {
            return partyOrder.Count;
        }
        /// <summary>
        /// Returns the size of all party members that can participate in battle.
        /// </summary>
        public int GetActivePartySize()
        {
            if (partyOrder.Count <= 0) return -1;
            if (partyOrder.Count >= activePartyMaxSize) return activePartyMaxSize;
            return partyOrder.Count;
        }
        public PartyMember GetPartyMember(Unit unit)
        {
            return party[unit.id];
        }
        public List<PartyMember> GetAllPartyMembers()
        {
            var playerParty = new List<PartyMember>();
            foreach (sbyte id in partyOrder)
            {
                playerParty.Add(party[id]);
            }
            return playerParty;
        }
    }
}
