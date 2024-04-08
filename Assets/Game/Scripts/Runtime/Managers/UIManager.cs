using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


namespace BucketsGame
{
    public class UIManager : MonoBehaviour
    {
        [Header("Pause")]
        [SerializeField] private TUFF.UIMenu pauseMenu;

        [Header("Cursor")]
        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private GameObject content;
        private Vector2 cursorHotspot;

        [SerializeField] private Image m_aimIndicator;
        [SerializeField] private Image m_nextRoomIndicator;

        [SerializeField] private TMP_Text retryText;
        [Header("Bar")]
        public Image focusFill;
        public ImageAnimator focusAnim;
        public ImageAnimator eyeAnim;
        private PlayerController Player { get => SceneProperties.mainPlayer; }
        private float m_timePassed = 0;

        [Header("Timer")]
        public TMP_Text timerText;

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
            //if (Application.isPlaying) TUFF.SceneLoaderManager.onSceneChanged.AddListener(UpdateContent);
        }
        private void OnDisable()
        {
            ScrollBarEffect(0);
            //if (Application.isPlaying) TUFF.SceneLoaderManager.onSceneChanged.RemoveListener(UpdateContent);
        }
        private void ScrollBarEffect(float newTime)
        {
            m_timePassed = newTime;
            if (focusFill && focusFill.material != null) focusFill.material.SetFloat("_PassTime", m_timePassed);
        }

        private void Update()
        {
            UpdateBar();
            UpdateAimIndicator();
            UpdateRoomIndicator();
            UpdateTimer();
            UpdateContent();
            if (SceneProperties.mainPlayer)
            {
                retryText?.gameObject.SetActive(SceneProperties.mainPlayer.dead);
                if (PlayerInputHandler.instance)
                    retryText.text = $"Press {PlayerInputHandler.instance.GetCurrentResetKeyText()} to retry";
            }
                
        }
        private void LateUpdate()
        {
            //UpdateContent();
        }

        private void UpdateContent()
        {
            if (content)
            {
                if (SceneManager.GetActiveScene().name == "TitleScreen")
                {
                    content.SetActive(false);
                    if (timerText) timerText.gameObject.SetActive(false);
                    return;
                }
                bool showContent = (!TUFF.CommonEventManager.interactableEventPlaying) 
                    //&& !TUFF.GameManager.disablePlayerInput // Removed cuz it hides the pause menu lol
                    //&& !TUFF.SceneLoaderManager.loading
                    && SceneProperties.mainPlayer;
                //if (!showContent) contentDisplayBuffer = 1;
                //if (showContent && contentDisplayBuffer > 0)
                    //{ showContent = false; contentDisplayBuffer--; }
                content.SetActive(showContent);
                
                timerText.gameObject.SetActive(TUFF.GameManager.instance.configData.bucketsTimer);
            }
        }

        private void UpdateAimIndicator()
        {

            if (!m_aimIndicator) return;
            m_aimIndicator.gameObject.SetActive(false);
            if (!SceneProperties.instance) return;
            var player = SceneProperties.instance.player;
            if (!player) return;
            Vector2 inputAim = player.input.aim.normalized;
            if (!player.input.InAimThreshold(inputAim)) return;
            m_aimIndicator.gameObject.SetActive(true);
            Vector2 worldPosition = (Vector2)player.transform.position + inputAim * 2f;
            Vector2 canvasPosition = SceneProperties.cam.WorldToScreenPoint(worldPosition);

            float angle = Vector2.SignedAngle(Vector2.right, inputAim);
            var rotation = Quaternion.Euler(0, 0, angle);
            m_aimIndicator.rectTransform.rotation = rotation;

            m_aimIndicator.rectTransform.position = canvasPosition;
        }
        private void UpdateRoomIndicator()
        {
            if (!m_nextRoomIndicator) return;
            if (!SceneProperties.instance) return;
            if (!SceneProperties.instance.nextRoomCheck)
            {
                m_nextRoomIndicator.gameObject.SetActive(false);
                return;
            }
            m_nextRoomIndicator.gameObject.SetActive(SceneProperties.instance.nextRoomAvailable);
            if (!m_nextRoomIndicator) return;
            var nextRoomDoor = SceneProperties.instance.nextRoomCheck;
            if (!nextRoomDoor) return;
            var worldPosition = nextRoomDoor.transform.position;
            Vector2 canvasPosition = SceneProperties.cam.WorldToScreenPoint(worldPosition);
            Vector2 screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
            // Clamp Position to be on the screen
            Vector2 padding = new Vector2(100f, 100f) + m_nextRoomIndicator.rectTransform.sizeDelta;
            //Debug.Log(padding);
            Vector2 min = new Vector2(0, 0) + padding;
            Vector2 max = new Vector2(Screen.width, Screen.height) - padding;
            
            canvasPosition = new Vector2(Mathf.Clamp(canvasPosition.x, min.x, max.x), Mathf.Clamp(canvasPosition.y, min.y, max.y));
            Debug.DrawLine(screenCenter, canvasPosition, Color.white);
            if (true)
            {
                Vector2 normal = (canvasPosition - screenCenter).normalized;
                float angle = Vector2.SignedAngle(normal, Vector2.right);
                var rotation = Quaternion.Euler(0, 0, -angle);
                m_nextRoomIndicator.rectTransform.rotation = rotation;
            }
            m_nextRoomIndicator.rectTransform.position = canvasPosition;
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
        private void UpdateTimer()
        {
            if (!timerText) return;
            System.TimeSpan timeSpan = TimerManager.instance.timeSpanElapsed;
            string hours = (timeSpan.Hours).ToString("00", System.Globalization.CultureInfo.InvariantCulture);
            string minutes = (timeSpan.Minutes).ToString("00", System.Globalization.CultureInfo.InvariantCulture);
            string seconds = (timeSpan.Seconds).ToString("00", System.Globalization.CultureInfo.InvariantCulture);
            string milliseconds = (timeSpan.Milliseconds).ToString("00", System.Globalization.CultureInfo.InvariantCulture).Substring(0, 2);
            timerText.text = $"{hours}:{minutes}:{seconds}.{milliseconds}";
        }
        public void ShowPauseMenu()
        {
            if (pauseMenu) pauseMenu.OpenMenu();
        }
    }
}
