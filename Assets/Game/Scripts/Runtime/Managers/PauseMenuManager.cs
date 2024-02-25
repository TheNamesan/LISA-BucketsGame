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

        }
        public void A_Exit()
        {
            uiMenu.CloseMenu();
            SceneManager.LoadScene(mainMenuScene);
        }
        public void OnClose()
        {
            BucketsGameManager.instance.Pause(false);
            
        }
    }
}
