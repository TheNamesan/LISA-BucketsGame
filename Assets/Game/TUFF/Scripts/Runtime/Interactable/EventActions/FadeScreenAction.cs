using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class FadeScreenAction : EventAction
    {
        [Tooltip("Fade's duration in seconds.")]
        public float duration = 0f;
        [Tooltip("Fade Out: Darkens the screen. Fade In: Brightens the screen.")]
        public FadeType fadeType = FadeType.FadeOut;
        [Tooltip("If true, action list will stop until the fade is finished.")]
        public bool waitForCompletion = false;
        public FadeScreenAction()
        {
            eventName = "Fade Screen";
            eventColor = EventGUIColors.screenEffects;
        }
        public override void Invoke()
        {
            if (fadeType == FadeType.FadeOut) UIController.instance.FadeOutScreen(duration);
            else UIController.instance.FadeInScreen(duration);
            if (waitForCompletion) GameManager.instance.StartCoroutine(WaitForCompletion(duration));
            else isFinished = true;
        }
        private IEnumerator WaitForCompletion(float duration)
        {
            yield return new WaitForSeconds(duration);
            isFinished = true;
        }
    }
}
