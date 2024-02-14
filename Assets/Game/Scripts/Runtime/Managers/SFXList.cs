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
        public SFX landSFX = new();
        public SFX jumpSFX = new();
        public SFX wallJumpSFX = new();
        public SFX playerDeadSFX = new();
        public SFX adrenalineActiveSFX = new();
        public SFX barrelBroWallHit = new();
    }
}
