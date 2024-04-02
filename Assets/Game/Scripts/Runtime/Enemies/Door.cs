using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Door : MovingEntity
    {
        public Sprite closedOpen;
        public Sprite rightOpen;
        public Sprite leftOpen;
        public bool open { get => m_open; }
        [SerializeField] private bool m_open;
        [SerializeField] private int m_dir = 0;

        private void Update()
        {
            if (!open)
            {
                sprite.sprite = closedOpen;
            }
            else
            {
                if (m_dir > 0) sprite.sprite = rightOpen;
                else sprite.sprite = leftOpen;
            }
        }
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
            m_dir = (dir > 0 ? 1 : -1);
            //Debug.Log($"Open: {openDir}");
            m_open = true;
            TUFF.AudioManager.instance.PlaySFX(SFXList.instance.doorOpenSFX);
            
            if (killEnemy) Hitbox(m_dir);
            col.enabled = false;

            return true;
        }
        public override bool Hurt(Vector2 launch)
        {
            return Open(launch.x, true);
        }
        public override bool TryKill(Vector2 launch)
        {
            return Open(launch.x, true);
        }
        private void Hitbox(int openDir)
        {
            Debug.Log("Hitbox");
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            Vector2 size = new Vector2(hurtbox.col.bounds.size.x, hurtbox.col.bounds.size.y);
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

