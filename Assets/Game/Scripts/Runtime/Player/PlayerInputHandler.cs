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
                if (value > 0) player.input.jumpPress = true;
            }
        }
        public void Pointer(InputAction.CallbackContext context)
        {
            if (!player) return;
            player.input.mousePoint = context.ReadValue<Vector2>();
            //Debug.Log("Mouse Position: " + player.input.mousePoint);
            //Debug.Log("Mouse Position in World: " + SceneProperties.instance.camManager.cam.ScreenToWorldPoint(player.input.mousePoint));
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

        public bool jumpPress;
    }
}
