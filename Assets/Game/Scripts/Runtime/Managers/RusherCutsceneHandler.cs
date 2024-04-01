using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class RusherCutsceneHandler : MonoBehaviour
    {
        public Rusher target;

        public void Start()
        {
            if (target)
            {
                if (BucketsGameManager.IsRusher())
                    target.hp = 999;
            }
        }
    }
}

