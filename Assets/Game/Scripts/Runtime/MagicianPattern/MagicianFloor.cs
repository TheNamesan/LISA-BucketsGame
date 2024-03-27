using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class MagicianFloor : MagicianPattern
    {
        public SpriteRenderer telegraph;
        public BoxCollider2D hitboxRef;
        public int telegraphDuration = 20;
        [SerializeField] protected int m_telegraphTime = 0;
        public int floorAscendDuration = 20;
        public int floorTotalDuration = 50;
        public float floorInitialPosition = -4.2f;
        public float floorFinalPosition = -2.7f;
        [SerializeField] protected int m_floorTime = 0;

        void Start()
        {
            
        }

        public override void Play()
        {
            m_inUse = true;
            gameObject.SetActive(true);
            m_telegraphTime = telegraphDuration;
            m_floorTime = floorTotalDuration;
            hitboxRef.transform.localPosition = 
                new Vector3(hitboxRef.transform.localPosition.x, floorInitialPosition, hitboxRef.transform.localPosition.z);
            telegraph.gameObject.SetActive(false);
            hitboxRef.gameObject.SetActive(false);
        }
        private void FixedUpdate()
        {
            FloorLogic();
        }
        private void FloorLogic()
        {
            if (m_telegraphTime > 0)
            {
                telegraph.gameObject.SetActive(true);
                hitboxRef.gameObject.SetActive(false);
                bool alternate = (m_telegraphTime % 4 == 2 || m_telegraphTime % 4 == 3);
                Color color = (alternate ? Color.red : Color.yellow);
                color.a = 0.5f;
                telegraph.color = color;
                m_telegraphTime--;
                return;
            }
            else telegraph.gameObject.SetActive(false);
            if (m_floorTime > 0)
            {
                if (m_floorTime == floorTotalDuration)
                    TUFF.AudioManager.instance.PlaySFX(SFXList.instance.magicianGroundSFX);
                hitboxRef.gameObject.SetActive(true);
                float t = Mathf.InverseLerp(floorTotalDuration, (floorTotalDuration - floorAscendDuration), m_floorTime);
                float y = Mathf.Lerp(floorInitialPosition, floorFinalPosition, t);
                Debug.Log(t);
                hitboxRef.transform.localPosition =
                    new Vector3(hitboxRef.transform.localPosition.x, y, hitboxRef.transform.localPosition.z);

                AttackHitbox();
                m_floorTime--;
            }
            else ReturnToPool();
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

