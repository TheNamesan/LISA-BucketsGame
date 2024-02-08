using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    public class Battle : MonoBehaviour
    {
        [Tooltip("Reference to the background GameObject.")]
        public Image background;
        [Tooltip("The list of Initial Enemies when the Battle starts.")]
        public List<EnemyReference> initialEnemies = new List<EnemyReference>();
        [Header("Battle Events")]
        public BattleEvent[] battleEvents = new BattleEvent[0];
        [Header("BGM")]
        public bool autoPlayBGM = false;
        public BGMPlayData bgm;
    }
}

