using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TUFF;

namespace BucketsGame
{
    public class SFXList : MonoBehaviour
    {
        public static SFXList instance { get => BucketsGameManager.instance.sfxs; }
        public SFX pistolShotSFX = new();
        public SFX dashSFX = new();
    }
}
