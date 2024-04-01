using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ConfigData
    {
        public int refreshRate = 60;
        public bool fullscreen = true;
        public int resolutionWidth = 0;
        public int resolutionHeight = 0;
        [Range(0f, 1f)] public float globalMusicVolume = 1f;
        [Range(0f, 1f)] public float globalSFXVolume = 1f;
        [Range(0f, 1f)] public float globalAmbienceVolume = 1f;
        public int textSpeed = 0;
        public bool lost = false;
        public bool bucketsAutoFire = false;
        public bool bucketsSwapDash = false;
        public bool bucketsComplete = false;
        public bool bucketsPainModeComplete = false;
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
            if (load == null) return;
            refreshRate = load.refreshRate;
            fullscreen = load.fullscreen;
            resolutionWidth = load.resolutionWidth;
            resolutionHeight = load.resolutionHeight;
            globalMusicVolume = load.globalMusicVolume;
            globalSFXVolume = load.globalSFXVolume;
            globalAmbienceVolume = load.globalAmbienceVolume;
            textSpeed = load.textSpeed;
            lost = load.lost;
            bucketsAutoFire = load.bucketsAutoFire;
            bucketsSwapDash = load.bucketsSwapDash;
            bucketsComplete = load.bucketsComplete;
            bucketsPainModeComplete = load.bucketsPainModeComplete;
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
            configData.refreshRate = res.refreshRate;
            configData.globalMusicVolume = 1f;
            configData.globalSFXVolume = 1f;
            configData.globalAmbienceVolume = 1f;
            configData.textSpeed = 0;
            configData.lost = false;
            configData.bucketsAutoFire = false;
            configData.bucketsSwapDash = false;
            configData.bucketsComplete = false;
            configData.bucketsPainModeComplete = false;
            configData.bucketsTimer = false;
            configData.bucketsPainMode = false;
            configData.bucketsRusherMode = false;


            return configData;
        }
    }
}
