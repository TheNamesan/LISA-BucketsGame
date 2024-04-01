using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUFF;
using System.Diagnostics;

namespace BucketsGame
{
    public class TimerManager : MonoBehaviour
    {
        public static TimerManager instance { get => BucketsGameManager.instance.timerManager; }
        private Stopwatch stopwatch = new();
        private bool m_initialized = false;
        public double secondsElapsed = 0;
        public System.TimeSpan timeSpanElapsed { get => stopwatch.Elapsed; }
        private void OnEnable()
        {
            if (Application.isPlaying)
                GameManager.instance.onPlayerInputToggle.AddListener(ToggleTimer);
        }
        private void OnDisable()
        {
            if (Application.isPlaying)
                GameManager.instance.onPlayerInputToggle.RemoveListener(ToggleTimer);
        }
        private void OnDestroy()
        {
            if (Application.isPlaying)
                GameManager.instance.onPlayerInputToggle.RemoveListener(ToggleTimer);
        }
        private void Start()
        {
        
        }
        public void Begin()
        {
            Initialize();
            stopwatch?.Start();
        }
        public void Initialize()
        {
            Stop();
            m_initialized = true;
            stopwatch = new Stopwatch();
        }

        public void Stop()
        {
            stopwatch?.Stop();
            m_initialized = false;
        }
        public void Pause()
        {
            stopwatch?.Stop();
        }
        public void Resume()
        {
            if (m_initialized) stopwatch?.Start();
        }
        private void ToggleTimer(bool input)
        {
            if (input) Resume();
            else Pause();
        }
    }

}
