using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Magician : Enemy
    {
        [Header("Magician Properties")]
        public Vector2 teleportPositionPadding = new Vector2();
        public Transform sineMove;
        public PlayerController player { get => SceneProperties.mainPlayer; }
        private void Start()
        {
            //AddAsRoomEnemy();
        }
        private void FixedUpdate()
        {
            if (!m_dead) rb.sharedMaterial = BucketsGameManager.instance.aliveMat;
            else rb.sharedMaterial = BucketsGameManager.instance.deadMat;

            GroundCheck();
            WallCheck();
            MoveHandler();
            TimerHandler();
        }
        private void MoveHandler()
        {
            if (m_dead) return;
            if (!grounded)
            {
                float amplitude = 0.5f;
                float value = Mathf.Sin(Time.time * 2f) * amplitude;
                sineMove.transform.localPosition = new Vector2(sineMove.transform.localPosition.x, value);
            }
        }
        private void DoRandomTeleport()
        {
            if (!SceneProperties.instance) return;
            if (!SceneProperties.instance.TUFFSceneProperties) return;
            var roomBoundsMin = SceneProperties.instance.TUFFSceneProperties.min;
            var roomBoundsMax = SceneProperties.instance.TUFFSceneProperties.max;
            float randomX = Random.Range(roomBoundsMin.x + teleportPositionPadding.x, roomBoundsMax.x - teleportPositionPadding.x);
            float randomY = Random.Range(roomBoundsMin.y + teleportPositionPadding.y, roomBoundsMax.y - teleportPositionPadding.y);
            var newPosition = new Vector2(randomX, randomY);
            rb.position = newPosition;
        }
        private void TimerHandler()
        {

        }
        public override bool Hurt(Vector2 launch)
        {
            if (m_dead) return false;
            DoRandomTeleport();
            return false;
            //hp--;
            //AlertEnemy();
            //if (hp > 0) { BucketsGameManager.instance.OnEnemyHit(); return true; }
            //return Kill(launch);
        }
    }
}
