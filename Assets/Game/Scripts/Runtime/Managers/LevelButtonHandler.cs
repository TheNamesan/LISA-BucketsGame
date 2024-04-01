using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUFF;

namespace BucketsGame
{
    public class LevelButtonHandler : MonoBehaviour
    {
        public string levelName = "";

        public void A_LoadLevel()
        {
            StartCoroutine(LoadLevel(levelName));
        }
        protected static IEnumerator LoadLevel(string name)
        {
            UIController.instance.SetMenu(null);
            UIController.instance.fadeScreen.TriggerFadeOut(1f);
            AudioManager.instance.FadeOutVolume(1f);
            yield return new WaitForSeconds(2f);
            TimerManager.instance.Initialize();
            SceneLoaderManager.instance.LoadSceneWithFadeIn(name, 0.5f, TUFFSettings.startingScenePosition, TUFFSettings.startingSceneFacing, true, true);
        }
    }

}
