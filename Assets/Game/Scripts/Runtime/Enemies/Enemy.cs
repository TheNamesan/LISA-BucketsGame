using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BucketsGame
{
    public enum EnemyAIState
    {
        Roaming = 0,
        Alert = 1,
        StandBy = 2
    }
    public class Enemy : MovingEntity
    {
        public EnemyAIState enemyState = EnemyAIState.Roaming;

        public Tween hurtTween;

        [Header("Line Of Sight")]
        public float coneAngle = 45f;
        public float coneAngleOffset = 0f;
        public float coneDistance = 8.5f;
        public int coneAccuracy = 12;
        public float nearbyEnemyAlertRadius = 3.5f;

        [Header("Pain Mode")]
        public int painMaxHP = 2;
        public float painMoveSpeed = 12;

        public int doorFadeTicks = 0;
        private int m_doorFadeDuration = 25;
        public bool InDoorFade { get => doorFadeTicks > 0; }
        public EnemyWallDoor targetWallDoor = null;
        
        

        protected EnemyAIState m_originalState = EnemyAIState.Roaming;
        protected void Awake()
        {
            AssignHP();
            SaveOriginalState();
            sprite.flipX = facing == Facing.Left;
        }

        protected override void AssignHP()
        {
            hp = (BucketsGameManager.IsPainMode() ? painMaxHP : maxHP);
        }
        protected void SaveOriginalState()
        {
            m_originalState = enemyState;
        }
        protected void AssignOriginalState()
        {
            var state = m_originalState;
            if (state == EnemyAIState.Alert) state = EnemyAIState.StandBy;
            enemyState = state;
        }

        private void Start()
        {
            //AddAsRoomEnemy();
        }
        public void AddAsRoomEnemy()
        {
            if (SceneProperties.instance)
            {
                SceneProperties.instance.AddRoomEnemy(this);
            }
        }
        private void Update()
        {
            if (sprite)
            {
                if (m_dead)
                {
                    sprite.color = Color.red;
                    rb.constraints = RigidbodyConstraints2D.None;
                }
                else rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }    
        }
        private void FixedUpdate()
        {
            if (!m_dead) rb.sharedMaterial = BucketsGameManager.instance.aliveMat;
            else rb.sharedMaterial = BucketsGameManager.instance.deadMat;
            GroundCheck();
            MoveHandler();
            FallOffMapCheck();
        }
        private void MoveHandler()
        {
            if (m_dead) return;
            if (grounded)
            {
                if (SceneProperties.mainPlayer != null)
                {
                    var player = SceneProperties.mainPlayer;
                    float distanceToPlayer = player.rb.position.x - rb.position.x;
                    int moveH = 0;
                    
                    if (Mathf.Abs(distanceToPlayer) < 4f) enemyState = EnemyAIState.Alert;
                    if (enemyState == EnemyAIState.Alert) moveH = (int)Mathf.Sign(distanceToPlayer);
                    float velX = moveH * moveSpeed;
                    Vector2 velocity = new Vector2(velX, 0);
                    Vector2 normal = groundNormal;
                    velocity = GetSlopeVelocity(moveH, velX, velocity, normal);
                    rb.velocity = velocity;
                    ChangeFacingOnMove(moveH);
                }
            }
            CapVelocity();
        }
        public void FacePlayer()
        {
            if (SceneProperties.mainPlayer)
            {
                float distanceToPlayerX = SceneProperties.mainPlayer.rb.position.x - rb.position.x;
                ChangeFacingOnMove((int)Mathf.Sign(distanceToPlayerX));
            }
        }
        public override bool Hurt(Vector2 launch)
        {
            if (m_dead) return false;
            hp--;
            AlertEnemy();
            HurtTween();
            if (hp > 0) {  BucketsGameManager.instance.OnEnemyHit(); return true; }
            return Kill(launch);
        }
        protected void HurtTween()
        {
            GameUtility.KillTween(ref hurtTween);
            hurtTween = sprite.DOColor(Color.white, 0.25f).From(Color.red);
        }
        public override bool TryKill(Vector2 launch)
        {
            bool killed = Kill(launch);
            if (killed) HurtTween();
            return killed;
        }
        public override bool Kill(Vector2 launch)
        {
            if (m_dead) return false;
            m_dead = true;
            SetAirborne();
            launch *= 40f;
            rb.velocity = (launch);
            BucketsGameManager.instance.OnEnemyKill();
            return true;
        }
        protected virtual void EnemyLineOfSight()
        {
            LayerMask layers = BucketsGameManager.instance.groundLayers | (1 << BucketsGameManager.instance.playerLayer);

            int max = coneAccuracy + 1;
            for (int i = 0; i < max; i++)
            {
                float a = -coneAngle * 0.5f;
                float angle = -coneAngleOffset + Mathf.Lerp(-a, a, Mathf.InverseLerp(0, max, i));
                float rad = Mathf.Deg2Rad * angle;
                Vector2 normal = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                Vector2 dir = new Vector2(normal.x * FaceToInt(), normal.y);
                float distance = coneDistance;
                RaycastHit2D[] losHits = Physics2D.RaycastAll(rb.position, dir, distance, layers);
                Color color = Color.white;
                for (int j = 0; j < losHits.Length; j++)
                {
                    var los = losHits[j];
                    if (los.collider.gameObject.layer == 6)
                    {
                        if (los.collider.TryGetComponent(out TUFF.TerrainProperties props))
                            if (props.enemyBulletsGoThrough) continue;
                        break;
                    }
                    else if (los.collider.gameObject.layer == BucketsGameManager.instance.playerLayer)
                    {
                        color = Color.green;
                        Debug.Log($"[{gameObject.name}] Found enemy on it: " + i + $". Pos: {SceneProperties.mainPlayer.transform.position}");
                        AlertEnemy();
                        return;
                    }
                }
                //Debug.DrawRay(rb.position, dir.normalized * distance, color, Time.fixedDeltaTime);
            }
        }
        public bool CheckIfDoorIsFaster(PlayerController player, float distanceToPlayer, out EnemyWallDoor nearest, bool enterIfClose = true, bool ignoreOneWayWalkables = true, bool ignoreGoThroughWalls = false)
        {
            nearest = null;
            if (InDoorFade) return false;
            nearest = EnemyWallDoor.FindNearestWallDoorWithLoS(rb.position, ignoreOneWayWalkables, ignoreGoThroughWalls);
            float distanceToDoor = 99999f;
            if (!nearest) return false;
            distanceToDoor = Vector2.Distance(rb.position, nearest.transform.position); //Mathf.Abs(rb.position.x - nearest.transform.position.x); }
            bool isCloserWithDoor = nearest && nearest.GetNeighbourDistance() + distanceToDoor < Mathf.Abs(distanceToPlayer);

            if (!HasDirectLoSWithPlayer(rb.position, player.rb.position, ignoreOneWayWalkables, ignoreGoThroughWalls)
                || isCloserWithDoor)
            {
                //color = Color.red;
                if (nearest)
                {
                    // If already at the door
                    var hit = Physics2D.BoxCast(rb.position, col.size, 0f, Vector3.down, 0f, (1 << 16));
                    if (enterIfClose && hit && hit.collider == nearest.col)
                    {
                        EnterWallDoor(nearest);
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public void EnterWallDoor(EnemyWallDoor door)
        {
            if (!door) return;
            doorFadeTicks = m_doorFadeDuration;
            targetWallDoor = door;
            door?.PlayEnterSFX();
        }
        protected void WallDoorTransitionTimer()
        {
            if (InDoorFade && targetWallDoor)
            {
                doorFadeTicks--;
                int half = m_doorFadeDuration / 2;
                float normalizedTime = (doorFadeTicks / (float)m_doorFadeDuration);
                if (doorFadeTicks == half)
                {
                    targetWallDoor?.TeleportToNeighbour(this);
                }
                if (sprite)
                {
                    float a = 1 - Mathf.Sin(normalizedTime * Mathf.PI);
                    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, a);
                }
                if (doorFadeTicks <= 0) targetWallDoor = null;
            }
        }
        protected virtual void AlertEnemy()
        {
            enemyState = EnemyAIState.Alert;
            AlertNearbyEnemies();
        }
        protected virtual void CheckDoorOpening(int moveH)
        {
            if (enemyState != EnemyAIState.Alert) return;
            Door targetDoor = null;
            int openDir = 0;
            if ((moveH > 0) && (IsVerticalWall(wallRightHit)))
            {
                if (wallRightHit.collider.TryGetComponent(out Door door)) { openDir = 1; targetDoor = door; }

            }
            else if ((moveH < 0) && IsVerticalWall(wallLeftHit))
            {
                if (wallLeftHit.collider.TryGetComponent(out Door door)) { openDir = -1; targetDoor = door; }
            }
            if (targetDoor) targetDoor.Open(openDir);
        }
        protected virtual void AlertNearbyEnemies()
        {
            var position = rb.position;
            var radius = nearbyEnemyAlertRadius;
            var hitboxLayers = BucketsGameManager.instance.hurtboxLayers;
            RaycastHit2D[] alert = Physics2D.CircleCastAll(position, radius, Vector2.zero, 0f, hitboxLayers);
            Debug.Log($"Enemies found: {alert.Length}");
            for (int i = 0; i < alert.Length; i++)
            {
                if (alert[i].collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (!hurtbox.callback) continue; // If hitbox has no callback, abort
                    Debug.Log(hurtbox.callback.gameObject.name);
                    Enemy enemy = hurtbox.callback as Enemy; 
                    if (!enemy) continue; // If hitbox is not an enemy, abort
                    if (enemy == this) continue; // If hitbox is this, abort
                    if (enemy.enemyState == EnemyAIState.Alert) continue; // If enemy is already alerted, abort
                    
                    enemy.AlertEnemy();
                }
            }
        }
        protected virtual void DrawLineOfSightGizmos()
        {
            int max = coneAccuracy + 1;
            for (int i = 0; i < max; i++)
            {
                float a = -coneAngle * 0.5f;
                float angle = -coneAngleOffset + Mathf.Lerp(-a, a, Mathf.InverseLerp(0, max, i));
                float rad = Mathf.Deg2Rad * angle;
                Vector2 normal = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

                Vector2 dir = new Vector2(normal.x * FaceToInt(), normal.y);
                float distance = coneDistance;
                if (rb) Gizmos.DrawRay(rb.position, dir.normalized * distance);
            }
        }
    }
}
