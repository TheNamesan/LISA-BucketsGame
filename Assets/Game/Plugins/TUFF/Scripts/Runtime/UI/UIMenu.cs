using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

namespace TUFF
{
    public class UIMenu : MonoBehaviour
    {
        [Tooltip("UI Menu Buttons. Each Element represents a row (Y). Every UIElement represents a column (X).")]
        public UIElementContainer[] UIElementContainers;
        public UIElement[][] UIElements;

        protected bool m_isOpen = false;
        public bool IsOpen { get => m_isOpen; }

        [Header("Position")]
        [Tooltip("Current highlighted element index on X.")]
        public int highlightX = 0;
        [Tooltip("Current hightlighted element index on Y.")]
        public int highlightY = 0;

        [Header("References")]
        [Tooltip("Reference to the transition handler component. Optional. If not empty, will play Appear and Dissapear transitions when opening/closing the menu.")]
        public BoxTransitionHandler transitionHandler = null;

        [Header("Parameters")]
        [Tooltip("If true, highlightX and highlightY will be remembered when closing the menu. Otherwise both return to 0.")]
        public bool rememberHighlight = false;
        [Tooltip("If true, menu will be closed when pressing the Cancel button. Overrides buttons' Cancel Actions.")]
        public bool closeMenuWithCancel = false;
        [Tooltip("If true, menu will be closed when pressing the Skip button. Overrides buttons' Skip Actions.")]
        public bool closeMenuWithSkip = false;
        [Tooltip("If true, this GameObject will remain active after closing the menu.")]
        public bool keepMenuActiveOnClose = false;
        [Tooltip("If true and GameObject is active, Player automatically takes control of this menu when starting the scene.")]
        public bool controlThisMenuOnStart = false;

        [Header("On Close")]
        [Tooltip("Only works when closeMenuWithCancel or closeMenuWithSkip is true. Opens these menus when closing.")]
        public List<UIMenu> onCloseMenuMenusToOpen = new List<UIMenu>();

        [Header("OnOpenMenu Actions")]
        [Tooltip("Called any time the menu is opened.")]
        public UnityEvent onOpenMenu;
        [Tooltip("Called any time the menu is closed.")]
        public UnityEvent onCloseMenu;

        [Header("OnCancelMenu Actions")]
        [Tooltip("Only works when closeMenuWithCancel is true. Called after onCloseMenu.")]
        public UnityEvent onCancelMenu;

        [Header("OnSkipMenu Actions")]
        [Tooltip("Only works when closeMenuWithSkip is true. Called after onCloseMenu.")]
        public UnityEvent onSkipMenu;

        [Header("Description Display")]
        [Tooltip("Optional. Displays the text from highlightDisplayText of the highlighted element.")]
        public TMP_Text descriptionDisplay;

        [Header("SFX")]
        [Tooltip("SFX to play when the menu opens.")]
        public SFX openSFX = new SFX();
        [Tooltip("SFX to play when the menu closes.")]
        public SFX closeSFX = new SFX();

        [Header("Override SFX")]
        [Tooltip("If false, will use the default SFX set under TUFF Settings.")]
        public bool useCustomHighlightSFX = false;
        [Tooltip("Custom Highlight SFX.")]
        public SFX customHighlightSFX = new SFX();
        [Tooltip("Only works when closeMenuWithCancel or closeMenuWithSkip is true. If true, will play the Custom SFX when closing.")]
        public bool useCustomCancelSFX = false;
        [Tooltip("Custom Cancel SFX.")]
        public SFX customCancelSFX = new SFX();

        bool ignoreInput = false;
        void Awake()
        {
            if (UIElements == null) SetupUIElements();
        }

        private void Start()
        {
            if (controlThisMenuOnStart)
            {
                OpenMenu();
            }
        }
        protected void AssignNextValidHighlight() //Optimize this later lmao
        {
            int startingIndexY = highlightY;
            UnhighlightCurrent();
            while (true)
            {
                highlightY += 1;
                if (highlightY >= UIElements.Length) highlightY = 0;
                else if (highlightY < 0) highlightY = UIElements.Length - 1;

                if (UIElements[highlightY].Length > 0) //If X has elements
                {
                    if (highlightX >= UIElements[highlightY].Length) highlightX = UIElements[highlightY].Length - 1;
                }
                else
                {
                    if (highlightY == startingIndexY) break;
                    continue;
                }
                if (UIElements[highlightY][highlightX].IsActiveInHierarchy())
                {
                    break;
                }
                if (highlightY == startingIndexY) break;
            }
            HighlightCurrent();
        }
        private void LateUpdate()
        {
            ignoreInput = false;
        }

        void ResetMenu()
        {
            UnhighlightCurrent();
            if (!rememberHighlight)
            { 
                highlightX = 0;
                highlightY = 0;
                //HighlightCurrent();
            }
        }

        public void SelectMenu(InputAction.CallbackContext context)
        {
            Select(context);
        }

