using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class SingletonCaller : MonoBehaviour
    {
        public void UICTriggerFadeOut(float duration)
        {
            UIController.instance.fadeScreen.TriggerFadeOut(duration);
        }
        public void UICTriggerFadeIn(float duration)
        {
            UIController.instance.fadeScreen.TriggerFadeIn(duration);
        }
        public void UICSetMenuNull()
        {
            UIController.instance.SetMenu(null);
        }
        public void PRemovePlayer()
        {
            if (FollowerInstance.player.controller != null)
            {
                Destroy(FollowerInstance.player.controller.gameObject);
            }
        }
        public void GMTriggerGameOver()
        {
            GameManager.instance.GameOver();
        }
        public void GMSetGameOverVariable(bool input)
        {
            GameManager.gameOver = input;
        }
    }
}
