using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

namespace TUFF
{
    public class UIController : MonoBehaviour
    {
        [Header("References")]
        public GameObject eventSystem;
        public Canvas cameraCanvas;
        public Canvas overlayCanvas;
        public GameObject uiContent;
        public string cameraCanvasSortingLayerName = "UI";
        [SerializeField]
        TextMeshProUGUI dosh;
        [SerializeField]
        List<RectTransform> HP;
        [SerializeField]
        List<RectTransform> SP;
        [SerializeField]
        List<TextMeshProUGUI> Names;
        [SerializeField]
        int DisplaySlots;
        [SerializeField] protected PauseMenuHUD pauseMenu;
        [SerializeField] protected OptionsMenuManager optionsMenu;
        [SerializeField] protected ChoicesMenu choicesMenu;
        [SerializeField] protected ShopMenu shopMenu;
        [SerializeField] protected RectTransform loadingIcon;
        public DialogueManager textbox;
        public DialogueManager dimTextbox;
        public FlashImageHandler flashScreen;
        public FadeScreenTrigger fadeScreen;
        [SerializeField] private BattleStartTrigger battleStartTrigger;

        public bool triggerFadeInOnStart = false;
        [Tooltip("If the vertical or horizontal input is held for this amount of seconds, the button will autofire.")]
        public float axisHeldTimeTilAutoFire = 0.5f;
        [Tooltip("The trigger interval in seconds of autofire.")]
        public float autoFireInterval = 0.05f;
        [SerializeField] float holdTime = 0;
        [SerializeField] float intervalTime = 0;

        [Tooltip("Currently active menus. Position 0 is the currently controlled menu.")]
        [SerializeField] private List<UIMenu> activeMenus = new List<UIMenu>();

        [Header("Input")]
        public bool actionButtonDown = false;
        public bool actionButtonHold = false;
        public bool skipButtonDown = false;
        public bool skipButtonHold = false;
        public bool cancelButtonDown = false;
        public bool cancelButtonHold = false;
        public float verticalAxisDown = 0f;
        public float verticalAxisHold = 0f;
        public float horizontalAxisDown = 0f;
        public float horizontalAxisHold = 0f;

        [Header("Other Inputs")]
        public bool QDown = false;
        public bool QHold = false;
        public bool WDown = false;
        public bool WHold = false;
        public bool ADown = false;
        public bool AHold = false;
        public bool SDown = false;
        public bool SHold = false;
        public bool DDown = false;
        public bool DHold = false;

