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
        public void CallAfterImage(Vector3 position, Quaternion rotation, Sprite sprite, bool flip, Color color, float duration, bool asAddedColor)
        {
            var available = pool.Find(o => !o.inUse);
            if (available == null) available = AddNew();
            available.Invoke(position, rotation, sprite, flip, color, duration, asAddedColor);
        }
        public void CallAfterImage(Vector3 position, Quaternion rotation, Sprite sprite, bool flip, Color color, float duration)
        {
            var available = pool.Find(o => !o.inUse);
            if (available == null) available = AddNew();
            available.Invoke(position, rotation, sprite, flip, color, duration);
        }
    }
}
