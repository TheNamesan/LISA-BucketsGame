using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class WeaponBehaviour : MonoBehaviour
    {
        [Header("Shoot")]
        public Bullet bulletPrefab;
        public int ticksFireRate = 10;
        private int m_ticks = 0;

        private void FixedUpdate()
        {
            if (m_ticks > 0)
                m_ticks--;
        }
        public void Shoot(Vector2 normal)
        {
            if (!bulletPrefab) return;
            if (m_ticks > 0) return;
            // Change this to a pool
            var newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            newBullet.Fire(normal);
            m_ticks = ticksFireRate;
            Debug.Log("Pew");
        }
    }
}

