using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class PrefabList : MonoBehaviour
    {
        public static PrefabList instance { get => BucketsGameManager.instance.prefabList; }
        public Rusher rusherPrefab;
        public Shielder shielderPrefab;
        public Flyer flyerPrefab;
        public Sniper sniperPrefab;
    }
}
