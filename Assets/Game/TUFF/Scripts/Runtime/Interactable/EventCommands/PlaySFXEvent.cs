using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "EVTPlaySFX", menuName = "TUFF/Events/Play SFX Event")]
    public class PlaySFXEvent : EventCommand
    {
        //[Tooltip("Clip to play")]
        //public AudioClip clip;
        //[Tooltip("Clip's volume")]
        //public float volume = 1f;
        //[Tooltip("Clip's pitch")]
        //public float pitch = 1f;
        public SFX sfx = new SFX();
        public override void Invoke()
        {
            AudioManager.instance.PlaySFX(sfx);
            isFinished = true;
        }
        public override void OnInstantiate()
        {
            eventName = "Play SFX";
            eventColor = new Color(0.5f, 0.85f, 1f, 1f);
        }
    }
}
