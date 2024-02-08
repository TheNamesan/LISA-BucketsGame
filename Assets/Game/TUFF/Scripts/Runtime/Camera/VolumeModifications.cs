using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

namespace TUFF
{
    public class VolumeModifications
    {
        public virtual void ApplyChanges(VolumeProfile profileOverride) { }
        public virtual void Instantiate() { }
    }
    [System.Serializable]
    public class ColorAdjustmentsMods : VolumeModifications
    {
        public float postExposureFadeDuration = 0f;
        public float contrastFadeDuration = 0f;
        public float colorFilterFadeDuration = 0f;
        public float hueShiftFadeDuration = 0f;
        public float saturationFadeDuration = 0f;

        //Runtime Profile References
        ColorAdjustments colorAdjustmentsRuntime = null;

        //Target Values
        static float postExposureValue = 0;
        static bool postExposureOvrState;
        static float contrastValue;
        static bool contrastOvrState;
        static Color colorFilterValue;
        static bool colorFilterOvrState;
        static float hueShiftValue;
        static bool hueShiftOvrState;
        static float saturationValue;
        static bool saturationOvrState;

        static Tween postExposureFade;
        static Tween contrastFade;
        static Tween colorFilterFade;
        static Tween hueShiftFade;
        static Tween saturationFade;

        public override void ApplyChanges(VolumeProfile profileOverride)
        {
            if (CameraVolumeManager.runtimeGlobalProfile == null || profileOverride == null) return;
            if (!CameraVolumeManager.runtimeGlobalProfile.TryGet(out colorAdjustmentsRuntime))
            {
                colorAdjustmentsRuntime = CameraVolumeManager.runtimeGlobalProfile.Add(typeof(ColorAdjustments)) as ColorAdjustments;
                Debug.Log("ADDED COLOR ADJUSTMENTS");
            }

            ColorAdjustments colorAdjustmentsOverride = null;
            profileOverride.TryGet(out colorAdjustmentsOverride);

            if (colorAdjustmentsRuntime == null) return;
            if (colorAdjustmentsOverride == null) return;

            KillTweens();
            AssignTweens(colorAdjustmentsOverride);
        }

        private void AssignTweens(ColorAdjustments colorAdjustmentsOverride)
        {
            //var idx = CameraVolumeManager.runtimeGlobalProfile.components.IndexOf(colorAdjustmentsRuntime);
            //CameraVolumeManager.runtimeGlobalProfile.components[idx] = colorAdjustmentsOverride;

            //postExposureValue = colorAdjustmentsOverride.postExposure.value;
            //postExposureOvrState = colorAdjustmentsOverride.postExposure.overrideState;
            //colorAdjustmentsRuntime.postExposure.overrideState = postExposureOvrState;
            //floatTarget = colorAdjustmentsRuntime.postExposure;
            //postExposureFade = DOTween.To(val => UpdateFloat(val), colorAdjustmentsRuntime.postExposure.value, postExposureValue, postExposureFadeDuration).SetEase(Ease.Linear);

            

            SetChange(colorAdjustmentsRuntime.postExposure, colorAdjustmentsOverride.postExposure, ref postExposureValue, ref postExposureOvrState);
            postExposureFade = DOTween.To(() => colorAdjustmentsRuntime.postExposure.value, val => colorAdjustmentsRuntime.postExposure.value = val, postExposureValue, postExposureFadeDuration);
            //postExposureFade = DOTween.To(val => colorAdjustmentsRuntime.postExposure.value = val, colorAdjustmentsRuntime.postExposure.value, postExposureValue, postExposureFadeDuration)
                //.SetEase(Ease.Linear);

            SetChange(colorAdjustmentsRuntime.contrast, colorAdjustmentsOverride.contrast, ref contrastValue, ref contrastOvrState);
            contrastFade = DOTween.To(val => colorAdjustmentsRuntime.contrast.value = val, colorAdjustmentsRuntime.contrast.value, contrastValue, contrastFadeDuration)
                .SetEase(Ease.Linear);

            SetChange(colorAdjustmentsRuntime.colorFilter, colorAdjustmentsOverride.colorFilter, ref colorFilterValue, ref colorFilterOvrState);
            colorFilterFade = DOTween.To(() => colorAdjustmentsRuntime.colorFilter.value, val => colorAdjustmentsRuntime.colorFilter.value = val, colorFilterValue, colorFilterFadeDuration)
                .SetEase(Ease.Linear);

            SetChange(colorAdjustmentsRuntime.hueShift, colorAdjustmentsOverride.hueShift, ref hueShiftValue, ref hueShiftOvrState);
            hueShiftFade = DOTween.To(val => colorAdjustmentsRuntime.hueShift.value = val, colorAdjustmentsRuntime.hueShift.value, hueShiftValue, hueShiftFadeDuration)
                .SetEase(Ease.Linear);

            SetChange(colorAdjustmentsRuntime.saturation, colorAdjustmentsOverride.saturation, ref saturationValue, ref saturationOvrState);
            saturationFade = DOTween.To(val => colorAdjustmentsRuntime.saturation.value = val, colorAdjustmentsRuntime.saturation.value, saturationValue, saturationFadeDuration)
                .SetEase(Ease.Linear);

            //contrastValue = colorAdjustmentsOverride.contrast.value;
            //contrastOvrState = colorAdjustmentsOverride.contrast.overrideState;
            //colorAdjustmentsRuntime.contrast.overrideState = contrastOvrState;
            //floatTarget = colorAdjustmentsRuntime.contrast;
            //contrastFade = DOTween.To(val => UpdateFloat(val), colorAdjustmentsRuntime.contrast.value, contrastValue, contrastFadeDuration).SetEase(Ease.Linear);

            //colorFilterValue = colorAdjustmentsRuntime.colorFilter.value;
            //colorAdjustmentsRuntime.colorFilter.overrideState = colorAdjustmentsOverride.colorFilter.overrideState;
            //colorFilterFade = DOTween
            //    .To(() => colorFilterValue, val => UpdateColorFilter(val), colorAdjustmentsOverride.colorFilter.value, colorFilterFadeDuration)
            //    .SetEase(Ease.Linear);
        }

        private void SetChange<T>(VolumeParameter runtimeParameter, VolumeParameter overrideParameter, ref T value, ref bool overrideState)
        {
            value = overrideParameter.GetValue<T>();
            overrideState = overrideParameter.overrideState;
            runtimeParameter.overrideState = overrideState;
//fadeTween = DOTween.To(val => UpdateFloat(val), colorAdjustmentsRuntime.postExposure.value, value, duration).SetEase(Ease.Linear);
        }
        private void UpdateColorFilter(Color color)
        {
            if (colorAdjustmentsRuntime == null) return;
            colorAdjustmentsRuntime.colorFilter.value = color;
        }
        private void KillTweens()
        {
            postExposureFade?.Complete();
            contrastFade?.Complete();
            colorFilterFade?.Complete();
            hueShiftFade?.Complete();
            saturationFade?.Complete();

            postExposureFade?.Kill();
            contrastFade?.Kill();
            colorFilterFade?.Kill();
            hueShiftFade?.Kill();
            saturationFade?.Kill();

            postExposureFade = null;
            contrastFade = null;
            colorFilterFade = null;
            hueShiftFade = null;
            saturationFade = null;
        }
    }
}