        public void CancelMenu(InputAction.CallbackContext context)
        {
            if (closeMenuWithCancel)
            {
                if (context.performed && !ignoreInput)
                {
                    PlaySound(CancelClip());
                    CloseMenu();
                    onCancelMenu?.Invoke();
                    OnCloseMenuMenusToOpen();
                }
            }
            else Cancel(context);
        }

        public void SkipMenu(InputAction.CallbackContext context)
        {
            if (closeMenuWithSkip)
            {
                if (context.performed && !ignoreInput)
                {
                    PlaySound(CancelClip());
                    CloseMenu();
                    onSkipMenu?.Invoke();
                    OnCloseMenuMenusToOpen();
                }
            }
            else Skip(context);
        }

        void OnCloseMenuMenusToOpen()
        {
            for (int i = 0; i < onCloseMenuMenusToOpen.Count; i++)
            {
                onCloseMenuMenusToOpen[i].OpenMenu();
            }
        }

        public void CloseMenu()
        {
            m_isOpen = false;
            if (openSFX != null) PlaySound(closeSFX);
            onCloseMenu?.Invoke();
            ResetMenu();
            UIController.instance.RemoveMenu(this);
            if (transitionHandler)
            {
                StartCoroutine(CloseMenuCoroutine());
            }
            else
            {
                if (!keepMenuActiveOnClose) gameObject.SetActive(false);
            }
        }
        private IEnumerator CloseMenuCoroutine()
        {
            if (transitionHandler == null) yield break;
            transitionHandler.Dissapear();
            while (transitionHandler.state == BoxTransitionState.Dissapearing)
                yield return null;
            if (transitionHandler.state != BoxTransitionState.Hidden) yield break;
            if (!keepMenuActiveOnClose) gameObject.SetActive(false);
        }
        public void OpenMenu()
        {
            m_isOpen = true;
            if (UIElements == null) SetupUIElements();
            UIController.instance.SetMenu(this);
            HighlightCurrent();
            gameObject.SetActive(true);
            ignoreInput = true;
            if (openSFX != null) PlaySound(openSFX);
            onOpenMenu?.Invoke();
            transitionHandler?.Appear();
            HighlightCurrent(); //Highlight two times in case elements are changed.
            CallElementsOnOpenMenu();
        }

        public void CallElementsOnOpenMenu()
        {
            if (!CheckIfUIMenuHasElements(this)) return;
            if (!HighlightInUIMenuIsValid(this)) return;
            if (!UIElements[highlightY][highlightX].IsActiveInHierarchy()) AssignNextValidHighlight();
            for (int i = 0; i < UIElements.Length; i++)
            {
                for(int j = 0; j < UIElements[i].Length; j++)
                {
                    UIElements[i][j].OnOpenMenu();
                }
            }
        }

        public void SetupUIElements()
        {
            //if (UIElements != null) return;
            highlightX = 0;
            highlightY = 0;
            if (UIElementContainers.Length == 0)
            {
                return; 
            }
            UIElements = new UIElement[UIElementContainers.Length][];
            for (int i = 0; i < UIElementContainers.Length; i++)
            {
                if (UIElementContainers[i].UIElements.Count == 0) return;
                UIElements[i] = new UIElement[UIElementContainers[i].UIElements.Count];
                for (int j = 0; j < UIElementContainers[i].UIElements.Count; j++)
                {
                    UIElements[i][j] = UIElementContainers[i].UIElements[j];
                    if (UIElements[i][j] != null) UIElements[i][j].Unhighlight();
                }
            }
        }

        public UIElement GetCurrentHighlight()
        {
            if (!HighlightInUIMenuIsValid(this)) return null;
            return UIElements[highlightY][highlightX]; 
        }

        public void HighlightCurrent()
        {
            if (!CheckIfUIMenuHasElements(this)) return;
            if (!HighlightInUIMenuIsValid(this)) return;
            HighlightElement(highlightX, highlightY);
            DisplayText(UIElements[highlightY][highlightX].highlightDisplayText);
        }
        protected void HighlightElement(int x, int y)
        {
            if (CheckIfUIMenuHasElements(this)) UIElements[y][x].Highlight();
        }

        public void UnhighlightCurrent()
        {
            if (!CheckIfUIMenuHasElements(this)) return;
            if (!HighlightInUIMenuIsValid(this)) return;
            UnhighlightElement(highlightX, highlightY);
        }
        protected void UnhighlightElement(int x, int y)
        {
            if(CheckIfUIMenuHasElements(this)) UIElements[y][x].Unhighlight();
        }
        public void ForceDisplayTextFromHighlight()
        {
            DisplayText(UIElements[highlightY][highlightX].highlightDisplayText);
        }
        private void DisplayText(string text)
        {
            if (descriptionDisplay == null) return;
            descriptionDisplay.text = text;
        }
        public static bool HighlightInUIMenuIsValid(UIMenu uiMenu)
        {
            if (uiMenu.UIElements == null) { /*Debug.Log("UIElements null");*/ return false; }
            if (uiMenu.UIElements[uiMenu.highlightY] == null) { Debug.Log("UIElements[uiMenu.highlightY] null", uiMenu); return false; }
            if (uiMenu.UIElements[uiMenu.highlightY][uiMenu.highlightX] == null) { Debug.Log("UIElements[uiMenu.highlightY][uiMenu.highlightX] null"); return false; }
            return true;
        }

