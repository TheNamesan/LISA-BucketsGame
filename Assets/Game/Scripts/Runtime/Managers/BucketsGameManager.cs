using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BucketsGame
{
    public class BucketsGameManager : MonoBehaviour
    {
        public GameObject eventSystem;
        public PlayerInputHandler inputHandler;
        public SFXList sfxs;
        public PrefabList prefabList;
        public bool hitstun = true;
        public int maxFocusTicks = 150;
        public int focusTicks = 0;
        public int hitFocusTicksRegain = 30;
        public bool focusMode = false;
        public int adrenalineCooldown = 20;
        private int m_cooldownTicks = 0;
        public LayerMask groundLayers;
        public LayerMask oneWayLayers;
        public LayerMask hurtboxLayers;
        public int playerLayer = 7;
        public PhysicsMaterial2D aliveMat;
        public PhysicsMaterial2D deadMat;
        private bool m_queuedReset = false;

        public static BucketsGameManager instance { 
            get
            {
                if (!m_instance)
                {
                    TUFF.GameManager.CheckInstance();
                    if (TUFF.GameManager.instance == null) return null;
                    if (!m_instance) AssignInstance(TUFF.GameManager.instance.GetComponentInChildren<BucketsGameManager>());
                    //AssignInstance(Instantiate(Resources.Load<BucketsGameManager>("GameManager")));
                }
                return m_instance;
            }
        }
        private static BucketsGameManager m_instance;
        [SerializeField] private Tween m_hitstunTween;
        private bool m_paused = false;
        private const float SLOWTIME = 0.25f;
        public static bool CheckInstance()
        {
            return instance;
        }
        private void Awake()
        {
            if (m_instance != null)
            {
                if (m_instance != this) {
                    Debug.Log($"Instance is: {m_instance}");
                    Destroy(gameObject); 
                }
            }
            else
            {
                AssignInstance(this);
            }
        }
        private static void AssignInstance(BucketsGameManager go)
        {
            if (go == null) return;
            m_instance = go;
            DOTween.Init();
            Debug.Log("Creating Event System"); if (m_instance.eventSystem) Instantiate(m_instance.eventSystem, m_instance.transform);
            DontDestroyOnLoad(go);
        }
        private void Start()
        {
            ResetGameManager();
        }
        private void FixedUpdate()
        {
            CheckResetQueue();
            FocusTimer();
        }
        private void CheckResetQueue()
        {
            if (m_queuedReset)
            {
                ResetLevel();
            }
            m_queuedReset = false;
        }
        public void Pause(bool pause)
        {
            if (m_paused == pause) return;
            m_paused = pause;
            if (m_paused) UIManager.instance.ShowPauseMenu();
            TUFF.GameManager.instance.DisablePlayerInput(pause);
        }
        public void ToggleFocus(bool enable)
        {
            if (focusMode == enable) return;
            if (focusMode && !enable)
                m_cooldownTicks = adrenalineCooldown;
            focusMode = enable;
            if (enable) TUFF.AudioManager.instance.PlaySFX(SFXList.instance.adrenalineActiveSFX);
            ToggleTimeScale();
        }

        private void ToggleTimeScale()
        {
            if (focusMode) Time.timeScale = SLOWTIME;
            else Time.timeScale = 1f;
        }

        private void FocusTimer()
        {
            var player = SceneProperties.mainPlayer;
            if (player != null)
            {
                if (player.input.focus && focusTicks > 0 && !player.dead && m_cooldownTicks <= 0)
                {
                    ToggleFocus(true);
                    focusTicks--;
                }
                else ToggleFocus(false);
            }
            if (m_cooldownTicks > 0)
                m_cooldownTicks--;
        }
        public float FocusFill()
        {
            return (float)focusTicks / (float)maxFocusTicks;
        }
        public void QueueReset()
        {
            m_queuedReset = true;
        }
        public void ResetLevel()
        {
            GameUtility.KillTween(ref m_hitstunTween);
            ResetGameManager();
            BulletsPool.instance.ResetPool();
            MagicianPatternPool.instance.ResetPool();
            SceneProperties.instance.ResetLevel();
            EntityResetCaller.onResetLevel.Invoke();
        }
        private void ResetGameManager()
        {
            ToggleFocus(false);
            focusTicks = maxFocusTicks;
            m_cooldownTicks = 0;
        }
        private void AddTicks(int value)
        {
            focusTicks += value;
            if (focusTicks > maxFocusTicks) focusTicks = maxFocusTicks;
        }
        public void OnDash()
        {
            if (SceneProperties.instance)
            {
                SceneProperties.instance.camManager.PlayDashOffset();
            }
        }
        public void OnEnemyHit()
        {
            if (SceneProperties.instance)
            {
                float amplitude = 4f;
                SceneProperties.instance.camManager.ShakeCamera(amplitude);
                TUFF.AudioManager.instance.PlaySFX(SFXList.instance.enemyHitSFX);
                PlayHitstun(0.06f);
            }
        }
        public void OnEnemyKill()
        {
            if (SceneProperties.instance)
            {
                float amplitude = 8f;
                SceneProperties.instance.camManager.ShakeCamera(amplitude);
                TUFF.AudioManager.instance.PlaySFX(SFXList.instance.enemyDeadSFX);
                PlayHitstun(0.15f);
            }
            AddTicks(hitFocusTicksRegain);
        }
        
        public void OnPlayerDead()
        {
            if (SceneProperties.instance)
            {
                float amplitude = 10f;
                SceneProperties.instance.camManager.ShakeCamera(amplitude, 1f);
                PlayHitstun(0.25f);
            }
        }
        private void PlayHitstun(float duration)
        {
            if (!hitstun) return;
            GameUtility.KillTween(ref m_hitstunTween);
            m_hitstunTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0f, duration).From(0f)
                .OnKill(ToggleTimeScale)
                .SetUpdate(true);
        }
        private void OnDestroy()
        {
            Debug.Log("Manager destroyed!");
        }
    }
}