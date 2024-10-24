using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Spike : MonoBehaviour
    {
        public Rigidbody2D rb;
        public BoxCollider2D col;
        private void FixedUpdate()
        {
            AttackRaycast();
        }
        private void AttackRaycast()
        {
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            Vector2 dir = Vector2.zero;
            RaycastHit2D[] hits = Physics2D.BoxCastAll(rb.position, col.bounds.size, 0f, dir, 0f, hitboxLayers);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (hurtbox.team == Team.Player && !hurtbox.invulnerable)
                    {
                        Vector2 launchDir = -hits[i].normal;
                        bool hitTarget = hurtbox.Collision(launchDir);
                        //if (hitTarget) TUFF.AudioManager.instance.PlaySFX(SFXList.instance.barrelBroWallHit);
                    }
                }
            }
        }
    }
}

