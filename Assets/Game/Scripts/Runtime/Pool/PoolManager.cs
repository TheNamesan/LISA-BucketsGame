using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{

    public class PoolManager<T> : MonoBehaviour where T : PoolObject
    {
        public int startingObjects = 30;
        public T prefab;
        public List<T> pool = new();
        protected bool m_init = false;
        protected virtual void Initialize()
        {
            if (m_init) return;
            if (prefab)
            {
                for (int i = 0; i <= startingObjects; i++)
                {
                    AddNew();
                }
            }
            m_init = true;
        }
        protected virtual T AddNew()
        {
            var newObj = Instantiate(prefab, transform);
            pool.Add(newObj);
            return newObj;
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
