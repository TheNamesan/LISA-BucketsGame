using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace TUFF
{
    public class TerrainProperties : MonoBehaviour
    {
        public bool playerBulletsGoThrough = false;
        public bool enemyBulletsGoThrough = false;
        public bool nonClimbable = false;

        public TerrainPropertiesData propertiesData = null;
        public UnityEvent onStepEvent = new UnityEvent();
        public Tilemap tilemap;
        [HideInInspector] public ParticleSystem stepEffect;
        [HideInInspector] public Dictionary<TerrainPropertiesData, ParticleSystem> stepEffects = new Dictionary<TerrainPropertiesData, ParticleSystem>();

        //private PlatformEffector2D platformEffector2D;
        private void Awake()
        {
            if (tilemap == null) tilemap = GetComponent<Tilemap>();
            //if (!platformEffector2D) platformEffector2D = GetComponent<PlatformEffector2D>();
        }
        private void FixedUpdate()
        {
            //if (platformEffector2D) platformEffector2D.surfaceArc = 180;
        }
        void Start()
        {
            if(propertiesData != null)
            {
                if (propertiesData.stepEffectPrefab != null)
                    stepEffect = Instantiate(propertiesData.stepEffectPrefab, transform);
            }
        }
        public void OnEnter(OverworldCharacterController collision)
        {
            if (collision == null) return;
            if (propertiesData == null) return;
            if (propertiesData.changeAnimationPackAlt) // If stepping on this changes the animation pack alt
                collision.animHandler.UsePackAlt(propertiesData.animationPackAltIndex); // Assign Alt
            if (propertiesData.overrideClimbMode)
                collision.ChangeClimbMode(propertiesData.climbMode);
        }
        public void Step(Vector3 position)
        {
            Step(position, new Vector3Int(0, 0, 0));
        }
        public void Step(Vector3 position, Vector3Int coordinatesOffset)
        {
            var propertiesData = (GetPropertiesDataFromPosition(position, coordinatesOffset) ?? this.propertiesData);
            if (propertiesData != null)
            {
                if (propertiesData.stepSFXs.Count > 0)
                {
                    int index = Random.Range(0, propertiesData.stepSFXs.Count);
                    var sfx = propertiesData.stepSFXs[index];
                    var pitchVar = propertiesData.stepPitchVariation;
                    AudioManager.instance.PlaySFX(sfx.GetAudioClip(), sfx.GetVolume(), sfx.GetPitch() + Random.Range(-pitchVar, pitchVar));
                }
                if (propertiesData.stepEffectPrefab != null)
                {
                    if (!stepEffects.ContainsKey(propertiesData))
                    {
                        var stepEffect = Instantiate(propertiesData.stepEffectPrefab, transform);
                        stepEffects.Add(propertiesData, stepEffect);
                    }
                    stepEffects[propertiesData].transform.position = position;
                    stepEffects[propertiesData].Play();
                }
                onStepEvent?.Invoke();
            }
        }
        public TerrainPropertiesData GetPropertiesDataFromPosition(Vector3 position, Vector3Int coordinatesOffset)
        {
            if (tilemap == null) return null;
            var coordinate = tilemap.WorldToCell(position);
            coordinate += coordinatesOffset;
            var tile = tilemap.GetTile(coordinate);
            if (tile is TerrainEffectTile ter)
            {
                return ter.terrainData;
            }
            return null;
        }
        public void WallHit(bool ignoreSFX = false)
        {
            if (propertiesData)
            {
                if (propertiesData.wallHitSFXs.Count > 0)
                {
                    int index = Random.Range(0, propertiesData.wallHitSFXs.Count);
                    var sfx = propertiesData.wallHitSFXs[index];
                    var pitchVar = propertiesData.wallHitVariation;
                    if (!ignoreSFX)
                        AudioManager.instance.PlaySFX(sfx.audioClip, sfx.volume, sfx.pitch + Random.Range(-pitchVar, pitchVar));
                }
            }
        }
    }
}

