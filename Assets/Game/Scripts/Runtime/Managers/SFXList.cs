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
        public SFX enemyHitSFX = new();
        public SFX enemyDeadSFX = new();
        public SFX adrenalineActiveSFX = new();
        public SFX barrelBroWallHit = new();
        public SFX doorOpenSFX = new();
        public SFX rusherPunchSFX = new();
        public SFX rusherPunchHitSFX = new();
        public SFX shielderShootSFX = new();
        public SFX shielderBlockSFX = new();
        public SFX shielderShoveSFX = new();
        public SFX shielderShoveHitSFX = new();
        public SFX flyerShootSFX = new();
        public SFX flyerShootHitSFX = new();
        public SFX magicianPortalSFX = new();
        public SFX magicianGroundSFX = new();
        public SFX magicianBarrageSFX = new();
        public SFX magicianBulletHitSFX = new();
        public SFX firebombHitSFX = new();
        public SFX roomClearedSFX = new();
        public SFX sniperFiringSFX = new();
    }
}
