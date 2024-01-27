using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{   
    public class AfterImagesPool : MonoBehaviour
    {
        public int startingObjects = 50;
        public AfterImage afterImagePrefab;
        public List<AfterImage> pool = new();
        private bool m_init = false;
        public static AfterImagesPool instance;
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
            if (afterImagePrefab)
            {
                for (int i = 0; i < startingObjects + 1; i++)
                {
                    AddNew();
                }
            }
            m_init = true;
        }

        private AfterImage AddNew()
        {
            var newObj = Instantiate(afterImagePrefab, transform);
            pool.Add(newObj);
            return newObj;
        }

        public void CallAfterImage(Vector3 position, Sprite sprite, bool flip, Color color, float duration)
        {
            var available = pool.Find(o => !o.inUse);
            if (available == null) available = AddNew();
            available.Invoke(position, sprite, flip, color, duration);
        }
    }
}
