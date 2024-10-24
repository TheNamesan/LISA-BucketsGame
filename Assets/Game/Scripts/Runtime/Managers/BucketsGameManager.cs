using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BucketsGame
{
    public class BucketsGameManager : MonoBehaviour
    {
        public GameObject eventSystem;
        public BucketsPlayerInputHandler inputHandler;
        public SFXList sfxs;
        public PrefabList prefabList;
        public TimerManager timerManager;
        public bool newGame { get => m_newGame; }
        private bool m_newGame = false;
        public bool forcePainMode = false;
        public bool forceRusherMode = false;
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
        private bool m_holdingSkip = false;
        private bool m_toggleFocus = false;

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
        public bool paused { get => m_paused; }
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
        public void OnEnable()
        {
            if (Application.isPlaying) { 
                TUFF.SceneLoaderManager.onSceneLoad.AddListener(ResetGameManager); 
                TUFF.GameManager.instance.onPlayerInputToggle.AddListener(CancelToggleFocus); 
            }
            StartCoroutine(LateFixedUpdate());
        }
        public void OnDisable()
        {
            if (Application.isPlaying) { 
                TUFF.SceneLoaderManager.onSceneLoad.RemoveListener(ResetGameManager);
                TUFF.GameManager.instance.onPlayerInputToggle.RemoveListener(CancelToggleFocus);
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
            FastForwardEvents();
            FocusTimer();
        }

        private void FastForwardEvents()
        {
            if (TUFF.CommonEventManager.interactableEventPlaying)
            {
                if (TUFF.UIController.instance.skipButtonHold)
                {
                    Time.timeScale = 10f;
                    m_holdingSkip = true;
                }
                else
                {
                    Time.timeScale = 1f;
                    m_holdingSkip = false;
                }
            }
            else if (m_holdingSkip)
            {
                Time.timeScale = 1f;
                m_holdingSkip = false;
            }
        }

        private IEnumerator LateFixedUpdate()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                CheckResetQueue();
            }
        }
        
        private void CheckResetQueue()
        {
            if (m_queuedReset)
            {
                if (SceneProperties.instance && !SceneProperties.instance.disableRoomReset)
                    ResetLevel();
            }
            m_queuedReset = false;
        }
        public void Pause(bool pause)
        {
            if (m_paused == pause) return;
            m_toggleFocus = false;
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
            else m_toggleFocus = false;
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
            if (SceneProperties.instance && SceneProperties.instance.cutsceneMode)
            {
                ToggleFocus(false);
                m_cooldownTicks = 0;
                return;
            }

            if (player != null)
            {
                bool activeFocusInput = player.input.focus;
                if (TUFF.ConfigData.instance.bucketsToggleSlowmo)
                {
                    if (player.input.focusDown) m_toggleFocus = !m_toggleFocus;
                    activeFocusInput = m_toggleFocus;
                }
                else m_toggleFocus = false;
                if (activeFocusInput && focusTicks > 0 && !player.dead)
                {
                    ToggleFocus(true);
                    focusTicks--;
                }
                else ToggleFocus(false);
            }
            else m_toggleFocus = false;
            

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
        private void ResetLevel()
        {
            Debug.Log("Resetting");
            GameUtility.KillTween(ref m_hitstunTween);
            ResetGameManager();
            SceneProperties.instance.ResetLevel();
            EntityResetCaller.onResetLevel.Invoke();
        }

        private static void ResetPools()
        {
            VFXPool.instance.ResetPool();
            MovingPropPool.instance.ResetPool();
            AfterImagesPool.instance.ResetPool();
            BulletsPool.instance.ResetPool();
            MagicianPatternPool.instance.ResetPool();
        }

        private void ResetGameManager()
        {
            ToggleFocus(false);
            focusTicks = maxFocusTicks;
            m_cooldownTicks = 0;
            m_toggleFocus = false;
            if (SceneProperties.instance) SceneProperties.instance.roomCleared = false;
            ResetPools();
        }
        private void CancelToggleFocus(bool toggle)
        {
            m_toggleFocus = false;
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
                if (SceneProperties.instance.nextRoomAvailable && SceneProperties.instance.nextRoomCheck
                    && !SceneProperties.instance.roomCleared)
                {
                    TUFF.AudioManager.instance.PlaySFX(SFXList.instance.roomClearedSFX);
                    SceneProperties.instance.roomCleared = true;
                }
            }
            AddTicks(hitFocusTicksRegain);
            
        }
        public void OnMagicianHit()
        {
            if (SceneProperties.instance)
            {
                float amplitude = 10f;
                SceneProperties.instance.camManager.ShakeCamera(amplitude, 1f);
                TUFF.AudioManager.instance.PlaySFX(SFXList.instance.enemyHitSFX);
                PlayHitstun(0.35f);
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
            if (Application.isPlaying) { 
                TUFF.SceneLoaderManager.onSceneLoad.RemoveListener(ResetGameManager);
                TUFF.GameManager.instance.onPlayerInputToggle.RemoveListener(CancelToggleFocus);
            }
            Debug.Log("Manager destroyed!");
        }
        public static bool IsPainMode()
        {
            if (Application.isEditor && instance.forcePainMode) return true;
            return TUFF.GameManager.instance.configData.bucketsPainMode;
        }
        public static PlayableCharacters GetCharacter()
        {
            if (TUFF.GameManager.instance.configData.bucketsRusherMode || (Application.isEditor && instance.forceRusherMode))
                return PlayableCharacters.Rusher;
            return PlayableCharacters.Buckets;
        }
        public static bool IsRusher()
        {
            return GetCharacter() == PlayableCharacters.Rusher;
        }
        public void SetNewGame(bool isNewGame)
        {
            m_newGame = isNewGame;
            if (isNewGame) Debug.Log("Marked New Game");
        }
    }
}