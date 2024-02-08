using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class SFXPlayer : MonoBehaviour
    {
        [Tooltip("If null, will play globaly.")]
        public AudioSource audioSource;
        public List<SFX> sfxs = new List<SFX>();
        
        public void Play(int index)
        {
            if (index < 0 || index >= sfxs.Count) return;
            var sfx = sfxs[index];
            if (audioSource != null)
            {
                
                audioSource.clip = sfx.audioClip;
                audioSource.volume = sfx.volume;
                audioSource.pitch = sfx.pitch;
                audioSource.Play();
                return;
            }
            AudioManager.instance.PlaySFX(sfx);
        }
    }
}
