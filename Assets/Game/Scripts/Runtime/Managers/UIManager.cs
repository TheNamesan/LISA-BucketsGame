using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BucketsGame
{
    public class UIManager : MonoBehaviour
    {
        [Header("Cursor")]
        [SerializeField] private Texture2D cursorTexture;
        private Vector2 cursorHotspot;

        [Header("Bar")]
        public Image focusFill;
        public ImageAnimator focusAnim;
        public ImageAnimator eyeAnim;
        private PlayerController Player { get => SceneProperties.mainPlayer; }
        private float m_timePassed = 0;

        public static UIManager instance { get { if (m_instance == null) AssignInstance(null); return m_instance; } }
        private static UIManager m_instance;

        private void Awake()
        {
            if (m_instance == null) AssignInstance(this);
            else Destroy(gameObject);
        }
        public static void AssignInstance(UIManager target)
        {
            if (target == null) return;
            m_instance = target;
            DontDestroyOnLoad(m_instance);
        }
        private void OnEnable()
        {
            
            if (focusFill && focusFill.material != null)
            {
                focusFill.material = Instantiate(focusFill.material); // Copy of material
            }
            ScrollBarEffect(0);
            if (cursorTexture) cursorHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
            Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
        }
        private void OnDisable()
        {
            ScrollBarEffect(0);
        }
        private void ScrollBarEffect(float newTime)
        {
            m_timePassed = newTime;
            if (focusFill && focusFill.material != null) focusFill.material.SetFloat("_PassTime", m_timePassed);
        }

        private void Update()
        {
            UpdateBar();
        }
        private void UpdateBar()
        {
            var focus = BucketsGameManager.instance.focusMode;
            if (focusFill)
            {
                focusFill.fillAmount = BucketsGameManager.instance.FocusFill();
                if (focus && focusFill.material != null)
                {
                    ScrollBarEffect(m_timePassed + Time.unscaledDeltaTime);
                }
            }
            if (focusAnim)
            {
                focusAnim.Play(focus);
            }
            if (eyeAnim) eyeAnim.Play(focus);
        }
    }
}
