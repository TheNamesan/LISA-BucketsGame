using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "EVTWaitSeconds", menuName = "TUFF/Events/Wait Seconds Event")]
    public class WaitSecondsEvent : EventCommand
    {
        [Tooltip("Amount of seconds to wait. Negative values will count as 0.")]
        public float seconds;
        public override void Invoke()
        {
            GameManager.instance.StartCoroutine(WaitSeconds());
        }
        public override void OnInstantiate()
        {
            eventName = "Wait Seconds";
            eventColor = new Color(0.85f, 1f, 0.9f, 1f);
        }

        private IEnumerator WaitSeconds()
        {
            float waitTime = (seconds < 0 ? 0 : seconds);
            yield return new WaitForSeconds(waitTime);
            isFinished = true;
        }
    }
}
