using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{   
    public class AfterImagesPool : PoolManager<AfterImage>
    {
        public static AfterImagesPool instance;
        private void Awake()
        {
            if (!instance) instance = this;
        }
        private void Start()
        {
            Initialize();
        }
        public void CallAfterImage(Vector3 position, Sprite sprite, bool flip, Color color, float duration)
        {
            var available = pool.Find(o => !o.inUse);
            if (available == null) available = AddNew();
            available.Invoke(position, sprite, flip, color, duration);
        }
    }
}
