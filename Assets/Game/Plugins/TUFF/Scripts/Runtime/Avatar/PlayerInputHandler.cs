using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace TUFF
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private OverworldCharacterController avatar;
        public MoveRouteHandler moveRouteHandler = null;
        public static PlayerInputHandler instance;

        [Header("Inputs")]
        public CharacterInputStream playerInput = new();

        [Header("Events")]
        public UnityEvent<InputAction.CallbackContext> onMove = new();

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                avatar = GetComponent<OverworldCharacterController>();
                moveRouteHandler = GetComponent<MoveRouteHandler>();
                DontDestroyOnLoad(gameObject);
            }
        }
        public void StopInput()
        {
            if (avatar == null) return;
            avatar.nextInput.horizontalInput = 0f;
            avatar.nextInput.horizontalInputTap = 0f;
            avatar.nextInput.verticalInput = 0f;
            avatar.nextInput.verticalInputTap = 0f;
            Debug.Log("STOP INPUT");
            avatar.StopRunMomentum();
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
                if (GameManager.disablePlayerInput) return;
                avatar.nextInput.horizontalInput = playerInput.horizontalInput;
                avatar.nextInput.horizontalInputTap = playerInput.horizontalInputTap;
            }
            onMove?.Invoke(context);
        }
        public void UpDownInput(InputAction.CallbackContext context)
        {
            float readValue = context.ReadValue<float>();
            if (context.performed || context.canceled)
            {
                playerInput.verticalInput = readValue;
                playerInput.verticalInputTap = readValue;
                if (GameManager.disablePlayerInput) return;
                avatar.nextInput.verticalInput = playerInput.verticalInput;
                avatar.nextInput.verticalInputTap = playerInput.verticalInputTap;
            }
        }

        public void Interaction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (GameManager.disablePlayerInput) return;
                avatar.nextInput.interactionButtonDown = true;
                avatar.nextInput.interactionButtonHold = true;
            }
            if (context.canceled)
            {
                avatar.nextInput.interactionButtonDown = false;
                avatar.nextInput.interactionButtonHold = false;
            }
        }
        public void Run(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // Do not disable input here to make arrow inputs buffer possible
                if (PlayerData.instance.charProperties.disableRun) return;
                avatar.nextInput.runButtonDown = true;
                avatar.nextInput.runButtonHold = true;
            }
            if (context.canceled)
            {
                avatar.nextInput.runButtonDown = false;
                avatar.nextInput.runButtonHold = false;
            }
        }
        public void Pause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (GameManager.disablePlayerInput) return;
                avatar.nextInput.pauseButtonDown = true;
                avatar.nextInput.pauseButtonHold = true;
            }
            if (context.canceled)
            {
                avatar.nextInput.pauseButtonDown = false;
                avatar.nextInput.pauseButtonHold = false;
            }
        }
    }
}

