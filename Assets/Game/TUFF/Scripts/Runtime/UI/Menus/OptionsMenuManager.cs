using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class OptionsMenuManager : MonoBehaviour
    {
        [Header("References")]
        public UIMenu uiMenu;
        public UIPicker refreshRate;
        public UIPicker windowMode;
        public UIPicker windowRes;
        public UISlider musicSlider;
        public UISlider sfxSlider;
        public UISlider ambienceSlider;
        public UIPicker textSpeed;
        public UIPicker autoFire;
        public UIPicker swapDash;

        public void OpenOptionsMenu()
        {
            uiMenu.OpenMenu();
        }
        public void SetOptionsValues() // Called from OptionsMenu OnOpenMenu
        {
            if (GameManager.instance == null) return;
            ConfigData configData = GameManager.instance.configData;
            musicSlider.fillAmount = configData.globalMusicVolume * 100;
            sfxSlider.fillAmount = configData.globalSFXVolume * 100;
            ambienceSlider.fillAmount = configData.globalAmbienceVolume * 100;
            windowMode.highlightedOption = configData.fullscreen ? 1 : 0;
            windowMode.UpdateText();

            Resolution currentResolution = new Resolution() { width = configData.resolutionWidth, height = configData.resolutionHeight, refreshRate = configData.refreshRate };
            
            // Refresh Rate
            refreshRate.options = new List<string>();
            var refreshRates = GameManager.instance.frameRates;
            for (int i = 0; i < refreshRates.Length; i++)
            {
                refreshRate.options.Add(LISAUtility.IntToString(refreshRates[i]));
            }
            int frameIndex = System.Array.IndexOf(refreshRates, configData.refreshRate);
            if (frameIndex < 0) frameIndex = 0;
            refreshRate.highlightedOption = frameIndex;
            refreshRate.UpdateText();

            // Resolution
            windowRes.options = new List<string>();
            var supportedResolutions = GameManager.instance.supportedResolutions;
            int index = -1;
            index = System.Array.FindIndex(supportedResolutions,
                e => currentResolution.width == e.width && currentResolution.height == e.height);
            //for (int i = 0; i < supportedResolutions.Length; i++)
            //{
            //    if (currentResolution.width == supportedResolutions[i].width &&
            //        currentResolution.height == supportedResolutions[i].height) { index = i; break; }
            //}
            if (index < 0) 
            {
                GameManager.instance.ExpandSupportedResolutions(currentResolution);
                supportedResolutions = GameManager.instance.supportedResolutions;
                index = supportedResolutions.Length - 1;
            }
            for (int i = 0; i < supportedResolutions.Length; i++)
            {
                windowRes.options.Add(supportedResolutions[i].width + " x " + supportedResolutions[i].height);
            }
            windowRes.highlightedOption = index;
            windowRes.UpdateText();

            // Text Speed
            textSpeed.highlightedOption = GameManager.instance.configData.textSpeed;
            textSpeed.UpdateText();

            autoFire.highlightedOption = GameManager.instance.configData.bucketsAutoFire ? 1 : 0;
            autoFire.UpdateText();

            //swapDash.highlightedOption = GameManager.instance.configData.bucketsSwapDash ? 1 : 0;
            //swapDash.UpdateText();
        }

        public void UpdateGlobalMusicVolume(float volume)
        {
            GameManager.instance.configData.globalMusicVolume = volume / 100;
            GameManager.instance.UpdateGlobalVolume();
        }

        public void UpdateGlobalSFXVolume(float volume)
        {
            GameManager.instance.configData.globalSFXVolume = volume / 100;
            GameManager.instance.UpdateGlobalVolume();
        }

        public void UpdateGlobalAMBSVolume(float volume)
        {
            GameManager.instance.configData.globalAmbienceVolume = volume / 100;
            GameManager.instance.UpdateGlobalVolume();
        }

        public void SetFullscreen()
        {
            //Debug.Log("Updated Fullscreen:" + System.Convert.ToBoolean(windowMode.highlightedOption));
            GameManager.instance.configData.fullscreen = System.Convert.ToBoolean(windowMode.highlightedOption);
            GameManager.instance.SetGameFullscreen(GameManager.instance.configData.fullscreen);
            SaveOptionsData();
        }

        public void SetResolution()
        {
            var res = GameManager.instance.supportedResolutions[windowRes.highlightedOption];
            var frameRate = GameManager.instance.frameRates[refreshRate.highlightedOption];
            GameManager.instance.configData.resolutionWidth = res.width;
            GameManager.instance.configData.resolutionHeight = res.height;
            GameManager.instance.configData.refreshRate = frameRate;
            GameManager.instance.SetGameResolution(res.width, res.height, frameRate);
            SaveOptionsData();
        }

        public void SetTextSpeed()
        {
            GameManager.instance.configData.textSpeed = textSpeed.highlightedOption;
            SaveOptionsData();
        }
        public void SetBucketsAutoFire()
        {
            Debug.Log("Updated Auto Fire: " + autoFire.highlightedOption);
            GameManager.instance.configData.bucketsAutoFire = System.Convert.ToBoolean(autoFire.highlightedOption);
            SaveOptionsData();
        }
        public void SetBucketsSwapDash()
        {
            //Debug.Log("Updated Swap Dash: " + swapDash.highlightedOption);
            //GameManager.instance.configData.bucketsSwapDash = System.Convert.ToBoolean(swapDash.highlightedOption);
            //SaveOptionsData();
        }

        public void SaveOptionsData()
        {
            GameManager.instance.configData.SaveData();
        }
    }
}
