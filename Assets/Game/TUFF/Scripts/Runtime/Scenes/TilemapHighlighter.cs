using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TUFF
{
    public static class TilemapUtilValues
    {
        public static bool hidden = false;
        public static Color tileColor = new Color(0, 1, 1, 0.2f);
        public static Vector2 tileSize = Vector2.one;
        public static Tilemap tilemap;

        public const float defaultColorR = 0f;
        public const float defaultColorG = 1f;
        public const float defaultColorB = 1f;
        public const float defaultColorA = 0.2f;
    }
    
    public class TilemapHighlighter : MonoBehaviour
    {
        public static TilemapHighlighter instance;
        [ExecuteInEditMode]
        public void OnEnable()
        {
            if (instance == null) instance = this;
        }
        public void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (TilemapUtilValues.tilemap != null && !TilemapUtilValues.hidden && enabled)
            {
                var oldColor = Gizmos.color;
                Gizmos.color = TilemapUtilValues.tileColor;
                Gizmos.DrawCube(transform.position, Vector2.one);

                BoundsInt bounds = TilemapUtilValues.tilemap.cellBounds;
                foreach (var position in bounds.allPositionsWithin)
                {
                    Vector3Int cellPosition = new Vector3Int(position.x, position.y, position.z);

                    if (TilemapUtilValues.tilemap.HasTile(cellPosition))
                    {
                        Vector3 tilePosition = TilemapUtilValues.tilemap.GetCellCenterWorld(cellPosition);
                        Gizmos.DrawCube(tilePosition, TilemapUtilValues.tileSize);
                    }
                }
                Gizmos.color = oldColor;
            }
#endif
        }
    }
}

