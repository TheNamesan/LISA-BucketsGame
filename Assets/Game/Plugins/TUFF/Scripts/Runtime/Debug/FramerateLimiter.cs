using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class FramerateLimiter : MonoBehaviour
    {
        public int framerate = 144;
        public float timeScale = 1;
        public bool enableFramerateControl = true;
        public bool enableTimeScaleControl = true;

        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
        }

        void Update()
        {
            if(enableFramerateControl)
            {
                if (Application.targetFrameRate != framerate)
                {
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = framerate;
                }
            }
        
            if(enableTimeScaleControl)
            {
                if (Time.timeScale != timeScale)
                {
                    Time.timeScale = timeScale;
                }
            }
        }
    }
}
