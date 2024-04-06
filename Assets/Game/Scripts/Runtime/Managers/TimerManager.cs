using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUFF;

namespace BucketsGame
{
    public class TimerManager : MonoBehaviour
    {
        public static TimerManager instance { get => BucketsGameManager.instance.timerManager; }
        private System.Diagnostics.Stopwatch stopwatch = new();
        private bool m_initialized = false;
        public System.TimeSpan timeSpanElapsed { get => stopwatch.Elapsed; }
        private double m_fastClearMiliseconds = 165000; // 2:45
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
        public void Begin()
        {
            Initialize();
            stopwatch?.Start();
        }
        public void Initialize()
        {
            Stop();
            m_initialized = true;
            stopwatch = new System.Diagnostics.Stopwatch();
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
            else if (!BucketsGameManager.instance.paused) Pause();
        }
        public bool FastTime()
        {
            double milliseconds = timeSpanElapsed.TotalMilliseconds;
            Debug.Log("TIME: " + milliseconds);
            return milliseconds <= m_fastClearMiliseconds;
        }
    }

}
