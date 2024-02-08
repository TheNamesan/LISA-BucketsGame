using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class AMBSPlayData
    {
        [Tooltip("AMBS to play.")]
        public AudioClip clip;
        [Tooltip("AMBS's volume.")]
        public float volume = 1f;
        [Tooltip("AMBS's pitch.")]
        public float pitch = 1f;
        [Tooltip("If AMBS should loop.")]
        public bool loop = true;
    }
}

