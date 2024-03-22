using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TUFF;


namespace BucketsGame
{
    public class BucketsTitleMenuManager : MonoBehaviour
    {
        public string newGameScene = "";
        public void A_NewGame()
        {
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
    }
}

