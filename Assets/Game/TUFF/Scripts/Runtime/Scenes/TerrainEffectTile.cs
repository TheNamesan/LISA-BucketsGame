using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TUFF
{
    [CreateAssetMenu(fileName = "TerrainEffectTile", menuName = "TUFF/Terrain Effect Tile")]
    public class TerrainEffectTile : Tile
    {
        public TerrainPropertiesData terrainData = null;
    }
}
