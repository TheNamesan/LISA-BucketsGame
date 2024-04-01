using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public enum MagicianPatternType
    {
        Idle = 0,
        Shoot = 1,
        Barrage = 2,
        Floor = 3
    }
    public class Magician : Enemy
    {
        [Header("Magician Properties")]
        public AfterImagesHandler afterImagesHandler;
        public Vector2 teleportPositionPadding = new Vector2();
        public Transform sineMove;
        public Vector2 roomBoundsMin { get => SceneProperties.instance.TUFFSceneProperties.min + teleportPositionPadding; }
        public Vector2 roomBoundsMax { get => SceneProperties.instance.TUFFSceneProperties.max - teleportPositionPadding; }
        public Vector2 normalizedRoomPosition { get
            {
                float normX = Mathf.InverseLerp(roomBoundsMin.x, roomBoundsMax.x, rb.position.x);
                float normY = Mathf.InverseLerp(roomBoundsMin.y, roomBoundsMax.y, rb.position.x);
                return new Vector2(normX, normY);
            }
        }
        public Vector2 roomMiddle { get => Vector2.Lerp(roomBoundsMin, roomBoundsMax, 0.5f); }

        [Header("Dash")]
        public float dashSpeed = 25;
        public int dashTicksDuration = 20;
        [SerializeField] private int m_dashTicks = 0;

        [SerializeField] private int m_dashDirection = 0;
        public bool dashing { get => m_dashTicks > 0; }
        public int dashDirection { get => m_dashDirection; }
        public Color maxDashesColor = new Color(0f, 1f, 0f, 0.5f);
        public Color zeroDashesColor = new Color(1f, 0f, 0f, 0.5f);
        public Color invincibilityColor = new Color(0f, 0f, 1f, 0.5f);

        [Header("Invincibility")]
        public int invincibilityFrames = 150;
        [SerializeField] private int m_invincibilityFramesLeft = 0;
        public bool invincible { get => m_invincibilityFramesLeft > 0; }

        [Header("Dashes")]
        public int dashes = 5;
        [SerializeField] private int m_dashesLeft = 0;

        [Header("Pattern")]
        public MagicianPatternType pattern = MagicianPatternType.Idle;
        public bool attacking { 
            get => 
                (pattern != MagicianPatternType.Idle && (m_activePattern && m_activePattern.inUse) || m_shootTime > 0); 
        }
        public Vector2 barragePivot = new Vector2(-16.9f, 1.2f);
        public Vector2 floorPivot = new Vector2(-16.9f, 1.2f);
        public int timeBetweenAttacks = 60;
        public int patternTimeReductionPerHPLost = 5;
        [SerializeField] private int m_patternTime = 0;
        [SerializeField] private MagicianPattern m_activePattern = null;

        [Header("Shoot")]
        public Color shootTelegraphColor = Color.magenta;
        public int shootTelegraphDuration = 20;
        public int shootPatternDuration = 70;
        [SerializeField] public int m_shootTime = 0;
        public int ShootDuration { get => (BucketsGameManager.IsPainMode() ? painShootPatternDuration : shootPatternDuration); }
        public bool InShootTelegraph { get => ShootDuration - m_shootTime <= shootTelegraphDuration; }

        [Header("Spike")]
        public CircleCollider2D spikeRef;

        [Header("Pain Mode")]
        public int painDashes = 8;
        public int painTimeBetweenAttacks = 45;
        public int painShootPatternDuration = 90;

        public string roomToLoadOnDefeat = "";

        public PlayerController player { get => SceneProperties.mainPlayer; }
        private new void Awake()
        {
            AssignHP();
            AssignPatternTime();
            ResetDashes();
            UpdateAfterImages();
        }

        private void AssignPatternTime()
        {
            int hpLost = maxHP - hp;
            int time = (BucketsGameManager.IsPainMode() ? painTimeBetweenAttacks : timeBetweenAttacks);
            m_patternTime = time - (hpLost * patternTimeReductionPerHPLost);
        }

        private void ResetDashes()
        {
            m_dashesLeft = (BucketsGameManager.IsPainMode() ? painDashes : dashes);
        }

        private void Start()
        {
            PreloadRoomOnDefeat();
            //AddAsRoomEnemy();
        }

        private void PreloadRoomOnDefeat()
        {
            if (!string.IsNullOrEmpty(roomToLoadOnDefeat))
                TUFF.SceneLoaderManager.instance.LoadNeighbourScene(roomToLoadOnDefeat);
        }

        private void Update()
        {
            UpdateAfterImages();
            if (sprite)
            {
                if (pattern == MagicianPatternType.Shoot && InShootTelegraph)
                {
                    float t = Mathf.InverseLerp(ShootDuration, (ShootDuration - shootTelegraphDuration), m_shootTime);
                    sprite.color = Color.Lerp(Color.white, shootTelegraphColor, t);
                }
                if (invincible)
                {
                    bool alternate = ((m_invincibilityFramesLeft - 1) % 8 >= 0 && (m_invincibilityFramesLeft - 1) % 8 <= 3);
                    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, (alternate ? 0.5f : 1f));
                }
                //else sprite.color = Color.white;
                sprite.flipX = facing == Facing.Left;
            }
        }

        private void UpdateAfterImages()
        {
            if (afterImagesHandler)
            {
                float time = (dashes == 0 ? 0f : (float)m_dashesLeft / dashes);
                var color = Color.Lerp(zeroDashesColor, maxDashesColor, time);
                if (invincible) color = invincibilityColor;
                afterImagesHandler.targetColor = color;
            }
        }

        private void OnEnable()
        {
            if (Application.isPlaying) EntityResetCaller.onResetLevel.AddListener(RecolorMagician);
        }
        private void OnDisable()
        {
            if (Application.isPlaying) EntityResetCaller.onResetLevel.RemoveListener(RecolorMagician);
        }
        private void OnDestroy()
        {
            if (Application.isPlaying) EntityResetCaller.onResetLevel.RemoveListener(RecolorMagician);
        }
        private void RecolorMagician()
        {
            if (sprite) sprite.color = Color.white;
        }    
        private void FixedUpdate()
        {
            if (!m_dead) rb.sharedMaterial = BucketsGameManager.instance.aliveMat;
            else rb.sharedMaterial = BucketsGameManager.instance.deadMat;

            GroundCheck();
            WallCheck();
            MoveHandler();
            TimerHandler();
            SpikeHitbox();
        }
        private void SpikeHitbox()
        {
            if (!spikeRef) return;
            if (!BucketsGameManager.IsPainMode())
            { spikeRef.gameObject.SetActive(false); return; }
            spikeRef.gameObject.SetActive(true);
            Vector2 origin = spikeRef.transform.position;
            float radius = spikeRef.radius * spikeRef.transform.localScale.y;
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            var launchDir = rb.velocity.normalized;
            var hits = Physics2D.CircleCastAll(origin, radius, Vector2.up, 0f, hitboxLayers);
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
        private float GetVelX(int moveH)
        {
            if (m_dead) return rb.velocity.x;
            float speed = moveSpeed;
            if (BucketsGameManager.IsPainMode()) speed = painMoveSpeed;
            float velocity = moveH * speed; // Walk Speed
            if (dashing) // Dash Speed
            {
                velocity = Mathf.Lerp(speed, dashSpeed, (float)m_dashTicks / dashTicksDuration) * m_dashDirection;
            }

            return velocity;
        }
        private void MoveHandler()
        {
            if (m_dead) return;
            var player = SceneProperties.mainPlayer;
            if (!player) return;
            float distanceToPlayer = player.rb.position.x - rb.position.x;
            if (!grounded)
            {
                // Floating anim
                float amplitude = 0.5f;
                float value = Mathf.Sin(Time.time * 2f) * amplitude;
                sineMove.transform.localPosition = new Vector2(sineMove.transform.localPosition.x, value);

                int moveH = 0;
                moveH = (int)Mathf.Sign(distanceToPlayer);

                float velX = GetVelX(moveH);
                Vector2 velocity = new Vector2(velX, 0);
                ChangeFacingOnMove(moveH);

                if (dashing)
                {
                    rb.velocity = velocity;
                }
                else
                {
                    float expectedVelocity = rb.velocity.x + (velocity.x * Time.fixedDeltaTime * 2f);
                    if (Mathf.Abs(expectedVelocity) <= Mathf.Abs(velocity.x))
                        rb.AddForce(velocity, ForceMode2D.Force);
                }
            }
        }
        private void DoRandomTeleport()
        {
            if (!SceneProperties.instance) return;
            if (!SceneProperties.instance.TUFFSceneProperties) return;

            float randomX = Random.Range(roomBoundsMin.x, roomBoundsMax.x);
            float randomY = Random.Range(roomBoundsMin.y, roomBoundsMax.y);
            var newPosition = new Vector2(randomX, randomY);
            rb.position = newPosition;
        }
        private void SetInvincible()
        {
            m_invincibilityFramesLeft = invincibilityFrames;
        }
        private void Dash()
        {
            m_dashTicks = dashTicksDuration;
            m_dashDirection = (normalizedRoomPosition.x >= 0.5f ? -1 : 1);//FaceToInt();
            m_dashesLeft--;
            if (m_dashesLeft < 0) m_dashesLeft = 0;
            TUFF.AudioManager.instance.PlaySFX(SFXList.instance.dashSFX);
        }
        private void StopDash()
        {
            m_dashTicks = 0;
            m_dashDirection = 0;
        } 
       
        private void TimerHandler()
        {
            PatternTimer();
            ShootTimer();
            DashTimer();
            InvincibleTimer();
        }
        private void PatternTimer()
        {
            if (invincible) return;
            if (attacking) return;
            m_patternTime--;
            if (m_patternTime <= 0)
            {
                if (pattern != MagicianPatternType.Idle)
                {
                    pattern = MagicianPatternType.Idle;
                    AssignPatternTime();
                }
                else
                {
                    DoRandomAttack();
                }
            }
        }
        private void DoRandomAttack()
        {
            var player = SceneProperties.mainPlayer;
            if (!player) return;
            if (player.dead) return;
            if (m_dead) return;
            var random = (MagicianPatternType)Random.Range(1, 4);
            pattern = random;
            switch (random)
            {
                case MagicianPatternType.Shoot:
                    m_shootTime = ShootDuration;
                    TUFF.AudioManager.instance.PlaySFX(SFXList.instance.magicianBarrageSFX);
                    break;
                case MagicianPatternType.Barrage:
                    var barrageDir = (BarrageDirection)Random.Range(0, 2);
                    m_activePattern = MagicianPatternPool.instance.InvokeBarrage(barragePivot, barrageDir);
                    break;
                case MagicianPatternType.Floor:
                    m_activePattern = MagicianPatternPool.instance.InvokeFloor(floorPivot);
                    break;
            }
            
        }
        private void ShootTimer()
        {
            if (pattern != MagicianPatternType.Shoot) return;
            if (m_shootTime <= 0) return;
            if (m_shootTime == ShootDuration - shootTelegraphDuration) 
                sprite.color = Color.white;
            m_shootTime--;
            if (!InShootTelegraph) ShootProjectile();
        }
        private void ShootProjectile()
        {
            var player = SceneProperties.mainPlayer;
            if (!player) return;
            Vector2 dir = player.rb.position - rb.position;
            Vector2 size = Vector2.one;
            Vector2 offset = dir.normalized * 0.5f;
            BulletsPool.instance.SpawnBullet(rb.position + offset, dir, BulletType.Magician, Team.Enemy);
        }
        private void DashTimer()
        {
            if (!dashing) return;
            m_dashTicks--;
            if (m_dashTicks < 0) StopDash();
        }
        private void InvincibleTimer()
        {
            if (!invincible) return;
            m_invincibilityFramesLeft--;
            if (m_invincibilityFramesLeft <= 0)
            { RecolorMagician(); m_invincibilityFramesLeft = 0; }
        }
        public override bool Hurt(Vector2 launch)
        {
            var player = SceneProperties.mainPlayer;
            if (player && player.dead) return false;
            if (m_dead) return false;
            //DoRandomTeleport();
            if (invincible) return false;
            if (!dashing)
            { 
                if (m_dashesLeft > 0) Dash();
                else
                {
                    CancelAttacks();
                    hp--;
                    if (hp > 0)
                    {
                        sprite.color = Color.red;
                        HurtTween(); BucketsGameManager.instance.OnMagicianHit();
                        SetInvincible(); ResetDashes(); return true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(roomToLoadOnDefeat))
                        TUFF.SceneLoaderManager.instance.LoadScene(roomToLoadOnDefeat, 
                            Vector2.zero, TUFF.FaceDirections.East, true, false, false);
                        return Kill(launch);
                    }
                }
            }
            return false;
        }
        public override bool TryKill(Vector2 launch)
        {
            return Hurt(launch);
        }

        private void CancelAttacks()
        {
            pattern = MagicianPatternType.Idle;
            m_shootTime = 0;
            AssignPatternTime();
            MagicianPatternPool.instance.ResetPool();
        }
    }
}
