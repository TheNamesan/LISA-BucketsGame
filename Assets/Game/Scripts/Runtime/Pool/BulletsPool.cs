using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public enum BulletType
    {
        Normal = 0,
        Spear = 1,
        Bottle = 2,
        Magician = 3,
        Firebomb = 4
    }
    [System.Serializable]
    public struct BulletProperties
    {
        public float velocity;
        public float painModeVelocity;
        public string animName;
        public Vector2 spriteSize;
        public BulletProperties(float velocity, float painModeVelocity, string animName, Vector2 spriteSize)
        {
            this.velocity = velocity;
            this.painModeVelocity = painModeVelocity;
            this.animName = animName;
            this.spriteSize = spriteSize;
        }
    }
    public class BulletsPool : PoolManager<Bullet>
    {
        public static BulletsPool instance;
        [SerializeField] private BulletProperties m_normalProperties = new(30f, 30f, "NormalBullet", new Vector2(0.3f, 0.3f));
        [SerializeField] private BulletProperties m_spearProperties = new(30f, 35f, "SpearBullet", new Vector2(1f, 1f));
        [SerializeField] private BulletProperties m_bottleProperties = new(30f, 40f, "BottleBullet", new Vector2(1f, 1f));
        [SerializeField] private BulletProperties m_magicianProperties = new(30f, 35f, "MagicianBullet", new Vector2(1f, 1f));
        [SerializeField] private BulletProperties m_firebombProperties = new(30f, 40f, "BottleBullet", new Vector2(1f, 1f));
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
                case BulletType.Firebomb: return m_firebombProperties;
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
                case BulletType.Firebomb: return SFXList.instance.flyerShootHitSFX;
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
            if (BucketsGameManager.IsPainMode()) velocity = properties.painModeVelocity;
            if (team == Team.Player) velocity *= (BucketsGameManager.instance.focusMode ? adrenalineVelocityScale : 1f);
            string animName = properties.animName;
            Vector2 spriteSize = properties.spriteSize;
            TUFF.SFX hitSFX = GetSFX(bulletType);

            available.Fire(dir, velocity, animName, spriteSize, hitSFX, bulletType, team);
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
