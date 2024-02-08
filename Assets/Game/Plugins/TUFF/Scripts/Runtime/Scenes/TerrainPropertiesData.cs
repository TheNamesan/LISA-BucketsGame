using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [CreateAssetMenu(fileName = "TerrainPropertyData", menuName = "TUFF/Terrain Properties Data")]
    public class TerrainPropertiesData : ScriptableObject
    {
        public List<SFX> stepSFXs = new List<SFX>();
        public float stepPitchVariation = 0.4f;
        public ParticleSystem stepEffectPrefab = null;

        [Header("Character")]
        public bool changeAnimationPackAlt = false;
        public int animationPackAltIndex = 0;
        public bool overrideClimbMode = false;
        public CharacterClimbMode climbMode = CharacterClimbMode.Default;
    }
}
