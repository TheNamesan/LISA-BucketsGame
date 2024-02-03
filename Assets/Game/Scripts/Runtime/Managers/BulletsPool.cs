using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class BulletsPool : PoolManager<Bullet>
    {
        public static BulletsPool instance;
        private void Awake()
        {
            if (!instance) instance = this;
        }
        private void Start()
        {
            Initialize();
        }
        public void SpawnBullet(Vector2 position, Vector2 dir, Team team = Team.Player)
        {
            Initialize();
            Bullet available = GetBullet(position);
            available.Fire(dir, team);
        }
        public void SpawnBullet(Vector2 position, Sprite sprite, Vector2 size, Vector2 dir, Team team = Team.Player)
        {
            Initialize();
            Bullet available = GetBullet(position);
            available.Fire(dir, sprite, size, team);
        }

        private Bullet GetBullet(Vector2 position)
        {
            var available = pool.Find(o => !o.inUse);
            if (available == null) available = AddNew();
            available.transform.position = position;
            return available;
        }
    }
}