        #region Singleton
        public static UIController instance;
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                //eventSystem.SetActive(true);
                SceneLoaderManager.onSceneLoad.AddListener(GetCanvasCamera);
                DontDestroyOnLoad(gameObject);
            }
        }
        #endregion

        public void Start()
        {
            if (fadeScreen != null && triggerFadeInOnStart)
            {
                fadeScreen.SetAlpha(1f);
                fadeScreen.TriggerFadeIn(1f);
            }
        }

        /*public void RefreshParty(List<Character> input)
        {
            for (int i = 0; i < DisplaySlots; i++)
            {
                if (i < input.Count)
                {
                    HP[i].localScale = new Vector3(input[i].CurrentHP / (float)input[i].HP, HP[i].localScale.y, HP[i].localScale.z);
                    SP[i].localScale = new Vector3(input[i].CurrentSP / (float)input[i].SP, SP[i].localScale.y, SP[i].localScale.z);
                    Names[i].text = input[i].title;
                }
            }
        }*/
        public void GetCanvasCamera()
        {
            if (cameraCanvas == null) return;
            Debug.Log(SceneLoaderManager.currentSceneNode);
            Debug.Log(SceneLoaderManager.currentSceneProperties);
            var currentSceneProp = SceneLoaderManager.currentSceneProperties;
            Debug.Log("Current Scene Prop: " + currentSceneProp);
            if (currentSceneProp == null) return;
            var cameraFollow = currentSceneProp.camFollow;
            if (cameraFollow == null) return;
            var newCamera = cameraFollow.cam;
            cameraCanvas.worldCamera = newCamera;
            cameraCanvas.sortingLayerName = cameraCanvasSortingLayerName;
            Debug.Log("Assigned camera: " + newCamera, this);
        }
        public void RefreshDosh(int input)
        {
            dosh.text = input.ToString();
        }

        public void Update()
        {
            CheckButtonHold();
        }

        private void CheckButtonHold()
        {
            if ((verticalAxisHold != 0 || horizontalAxisHold != 0) && !GameManager.disableUIInput)
            {
                holdTime += Time.deltaTime;
                if (holdTime >= axisHeldTimeTilAutoFire)
                    holdTime = axisHeldTimeTilAutoFire;
            }
            else
            {
                holdTime = 0;
                intervalTime = 0;
            }
            if (holdTime >= axisHeldTimeTilAutoFire)
            {
                intervalTime += Time.deltaTime;
                if (intervalTime >= autoFireInterval)
                {
                    if (activeMenus.Count != 0)
                    {
                        activeMenus[0].HighlightVMove(LISAUtility.Sign(verticalAxisHold));
                        activeMenus[0].HighlightHMove(LISAUtility.Sign(horizontalAxisHold));
                    }
                    intervalTime = 0;
                }
            }
        }

        public void LateUpdate()
        {
            ButtonDownHandler();
        }

        public void SetMenu(UIMenu uiMenu)
        {
            if (uiMenu == null)
            {
                if (activeMenus.Count > 0) activeMenus.RemoveAt(0);
            }
            else activeMenus.Insert(0, uiMenu);
            CheckActiveMenusCount();
        }

        private void CheckActiveMenusCount()
        {
            if (activeMenus.Count == 0) GameManager.instance.DisableUIInput(true);
            else
            {
                activeMenus[0].HighlightCurrent();
                GameManager.instance.DisableUIInput(false); 
            }
        }

        public void RemoveMenu(UIMenu uiMenu)
        {
            int index = activeMenus.IndexOf(uiMenu);
            if (index >= 0) activeMenus.RemoveAt(index);
            CheckActiveMenusCount();
        }
        public void CloseAllMenus()
        {
            while(activeMenus.Count > 0)
            {
                activeMenus[0].CloseMenu();
            }
        }
        public UIMenu GetControlledMenu()
        {
            return activeMenus[0];
        }
        public void OpenOptionsMenu()
        {
            optionsMenu.OpenOptionsMenu();
        }
        public void OpenShop(ShopData shopData, EventAction actionCallback = null)
        {
            shopMenu.OpenShop(shopData, actionCallback);
        }
        public void ShowChoices(EventAction callback, List<string> options, bool closeWithCancel, System.Action onMenuCancel = null)
        {
            choicesMenu.DisplayChoices(callback, options, closeWithCancel, onMenuCancel);
        }
        public void InvokePauseMenu()
        {
            //AudioManager.instance.PlaySFX(pauseClip, 1f, 1f);
            pauseMenu.InvokePauseMenu();
        }

        private void ButtonDownHandler()
        {
            if (activeMenus.Count != 0 && !GameManager.disableUIInput && holdTime < axisHeldTimeTilAutoFire)
            {
                activeMenus[0].HighlightVMove(LISAUtility.Sign(verticalAxisDown));
                activeMenus[0].HighlightHMove(LISAUtility.Sign(horizontalAxisDown));
            }
            if (actionButtonDown) actionButtonDown = false;
            if (skipButtonDown) skipButtonDown = false;
            if (cancelButtonDown) cancelButtonDown = false;
            if (verticalAxisDown != 0f) verticalAxisDown = 0f;
            if (horizontalAxisDown != 0f) horizontalAxisDown = 0f;
            if (QDown) QDown = false;
            if (WDown) WDown = false;
            if (ADown) ADown = false;
            if (SDown) SDown = false;
            if (DDown) DDown = false;
        }

        public void TriggerLoadingIcon(bool input)
        {
            loadingIcon.gameObject.SetActive(input);
        }
        public void TriggerBattleStart()
        {
            battleStartTrigger.TriggerBattleStart();
        }
        public void HideBattleStart()
        {
            battleStartTrigger.HideBattleStart();
        }
        public bool BattleStartIsFinished()
        {
            return battleStartTrigger.isFinished;
        }
        public void FlashScreen(Color color, float duration)
        {
            flashScreen.Flash(color, duration);
        }
        void ButtonHandler(InputAction.CallbackContext context, ref bool buttonDown, ref bool buttonHold)
        {
            if (context.performed)
            {
                buttonDown = true;
                buttonHold = true;
            }
            if (context.canceled)
            {
                buttonDown = false;
                buttonHold = false;
            }
        }
        void AxisHandler(InputAction.CallbackContext context, ref float axisDown, ref float axisHold)
        {
            float value = context.ReadValue<float>();
            Debug.Log(value);
            if (value == axisHold) return;
            // Wtf
            if (value == -1 || value == 0 || value == 1)
            {
                axisDown = value;
                axisHold = value;
                
                holdTime = 0;
                intervalTime = 0;
            }
        }

        public void ActionButton(InputAction.CallbackContext context)
        {
            ButtonHandler(context, ref actionButtonDown, ref actionButtonHold);
            if (activeMenus.Count != 0 && !GameManager.disableUIInput)
            {
                activeMenus[0].SelectMenu(context);
            }
        }

        public void SkipButton(InputAction.CallbackContext context)
        {
            ButtonHandler(context, ref skipButtonDown, ref skipButtonHold);
            if (activeMenus.Count != 0 && !GameManager.disableUIInput)
            {
                activeMenus[0].SkipMenu(context);
            }
        }
        public void CancelButton(InputAction.CallbackContext context)
        {
            ButtonHandler(context, ref cancelButtonDown, ref cancelButtonHold);
            if (activeMenus.Count != 0 && !GameManager.disableUIInput)
            {
                activeMenus[0].CancelMenu(context);
            }
        }
        public void VerticalAxis(InputAction.CallbackContext context)
        {
            AxisHandler(context, ref verticalAxisDown, ref verticalAxisHold);
        }

        public void HorizontalAxis(InputAction.CallbackContext context)
        {
            AxisHandler(context, ref horizontalAxisDown, ref horizontalAxisHold);
        }
        public void QKey(InputAction.CallbackContext context)
        {
            ButtonHandler(context, ref QDown, ref QHold);
        }
        public void WKey(InputAction.CallbackContext context)
        {
            ButtonHandler(context, ref WDown, ref WHold);
        }
        public void AKey(InputAction.CallbackContext context)
        {
            ButtonHandler(context, ref ADown, ref AHold);
        }
        public void SKey(InputAction.CallbackContext context)
        {
            ButtonHandler(context, ref SDown, ref SHold);
        }
        public void DKey(InputAction.CallbackContext context)
        {
            ButtonHandler(context, ref DDown, ref DHold);
        }
    }
}
