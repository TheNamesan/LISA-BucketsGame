using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class TintScreenAction : EventAction
    {
        [Tooltip("Color to change the screen to.")]
        public Color tint = Color.white;
        [Tooltip("Fade's duration in seconds.")]
        public float duration = 0f;
        [Tooltip("If true, action list will stop until the tint change is finished.")]
        public bool waitForCompletion = false;
        public TintScreenAction()
        {
            eventName = "Tint Screen";
            eventColor = EventGUIColors.screenEffects;
        }
        public override void Invoke()
        {
            UIController.instance.TintScreen(tint, duration);
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
