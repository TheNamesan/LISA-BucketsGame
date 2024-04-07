using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BucketsGame
{
    public class ImageAnimator : MonoBehaviour
    {
        public Image image;
        public bool playOnEnable = false;
        public bool unscaledTime = false;
        public bool returnToOriginalOnStop = false;

        [Header("Loop")]
        public float timeDelay = 0.1f;
        public List<Sprite> frames = new();
        private IEnumerator m_loopCoroutine = null;
        private int m_frame = 0;
        private Sprite m_original = null;
        private bool m_running = false;

        [Header("Transition")]
        public bool useEndTransition = false;
        public float endTimeDelay = 0.1f;
        public List<Sprite> endFrames = new();
        private int m_endFrame = 0;
        private bool m_endRunning = false;
        private IEnumerator m_transitionCoroutine = null;

        private void Awake()
        {
            if (!image) image = GetComponent<Image>();
            if (image) m_original = image.sprite;
        }
        private void OnEnable()
        {
            if (image) image.sprite = m_original;
            if (playOnEnable) Play(true);
            if (Application.isPlaying)
            {
                EntityResetCaller.onResetLevel.AddListener(Stop);
                TUFF.SceneLoaderManager.onSceneChanged.AddListener(Stop);
                TUFF.GameManager.instance.onPlayerInputToggle.AddListener(Stop);
            }
        }
        public void Play(bool play)
        {
            if (!gameObject.activeInHierarchy)
            { m_running = false; return; }
            if (play)
            {
                if (m_running) return;
                m_running = true;
                if (m_transitionCoroutine != null)
                {
                    m_endRunning = false;
                    StopCoroutine(m_transitionCoroutine);
                    m_transitionCoroutine = null;
                }
                //if (gameObject.activeInHierarchy)
                {
                    if (m_loopCoroutine == null) m_loopCoroutine = AnimationLoop();
                    StartCoroutine(m_loopCoroutine);
                }
            }
            else
            {
                if (!m_running) return;

                if (m_loopCoroutine != null)
                { 
                    StopCoroutine(m_loopCoroutine);
                    m_loopCoroutine = null;
                }
                if (m_running && !m_endRunning && useEndTransition)
                {
                    m_endFrame = 0;
                    m_endRunning = true;

                    if (m_transitionCoroutine == null) m_transitionCoroutine = EndTransition();
                    StartCoroutine(m_transitionCoroutine);
                    
                }
                //else if (returnToOriginalOnStop && image)
                //    image.sprite = m_original;
                m_running = false;
            }
        }
        public IEnumerator AnimationLoop()
        {
            while (true)
            {
                ChangeFrame(ref m_frame, frames);
                if (unscaledTime) yield return new WaitForSecondsRealtime(timeDelay);
                else yield return new WaitForSeconds(timeDelay);
                m_frame++;
            }
        }
        public IEnumerator EndTransition()
        {
            while (m_endFrame < endFrames.Count)
            {
                ChangeFrame(ref m_endFrame, endFrames);
                if (unscaledTime) yield return new WaitForSecondsRealtime(endTimeDelay);
                else yield return new WaitForSeconds(endTimeDelay);
                m_endFrame++;
            }
            m_endRunning = false;
        }
        public void ChangeFrame(ref int target, List<Sprite> frames)
        {
            if (frames == null || frames.Count <= 0) return;
            if (target < 0 || target >= frames.Count) target = 0;
            if (image) image.sprite = frames[target];
        }
        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                EntityResetCaller.onResetLevel.RemoveListener(Stop);
                TUFF.SceneLoaderManager.onSceneChanged.RemoveListener(Stop);
                TUFF.GameManager.instance.onPlayerInputToggle.RemoveListener(Stop);
            }
        }
        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                EntityResetCaller.onResetLevel.RemoveListener(Stop);
                TUFF.SceneLoaderManager.onSceneChanged.RemoveListener(Stop);
                TUFF.GameManager.instance.onPlayerInputToggle.RemoveListener(Stop);
            }
        }
        private void Stop()
        {
            Play(false);
            StopAllCoroutines();
            if (image) image.sprite = m_original;
        }
        private void Stop(bool enabledInput)
        {
            Stop();
        }
    }
}
