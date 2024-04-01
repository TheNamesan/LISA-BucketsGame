using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUFF;

namespace BucketsGame
{
    public class EndGameManager : MonoBehaviour
    {
        public InteractableObject interactable;
        public void A_CheckGameCompletion()
        {
            if (!BucketsGameManager.instance.newGame) return;
            if (!GameManager.instance.configData.bucketsComplete)
            {
                GameManager.instance.configData.bucketsComplete = true;
                GameManager.instance.configData.SaveData();
                interactable.currentSwitch = 1;
            }
            else
            {
                if (GameManager.instance.configData.bucketsPainMode)
                {
                    if (!GameManager.instance.configData.bucketsPainModeComplete)
                    {
                        GameManager.instance.configData.bucketsPainModeComplete = true;
                        GameManager.instance.configData.SaveData();
                        interactable.currentSwitch = 2;
                    }
                }
            }
        }
    }
}
