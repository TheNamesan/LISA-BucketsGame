using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class FramerateLimiter : MonoBehaviour
    {
        public bool limitFramerate = false;
        public int framerate = 30;
        public bool limitTimeScale = false;
        public float timeScale = 1f;

        private void Update()
        {
            if (limitTimeScale) Time.timeScale = timeScale;
            if (limitFramerate)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = framerate;
            }
        }
    }
}