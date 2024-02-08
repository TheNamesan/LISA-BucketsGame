using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

namespace TUFF
{
    public abstract class UIElement : MonoBehaviour
    {
        [Tooltip("Is this UI Element currently highlighted?")]
        public bool highlighted = false;
        [Tooltip("If true, element can't be used and will play the disabledClip sound when trying to use it.")]
        [SerializeField] protected bool m_disabled = false;
        public bool disabled
        {
            get { return m_disabled; }
            set { m_disabled = value; DisabledHandler(); }
        }
        [Header("References")]
        [Tooltip("Game Object to display text.")]
        public TMP_Text text;


        [Header("Disabled")]
        [Tooltip("When the element is unmarked as disabled, it will invoke this event.")]
        public UnityEvent onEnabled;
        [Tooltip("When the element is marked as disabled, it will invoke this event.")]
        public UnityEvent onDisabled;

        [Header("Highlight")]
        public UnityEvent onHighlight = new UnityEvent();
        public UnityEvent onUnhighlight = new UnityEvent();

        [Header("Input Actions")]
        public UnityEvent onSelect;
        public UnityEvent onSelectCanceled;
        public UnityEvent onCancel;
        public UnityEvent onCancelCanceled;
        public UnityEvent onSkip;
        public UnityEvent onSkipCanceled;
        public UnityEvent<int> onHorizontalInput;
        public UnityEvent<int> onVerticalInput;

        [Header("Description Display")]
        [Tooltip("If the UIMenu's descriptionDisplay is not set to none, it will display this text when the element is highlighted.")]
        [TextArea(1, 3)] public string highlightDisplayText;

        [Header("Override Sound clips")]
        [Tooltip("If true, mutes the clip played when selecting this element and not disabled.")]
        public bool useCustomSelectSFX;
        [Tooltip("If none, will use the default clip set under TUFF Settings.")]
        public SFX customSelectSFX = new SFX();
        [Tooltip("If true, mutes the clip played when selecting this element and it is disabled.")]
        public bool useCustomDisabledSFX;
        [Tooltip("If none, will use the default clip set under TUFF Settings.")]
        public SFX customDisabledSFX = new SFX();

        protected bool disableMenuHInput = false;
        protected bool disableMenuVInput = false;

        protected virtual void OnEnable()
        {
            DisabledHandler();
        }
        protected virtual void DisabledHandler()
        {
            if (m_disabled)
            {
                onDisabled?.Invoke();
            }
            else onEnabled?.Invoke();
        }
        public virtual void OnOpenMenu()
        {

        }
        public virtual void Highlight()
        {
            highlighted = true;
            onHighlight?.Invoke();
        }
        public virtual void Unhighlight()
        {
            highlighted = false;
            onUnhighlight?.Invoke();
        }
        public virtual void HorizontalInput(int direction, bool inAutoFire = false)
        {
            onHorizontalInput?.Invoke(direction);
        }
        public virtual void VerticalInput(int direction, bool inAutoFire = false)
        {
            onVerticalInput?.Invoke(direction);
        }
        public virtual void Select(InputAction.CallbackContext context)
        {
            if (GameManager.disableUIInput) return;
            if (context.performed)
            {
                Debug.Log("Select Performed");
                onSelect?.Invoke();
            }
            if (context.canceled)
            {
                onSelectCanceled?.Invoke();
            }
        }

        public virtual void Cancel(InputAction.CallbackContext context)
        {
            if (GameManager.disableUIInput) return;
            if (context.performed)
            {
                onCancel?.Invoke();
            }
            if (context.canceled)
            {
                onCancelCanceled?.Invoke();
            }
        }
        public virtual void Skip(InputAction.CallbackContext context)
        {
            if (GameManager.disableUIInput) return;
            if (context.performed)
            {
                onSkip?.Invoke();
            }
            if (context.canceled)
            {
                onSkipCanceled?.Invoke();
            }
        }

        public bool IsActiveInHierarchy()
        {
            return gameObject.activeInHierarchy;
        }
        protected void PlaySound(SFX sfx)
        {
            AudioManager.instance.PlaySFX(sfx);
        }
        protected SFX SelectClip()
        {
            if (useCustomSelectSFX) return customSelectSFX;
            return TUFFSettings.selectSFX;
        }
        protected SFX DisabledClip()
        {
            if (useCustomDisabledSFX) return customDisabledSFX;
            return TUFFSettings.disabledSFX;
        }
        public void ForcePlaySelectClip()
        {
            PlaySound(SelectClip());
        }
        public bool GetMenuHInputDisabled()
        {
            return disableMenuHInput;
        }
        public bool GetMenuVInputDisabled()
        {
            return disableMenuVInput;
        }
    }
}
