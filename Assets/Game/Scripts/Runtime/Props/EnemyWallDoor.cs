using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TUFF;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace BucketsGame
{
    public class EnemyWallDoor : MonoBehaviour
    {
        public EnemyWallDoor neighbourDoor = null;
        public SFX enterSFX = new();

        [Header("References")]
        public BoxCollider2D col;

        [Header("Line Of Sight")]
        public float coneAngle = 45f;
        public float coneAngleOffset = -88f;
        public float coneDistance = 15f;
        public int coneAccuracy = 40;

        public bool hasBucketsLoS = false;
        public float distanceToBuckets = 999f;

        public static List<EnemyWallDoor> sceneEnemyWallDoors = new();

        private void OnEnable()
        {
            sceneEnemyWallDoors?.Add(this);
        }
        private void OnDisable()
        {
            sceneEnemyWallDoors?.Remove(this);
        }
        private void OnDestroy()
        {
            sceneEnemyWallDoors?.Remove(this);
        }
        public void PlayEnterSFX()
        {
            AudioManager.instance.PlaySFX(enterSFX);
        }
        public static EnemyWallDoor FindNearestWallDoorWithLoS(Vector3 enemyPos)
        {
            EnemyWallDoor nearest = null;
            float lastDistance = 999999f;

            for (int i = 0; i < sceneEnemyWallDoors.Count; i++)
            {
                EnemyWallDoor wallDoor = sceneEnemyWallDoors[i];
                float curDistance = Vector3.Distance(enemyPos, wallDoor.transform.position);
                if (curDistance < lastDistance && wallDoor.NeighbourHasLoS())
                {
                    nearest = sceneEnemyWallDoors[i];
                    lastDistance = curDistance;
                }
            }
            return nearest;
        }
        public void TeleportToNeighbour(Enemy enemy)
        {
            if (!neighbourDoor) return;
            if (!enemy) return;
            enemy.rb.position = neighbourDoor.transform.position;
        }
        public bool NeighbourHasLoS()
        {
            if (!neighbourDoor) return false;
            return neighbourDoor.hasBucketsLoS;
        }
        public float GetNeighbourDistance()
        {
            if (!neighbourDoor) return 9999f;
            return neighbourDoor.distanceToBuckets;
        }
        private void FixedUpdate()
        {
            EnemyLineOfSight();
        }
        protected virtual void EnemyLineOfSight()
        {
            hasBucketsLoS = false;
            distanceToBuckets = 99999f;

            LayerMask layers = BucketsGameManager.instance.groundLayers | (1 << BucketsGameManager.instance.playerLayer);

            int max = coneAccuracy + 1;
            for (int i = 0; i < max; i++)
            {
                float a = -coneAngle * 0.5f;
                float angle = -coneAngleOffset + Mathf.Lerp(-a, a, Mathf.InverseLerp(0, max, i));
                float rad = Mathf.Deg2Rad * angle;
                Vector2 normal = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                Vector2 dir = new Vector2(normal.x, normal.y);
                float distance = coneDistance;
                Vector3 pos = transform.position;
                if (col) pos.y -= col.size.y * 0.5f - 0.1f;
                RaycastHit2D[] losHits = Physics2D.RaycastAll(pos, dir, distance, layers);
                Color color = Color.clear;
                for (int j = 0; j < losHits.Length; j++)
                {
                    var los = losHits[j];
                    // If ground layer
                    if (los.collider.gameObject.layer == 6)
                    {
                        if (los.collider.TryGetComponent(out TUFF.TerrainProperties props))
                            if (props.enemyBulletsGoThrough) continue;
                        if (los.collider.TryGetComponent(out Door door))
                            continue;
                        break;
                    }
                    else if (los.collider.gameObject.layer == BucketsGameManager.instance.playerLayer)
                    {
                        color = Color.green;
                        //Debug.Log($"[{gameObject.name}] Found enemy on it: " + i + $". Pos: {SceneProperties.mainPlayer.transform.position}");
                        //AlertEnemy();
                        hasBucketsLoS = true;
                        distanceToBuckets = Vector2.Distance(los.collider.transform.position, pos);//Mathf.Abs(los.collider.transform.position.x - pos.x);
                        break;
                    }
                }
                Debug.DrawRay(pos, dir.normalized * distance, color, Time.fixedDeltaTime);
            }
        }
        protected virtual void DrawLineOfSightGizmos()
        {
            int max = coneAccuracy + 1;
            for (int i = 0; i < max; i++)
            {
                float a = -coneAngle * 0.5f;
                float angle = -coneAngleOffset + Mathf.Lerp(-a, a, Mathf.InverseLerp(0, max, i));
                float rad = Mathf.Deg2Rad * angle;
                Vector2 normal = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

                Vector2 dir = new Vector2(normal.x, normal.y);
                float distance = coneDistance;
                Vector3 pos = transform.position;
                if (col) pos.y -= col.size.y * 0.5f - 0.1f;
                Color prev = Gizmos.color;
                Color color = Color.white;
                color.a = 0.2f;
                Gizmos.color = color;
                Gizmos.DrawRay(pos, dir.normalized * distance);
                Gizmos.color = prev;
            }
        }
        private void OnDrawGizmos()
        {
            DrawLineOfSightGizmos();
        }
        private void OnDrawGizmosSelected()
        {
            if (neighbourDoor)
            {
                Color prev = Gizmos.color;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, neighbourDoor.transform.position);
                Gizmos.color = prev;
            }
        }
    }
}
