using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TUFF
{
    public class UIButton : UIElement
    {
        [Header("OnSelect Actions")]
        [Tooltip("Menus to close when pressing this button.")]
        public List<UIMenu> menusToClose = new List<UIMenu>();
        [Tooltip("Menus to open when pressing this button.")]
        public List<UIMenu> menusToOpen = new List<UIMenu>();

        public override void Select(InputAction.CallbackContext context)
        {
            if (GameManager.disableUIInput) return;
            if(context.performed)
            {
                if (disabled)
                {
                    PlaySound(DisabledClip());
                    return;
                }
                PlaySound(SelectClip());
                onSelect?.Invoke();
                ChangeMenus();
            }
            if(context.canceled)
            {
                onSelectCanceled?.Invoke();
            }
        }

        private void ChangeMenus()
        {
            CloseMenus();
            OpenMenus();
        }

        private void CloseMenus()
        {
            for(int i = 0; i < menusToClose.Count; i++)
            {
                menusToClose[i]?.CloseMenu();
            }
        }

        private void OpenMenus()
        {
            for (int i = 0; i < menusToOpen.Count; i++)
            {
                menusToOpen[i].OpenMenu();
            }
        }
    }
}
