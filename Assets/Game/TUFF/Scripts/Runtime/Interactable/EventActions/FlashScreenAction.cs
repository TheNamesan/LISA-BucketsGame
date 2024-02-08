using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class FlashScreenAction : EventAction
    {
        public FlashData flashData;
        public bool waitForCompletion = false;
        public FlashScreenAction()
        {
            eventName = "Flash Screen";
            eventColor = EventGUIColors.screenEffects;
        }
        public override void Invoke()
        {
            UIController.instance.FlashScreen(flashData.flashColor, flashData.flashDuration);
            if (waitForCompletion) GameManager.instance.StartCoroutine(WaitForCompletion(flashData.flashDuration));
            else isFinished = true;
        }
        private IEnumerator WaitForCompletion(float duration)
        {
            yield return new WaitForSeconds(duration);
            isFinished = true;
        }
    }
}

