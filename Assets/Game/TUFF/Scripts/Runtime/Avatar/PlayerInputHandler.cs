using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace TUFF
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private OverworldCharacterController avatar { get 
            {
                if (FollowerInstance.player)
                    return FollowerInstance.player.controller;
                return null;
            } 
        }

        [Header("Inputs")]
        public CharacterInputStream playerInput = new();

        [Header("Events")]
        public UnityEvent<InputAction.CallbackContext> onMove = new();

        private IEnumerator lateFixedUpdate;

        private void OnEnable()
        {
            if (lateFixedUpdate == null) lateFixedUpdate = LateFixedUpdate();
            StartCoroutine(lateFixedUpdate);
        }
        private void OnDisable()
        {
            if (lateFixedUpdate != null) StopCoroutine(lateFixedUpdate);
        }
        private IEnumerator LateFixedUpdate()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                playerInput.interactionButtonDown = false;
                playerInput.runButtonDown = false;
                playerInput.pauseButtonDown = false;
                playerInput.horizontalInputTap = 0f;
                playerInput.verticalInputTap = 0f;
            }
        }
        private void FixedUpdate()
        {
            UpdateInput();
        }

        private void UpdateInput()
        {
            if (avatar)
            {
                if (!GameManager.disablePlayerInput)
                {
                    avatar.nextInput = playerInput;
                }
            }
        }

        public void StopInput()
        {
            if (avatar == null) return;
            Debug.Log("STOP INPUT");
            avatar.StopInput();
        }
        public void ResumeInput()
        {
            if (avatar == null) return;
            //avatar.nextInput = playerInput;
            avatar.nextInput.horizontalInput = playerInput.horizontalInput;
            avatar.nextInput.horizontalInputTap = playerInput.horizontalInputTap;
            avatar.nextInput.verticalInput = playerInput.verticalInput;
            avatar.nextInput.verticalInputTap = playerInput.verticalInputTap;
        }
        public void Move(InputAction.CallbackContext context)
        {
            float readValue = context.ReadValue<float>();
            if (context.performed || context.canceled)
            {
                playerInput.horizontalInput = readValue;
                playerInput.horizontalInputTap = readValue;
            }

            //float readValue = context.ReadValue<float>();
            //if (context.performed || context.canceled)
            //{
            //    playerInput.horizontalInput = readValue;
            //    playerInput.horizontalInputTap = readValue;
            //    if (GameManager.disablePlayerInput) return;
            //    avatar.nextInput.horizontalInput = playerInput.horizontalInput;
            //    avatar.nextInput.horizontalInputTap = playerInput.horizontalInputTap;
            //}
            //onMove?.Invoke(context);
        }
        public void UpDownInput(InputAction.CallbackContext context)
        {
            float readValue = context.ReadValue<float>();
            if (context.performed || context.canceled)
            {
                playerInput.verticalInput = readValue;
                playerInput.verticalInputTap = readValue;
            }
            //float readValue = context.ReadValue<float>();
            //if (context.performed || context.canceled)
            //{
            //    playerInput.verticalInput = readValue;
            //    playerInput.verticalInputTap = readValue;
            //    if (GameManager.disablePlayerInput) return;
            //    //avatar.nextInput.verticalInput = playerInput.verticalInput;
            //    //avatar.nextInput.verticalInputTap = playerInput.verticalInputTap;
            //}
        }

        public void Interaction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                //if (GameManager.disablePlayerInput) return;
                playerInput.interactionButtonDown = true;
                playerInput.interactionButtonHold = true;
            }
            if (context.canceled)
            {
                playerInput.interactionButtonDown = false;
                playerInput.interactionButtonHold = false;
            }
            //if (context.performed)
            //{
            //    if (GameManager.disablePlayerInput) return;
            //    //avatar.nextInput.interactionButtonDown = true;
            //    //avatar.nextInput.interactionButtonHold = true;
            //}
            //if (context.canceled)
            //{
            //    avatar.nextInput.interactionButtonDown = false;
            //    avatar.nextInput.interactionButtonHold = false;
            //}
        }
        public void Run(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                playerInput.runButtonDown = true;
                playerInput.runButtonHold = true;
            }
            if (context.canceled)
            {
                playerInput.runButtonDown = false;
                playerInput.runButtonHold = false;
            }
            //if (context.performed)
            //{
            //    // Do not disable input here to make arrow inputs buffer possible
            //    if (PlayerData.instance.charProperties.disableRun) return;
            //    avatar.nextInput.runButtonDown = true;
            //    avatar.nextInput.runButtonHold = true;
            //}
            //if (context.canceled)
            //{
            //    avatar.nextInput.runButtonDown = false;
            //    avatar.nextInput.runButtonHold = false;
            //}
        }
        public void Pause(InputAction.CallbackContext context)
        {
            if (!FollowerInstance.player) return;
            if (PlayerData.instance != null && PlayerData.instance.charProperties.disableMenuAccess) return;
            if (context.performed)
            {
                if (GameManager.disablePlayerInput) return;
                UIController.instance.InvokePauseMenu();
            }
            //if (context.canceled)
            //{
            //    playerInput.pauseButtonDown = false;
            //    playerInput.pauseButtonHold = false;
            //}
            //if (context.performed)
            //{
            //    if (GameManager.disablePlayerInput) return;
            //    avatar.nextInput.pauseButtonDown = true;
            //    avatar.nextInput.pauseButtonHold = true;
            //}
            //if (context.canceled)
            //{
            //    avatar.nextInput.pauseButtonDown = false;
            //    avatar.nextInput.pauseButtonHold = false;
            //}
        }
    }
}

