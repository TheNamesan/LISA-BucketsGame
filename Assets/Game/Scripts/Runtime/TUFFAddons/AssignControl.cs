using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUFF;

namespace BucketsGame
{
    public class AssignControl : MonoBehaviour
    {
        private void Awake()
        {
            if (TryGetComponent(out OverworldCharacterController controller))
            {
                PlayerInputHandler.avatar = controller;
            }
        }
    }
}

