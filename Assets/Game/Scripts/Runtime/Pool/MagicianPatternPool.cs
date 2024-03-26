using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{

    public class MagicianPatternPool : PoolManager<MagicianPattern>
    {
        public static MagicianPatternPool instance;
        public MagicianBarrage barragePrefab;
        public MagicianFloor floorPrefab;
        private void Awake()
        {
            if (!instance) instance = this;
        }
        private void Start()
        {
            Initialize();
        }
        protected override void Initialize()
        {
            if (m_init) return;
            if (barragePrefab)
            {
                AddPattern(barragePrefab);
                AddPattern(floorPrefab);
            }
            m_init = true;
        }
        private MagicianPattern AddPattern(MagicianPattern prefab)
        {
            var newObj = Instantiate(prefab, transform);
            pool.Add(newObj);
            return newObj;
        }
        public MagicianPattern InvokePattern(Vector2 position, MagicianPatternType pattern)
        {
            System.Type targetType = typeof(MagicianPattern);
            MagicianPattern targetPrefab = null;
            if (pattern == MagicianPatternType.Barrage)
            { targetType = typeof(MagicianBarrage); targetPrefab = barragePrefab; }

            var available = pool.Find(o => o.GetType() == targetType && !o.inUse);
            if (available == null) available = AddPattern(targetPrefab);
            available.transform.position = position;
            available.Play();
            return available;
        }
        public MagicianPattern InvokeBarrage(Vector2 position, BarrageDirection direction)
        {
            System.Type targetType = typeof(MagicianBarrage);
            var available = pool.Find(o => o.GetType() == targetType && !o.inUse);
            if (available == null) available = AddPattern(barragePrefab);

            available.transform.position = position;
            var bar = available as MagicianBarrage;
            bar.barrageDir = direction;
            available.Play();
            return available;
        }
        public MagicianPattern InvokeFloor(Vector2 position)
        {
            System.Type targetType = typeof(MagicianFloor);
            var available = pool.Find(o => o.GetType() == targetType && !o.inUse);
            if (available == null) available = AddPattern(barragePrefab);

            available.transform.position = position;
            
            available.Play();
            var bar = available as MagicianFloor;
            return available;
        }
    }

}
