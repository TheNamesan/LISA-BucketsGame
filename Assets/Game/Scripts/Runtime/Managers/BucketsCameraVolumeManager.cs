using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BucketsGame
{
    public class BucketsCameraVolumeManager : MonoBehaviour
    {
        public CameraManager camManager;
        public Volume volume;
        public static VolumeProfile runtimeGlobalProfile;
        public static Vignette runtimeVignette;
        [Header("Vignette Properties")]
        public float maxIntensity = 0.666f;
        public float intensityTimerCap = 0.3f;
        public float shakeScale = 0.002f;
        private float m_intensityTime = 0f;
        public float NormalizedIntensityTime { get => m_intensityTime / intensityTimerCap; }
        private Tween m_shakeTween;
        private Vector3 m_shakeVector;
        
        private void Awake()
        {
            if (volume == null) TryGetComponent(out volume);
            SetGlobalVolume();
        }
        private void OnEnable()
        {
            GameUtility.KillTween(ref m_shakeTween);
            m_shakeVector = Vector3.zero;
            m_shakeTween = DOTween.Shake(()=>m_shakeVector, x => m_shakeVector = x, 1f, vibrato: 20, fadeOut: false).SetUpdate(true).SetLoops(-1);
        }
        private void SetGlobalVolume()
        {
            if (volume == null) return;
            if (runtimeGlobalProfile == null)
            {
                var copy = Instantiate(volume.profile);
                copy.name = "Runtime " + volume.profile.name;
                runtimeGlobalProfile = copy;
                if (runtimeGlobalProfile.TryGet(out Vignette vig))
                    runtimeVignette = vig;
            }
            volume.profile = runtimeGlobalProfile;
        }
        private void Update()
        {
            IntensityTimer();
            VolumeUpdate();
        }
        private void VolumeUpdate()
        {
            if (runtimeGlobalProfile == null) SetGlobalVolume();
            if (runtimeGlobalProfile == null) return;
            if (!runtimeVignette) return;
            bool slowmo = BucketsGameManager.instance.focusMode;
            float intensity = maxIntensity * NormalizedIntensityTime;
            runtimeVignette.intensity.value = intensity;
            Vector2 position = new Vector2(0.5f, 0.5f);
            //if (camManager && camManager.cam && camManager.virtualCam && camManager.virtualCam.Follow)
            //{
            //    Transform target = camManager.virtualCam.Follow;
            //    Vector2 targetScreenPos = camManager.cam.WorldToScreenPoint(target.position);
            //    Vector2 normalizedPos = new Vector2(targetScreenPos.x / Screen.width, targetScreenPos.y / Screen.height);
            //    position = normalizedPos;
            //}
            if (camManager && PlayerInputHandler.instance)
            {
                //Transform target = PlayerInputHandler.instance.gameInput;
                Vector2 targetScreenPos = PlayerInputHandler.instance.gameInput.mousePoint;
                Vector2 normalizedPos = new Vector2(targetScreenPos.x / Screen.width, targetScreenPos.y / Screen.height);
                position = normalizedPos;
            }
            position += (Vector2)m_shakeVector * shakeScale;
            runtimeVignette.center.value = position;
        }

        private void IntensityTimer()
        {
            bool slowmo = BucketsGameManager.instance.focusMode;
            m_intensityTime += Time.unscaledDeltaTime * (slowmo ? 1f : -1f);
            m_intensityTime = Mathf.Clamp(m_intensityTime, 0f, intensityTimerCap);
        }
        private void OnDisable()
        {
            GameUtility.KillTween(ref m_shakeTween);
        }
        private void OnDestroy()
        {
            GameUtility.KillTween(ref m_shakeTween);
        }
    }
}
