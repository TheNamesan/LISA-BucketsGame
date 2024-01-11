using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace BucketsGame
{
    public class CameraManager : MonoBehaviour
    {
        public Camera cam;
        public CinemachineVirtualCamera virtualCam;
        [SerializeField] private float m_minOffset = 0;
        [SerializeField] private float m_maxOffset = 3;
        [SerializeField] private float m_minDistance = 0;
        [SerializeField] private float m_maxDistance = 3;

        private void LateUpdate()
        {
            FollowPointer();
        }
        private void FollowPointer()
        {
            if (!virtualCam) return;
            var player = SceneProperties.mainPlayer;
            if (!player) return;
            Vector2 distance = player.DistanceToMouse();
            float magnitude = distance.magnitude;
            float offsetMagnitude = Mathf.Lerp(m_minOffset, m_maxOffset, Mathf.InverseLerp(m_minDistance, m_maxDistance, magnitude));
            Vector2 offset = distance.normalized * offsetMagnitude;
            Debug.Log("offset = " + offset);
            virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = offset;
        }
    }
}

