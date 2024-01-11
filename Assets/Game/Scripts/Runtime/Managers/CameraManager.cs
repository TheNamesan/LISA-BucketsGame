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
        [SerializeField] private Vector2 m_minOffset = Vector2.zero;
        [SerializeField] private Vector2 m_maxOffset = Vector2.one;
        [SerializeField] private Vector2 m_minDistance = Vector2.zero;
        [SerializeField] private Vector2 m_maxDistance = Vector2.one * 10;

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
            Vector2 sign = new Vector2(Mathf.Sign(distance.x), Mathf.Sign(distance.y));
            float magnitude = distance.magnitude;
            Vector2 inverseLerp = new Vector2(Mathf.InverseLerp(m_minDistance.x, m_maxDistance.x, Mathf.Abs(distance.x)), Mathf.InverseLerp(m_minDistance.y, m_maxDistance.y, Mathf.Abs(distance.y)));
            Vector2 lerp = new Vector2(Mathf.Lerp(m_minOffset.x, m_maxOffset.x, Mathf.Abs(inverseLerp.x)), Mathf.Lerp(m_minOffset.y, m_maxOffset.y, Mathf.Abs(inverseLerp.y)));
            Vector2 offsetMagnitude = lerp;
            Vector2 offset = distance.normalized * offsetMagnitude;
            //Vector2 offset = new Vector2(distance.normalized.x * offsetMagnitude.x, distance.normalized.y * offsetMagnitude.y);

            // Debug
            Color color = Color.Lerp(Color.green, Color.red, inverseLerp.magnitude);
            if (inverseLerp.magnitude >= 1) color = Color.black;
            Debug.DrawLine(player.rb.position, player.input.MousePointWorld, color);
            virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = offset;
        }
    }
}

