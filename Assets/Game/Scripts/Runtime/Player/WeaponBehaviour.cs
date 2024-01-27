using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public enum WeaponMode
    {
        Pistols = 0,
        Shotgun = 1
    }
    public class WeaponBehaviour : MonoBehaviour
    {
        [Header("Shoot")]
        public Bullet bulletPrefab;
        public WeaponMode weaponMode = WeaponMode.Pistols;
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
            
            if (weaponMode == WeaponMode.Pistols)
            {
                Vector3 offset = new Vector3(0, 0.1f, 0);
                // Change this to a pool
                SpawnBullet(normal);
            }
            else if (weaponMode == WeaponMode.Shotgun)
            {
                float bullets = 8;
                float spreadAngle = 30f;
                for (int i = 0; i < bullets + 1; i++)
                {
                    float a = -spreadAngle * 0.5f;
                    float angle = -Vector2.SignedAngle(normal, Vector2.right) + Mathf.Lerp(-a, a, Mathf.InverseLerp(0, bullets + 1, i));
                    float rad = Mathf.Deg2Rad * angle;
                    Vector2 nor = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                    Vector2 dir = (nor).normalized;
                    Debug.DrawRay(transform.position, dir, Color.white, 1f);
                    
                    SpawnBullet(dir);
                }
            }
            m_ticks = GetFireRate();
            Debug.Log("Pew");
        }

        private void SpawnBullet(Vector2 dir) // Change this to a pool
        {
            var newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            newBullet.Fire(dir);
        }

        public int GetFireRate()
        {
            return ticksFireRate;
        }
    }
}

