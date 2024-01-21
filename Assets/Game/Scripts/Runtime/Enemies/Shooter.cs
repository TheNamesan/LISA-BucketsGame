using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Shooter : MonoBehaviour
    {
        [Header("Shoot")]
        public Bullet bulletPrefab;
        public int ticksFireRate = 50;
        public Team team = Team.Enemy;
        private int m_ticks = 0;
        

        private void OnEnable()
        {
            m_ticks = ticksFireRate;
        }
        private void FixedUpdate()
        {
            ShootLoop();
        }
        private void ShootLoop()
        {
            m_ticks--;
            if (m_ticks <= 0)
            {
                Shoot(transform.right);
                m_ticks = ticksFireRate;
            }
        }
        public void Shoot(Vector2 normal)
        {
            // Change this to a pool
            if (!bulletPrefab) return;
            var newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            newBullet.Fire(normal, team);
        }
    }
}
