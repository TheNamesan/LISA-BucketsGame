using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class GameOverScreenManager : MonoBehaviour
    {
        public string titleScreenSceneName = "TitleScreen";
        private void Start()
        {
            GameManager.gameOver = false;
        }

        public void ContinueButton()
        {
            StartCoroutine(Continue());
        }

        IEnumerator Continue()
        {
            UIController.instance.SetMenu(null);
            UIController.instance.UIFadeScreen.FadeOut(0.125f);
            AudioManager.instance.FadeOutVolume(1f);
            bool loaded = GameManager.instance.LoadSaveData(GameManager.instance.lastLoadedFile); // Load Last Loaded Save
            GameManager.instance.stopPlaytime = true;
            if (!loaded) // No save file detected
            {
                GameManager.instance.StartNewGame(); //Start a new game
            }
            yield return new WaitForSeconds(0.125f);
            GameManager.instance.stopPlaytime = false;
            PlayerData.instance.GetSceneData(out string sceneName, out Vector3 playerPosition, out FaceDirections playerFacing);
            SceneLoaderManager.instance.LoadSceneWithFadeIn(sceneName, 0.25f, playerPosition, playerFacing, true, true);
        }

        public void ToTitleButton()
        {
            StartCoroutine(ToTitle());
        }   
    
        IEnumerator ToTitle()
        {
            UIController.instance.SetMenu(null);
            UIController.instance.UIFadeScreen.FadeOut(1f);
            AudioManager.instance.FadeOutVolume(1f);
            yield return new WaitForSeconds(1.5f);
            SceneLoaderManager.instance.LoadSceneWithFadeIn(titleScreenSceneName, 1f);
        }
    }
}
