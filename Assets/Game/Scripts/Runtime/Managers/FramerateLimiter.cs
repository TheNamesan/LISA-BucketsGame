using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class FramerateLimiter : MonoBehaviour
    {
        public bool limitTimeScale = false;
        public float timeScale = 1f;

        private void Update()
        {
            if (limitTimeScale) Time.timeScale = timeScale;
        }
    }
}