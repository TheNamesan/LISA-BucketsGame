using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public enum SpawnLocationType
    {
        RelativeToObject = 0,
        WorldPosition = 1
    }
    public class EnemySpawner : MonoBehaviour
    {
        public bool spawnOnProximity = false;
        public bool spawnOnEnemiesKilled = false;
        [Header("Spawn Location")]
        public SpawnLocationType spawnLocationType;
        public Vector2 spawnPosition = Vector2.zero;
        [Header("Proximity")]
        public float proximityRadius = 2f;
        public Vector2 proximityCheckPositionOffset = Vector2.zero;
        public Vector2 proximityCheckPosition { get => transform.position + (Vector3)proximityCheckPositionOffset; }
        [Header("Enemies Killed")]
        public List<Enemy> enemiesToCheck = new();
        [Header("Spawn Count")]
        public int spawnRushersCount = 0;
        public int spawnShieldersCount = 0;
        public int spawnFlyersCount = 0;
        public int spawnSnipersCount = 0;
        [Header("Spawn Properties")]
        public EnemyAIState spawnState = EnemyAIState.Roaming;
        public Facing spawnFacing = Facing.Right;
        [SerializeField] private bool m_spawned = false;
        
        private void Awake()
        {
            PreloadEnemies();
        }
        private void FixedUpdate()
        {
            SpawnCheck();
        }
        private void SpawnCheck()
        {
            if (m_spawned) return;
            bool inProximity = false;
            bool enemiesKilled = false;
            if (spawnOnProximity) inProximity = ProximityCheck();
            else inProximity = true;
            if (spawnOnEnemiesKilled) enemiesKilled = EnemiesKilledCheck();
            else enemiesKilled = true;
            if (inProximity && enemiesKilled)
            {
                SpawnEnemies();
                m_spawned = true;
            }
        }
        private bool ProximityCheck()
        {
            LayerMask playerLayer = 1 << BucketsGameManager.instance.playerLayer;
            RaycastHit2D playerHit = 
                Physics2D.CircleCast(proximityCheckPosition, proximityRadius, Vector2.zero, 0f, playerLayer);
            return playerHit;
        }
        private bool EnemiesKilledCheck()
        {
            for (int i = 0; i < enemiesToCheck.Count; i++)
            {
                if (!enemiesToCheck[i]) continue;
                if (!enemiesToCheck[i].dead) return false;
            }
            return true;
        }
        private void PreloadEnemies()
        {
            for (int i = 0; i < spawnRushersCount; i++)
            {
                var enemy = Instantiate(PrefabList.instance.rusherPrefab, transform);
                enemy.gameObject.SetActive(false);
            }
            for (int i = 0; i < spawnShieldersCount; i++)
            {
                var enemy = Instantiate(PrefabList.instance.shielderPrefab, transform);
                enemy.gameObject.SetActive(false);
            }
            for (int i = 0; i < spawnFlyersCount; i++)
            {
                var enemy = Instantiate(PrefabList.instance.flyerPrefab, transform);
                enemy.gameObject.SetActive(false);
            }
            for (int i = 0; i < spawnSnipersCount; i++)
            {
                var enemy = Instantiate(PrefabList.instance.sniperPrefab, transform);
                enemy.gameObject.SetActive(false);
            }
            if (Application.isPlaying)
                EntityResetCaller.onResetLevel.AddListener(HideEnemies);
        }
        private void SpawnEnemies()
        {
            foreach(Transform child in transform)
            {
                child.position = GetSpawnLocation();
                if (child.TryGetComponent(out Enemy enemy))
                {
                    enemy.enemyState = spawnState;
                    enemy.ChangeFacing(spawnFacing);
                }
                child.gameObject.SetActive(true);
            }
        }
        private void HideEnemies()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        private Vector3 GetSpawnLocation()
        {
            if (spawnLocationType == SpawnLocationType.RelativeToObject)
                return transform.position + (Vector3)spawnPosition;
            return spawnPosition;
        }
        private void OnDestroy()
        {
            if (Application.isPlaying)
                EntityResetCaller.onResetLevel.RemoveListener(HideEnemies);
        }
        private void OnDrawGizmos()
        {
            if (spawnOnProximity) Gizmos.DrawWireSphere(proximityCheckPosition, proximityRadius);
        }
    }
}