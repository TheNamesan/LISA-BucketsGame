using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BucketsGame
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public GamePlayerInput gameInput = new();
        public PlayerController player { get => SceneProperties.mainPlayer; }

        public void Test(InputAction.CallbackContext context)
        {
            Debug.Log("Test: " + context);
        }
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
                if (value > 0) gameInput.jumpDown = true;
            }
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
                gameInput.shootDown = true;
            if (context.canceled)
                gameInput.shootDown = false;
        }
        public void Dash(InputAction.CallbackContext context)
        {
            if (!player) return;
            if (context.performed)
                gameInput.dashDown = true;
            if (context.canceled)
                gameInput.dashDown = false;
        }
        public void Focus(InputAction.CallbackContext context)
        {
            if (!player) return;
            if (context.performed)
                gameInput.focus = true;
            if (context.canceled)
                gameInput.focus = false;
        }
        public void ResetLevel(InputAction.CallbackContext context)
        {
            if (!player) return;
            if (context.performed)
                BucketsGameManager.instance.ResetLevel();
        }

        public void OnEnable()
        {
            StartCoroutine(LateFixedUpdate());
        }
        public void FixedUpdate()
        {
            if (player)
            {
                if (TUFF.GameManager.disablePlayerInput)
                {
                    
                    player.input = new GamePlayerInput(); 
                }
                else
                {
                    //Debug.Log(player, player);
                    player.input = gameInput;
                }
                    
            }
        }
        public IEnumerator LateFixedUpdate()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                gameInput.jumpDown = false;
                gameInput.dashDown = false;
                gameInput.shootDown = false;
            }
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
        public bool shootDown;
        public bool dashDown;
        public bool jumpDown;
        public bool focus;
    }
}
