using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using TUFF;

namespace BucketsGame
{
    public class BucketsPlayerInputHandler : MonoBehaviour
    {
        public static BucketsPlayerInputHandler instance { get => BucketsGameManager.instance.inputHandler; }
        public PlayerInput playerInput;
        public InputActionReference resetAction;
        public GamePlayerInput gameInput = new();
        public Vector2 bufferedPointerWorld = new Vector2();
        public Vector2 bufferedPointer = new Vector2();
        public string keyboardResetId = "";
        public string gamepadResetId = "";
        public PlayerController player { get => SceneProperties.mainPlayer; }
        public void Move(InputAction.CallbackContext context)
        {
            if (!player) return;
            if (context.performed || context.canceled)
                gameInput.inputH = context.ReadValue<float>();
        }
        public void Vertical(InputAction.CallbackContext context)
        {
            if (!player) return;
            if (context.performed || context.canceled)
            {
                float value = context.ReadValue<float>();
                gameInput.inputV = value;
                if (value < 0)
                {
                    gameInput.jump = false;
                    gameInput.fastFallDown = true; 
                }
            }
        }
        public void Jump(InputAction.CallbackContext context)
        {
            if (!player) return;
            if (context.performed)
            {
                gameInput.jump = true;
                gameInput.jumpDown = true;
                gameInput.fastFallDown = false;
            }
            if (context.canceled)
            {
                gameInput.jump = false;
                gameInput.jumpDown = false;
            }
        }
        public void AimH(InputAction.CallbackContext context)
        {
            if(!player) return;
            
            gameInput.aim.x = context.ReadValue<float>();
            if (gameInput.inAimThreshold) gameInput.lastAimDirection = gameInput.aim;
        }
        public void AimV(InputAction.CallbackContext context)
        {
            if (!player) return;

            gameInput.aim.y = context.ReadValue<float>();
            if (gameInput.inAimThreshold) gameInput.lastAimDirection = gameInput.aim;
        }
        public void Pointer(InputAction.CallbackContext context)
        {
            if (!player) return;
            gameInput.mousePoint = context.ReadValue<Vector2>();
        }
        public void Shoot(InputAction.CallbackContext context)
        {
            if (!player) return;
            if (context.performed)
            {
                gameInput.shoot = true;
                gameInput.shootDown = true;
            }
            if (context.canceled)
            {
                gameInput.shoot = false;
                gameInput.shootDown = false;
            }
        }
        public void Dash(InputAction.CallbackContext context)
        {
            DashHandler(context);
        }

        private void DashHandler(InputAction.CallbackContext context)
        {
            if (!player) return;
            if (context.performed)
                gameInput.dashDown = true;
            if (context.canceled)
                gameInput.dashDown = false;
        }

        public void Focus(InputAction.CallbackContext context)
        {
            FocusHandler(context);
        }

        private void FocusHandler(InputAction.CallbackContext context)
        {
            if (!player) return;
            if (context.performed)
            {
                gameInput.focus = true;
                gameInput.focusDown = true;
            }
            if (context.canceled)
            {
                gameInput.focus = false;
                gameInput.focusDown = false;
            }
        }

        public void ResetLevel(InputAction.CallbackContext context)
        {
            if (!player || TUFF.GameManager.disablePlayerInput) return;
            if (context.performed)
                BucketsGameManager.instance.QueueReset();
        }
        public void Pause(InputAction.CallbackContext context)
        {
            if (!player || TUFF.GameManager.disablePlayerInput) return;
            if (context.performed)
                BucketsGameManager.instance.Pause(true);
        }

        public void OnEnable()
        {
            TUFF.GameManager.instance.onPlayerInputToggle.AddListener(ToggleInput);
            StartCoroutine(LateFixedUpdate());
        }
        public void OnDisable()
        {
            TUFF.GameManager.instance.onPlayerInputToggle.RemoveListener(ToggleInput);
        }
        public void OnDestroy()
        {
            if (Application.isPlaying)
            {
                TUFF.GameManager.instance.onPlayerInputToggle.RemoveListener(ToggleInput);
            }
        }
        public void FixedUpdate()
        {
            if (player)
            {
                if (!TUFF.GameManager.disablePlayerInput)
                {
                    player.input = gameInput;
                }
            }
        }
        public void ToggleInput(bool enabled)
        {
            if (!enabled)
            {
                // Stop Input
                StopInput();
            }
            else
            {
                if (player)
                    player.input.mousePoint = bufferedPointer;
            }
        }

        private void StopInput()
        {
            if (!player) return;
            bufferedPointer = player.input.mousePoint;
            bufferedPointerWorld = player.input.MousePointWorld;
            player.input = new GamePlayerInput();
        }

        private IEnumerator LateFixedUpdate()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                gameInput.fastFallDown = false;
                gameInput.jumpDown = false;
                gameInput.dashDown = false;
                gameInput.shootDown = false;
                gameInput.focusDown = false;
            }
        }
        public void OnControlSchemeChange()
        {
            Debug.Log(playerInput.currentControlScheme);
        }
        public bool IsGamepad()
        {
            if (!playerInput) return false;
            return playerInput.currentControlScheme == "Gamepad";
        }
        public string GetCurrentResetKeyText()
        {
            if (!resetAction) return "null";
            var displayString = string.Empty;
            var deviceLayoutName = default(string);
            var controlPath = default(string);

            var action = resetAction.action;
            // Get display string from action.
            if (action != null)
            {
                var id = (IsGamepad() ? gamepadResetId : keyboardResetId);
                ReadOnlyArray<InputBinding> bind = action.bindings;
                var bindingIndex = bind.IndexOf(x => x.id.ToString() == id);
                if (bindingIndex != -1)
                    displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, 0);
            }

            return displayString;
        }
    }
    [System.Serializable]
    public struct GamePlayerInput
    {
        public float inputH;
        public float inputV;
        public Vector2 mousePoint;
        public Vector2 MousePointWorld { get {
                var cam = SceneProperties.cam;
                if (!cam) return Vector2.zero;
                return cam.ScreenToWorldPoint(mousePoint);
        }}
        public bool shoot;
        public bool shootDown;
        public bool jump;
        public bool jumpDown;
        public bool fastFallDown;
        public bool dashDown;
        public bool focus;
        public bool focusDown;
        public Vector2 aim;
        public Vector2 lastAimDirection;
        public bool inAimThreshold { get => InAimThreshold(aim); }
        public bool InAimThreshold(Vector2 dir)
        {
            return Mathf.Abs(dir.x) > 0.05f || Mathf.Abs(dir.y) > 0.05f;
        }
    }
}
