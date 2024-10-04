using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class FollowerInstance : SceneCharacter
    {
        public static FollowerInstance instance;
        public static FollowerInstance player { get { return instance; } } // Tmp

        private void Awake()
        {
            if (instance != null)
            { Destroy(gameObject); }
            else
            {
                instance = this;
                PlayerInputHandler.avatar = controller;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
