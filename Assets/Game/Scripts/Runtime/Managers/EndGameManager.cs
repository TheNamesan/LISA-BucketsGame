using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUFF;

namespace BucketsGame
{
    // 1 is Game Complete Pop Up
    // 2 is Pain Mode Complete Pop Up
    // 3 is Fast Clear Pop Up
    public class EndGameManager : MonoBehaviour
    {
        public InteractableObject interactable;
        public void A_CheckGameCompletion()
        {
            if (BucketsGameManager.instance.newGame)
            {
                if (!GameManager.instance.configData.bucketsComplete)
                {
                    GameManager.instance.configData.bucketsComplete = true;
                    GameManager.instance.configData.SaveData();
                    interactable.currentSwitch = 1;
                    return;
                }
                if (GameManager.instance.configData.bucketsPainMode)
                {
                    if (!GameManager.instance.configData.bucketsPainModeComplete)
                    {
                        GameManager.instance.configData.bucketsPainModeComplete = true;
                        GameManager.instance.configData.SaveData();
                        interactable.currentSwitch = 2;
                        return;
                    }
                }
                if (GameManager.instance.configData.bucketsPainMode)
                {
                    if (!GameManager.instance.configData.bucketsPainModeComplete)
                    {
                        GameManager.instance.configData.bucketsPainModeComplete = true;
                        GameManager.instance.configData.SaveData();
                        interactable.currentSwitch = 2;
                        return;
                    }
                }
                if (TimerManager.instance.FastTime() && !GameManager.instance.configData.bucketsRandoLevelsUnlocked)
                {
                    GameManager.instance.configData.bucketsRandoLevelsUnlocked = true;
                    GameManager.instance.configData.SaveData();
                    interactable.currentSwitch = 3;
                    return;
                }
            }
            interactable.currentSwitch = 99;
        }
    }
}
