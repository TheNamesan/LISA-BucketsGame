using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class VFXPool : PoolManager<VFXObject>
    {
        public static VFXPool instance;
        private void Awake()
        {
            if (!instance) instance = this;
        }
        private void Start()
        {
            Initialize();
        }
        public void PlayVFX(string animName, Vector2 position)
        {
            var available = pool.Find(o => !o.inUse);
            if (available == null) available = AddNew();
            available.transform.position = position;
            available.Play(animName);
        }
    }
}
