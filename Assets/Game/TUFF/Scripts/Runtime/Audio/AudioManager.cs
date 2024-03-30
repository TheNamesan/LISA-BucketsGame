using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

namespace TUFF
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Global Settings")]
        [Range(0f, 1f)] public float globalMusicVolume = 1f;
        [Range(0f, 1f)] public float globalSFXVolume = 1f;
        [Range(0f, 1f)] public float globalAmbienceVolume = 1f;
        [Header("References")]
        public AudioMixerGroup musicMixerGroup;
        public AudioMixerGroup sfxMixerGroup;
        public AudioMixerGroup ambientMixerGroup;
        public SFXManager sfxManager;
        [SerializeField] AudioSource auIntro;
        [SerializeField] AudioSource auLoop;
        [SerializeField] AudioSource ambsSource;


        [Header("Music Manager")]
        [Tooltip("Currently playing BGM")]
        [SerializeField] BGM currentBGM;
        [Tooltip("Currently playing AMBS")]
        [SerializeField] AudioClip currentAMBS;
        [Header("BGM Data")]
        [SerializeField] float volume = 1f;
        [SerializeField] float pitch = 1f;
        [SerializeField] bool loop = true;
        public AudioClip introClip = null;
        public AudioClip loopClip = null;
        [SerializeField] private float targetVolume = 1;
        private bool queueLoopMusic = false;
        [Header("AMBS Data")]
        [SerializeField] float ambsVolume = 1f;
        [SerializeField] float ambsPitch = 1f;
        [SerializeField] bool ambsLoop = true;
        [SerializeField] private float ambsTargetVolume = 1f;

        Tween tweenIntro;
        Tween tweenLoop;

        #region Singleton
        public static AudioManager instance;
        private void Awake()
        {
            if (instance != null)
            {
                if (instance != this) Destroy(gameObject);
            }
            else
            {
                instance = this;

                auIntro.outputAudioMixerGroup = musicMixerGroup;
                auLoop.outputAudioMixerGroup = musicMixerGroup;
                ambsSource.outputAudioMixerGroup = ambientMixerGroup;
                sfxManager.InitializeSources();
            }
        }
        #endregion

        private void Start()
        {
            UpdateVolumeFromConfig();
        }
        public void UpdateVolumeFromConfig()
        {
            globalMusicVolume = GameManager.instance.configData.globalMusicVolume;
            globalSFXVolume = GameManager.instance.configData.globalSFXVolume;
            globalAmbienceVolume = GameManager.instance.configData.globalAmbienceVolume;
            SetGlobalVolume();
        }
        public void Update()
        {
            SetGlobalVolume();
        }
        public void LateUpdate()
        {
            CheckQueuedMusic();
        }
        private void CheckQueuedMusic()
        {
            if (!queueLoopMusic) return;
            double introStart = AudioSettings.dspTime;
            auIntro.PlayScheduled(introStart);
            double introLength = (double)introClip.samples / introClip.frequency;
            introLength /= (pitch <= 0 ? 0.01f : pitch);
            auIntro.SetScheduledEndTime(introStart + introLength);
            auLoop.PlayScheduled(introStart + introLength);
            
            queueLoopMusic = false;
        }
        private void SetVolume()
        {
            targetVolume = volume;
            SetGlobalVolume();
        }

        public void SetGlobalVolume()
        {
            musicMixerGroup.audioMixer.SetFloat("MusicVolume", LISAUtility.PercentTodB(globalMusicVolume));
            sfxMixerGroup.audioMixer.SetFloat("SFXVolume", LISAUtility.PercentTodB(globalSFXVolume));
            ambientMixerGroup.audioMixer.SetFloat("AmbienceVolume", LISAUtility.PercentTodB(globalAmbienceVolume));
        }

        void ApplyVolume()
        {
            auIntro.volume = targetVolume;
            auLoop.volume = targetVolume;
        }

        void SetAndApplyVolume()
        {
            KillTweens();
            SetVolume();
            ApplyVolume();
        }
        private void SetupMusicLoop(bool forceInstantPlay = false)
        {
            if (currentBGM.loopStart < 0 || currentBGM.loopEnd < 0)
            {
                Debug.LogWarning("Loop Start and Loop End must be 0 or higher.");
                return;
            }
            if (currentBGM.loopStart == currentBGM.loopEnd || !loop) // If BGM loops itself, or song doesn't loop, set the song to play from start to finish
            {
                introClip = null;
                loopClip = currentBGM.clip;
                auLoop.loop = loop;
                auLoop.clip = loopClip;
                auLoop.pitch = pitch;
                auLoop.Play();
            }
            else // If it loops dynamically
            {
                // Intro
                if (currentBGM.prebakedLoopIntro != null)
                {
                    introClip = currentBGM.prebakedLoopIntro;
                }
                else if (currentBGM.loopStart > 0)
                {
                    introClip = SetClipLoop(currentBGM.clip, 0, currentBGM.loopStart, "_introI");
                }
                if (introClip != null)
                {
                    auIntro.clip = introClip;
                    auIntro.loop = false;
                    auIntro.pitch = pitch;
                }
                
                // Loop
                if (currentBGM.prebakedLoopMain != null)
                {
                    loopClip = currentBGM.prebakedLoopMain;
                }
                else loopClip = SetClipLoop(currentBGM.clip, currentBGM.loopStart, currentBGM.loopEnd, "_loopI");
                if (loopClip != null)
                {
                    auLoop.clip = loopClip;
                    auLoop.loop = true;
                    auLoop.pitch = pitch;

                    //queueLoopMusic = true;
                    //if (forceInstantPlay) CheckQueuedMusic();

                    if (currentBGM.loopStart > 0 || currentBGM.prebakedLoopMain != null)
                    {
                        queueLoopMusic = true;
                        if (forceInstantPlay) CheckQueuedMusic();
                    }
                    else auLoop.Play();
                }
            }
        }
        private void SetMusic(BGM bgm, bool forceInstantPlay = false)
        {
            currentBGM = bgm;
            StopClips();
            SetupMusicLoop(forceInstantPlay);
        }
        private void SetAmbience(AudioClip clip)
        {
            currentAMBS = clip;
            ambsSource.loop = ambsLoop;
            ambsSource.clip = clip;
            ambsSource.pitch = ambsPitch;
            ambsSource.Play();
        }
        public void PlayMusic(BGM bgm, float fadeInDuration = 0)
        {
            if (bgm == currentBGM) return;
            if (bgm == null) { StopMusic(); return; }
            volume = 1f;
            pitch = 1f;
            loop = true;
            if (fadeInDuration <= 0) SetAndApplyVolume();
            else FadeInVolume(fadeInDuration);
            SetMusic(bgm);
        }
        public void PlayMusic(BGMPlayData bgmPlayData, float fadeInDuration = 0)
        {
            Debug.Log("Play");
            if (bgmPlayData == null) { StopMusic(); return; }
            if (bgmPlayData.bgm == null) { StopMusic(); return; }
            if (bgmPlayData.bgm == currentBGM && bgmPlayData.pitch == pitch && bgmPlayData.loop == loop) return;
            volume = bgmPlayData.volume;
            pitch = bgmPlayData.pitch;
            loop = bgmPlayData.loop;
            UpdateVolumeFromConfig();
            if (fadeInDuration <= 0) SetAndApplyVolume();
            else FadeInVolume(fadeInDuration);
            SetMusic(bgmPlayData.bgm);
        }
        public void PlayBGMAsPreview(BGMPlayData bgmPlayData)
        {
            if (bgmPlayData == null) { return; }
            if (bgmPlayData.bgm == null) { return; }
            volume = bgmPlayData.volume;
            pitch = bgmPlayData.pitch;
            loop = bgmPlayData.loop;
            SetAndApplyVolume();
            SetMusic(bgmPlayData.bgm, true);
        }
        public void PlaySFX(AudioClip clip, float volume, float pitch)
        {
            sfxManager.PlaySFX(clip, volume, pitch);
        }
        public void PlaySFX(SFX sfx)
        {
            if (sfx == null) return;
            sfxManager.PlaySFX(sfx.GetAudioClip(), sfx.GetVolume(), sfx.GetPitch());
        }
        public void PlayAMBS(AMBSPlayData ambsPlayData, float fadeInDuration = 0)
        {
            if (ambsPlayData == null) { StopAmbience(); return; }
            if (ambsPlayData.clip == null) { StopAmbience(); return; }
            if (ambsPlayData.clip == currentAMBS && ambsPlayData.pitch == pitch && ambsPlayData.loop == loop) return;
            
            ambsVolume = ambsPlayData.volume;
            ambsPitch = ambsPlayData.pitch;
            ambsLoop = ambsPlayData.loop;
            UpdateVolumeFromConfig();

            // Add fade stuff here
            ambsTargetVolume = ambsVolume; //Set Volume
            ambsSource.volume = ambsTargetVolume; // Apply volume
            SetGlobalVolume();
            SetAmbience(ambsPlayData.clip);
        }
        public void PlayGameOverMusic()
        {
            PlayMusic(TUFFSettings.gameOverBGM);
        }
        public void StopMusic()
        {
            Debug.Log("Stop");
            StopClips();
            currentBGM = null;
        }
        public void StopAmbience()
        {
            StopAmbienceClips();
            currentAMBS = null;
        }
        private void StopClips()
        {
            queueLoopMusic = false;
            auIntro.Stop();
            auLoop.Stop();
        }
        private void StopAmbienceClips()
        {
            ambsSource.Stop();
        }
        public void StopMusic(float fadeOutDuration)
        {
            if(fadeOutDuration <= 0)
            {
                StopMusic();
                return;
            }
            FadeOutVolume(fadeOutDuration, true);
        }
        public void ChangeAmbienceVolume(float volume, float fadeDuration = 0)
        {
            ambsSource.volume = volume;
        }
        public void RestoreAmbienceVolume()
        {
            ambsSource.volume = ambsTargetVolume;
        }
        public void FadeOutVolume(float timeDuration)
        {
            KillTweens();
            SetVolume();
            tweenIntro = auIntro.DOFade(0f, timeDuration).From(targetVolume).SetAutoKill();
            tweenLoop = auLoop.DOFade(0f, timeDuration).From(targetVolume).SetAutoKill();
        }
        public void FadeOutVolume(float timeDuration, bool stopOnComplete)
        {
            KillTweens();
            SetVolume();
            tweenIntro = auIntro.DOFade(0f, timeDuration).From(targetVolume).SetAutoKill();
            tweenLoop = auLoop.DOFade(0f, timeDuration).From(targetVolume).SetAutoKill().OnComplete(() => { if(stopOnComplete) StopMusic(); });
        }
        public void FadeInVolume(float timeDuration)
        {
            KillTweens();
            SetVolume();
            tweenIntro = auIntro.DOFade(targetVolume, timeDuration).From(0f).SetAutoKill();
            tweenLoop = auLoop.DOFade(targetVolume, timeDuration).From(0f).SetAutoKill();
        }

        AudioClip SetClipLoop(AudioClip inputAudio, int startSample, int endSample, string nameSuffix)
        {
            return LISAUtility.CutAudioClip(inputAudio, startSample, endSample, nameSuffix);
        }

        void KillTweens()
        {
            tweenIntro.Kill();
            tweenIntro = null;
            tweenLoop.Kill();
            tweenLoop = null;
        }

        void OnAudioRead(float[] data) {}
        void OnAudioSetPosition(int newPosition) {}
    }
}