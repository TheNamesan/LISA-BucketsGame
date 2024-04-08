using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TUFF
{
    [System.Serializable]
    public class ConfigData
    {
        public static ConfigData instance { get => GameManager.instance.configData; }
        public int refreshRate = 60;
        public bool fullscreen = true;
        public int resolutionWidth = 0;
        public int resolutionHeight = 0;
        [Range(0f, 1f)] public float globalMusicVolume = 1f;
        [Range(0f, 1f)] public float globalSFXVolume = 1f;
        [Range(0f, 1f)] public float globalAmbienceVolume = 1f;
        public int textSpeed = 0;
        public string keybinds = "";
        public bool lost = false;
        public bool bucketsAutoFire = false;
        public bool bucketsToggleSlowmo = false;
        public bool bucketsAimAutoFire = true;
        public bool bucketsComplete = false;
        public bool bucketsPainModeComplete = false;
        public bool bucketsRandoLevelsUnlocked = false;
        public bool bucketsTimer = false;
        public bool bucketsPainMode = false;
        public bool bucketsRusherMode = false;
        

        public void SaveData()
        {
            SaveDataConverter.SaveConfigData(this);
        }

        public void LoadData()
        {
            ConfigData load = SaveDataConverter.LoadConfigData();
            if (load == null) { Debug.LogWarning("Config file could not be loaded!"); return; }
            refreshRate = load.refreshRate;
            fullscreen = load.fullscreen;
            resolutionWidth = load.resolutionWidth;
            resolutionHeight = load.resolutionHeight;
            globalMusicVolume = load.globalMusicVolume;
            globalSFXVolume = load.globalSFXVolume;
            globalAmbienceVolume = load.globalAmbienceVolume;
            textSpeed = load.textSpeed;
            keybinds = load.keybinds;
            lost = load.lost;
            bucketsAutoFire = load.bucketsAutoFire;
            bucketsToggleSlowmo = load.bucketsToggleSlowmo;
            bucketsAimAutoFire = load.bucketsAimAutoFire;
            bucketsComplete = load.bucketsComplete;
            bucketsPainModeComplete = load.bucketsPainModeComplete;
            bucketsRandoLevelsUnlocked = load.bucketsRandoLevelsUnlocked;
            bucketsTimer = load.bucketsTimer;
            bucketsPainMode = load.bucketsPainMode;
            bucketsRusherMode = load.bucketsRusherMode;
            
        }
        public static ConfigData GetDefaultData() // Set Best Settings for current device on first load here
        {
            ConfigData configData = new ConfigData();
            
            configData.fullscreen = true;
            var res = GameManager.instance.highestResolution;
            configData.resolutionWidth = res.width;
            configData.resolutionHeight = res.height;
            configData.refreshRate = GameManager.instance.GetMaxUserRefreshRate();
            configData.globalMusicVolume = 1f;
            configData.globalSFXVolume = 1f;
            configData.globalAmbienceVolume = 1f;
            configData.textSpeed = 0;
            configData.keybinds = GameManager.instance.GetDefaultKeybinds();
            configData.lost = false;
            configData.bucketsAutoFire = true;
            configData.bucketsToggleSlowmo = false;
            configData.bucketsAimAutoFire = true;
            configData.bucketsComplete = false;
            configData.bucketsPainModeComplete = false;
            configData.bucketsRandoLevelsUnlocked = false;
            configData.bucketsTimer = false;
            configData.bucketsPainMode = false;
            configData.bucketsRusherMode = false;


            return configData;
        }
    }
}
