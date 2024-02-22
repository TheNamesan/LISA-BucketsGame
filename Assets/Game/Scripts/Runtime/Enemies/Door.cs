using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Door : MovingEntity
    {
        public bool open { get => m_open; }
        [SerializeField] private bool m_open;
        
        public void OpenRight()
        {
            Open(1, true);
        }
        public void OpenLeft()
        {
            Open(-1, true);
        }
        public bool Open(float dir)
        {
            return Open(dir, false);
        }
        public bool Open(float dir, bool killEnemy = false)
        {
            if (m_open) return false;
            int openDir = (dir > 0 ? 1 : -1);
            Debug.Log($"Open: {openDir}");
            // Tmp below until we have proper sprites
            sprite.transform.localPosition = new Vector3(openDir * 0.5f, sprite.transform.localPosition.y, sprite.transform.localPosition.z);
            sprite.transform.localScale = new Vector3(openDir * 1.5f, sprite.transform.localScale.y, sprite.transform.localScale.z);
            m_open = true;
            
            if (killEnemy) Hitbox(openDir);
            col.enabled = false;

            return true;
        }
        public override bool Hurt(Vector2 launch)
        {
            return Open(launch.x, true);
        }
        private void Hitbox(int openDir)
        {
            Debug.Log("Hitbox");
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            Vector2 size = new Vector2(col.bounds.size.x, col.bounds.size.y);
            Vector2 dir = transform.right * openDir;
            float distance = 1.5f;
            Debug.Log(size);
            RaycastHit2D[] hits = Physics2D.BoxCastAll(rb.position, size, 0f, dir, distance, hitboxLayers);
            Debug.DrawRay(rb.position, dir * distance, Color.magenta, 1f);
            Debug.Log(hits.Length);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider == this.hurtbox.col) continue;
                if (hits[i].collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    //Debug.Log($"FOUND HURTBOX: {hurtbox.gameObject.name}!!");
                    if (hurtbox.team == Team.Enemy && !hurtbox.invulnerable)
                    {
                        //Debug.Log("FOUND ENEMY!!");
                        bool hitTarget = hurtbox.Kill(dir);
                    }
                }
            }
        }
    }
}

