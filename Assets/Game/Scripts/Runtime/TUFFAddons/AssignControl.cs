using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUFF;

namespace BucketsGame
{
    public class AssignControl : MonoBehaviour
    {
        //private void Start()
        //{
        //    if (TryGetComponent(out OverworldCharacterController controller))
        //    {
        //        if (PlayerInputHandler.avatar == null)
        //            PlayerInputHandler.avatar = controller;
        //    }
        //}
        private void OnEnable()
        {
            if (TryGetComponent(out OverworldCharacterController controller))
            {
                if (PlayerInputHandler.avatar == null)
                    PlayerInputHandler.avatar = controller;
            }
        }
        private void OnDisable()
        {
            if (TryGetComponent(out OverworldCharacterController controller))
            {
                if (PlayerInputHandler.avatar == controller)
                    PlayerInputHandler.avatar = null;
            }
        }
    }
}

