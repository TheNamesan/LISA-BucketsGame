using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class PoolObject : MonoBehaviour
    {
        public bool inUse { get => m_inUse; }
        protected bool m_inUse = false;
        public virtual void ReturnToPool()
        {
            m_inUse = false;
            gameObject.SetActive(false);
        }
    }
}
