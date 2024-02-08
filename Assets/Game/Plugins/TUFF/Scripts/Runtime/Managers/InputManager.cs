using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TUFF
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        PlayerInput input;
        [HideInInspector] public InputActionMap playerActionMap;
        [HideInInspector] public InputActionMap uiActionMap;
        public void Awake()
        {
            input = GetComponent<PlayerInput>();
            playerActionMap = input.actions.FindActionMap("Player");
            uiActionMap = input.actions.FindActionMap("UIController");
            playerActionMap.Enable();
            uiActionMap.Enable();
        }
        ///Player
        public void PlayerVerticalInput(InputAction.CallbackContext context) {
            if (PlayerInputHandler.instance != null) PlayerInputHandler.instance.UpDownInput(context);
        }
        public void PlayerMove(InputAction.CallbackContext context) {
            if (PlayerInputHandler.instance != null) PlayerInputHandler.instance.Move(context);
        }
        public void PlayerInteraction(InputAction.CallbackContext context) {
            if (PlayerInputHandler.instance != null) PlayerInputHandler.instance.Interaction(context); 
        }
        public void PlayerRun(InputAction.CallbackContext context) {
            if (PlayerInputHandler.instance != null) PlayerInputHandler.instance.Run(context);
        }
        public void PlayerPause(InputAction.CallbackContext context) {
            if (PlayerInputHandler.instance != null) PlayerInputHandler.instance.Pause(context);
        }

        ///UI
        public void UIActionButton(InputAction.CallbackContext context) {
            if(UIController.instance != null) UIController.instance.ActionButton(context); }
        public void UICancelButton(InputAction.CallbackContext context)
        {
            if (UIController.instance != null) UIController.instance.CancelButton(context);
        }
        public void UISkipButton(InputAction.CallbackContext context)
        {
            if (UIController.instance != null) UIController.instance.SkipButton(context);
        }
        public void UIUpDown(InputAction.CallbackContext context)
        {
            if (UIController.instance != null) UIController.instance.VerticalAxis(context);
        }
        public void UILeftRight(InputAction.CallbackContext context)
        {
            if (UIController.instance != null) UIController.instance.HorizontalAxis(context);
        }
        public void UI_Q(InputAction.CallbackContext context)
        {
            if (UIController.instance != null) UIController.instance.QKey(context);
        }
        public void UI_W(InputAction.CallbackContext context)
        {
            if (UIController.instance != null) UIController.instance.WKey(context);
        }
        public void UI_A(InputAction.CallbackContext context)
        {
            if (UIController.instance != null) UIController.instance.AKey(context);
        }
        public void UI_S(InputAction.CallbackContext context)
        {
            if (UIController.instance != null) UIController.instance.SKey(context);
        }
        public void UI_D(InputAction.CallbackContext context)
        {
            if (UIController.instance != null) UIController.instance.DKey(context);
        }
    }
}

