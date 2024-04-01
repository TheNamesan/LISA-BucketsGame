using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TUFF;
using TMPro;


namespace BucketsGame
{
    public class BucketsTitleMenuManager : MonoBehaviour
    {
        public string newGameScene = "";
        public UIButton painModeButton;
        public UIButton rusherModeButton;

        public void Start()
        {
            UpdatePainModeText();
            UpdateRusherModeText();
            if (painModeButton) 
                painModeButton.gameObject.SetActive(GameManager.instance.configData.bucketsComplete);
            if (rusherModeButton)
                rusherModeButton.gameObject.SetActive(GameManager.instance.configData.bucketsPainModeComplete);
        }
        public void A_NewGame()
        {
            BucketsGameManager.instance.SetNewGame(true);
            StartCoroutine(NewGame());
        }
        protected IEnumerator NewGame()
        {
            UIController.instance.SetMenu(null);
            UIController.instance.fadeScreen.TriggerFadeOut(1f);
            AudioManager.instance.FadeOutVolume(1f);
            //GameManager.instance.StartNewGame();
            yield return new WaitForSeconds(2f);
            SceneLoaderManager.instance.LoadSceneWithFadeIn(newGameScene, 0.5f, TUFFSettings.startingScenePosition, TUFFSettings.startingSceneFacing, true, true);
        }
        public void A_Options()
        {
            UIController.instance.OpenOptionsMenu();
        }
        public void A_Exit()
        {
            StartCoroutine(Exit());
        }
        protected IEnumerator Exit()
        {
            UIController.instance.SetMenu(null);
            UIController.instance.fadeScreen.TriggerFadeOut(1f);
            AudioManager.instance.FadeOutVolume(1f);
            yield return new WaitForSeconds(1f);
            Debug.Log("Exiting Game");
            Application.Quit();
        }
        public void A_PainModeToggle()
        {
            GameManager.instance.configData.bucketsPainMode = !GameManager.instance.configData.bucketsPainMode;
            GameManager.instance.configData.SaveData();
            UpdatePainModeText();
        }
        public void A_RusherModeToggle()
        {
            GameManager.instance.configData.bucketsRusherMode = !GameManager.instance.configData.bucketsRusherMode;
            GameManager.instance.configData.SaveData();
            UpdateRusherModeText();
        }

        public void UpdatePainModeText()
        {
            if (painModeButton && painModeButton.text)
                painModeButton.text.text = $"Pain Mode {(GameManager.instance.configData.bucketsPainMode ? "ON" : "OFF")}";
        }
        public void UpdateRusherModeText()
        {
            if (rusherModeButton && rusherModeButton.text)
                rusherModeButton.text.text = $"Rusher Mode {(GameManager.instance.configData.bucketsRusherMode ? "ON" : "OFF")}";
        }
        
    }
}

