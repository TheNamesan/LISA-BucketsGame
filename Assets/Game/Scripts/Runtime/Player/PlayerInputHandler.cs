using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BucketsGame
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public PlayerController player;
        
        public void Move(InputAction.CallbackContext context)
        {
            if (!player) return;
            if (context.performed || context.canceled)
                player.input.inputH = context.ReadValue<float>();
        }
        public void Vertical(InputAction.CallbackContext context)
        {
            if (!player) return;
            if (context.performed || context.canceled)
            {
                float value = context.ReadValue<float>();
                player.input.inputV = value;
                if (value > 0) player.input.jumpDown = true;
            }
        }
        public void Pointer(InputAction.CallbackContext context)
        {
            if (!player) return;
            player.input.mousePoint = context.ReadValue<Vector2>();
        }
        public void Shoot(InputAction.CallbackContext context)
        {
            if (!player) return;
            if (context.performed)
                player.input.shootDown = true;
            if (context.canceled)
                player.input.shootDown = false;
        }
        public void Dash(InputAction.CallbackContext context)
        {
            if (!player) return;
            if (context.performed)
                player.input.dashDown = true;
            if (context.canceled)
                player.input.dashDown = false;
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
    }
}
