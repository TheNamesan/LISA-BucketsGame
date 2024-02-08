using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class SFX
    {
        public int audioClipMode = 0;
        public AudioClip audioClip = null;
        public AudioClipElement[] randomClips = new AudioClipElement[0];

        public int volumeMode = 0;
        public float volume = 1;
        public float minVolume = 1;

        public int pitchMode = 0;
        public float pitch = 1;
        public float minPitch = 1;

        public SFX() {}
        public SFX(AudioClip audioClip, float volume, float pitch)
        {
            this.audioClip = audioClip;
            this.volume = volume;
            this.pitch = pitch;
        }
        public AudioClip GetAudioClip()
        {
            if (audioClipMode == 0) return audioClip;
            return RollForClip();
        }
        public float GetVolume()
        {
            if (volumeMode == 0) return volume;
            return Random.Range(minVolume, volume);
        }
        public float GetPitch()
        {
            if (pitchMode == 0) return pitch;
            return Random.Range(minPitch, pitch);
        }
        private AudioClip RollForClip()
        {
            int totalWeight = System.Linq.Enumerable.Sum(randomClips, e => e.weight);
            if (totalWeight <= 0) return null;
            int target = Random.Range(1, totalWeight + 1);
            int curWeight = 0;
            for (int i = 0; i < randomClips.Length; i++)
            {
                if (randomClips[i].weight <= 0) continue;
                int maxWeight = curWeight + randomClips[i].weight;
                if (target <= maxWeight)
                {
                    return randomClips[i].clip;
                }
                curWeight += randomClips[i].weight;
            }
            return audioClip;
        }
    }
    [System.Serializable]
    public class AudioClipElement
    {
        public AudioClip clip;
        [Range(0, 1000)] public int weight = 1;
    }
}

