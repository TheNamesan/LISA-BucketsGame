using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class TitleScreenManager : MonoBehaviour
    {
        public string continueSceneName = "TilesTest"; //tmp
        private void Start()
        {
            GameManager.gameOver = false;
        }

        public void NewGameButton()
        {
            StartCoroutine(NewGame());
        }

        public void ContinueButton()
        {
            StartCoroutine(Continue());
        }

        protected IEnumerator NewGame()
        {
            UIController.instance.SetMenu(null);
            UIController.instance.fadeScreen.TriggerFadeOut(1f);
            AudioManager.instance.FadeOutVolume(1f);
            GameManager.instance.StartNewGame();
            yield return new WaitForSeconds(2f);
            SceneLoaderManager.instance.LoadSceneWithFadeIn(TUFFSettings.startingSceneName, 0.5f, TUFFSettings.startingScenePosition, TUFFSettings.startingSceneFacing, true, true);
        }
        protected IEnumerator Continue()
        {
            UIController.instance.SetMenu(null);
            UIController.instance.fadeScreen.TriggerFadeOut(1f);
            AudioManager.instance.FadeOutVolume(1f);
            yield return new WaitForSeconds(2f);
            SceneLoaderManager.instance.LoadSceneWithFadeIn(continueSceneName, 0.5f, TUFFSettings.startingScenePosition, TUFFSettings.startingSceneFacing, true, true); //Replace with save data scene name and start position
        }
    }
}
