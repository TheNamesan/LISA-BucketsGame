using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class BulletsPool : MonoBehaviour
    {
        public int startingObjects = 30;
        public Bullet bulletPrefab;
        public List<Bullet> pool = new();
        private bool m_init = false;
        public static BulletsPool instance;
        private void Awake()
        {
            if (!instance) instance = this;
        }
        private void Start()
        {
            Initialize();
        }
        private void Initialize()
        {
            if (m_init) return;
            if (bulletPrefab)
            {
                for (int i = 0; i <= startingObjects; i++)
                {
                    AddNew();
                }
            }
            m_init = true;
        }
        private Bullet AddNew()
        {
            var newObj = Instantiate(bulletPrefab, transform);
            pool.Add(newObj);
            return newObj;
        }
        public void SpawnBullet(Vector2 position, Vector2 dir, Team team = Team.Player)
        {
            //var newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, SceneProperties.instance.transform);
            var available = pool.Find(o => !o.inUse);
            if (available == null) available = AddNew();
            available.transform.position = position;
            available.Fire(dir, team);
        }
        public void ResetPool()
        {
            for (int i = 0; i < pool.Count; i++)
            {
                pool[i]?.ReturnToPool();
            }
        }
    }
}
