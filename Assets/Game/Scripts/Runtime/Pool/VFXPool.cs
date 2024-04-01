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
        public void PlayVFX(string animName, Vector2 position, bool flipX = false, float rotation = 0, string sortingLayerName = "Default")
        {
            var available = pool.Find(o => !o.inUse);
            if (available == null) available = AddNew();
            available.transform.position = position;
            available.transform.rotation = Quaternion.Euler(available.transform.eulerAngles.x, available.transform.eulerAngles.y, rotation);
            available.Play(animName, flipX, sortingLayerName);
        }
    }
}
