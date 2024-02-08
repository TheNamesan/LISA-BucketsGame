using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace TUFF
{
    public class SFXManager : MonoBehaviour
    {
        public AudioMixerGroup sfxMixerGroup;
        public int initialSources = 1;
        [SerializeField] private List<AudioSource> sources = new List<AudioSource>();
        public void InitializeSources()
        {
            for(int i = 0; i < initialSources; i++)
            {
                AddSource();
            }
        }
        public void PlaySFX(AudioClip clip, float volume, float pitch)
        {
            if (clip == null) return;
            for(int i = 0; i < sources.Count; i++)
            {
                if(sources[i].clip == null)
                {
                    SetClipToSource(i, clip, volume, pitch);
                    return;
                }
            }
            AddSource(); //Couldn't find an empty source, add one where it can play the clip
            SetClipToSource(sources.Count - 1, clip, volume, pitch);
        }

        private void AddSource()
        {
            sources.Add(gameObject.AddComponent<AudioSource>());
            sources[sources.Count - 1].outputAudioMixerGroup = sfxMixerGroup;
        }

        private void SetClipToSource(int idx, AudioClip clip, float volume, float pitch)
        {
            sources[idx].pitch = pitch;
            sources[idx].volume = volume;
            sources[idx].clip = clip;
            sources[idx].Play();
            StartCoroutine(QueueClipRemoval(idx));
        }

        private IEnumerator QueueClipRemoval(int idx)
        {
            yield return new WaitUntil(() => !sources[idx].isPlaying);
            sources[idx].clip = null;
        }
    }
}
