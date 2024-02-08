using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace TUFF
{
    public class ChangeAudioSourceEvent : EventCommand
    {
        [Tooltip("Reference to the Audio Source.")]
        public AudioSource audioSource;

        [Header("Clip")]
        [Tooltip("If true, the Audio Source's clip will stay the same.")]
        public bool keepClip = false;
        [Tooltip("Clip to change to.")]
        public AudioClip clip = null;

        [Header("Volume")]
        [Tooltip("If true, the Audio Source's volume will stay the same.")]
        public bool keepVolume = false;
        [Tooltip("Audio Source's target volume.")]
        public float volume = 0f;
        [Tooltip("Fades the Audio Source's volume in seconds. Set to 0 or lower to change instantly.")]
        public float volumeFadeDuration = 0f;

        private Tween volumeFadeTween;

        public override void Invoke()
        {
            if (audioSource != null)
            {
                KillTweens();
                if (!keepClip) audioSource.clip = clip;
                if (!keepVolume) volumeFadeTween = audioSource.DOFade(volume, volumeFadeDuration);
            }
            isFinished = true;
        }
        public override void OnInstantiate()
        {
            eventName = "Change Audio Source";
            eventColor = new Color(0.5f, 0.85f, 1f, 1f);
        }
        private void KillTweens()
        {
            volumeFadeTween?.Kill();
            volumeFadeTween = null;
        }
        public override EventAction Port()
        {
            var clone = base.Port() as ChangeAudioSourceAction;
            clone.audioSource = audioSource;
            clone.keepClip = keepClip;
            clone.clip = clip;
            clone.keepVolume = keepVolume;
            clone.volume = volume;
            clone.volumeFadeDuration = volume;
            return clone;
        }
    }
}

