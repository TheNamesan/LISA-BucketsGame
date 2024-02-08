using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace TUFF
{
    public class PixelPerfectCameraResolution : MonoBehaviour
    {
        public const int baseHeight = 540;
        public float targetSize = 6f;
        public PixelPerfectCamera pixelPerfectCamera;

        private int height = 0;
        private void Awake()
        {
            if (!pixelPerfectCamera) pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
            AdjustToResolution();
        }
        void Update()
        {
            AdjustToResolution();
        }

        private void AdjustToResolution()
        {
            if (!pixelPerfectCamera) return;
            if (!pixelPerfectCamera.enabled) return;
            height = Screen.height;
            pixelPerfectCamera.refResolutionY = height;
            pixelPerfectCamera.refResolutionX = Mathf.RoundToInt(height / 9 * 16f);

            pixelPerfectCamera.assetsPPU = LISAUtility.Truncate((pixelPerfectCamera.refResolutionY * 0.5f) / targetSize);
        }
    }
}

