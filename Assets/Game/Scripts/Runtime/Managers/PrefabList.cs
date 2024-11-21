using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class PrefabList : MonoBehaviour
    {
        public static PrefabList instance { get => BucketsGameManager.instance.prefabList; }

        [Header("Normal Enemies")]
        public Rusher rusherPrefab;
        public Shielder shielderPrefab;
        public Flyer flyerPrefab;
        public Sniper sniperPrefab;
        public BarrelBro barrelBroPrefab;
        public Digger DiggerPrefab;

        [Header("Rando Enemies")]
        public Rusher r_RusherPrefab;
        public Shielder r_ShielderPrefab;
        public Flyer r_FlyerPrefab;
    }
}
