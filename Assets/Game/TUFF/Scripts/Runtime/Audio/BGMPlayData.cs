using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class BGMPlayData
    {
        [Tooltip("BGM to play.")]
        public BGM bgm;
        [Tooltip("BGM's volume.")]
        public float volume = 1f;
        [Tooltip("BGM's pitch.")]
        public float pitch = 1f;
        [Tooltip("If true, the BGM will play on loop dynamically based on the Loop Start and Loop End values, or by itself. Else it will play once from start to finish.")]
        public bool loop = true;
    }
}
