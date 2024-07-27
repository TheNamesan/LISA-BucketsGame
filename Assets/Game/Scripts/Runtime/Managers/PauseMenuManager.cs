using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUFF;
using UnityEngine.SceneManagement;

namespace BucketsGame
{
    public class PauseMenuManager : MonoBehaviour
    {
        public UIMenu uiMenu;
        public string mainMenuScene = "";
        public void A_Continue()
        {
            uiMenu.CloseMenu();
        }
        public void A_Retry()
        {
            BucketsGameManager.instance.QueueReset();
            uiMenu.CloseMenu();
        }
        public void A_Exit()
        {
            BucketsGameManager.instance.SetNewGame(false);
            TimerManager.instance.Stop();
            uiMenu.CloseMenu();
            StartCoroutine(Exit());
            //SceneManager.LoadScene(mainMenuScene);
        }
        private IEnumerator Exit()
        {
            GameManager.instance.DisablePlayerAndUIInput(true);
            UIController.instance.fadeScreen.FadeOut(1f);
            AudioManager.instance.FadeOutVolume(1f);
            yield return new WaitForSeconds(1f);
            SceneLoaderManager.instance.LoadSceneWithFadeIn(mainMenuScene, 0.5f, TUFFSettings.startingScenePosition, TUFFSettings.startingSceneFacing, true, true);
        }
    
        public void A_Options()
        {
            UIController.instance.OpenOptionsMenu();
        }
        public void OnClose()
        {
            BucketsGameManager.instance.Pause(false);
        }
    }
}
