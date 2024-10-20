using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

namespace BucketsGame
{
    public class CameraManager : MonoBehaviour
    {
        public bool disableFollow = false;
        public Transform disableFollowTransform;
        public Camera cam;
        public CinemachineVirtualCamera virtualCam;
        public TUFF.CameraFollow followCam;
        public CinemachineCameraOffset camOffset;
        public CinemachineConfiner camConfiner;
        public float dashOffsetIntensity = 0.5f;
        [SerializeField] private Vector2 m_minOffset = Vector2.zero;
        [SerializeField] private Vector2 m_maxOffset = Vector2.one;
        [SerializeField] private Vector2 m_minDistance = Vector2.zero;
        [SerializeField] private Vector2 m_maxDistance = Vector2.one * 5;
        [SerializeField] private Tween m_dashingTween = null;
        [SerializeField] private Vector2 m_dashOffset = new Vector2();
        [SerializeField] private Tween m_shakeTween = null;
        [SerializeField] private CinemachineFramingTransposer framing;
        [SerializeField] private CinemachineBasicMultiChannelPerlin noise;
        // Effect

        private void LateUpdate()
        {
            FollowUpdate();
        }
        private void OnEnable()
        {
            if (followCam) followCam.onCameraFollowingToggle.AddListener(ToggleFollow);
        }
        private void OnDisable()
        {
            if (followCam) followCam.onCameraFollowingToggle.RemoveListener(ToggleFollow);
        }
        private void ToggleFollow(bool follow)
        {
            disableFollow = !follow;
        }
        public void SetBounds(Collider2D worldBoundCol)
        {
            camConfiner.m_BoundingShape2D = worldBoundCol;
        }
        private void FollowUpdate()
        {
            if (disableFollow)
            {
                if (virtualCam.Follow != disableFollowTransform) virtualCam.Follow = disableFollowTransform;
                // This will fix the camera from moving slightly to its last damping position when
                // the camera follow is disabled.
                virtualCam.ForceCameraPosition(cam.transform.position, cam.transform.rotation);
                return; 
            }
           if (SceneProperties.mainPlayer)
                if (!SceneProperties.instance.disableCameraFollowForThisRoom)
                    virtualCam.Follow = SceneProperties.mainPlayer.transform;
            Vector2 result = new Vector2();
            result += FollowPointer();
            result += FollowPlayer();
            ApplyOffset(result);
        }

        private Vector2 FollowPointer()
        {
            if (!virtualCam) return Vector2.zero;
            var player = SceneProperties.mainPlayer;
            if (!player) return Vector2.zero;
            Vector2 mousePosWorld = player.input.MousePointWorld;
            if (BucketsPlayerInputHandler.instance.IsGamepad())
            {
                mousePosWorld = player.rb.position + player.input.lastAimDirection.normalized * 10f + player.rb.velocity.normalized * 0.5f;
                //mousePosWorld = player.DistanceToPoint(pos);
            }
            else
            {
                if (TUFF.GameManager.disablePlayerInput)
                {
                    mousePosWorld = BucketsPlayerInputHandler.instance.bufferedPointerWorld;
                }
            }
            Vector2 distance = player.DistanceToPoint(mousePosWorld);
            float magnitude = distance.magnitude;
            Vector2 normDist = distance.normalized;

            Vector2 minDist = normDist * m_minDistance.magnitude;
            Vector2 maxDist = normDist * m_maxDistance.magnitude;

            float magInverseLerp = Mathf.InverseLerp(m_minDistance.magnitude, m_maxDistance.magnitude, distance.magnitude);
            float magLerp = Mathf.Lerp(m_minOffset.magnitude, m_maxOffset.magnitude, magInverseLerp);
            Vector2 offset = distance.normalized * magLerp;
            //Vector2 inverseLerp = new Vector2(
            //    Mathf.InverseLerp(minDist.x, maxDist.x, Mathf.Abs(distance.x)), 
            //    Mathf.InverseLerp(minDist.y, maxDist.y, Mathf.Abs(distance.y)));
            //Vector2 lerp = new Vector2(
            //    Mathf.Lerp(m_minOffset.x, m_maxOffset.x, Mathf.Abs(inverseLerp.x)), 
            //    Mathf.Lerp(m_minOffset.y, m_maxOffset.y, Mathf.Abs(inverseLerp.y)));
            //Vector2 offsetMagnitude = lerp;
            //Vector2 offset = distance.normalized * offsetMagnitude;
            //Vector2 offset = new Vector2(distance.normalized.x * offsetMagnitude.x, distance.normalized.y * offsetMagnitude.y);

            // Debug
            Color color = Color.Lerp(Color.green, Color.red, magInverseLerp);
            if (magInverseLerp >= 1) color = Color.black;
            Debug.DrawLine(player.rb.position, mousePosWorld, color);
            return offset;
        }
        private Vector2 FollowPlayer()
        {
            var player = SceneProperties.mainPlayer;
            if (!player) return Vector2.zero;
            var vel = player.rb.velocity.normalized.x;
            Vector2 value = new Vector2(vel, 0); // Follow only velocity X
            //if (vel.y < 1) return Vector2.zero;
            return value;
        }
        private void ApplyOffset(Vector2 value)
        {
            if (!framing) framing = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (framing)
            {
                framing.m_TrackedObjectOffset = value;
            }
            if (camOffset)
            {
                camOffset.m_Offset.x = m_dashOffset.x;
                camOffset.m_Offset.y = m_dashOffset.y;
            }
        }
        public void PlayDashOffset()
        {
            var player = SceneProperties.mainPlayer;
            if (!player) return;
            var distanceToPlayer = player.input.MousePointWorld.y - player.transform.position.y;
            //Debug.Log(distanceToPlayer);
            float duration = 0.05f;
            Vector2 to = new Vector2(-player.dashDirection.x * 0.5f, Mathf.Sign(distanceToPlayer) * -0.2f) * dashOffsetIntensity;
            GameUtility.KillTween(ref m_dashingTween);
            m_dashingTween = DOTween.To(() => m_dashOffset, x => m_dashOffset = x, to, duration).From(Vector2.zero)
                .SetLoops(2, LoopType.Yoyo).SetUpdate(true)
                .SetEase(Ease.Linear);
        }
        public void ShakeCamera(float initialAmplitude, float duration = 0.5f, bool unscaledTime = true)
        {
            if (!virtualCam) return;
            if (!noise) noise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (!noise) return;
            GameUtility.KillTween(ref m_shakeTween);
            noise.m_AmplitudeGain = initialAmplitude;
            m_shakeTween = DOTween.To(() => noise.m_AmplitudeGain, value => ShakeUpdate(value), 0f, duration).SetUpdate(unscaledTime);
        }
        private void ShakeUpdate(float value)
        {
            noise.m_AmplitudeGain = value;
        }
        public Vector3 GetWorldToScreenPoint(Vector3 position)
        {
            if (!cam) return Vector2.zero;
            return cam.WorldToScreenPoint(position);
        }
        private void OnDestroy()
        {
            if (followCam && Application.isPlaying) followCam.onCameraFollowingToggle.RemoveListener(ToggleFollow);
        }
    }
}

