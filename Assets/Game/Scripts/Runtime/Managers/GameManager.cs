using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class GameManager : MonoBehaviour
    {
        public int maxFocusTicks = 150;
        public int focusTicks = 0;
        public bool focusMode = false;
        public LayerMask groundLayers;
        public LayerMask hurtboxLayers;
        public static GameManager instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
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
            if (focusMode) Time.timeScale = 0.25f;
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
    }
}