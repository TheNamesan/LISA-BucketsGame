using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    [System.Serializable]
    public struct BulletProperties
    {
        public float velocity;
        public string animName;
        public Vector2 spriteSize;
        public BulletProperties(float velocity, string animName, Vector2 spriteSize)
        {
            this.velocity = velocity;
            this.animName = animName;
            this.spriteSize = spriteSize;
        }
    }
    public class BulletsPool : PoolManager<Bullet>
    {
        public static BulletsPool instance;
        [SerializeField] private BulletProperties m_normalProperties = new(30f, "NormalBullet", new Vector2(0.3f, 0.3f));
        [SerializeField] private BulletProperties m_spearProperties = new(30f, "SpearBullet", new Vector2(1f, 1f));
        [SerializeField] private BulletProperties m_bottleProperties = new(30f, "BottleBullet", new Vector2(1f, 1f));
        [SerializeField] private BulletProperties m_magicianProperties = new(30f, "MagicianBullet", new Vector2(1f, 1f));
        public float adrenalineVelocityScale = 1.4f;
        private void Awake()
        {
            if (!instance) instance = this;
        }
        private void Start()
        {
            Initialize();
        }
        private BulletProperties GetTargetProperties(BulletType type)
        {
            switch (type)
            {
                case BulletType.Spear: return m_spearProperties;
                case BulletType.Bottle: return m_bottleProperties;
                case BulletType.Magician: return m_magicianProperties;
            }
            return m_normalProperties;
        }
        private TUFF.SFX GetSFX(BulletType type)
        {
            switch (type)
            {
                case BulletType.Spear: return null;
                case BulletType.Bottle: return SFXList.instance.flyerShootHitSFX;
                case BulletType.Magician: return SFXList.instance.magicianBulletHitSFX;
            }
            return null;
        }
        public void SpawnBullet(Vector2 position, Vector2 dir, Team team = Team.Player)
        {
            SpawnBullet(position, dir, BulletType.Normal, team);
        }
        public void SpawnBullet(Vector2 position, Vector2 dir, BulletType bulletType, Team team = Team.Player)
        {
            Initialize();
            Bullet available = GetBullet(position);

            var properties = GetTargetProperties(bulletType);
            // Scale with adrenaline slow mo
            float velocity = properties.velocity;
            if (team == Team.Player) velocity *= (BucketsGameManager.instance.focusMode ? adrenalineVelocityScale : 1f);
            string animName = properties.animName;
            Vector2 spriteSize = properties.spriteSize;
            TUFF.SFX hitSFX = GetSFX(bulletType);

            available.Fire(dir, velocity, animName, spriteSize, hitSFX, team);
        }

        private Bullet GetBullet(Vector2 position)
        {
            var available = pool.Find(o => !o.inUse);
            if (available == null) available = AddNew();
            available.transform.position = position;
            return available;
        }
    }
}
