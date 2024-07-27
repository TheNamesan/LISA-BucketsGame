using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    public class ChoicesMenu : MonoBehaviour
    {
        // TODO: Implementation for adding new elements
        public UIMenu uiMenu;
        public RectTransform content;
        public CanvasGroup contentCanvasGroup;
        public List<UIButton> elements = new();

        [System.NonSerialized]
        public EventAction actionCallback = null;
        protected bool initialized = false;
        protected List<string> choices = new();
        protected System.Action onCancelMenu = null;

        private bool m_inUse = false;
        public bool InUse { get => m_inUse; }

        public RectTransform rect { get => transform as RectTransform; }
        private RectTransform parentRect { get => transform.parent as RectTransform; }
        private void Awake()
        {
            Initialize();
        }
        private void ForceRebuild()
        {
            if (content)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                LayoutRebuilder.ForceRebuildLayoutImmediate(content); //called two times on purpose
            }
        }
        private void Initialize()
        {
            if (initialized) return;
            VerifyMenuArrays(0);
            int index = 0;
            foreach (Transform child in content) // Add existing GameObjects to list
            {
                if (child.TryGetComponent(out UIButton existing))
                {
                    elements.Add(existing);
                    AddToMenu(index, existing);
                }
                index++;
            }
            ShowContent(false);
            SetupUIMenu();
            if (uiMenu)
                uiMenu.onCancelMenu.AddListener(CancelMenu);
            if (uiMenu.transitionHandler)
                uiMenu.transitionHandler.onTransitionChange.AddListener(UpdateOnState);
            initialized = true;
        }
        private IEnumerator InitialValues(EventAction callback, List<string> options, bool closeWithCancel, System.Action onMenuCancel = null)
        {
            m_inUse = true;
            actionCallback = callback;
            choices = options;
            if (choices == null || choices.Count <= 0)
            {
                EndChoices(-1);
                yield break;
            }
            onCancelMenu = onMenuCancel;
            uiMenu.closeMenuWithCancel = closeWithCancel;
            UpdateElements();
            ShowContent(false);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            SetPosition();
            //yield return new WaitForEndOfFrame();
            uiMenu?.OpenMenu();
            ForceRebuild(); // Fixes Content Size Fitter compontent not updating when opening the choices menu
        }
        private void SetPosition()
        {
            LISAUtility.SetPivot(rect, new Vector2(rect.pivot.x, 0.5f));
            Vector2 position = new Vector2(0, 0);
            if (DialogueManager.openBoxes.Count > 0 && DialogueManager.openBoxes[0])
            {
                var relativeBox = DialogueManager.openBoxes[0];
                Debug.Log("RelativeBoxPos: " + relativeBox.rect.anchoredPosition);
                Debug.Log("RelativeBoxSize: " + relativeBox.rect.sizeDelta);
                Debug.Log("SelfPos: " + rect.anchoredPosition);
                Debug.Log("SelfSize: " + rect.sizeDelta);
                //if (uiMenu.transitionHandler) uiMenu.transitionHandler.originalSize = rect.sizeDelta;
                var offsetX = (relativeBox.rect.sizeDelta.x - rect.sizeDelta.x) / 2f;
                if (relativeBox.dialogue.textboxType == TextboxType.Fixed) offsetX /= 2f;
                float middleAdjustment = ((relativeBox.rect.anchoredPosition.y < 0f) ? 1 : (-1));
                var offsetY = (rect.sizeDelta.y / 2f + relativeBox.rect.sizeDelta.y / 2f) * middleAdjustment;
                //offsetY *= middleAdjustment;

                position = relativeBox.rect.anchoredPosition + new Vector2(offsetX, offsetY);
            }

            rect.anchoredPosition = position;
        }
        private void UpdateElements()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (i >= choices.Count)
                {
                    elements[i].gameObject.SetActive(false);
                    continue;
                }
                UIButton button = elements[i];
                button.text.text = TUFFTextParser.ParseText(choices[i]);
                button.gameObject.SetActive(true);
                //if (button.TryGetComponent(out AdjustToPreferredTextSize textAdjust))
                //{
                //    Debug.Log(textAdjust);
                //    textAdjust.Adjust();
                //}
            }
            //if (adjustRect) adjustRect.Adjust();
            SetupUIMenu();
        }
        private void ShowContent(bool show)
        {
            if (!contentCanvasGroup) return;
            contentCanvasGroup.alpha = (show ? 1 : 0);
        }

        public void DisplayChoices(EventAction callback, List<string> options, bool closeWithCancel, System.Action onMenuCancel = null)
        {
            Initialize();
            StartCoroutine(InitialValuesCoroutine(callback, options, closeWithCancel, onMenuCancel));
        }
        protected IEnumerator InitialValuesCoroutine(EventAction callback, List<string> options, bool closeWithCancel, System.Action onMenuCancel = null)
        {
            while (m_inUse) yield return null;
            yield return InitialValues(callback, options, closeWithCancel, onMenuCancel);
        }
        protected void UpdateOnState(BoxTransitionState state)
        {
            if (state == BoxTransitionState.Visible) ShowContent(true);
        }
        protected void ToggleAll(bool enabled)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].gameObject.SetActive(enabled);
            }
        }
        private void VerifyMenuArrays(int rows)
        {
            if (uiMenu == null) return;
            if (uiMenu.UIElementContainers == null)
            {
                uiMenu.UIElementContainers = new UIElementContainer[rows];
            }
            if (rows >= uiMenu.UIElementContainers.Length) // If row is out of row count
            {
                var newArray = new UIElementContainer[rows + 1];
                System.Array.Copy(uiMenu.UIElementContainers, newArray, uiMenu.UIElementContainers.Length);
                uiMenu.UIElementContainers = newArray;
                //Debug.Log("Resize: " + uiMenu.UIElementContainers.Length);
            }
            if (uiMenu.UIElementContainers[0] == null) // Check Index 0
                uiMenu.UIElementContainers[0] = new UIElementContainer();
        }
        protected void SetupUIMenu()
        {
            if (uiMenu == null) return;
            uiMenu.SetupUIElements();
        }
        protected void AddToMenu(int index, UIButton element)
        {
            if (uiMenu == null) return;
            if (index >= uiMenu.UIElementContainers.Length)
            {
                VerifyMenuArrays(index);
            }
            //Debug.Log($"{uiMenu.UIElementContainers.Length} VS {index}");
            if (uiMenu.UIElementContainers[index] == null)
                uiMenu.UIElementContainers[index] = new UIElementContainer();
            uiMenu.UIElementContainers[index].UIElements.Add(element);
            element.onSelect.AddListener(() => PickOption(index));
        }
        private void PickOption(int index)
        {
            Debug.Log($"Selected: {index}");
            StartCoroutine(EndChoices(index));
        }
        private void CancelMenu()
        {
            Debug.Log("Menu Canceled");
            StartCoroutine(EndCancelChoices());
        }
        private IEnumerator EndCancelChoices()
        {
            ShowContent(false);
            CloseTextbox();

            yield return new WaitForEndOfFrame(); // Little hack so it doesn't automatically skip text when this ends, fix later?
            onCancelMenu?.Invoke();
            m_inUse = false;
            yield break;
        }
        private IEnumerator EndChoices(int index)
        {
            ShowContent(false);
            CloseTextbox();
            if (uiMenu.IsOpen) uiMenu.CloseMenu();
            
            yield return new WaitForEndOfFrame(); // Little hack so it doesn't automatically skip text when this ends, fix later?
            if (actionCallback is ShowChoicesAction showChoices)
            {
                showChoices.PickOption(index);
            }
            else if (actionCallback != null) actionCallback.isFinished = true;
            m_inUse = false;
            yield break;
        }

        private static void CloseTextbox()
        {
            // If open textbox exists.
            if (DialogueManager.openBoxes.Count > 0 && DialogueManager.openBoxes[0])
            {
                DialogueManager.openBoxes[0].CloseTextbox();
            }
        }
    }
}
