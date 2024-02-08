using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class PlayBGMAction : EventAction
    {
        [Tooltip("BGM to play.")]
        public BGMPlayData bgmPlayData;
        [Tooltip("Optional. Adds a fade in in seconds when playing the BGM. Set to 0 or lower to play normally.")]
        public float fadeInDuration = 0f;
        public PlayBGMAction()
        {
            eventName = "Play BGM";
            eventColor = new Color(0.5f, 0.85f, 1f, 1f);
        }
        public override void Invoke()
        {
            AudioManager.instance.PlayMusic(bgmPlayData, fadeInDuration);
            isFinished = true;
        }
        
    }
}

