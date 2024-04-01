using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class MovingPropPool : PoolManager<MovingProp>
    {
        public static MovingPropPool instance;
        private void Awake()
        {
            if (!instance) instance = this;
        }
        private void Start()
        {
            Initialize();
        }

        public void SpawnProp(Vector2 position, float rotation, Vector2 launchForce, Sprite sprite)
        {
            var available = pool.Find(o => !o.inUse);
            if (available == null) available = AddNew();
            available.transform.position = position;
            available.transform.rotation = Quaternion.Euler(available.transform.eulerAngles.x, available.transform.eulerAngles.y, rotation);
            available.Spawn(launchForce, sprite);
        }
    }

}
