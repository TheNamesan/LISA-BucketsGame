using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class WaitSecondsAction : EventAction
    {
        [Tooltip("Amount of seconds to wait. Negative values will count as 0.")]
        public float seconds;
        
        public WaitSecondsAction()
        {
            eventName = "Wait Seconds";
            eventColor = new Color(0.85f, 1f, 0.9f, 1f);
        }
        public override void Invoke()
        {
            GameManager.instance.StartCoroutine(WaitSeconds());
        }
        private IEnumerator WaitSeconds()
        {
            float waitTime = (seconds < 0 ? 0 : seconds);
            yield return new WaitForSeconds(waitTime);
            isFinished = true;
        }
    }
}