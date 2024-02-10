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
        public WeaponMode weaponMode = WeaponMode.Pistols;
        
        [Header("Pistol")]
        public int ticksFireRate = 15;
        public int pistolAnimDuration = 22;
        public int ticks { get => m_ticks; }
        private int m_ticks = 0;
        public int animTicks { get => m_animTicks; }
        private int m_animTicks = 0;

        public void CancelAnim()
        {
            m_animTicks = 0;
        }
        private void FixedUpdate()
        {
            if (m_ticks > 0)
                m_ticks--;
            if (m_animTicks > 0)
                m_animTicks--;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="normal"></param>
        /// <returns>Returns true if bullet was shot.</returns>
        public bool Shoot(Vector2 normal)
        {
            if (m_ticks > 0) return false;
            Vector2 position = transform.position;
            if (weaponMode == WeaponMode.Pistols)
            {
                Vector3 offset = new Vector3(0, 0.1f, 0);
                
                BulletsPool.instance.SpawnBullet(position, normal);
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
                    
                    BulletsPool.instance.SpawnBullet(position, dir);
                }
                SceneProperties.instance.camManager.ShakeCamera(10, 0.5f);
            }
            m_ticks = GetFireRate();
            m_animTicks = GetAnimDuration();
            Debug.Log("Pew");
            return true;
        }

        public int GetFireRate()
        {
            return ticksFireRate;
        }
        public int GetAnimDuration()
        {
            return pistolAnimDuration;
        }
    }
}

