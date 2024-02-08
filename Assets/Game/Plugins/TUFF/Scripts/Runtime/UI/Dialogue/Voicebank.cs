using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "Voicebank", menuName = "TUFF/Audio/Voicebank")]
    public class Voicebank : ScriptableObject
    {
        public AudioClip clip;
        public float pitch = 1;
        public float pitchVariation = 0.1f;
    }
}
