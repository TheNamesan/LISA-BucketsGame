using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TUFF;

namespace BucketsGame
{
    public class LevelSelectMenuManager : MonoBehaviour
    {
        public void A_LoadLevel(string name)
        {
            StartCoroutine(LoadLevel(name));
        }
        protected IEnumerator LoadLevel(string name)
        {
            UIController.instance.SetMenu(null);
            UIController.instance.fadeScreen.TriggerFadeOut(1f);
            AudioManager.instance.FadeOutVolume(1f);
            yield return new WaitForSeconds(2f);
            SceneLoaderManager.instance.LoadSceneWithFadeIn(name, 0.5f, TUFFSettings.startingScenePosition, TUFFSettings.startingSceneFacing, true, true);
        }
    }
}
