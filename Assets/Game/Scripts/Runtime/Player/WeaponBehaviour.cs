using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUFF;

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
        public Vector2 shootNormal;
        public int adrenalineFireRateIterations = 4;

        [Header("Velocity")]
        public float normalVelocity = 30;
        public float adrenalineVelocityScale = 1.4f;

        [Header("Pistol")]
        public Sprite casingSprite;
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
            {
                int iterations = (BucketsGameManager.instance.focusMode ? adrenalineFireRateIterations : 1);
                m_ticks -= iterations;
                if (m_ticks < 0) m_ticks = 0;
            }
            if (m_animTicks > 0)
            {
                m_animTicks--;
            }
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
            shootNormal = normal;
            if (weaponMode == WeaponMode.Pistols)
            {
                Vector2 offset = normal * 1f;

                // Adjust so it doesn't shoot out of bounds
                RaycastHit2D hit = Physics2D.Linecast(position, position + offset, BucketsGameManager.instance.groundLayers);
                if (hit)
                {
                    offset = hit.point - position;
                }
                position += offset;
                BulletsPool.instance.SpawnBullet(position, normal);
                Vector2 forceA = new Vector2(Random.Range(-1.25f, 1.25f), Random.Range(-1.25f, 1.25f)) * 5f;
                Vector2 forceB = new Vector2(Random.Range(-1.25f, 1.25f), Random.Range(-1.25f, 1.25f)) * 5f;
                //forceA *= new Vector2(-normal.x, normal.y);
                //forceB *= new Vector2(-normal.x, normal.y);
                Vector2 size = new Vector2(0.19f, 0.07f);
                MovingPropPool.instance.SpawnProp(transform.position, 0f, forceA, casingSprite, 5f, size, 2f);
                MovingPropPool.instance.SpawnProp(transform.position, 0f, forceB, casingSprite, 5f, size, 2f);
                AudioManager.instance.PlaySFX(SFXList.instance.pistolShotSFX);
            }
            else if (weaponMode == WeaponMode.Shotgun)
            {
                float bullets = 8;
                float spreadAngle = 30f;
                for (int i = 0; i < bullets + 1; i++)
                {
                    float a = -spreadAngle * 0.5f;

                    float baseAngle = -Vector2.SignedAngle(normal, Vector2.right);
                    float spread = Mathf.Lerp(-a, a, Mathf.InverseLerp(0, bullets + 1, i));
                    //Debug.Log($"It #{i}: {spread}");
                    float angle = baseAngle + spread;
                    float rad = Mathf.Deg2Rad * angle;
                    Vector2 nor = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                    Vector2 dir = (nor).normalized;

                    position = transform.position;
                    Vector2 offset = dir * 1f;
                    // Adjust so it doesn't shoot out of bounds
                    RaycastHit2D hit = Physics2D.Linecast(position, position + offset, BucketsGameManager.instance.groundLayers);
                    if (hit)
                    {
                        offset = hit.point - position;
                    }
                    position += offset;
                    Debug.DrawRay(position, dir, Color.white, 1f);
                    BulletsPool.instance.SpawnBullet(position, dir);
                }
                SceneProperties.instance.camManager.ShakeCamera(10, 0.5f);
            }
            m_ticks = GetFireRate();
            m_animTicks = GetAnimDuration();
            Debug.Log("Pew");
            return true;
        }
        public float GetVelocity()
        {
            float vel = normalVelocity * (BucketsGameManager.instance.focusMode ? adrenalineVelocityScale : 1f );
            return vel;
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

