using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class WeaponBehaviour : MonoBehaviour
    {
        [Header("Shoot")]
        public Bullet bulletPrefab;
        
        public void Shoot(Vector2 normal)
        {
            if (bulletPrefab != null) // Change this to a pool
            {
                var newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                newBullet.Fire(normal);
                Debug.Log("Pew");
            }
        }
    }
}

