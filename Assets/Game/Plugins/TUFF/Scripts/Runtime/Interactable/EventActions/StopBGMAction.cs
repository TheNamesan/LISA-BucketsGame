using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class StopBGMAction : EventAction
    {
        [Tooltip("Optional. Adds a fade out in seconds when stopping the BGM. Set to 0 or lower to stop normally.")]
        public float fadeOutDuration = 0f;
        public StopBGMAction()
        {
            eventName = "Stop BGM";
            eventColor = new Color(0.5f, 0.85f, 1f, 1f);
        }
        public override void Invoke()
        {
            AudioManager.instance.StopMusic(fadeOutDuration);
            isFinished = true;
        }
    }
}

