using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class MagicianBarragePillar : MonoBehaviour
    {
        public BoxCollider2D hitboxRef;
        public int fireDelay = 25;
        public int stayTime = 5;
        public int endTimer = 10;
        [SerializeField] private int m_delay = 0;
        [SerializeField] private int m_stay = 0;
        [SerializeField] private int m_end = 0;
        
        private void OnEnable()
        {
            m_delay = fireDelay;
            m_stay = stayTime;
            m_end = endTimer;
            
            TmpShowSprite(false); // Tmp;
        }

        private void TmpShowSprite(bool show)
        {
            if (hitboxRef && hitboxRef.TryGetComponent(out SpriteRenderer spriteRdr))
            {
                spriteRdr.enabled = show;
            }
        }

        private void FixedUpdate()
        {
            DelayTimer();
            StayTimer();
            EndTimer();
        }
        
        private void DelayTimer()
        {
            if (m_delay <= 0) return;
            m_delay--;
        }
        private void StayTimer()
        {
            if (m_delay > 0 || m_stay <= 0) { TmpShowSprite(false); return; }
            if (m_stay == stayTime) TUFF.AudioManager.instance.PlaySFX(SFXList.instance.magicianPortalSFX);
            m_stay--;
            AttackHitbox();
            TmpShowSprite(true);
        }
        private void EndTimer()
        {
            if (m_delay > 0 || m_stay > 0) return;
            if (m_end <= 0) return;
            m_end--;
            if (m_end <= 0) gameObject.SetActive(false);
        }
        private void AttackHitbox()
        {
            if (!hitboxRef) return;
            Vector2 origin = hitboxRef.transform.position;
            Vector2 size = hitboxRef.size * hitboxRef.transform.localScale;
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            var launchDir = new Vector2((Random.Range(0, 2) == 0 ? -1 : 1), 0f);
            var hits = Physics2D.BoxCastAll(origin, size, 0f, Vector2.up, 0f, hitboxLayers);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (hurtbox.team == Team.Player && !hurtbox.invulnerable)
                    {
                        bool hitTarget = hurtbox.Collision(launchDir);
                    }
                }
            }
        }
    }
}
