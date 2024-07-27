using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class SFXGroup
    {
        public AudioSource audioSource;
        public List<SFX> sfxs = new List<SFX>();
    }
    public class RandomSFXPlayer : MonoBehaviour
    {
        public bool muteIfPlayerInputDisabled = false;
        public List<SFXGroup> sfxGroups = new List<SFXGroup>();

        public void PlayRandomSFX(int groupIndex)
        {
            if (muteIfPlayerInputDisabled && GameManager.disablePlayerInput) return;
            if (sfxGroups.Count <= 0) return;
            PlaySFX(sfxGroups[groupIndex].audioSource, sfxGroups[groupIndex].sfxs);
        }

        private static void PlaySFX(AudioSource audioSource, List<SFX> sfxs)
        {
            if (sfxs.Count <= 0) return;
            int index = Random.Range(0, sfxs.Count);
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

