using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "EVTPlayBGM", menuName = "TUFF/Events/Play BGM Event")]
    public class PlayBGMEvent : EventCommand
    {
        [Tooltip("BGM to play.")]
        public BGMPlayData bgmPlayData;
        [Tooltip("Optional. Adds a fade in in seconds when playing the BGM. Set to 0 or lower to play normally.")]
        public float fadeInDuration = 0f;
        public override void Invoke()
        {
            AudioManager.instance.PlayMusic(bgmPlayData, fadeInDuration);
            isFinished = true;
        }
        public override void OnInstantiate()
        {
            eventName = "Play BGM";
            eventColor = new Color(0.5f, 0.85f, 1f, 1f);
        }
    }
}
