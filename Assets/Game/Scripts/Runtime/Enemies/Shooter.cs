using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Shooter : MonoBehaviour
    {
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
                BulletsPool.instance.SpawnBullet(transform.position, transform.right, Team.Enemy);
                m_ticks = ticksFireRate;
            }
        }
    }
}
