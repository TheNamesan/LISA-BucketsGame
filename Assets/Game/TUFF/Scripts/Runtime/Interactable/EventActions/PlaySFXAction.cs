using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{ 
    [System.Serializable]
    public class PlaySFXAction : EventAction
    {
        public List<SFX> sfxs = new List<SFX>();
        public PlaySFXAction()
        {
            eventName = "Play SFX";
            eventColor = new Color(0.5f, 0.85f, 1f, 1f);
        }
        public override void Invoke()
        {
            foreach(SFX sfx in sfxs)
                AudioManager.instance.PlaySFX(sfx);
            isFinished = true;
        }
    }
}