        public static bool CheckIfUIMenuHasElements(UIMenu uiMenu)
        {
            if (uiMenu == null) return false;
            if(uiMenu.UIElements != null)
            {
                if(uiMenu.UIElements.Length != 0)
                {
                    for(int i=0; i < uiMenu.UIElements.Length; i++)
                    {
                        if (uiMenu.UIElements[i] == null)
                            return false;
                        else if (uiMenu.UIElements[i].Length == 0)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public void HighlightVMove(int directionY)
        {
            if (directionY == 0) return;
            //Debug.Log(directionY);
            if (!HighlightInUIMenuIsValid(this)) return;
            UIElements[highlightY][highlightX].VerticalInput(directionY, UIController.instance.verticalAxisDown == 0f);
            if (UIElements[highlightY][highlightX].GetMenuVInputDisabled()) return;
            if (UIElements.Length <= 1) return;
            int startingIndexY = highlightY;
            UnhighlightCurrent();
            while(true)
            {
                highlightY -= directionY;
                if (highlightY >= UIElements.Length) highlightY = 0;
                else if (highlightY < 0) highlightY = UIElements.Length - 1;

                if (UIElements[highlightY].Length > 0) //If X has elements
                {
                    if (highlightX >= UIElements[highlightY].Length) highlightX = UIElements[highlightY].Length - 1;
                }
                else
                {
                    if (highlightY == startingIndexY) return;
                    continue;
                }

                if(UIElements[highlightY][highlightX].IsActiveInHierarchy())
                {
                    break;
                }
                if (highlightY == startingIndexY) return;
            }
            if (highlightY != startingIndexY) PlaySound(HighlightClip());
            HighlightCurrent();
        }

        public void HighlightHMove(int directionX)
        {
            if (directionX == 0) return;
            if (!HighlightInUIMenuIsValid(this)) return;
            UIElements[highlightY][highlightX].HorizontalInput(directionX, UIController.instance.horizontalAxisDown == 0f);
            if (UIElements[highlightY][highlightX].GetMenuHInputDisabled()) return;
            if (UIElements[highlightY].Length <= 1) return;
            int startingIndexX = highlightX;
            UnhighlightCurrent();
            while (true)
            {
                highlightX += directionX;
                if (highlightX >= UIElements[highlightY].Length) highlightX = 0;
                else if (highlightX < 0) highlightX = UIElements[highlightY].Length - 1;

                if (UIElements[highlightY][highlightX].IsActiveInHierarchy())
                {
                    break;
                }
                if (highlightX == startingIndexX) return;
            }
            if (highlightX != startingIndexX) PlaySound(HighlightClip());
            HighlightCurrent();
        }
        public void HighlightFirstValidElement()
        {
            if (!CheckIfUIMenuHasElements(this)) return;
            UnhighlightCurrent();
            for (int y = 0; y < UIElements.Length; y++)
            {
                for (int x = 0; x < UIElements[y].Length; x++)
                {
                    if (UIElements[y][x].IsActiveInHierarchy())
                    {
                        UnhighlightCurrent();
                        highlightX = x;
                        highlightY = y;
                        HighlightCurrent();
                        return;
                    }
                }
            }
        }
        public void HighlightLastValidElement()
        {
            if (!CheckIfUIMenuHasElements(this)) return;
            for (int y = UIElements.Length - 1; y >= 0; y--)
            {
                for (int x = UIElements[y].Length - 1; x >= 0; x--)
                {
                    if (UIElements[y][x].IsActiveInHierarchy())
                    {
                        UnhighlightCurrent();
                        highlightX = x;
                        highlightY = y;
                        HighlightCurrent();
                        return;
                    }
                }
            }
        }
        public void Select(InputAction.CallbackContext context)
        {
            if (HighlightInUIMenuIsValid(this)) UIElements[highlightY][highlightX].Select(context);
        }
        public void Cancel(InputAction.CallbackContext context)
        {
            if (HighlightInUIMenuIsValid(this)) UIElements[highlightY][highlightX].Cancel(context);
        }
        public void Skip(InputAction.CallbackContext context)
        {
            if (HighlightInUIMenuIsValid(this)) UIElements[highlightY][highlightX].Skip(context);
        }
        private void PlaySound(SFX sfx)
        {
            AudioManager.instance.PlaySFX(sfx);
        }
        protected SFX HighlightClip()
        {
            if (useCustomHighlightSFX) return customHighlightSFX;
            return TUFFSettings.highlightSFX;
        }
        protected SFX CancelClip()
        {
            if (useCustomCancelSFX) return customCancelSFX;
            return TUFFSettings.cancelSFX;
        }
        public void ForcePlayHighlightClip()
        {
            PlaySound(HighlightClip());
        }
    }
}
