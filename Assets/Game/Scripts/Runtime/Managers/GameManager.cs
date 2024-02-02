using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BucketsGame
{
    public class GameManager : MonoBehaviour
    {
        public bool hitstun = true;
        public int maxFocusTicks = 150;
        public int focusTicks = 0;
        public int hitFocusTicksRegain = 30;
        public bool focusMode = false;
        public LayerMask groundLayers;
        public LayerMask oneWayLayers;
        public LayerMask hurtboxLayers;
        public int playerLayer = 7;
        public PhysicsMaterial2D aliveMat;
        public PhysicsMaterial2D deadMat;
        public static GameManager instance;
        [SerializeField] private Tween m_hitstunTween;
        private const float SLOWTIME = 0.25f;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DOTween.Init();
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }
        private void Start()
        {
            focusTicks = maxFocusTicks;
        }
        private void FixedUpdate()
        {
            FocusTimer();
        }
        public void ToggleFocus(bool enable)
        {
            if (focusMode == enable) return;
            focusMode = enable;
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
                if (player.input.focus && focusTicks > 0 && !player.dead)
                {
                    ToggleFocus(true);
                    focusTicks--;
                }
                else ToggleFocus(false);
            }
        }
        public float FocusFill()
        {
            return (float)focusTicks / (float)maxFocusTicks;
        }
        public void ResetLevel()
        {
            GameUtility.KillTween(ref m_hitstunTween);
            focusTicks = maxFocusTicks;
            BulletsPool.instance.ResetPool();
            SceneProperties.instance.ResetLevel();
            EntityResetCaller.onResetLevel.Invoke();
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

        public void OnEnemyKill()
        {
            if (SceneProperties.instance)
            {
                float amplitude = 8f;
                SceneProperties.instance.camManager.ShakeCamera(amplitude);
                PlayHitstun(0.1f);
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
    }
}