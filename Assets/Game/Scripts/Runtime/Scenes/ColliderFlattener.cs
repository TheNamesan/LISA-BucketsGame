using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BucketsGame
{
    public class ColliderFlattener : MonoBehaviour
    {
        public TilemapCollider2D tilemapCol;
        public CompositeCollider2D composite;
        public PolygonCollider2D polygon;

        private void OnEnable()
        {
            CheckTilemapShapes();   
        }
        private void CheckTilemapShapes()
        {
            if (!composite) return;

            polygon.pathCount = composite.pathCount;
            for (int j = 0; j < composite.pathCount; j++)
            {
                Vector2[] points = new Vector2[composite.GetPathPointCount(j)];
                composite.GetPath(j, points);
                polygon.SetPath(j, points);

                for (int p = 0; p < points.Length; p++)
                {
                    points[p] += (Vector2)composite.transform.position;
                }
                for (int p = 0; p < points.Length; p++)
                {
                    if (p == 0) continue;
                    var a = points[p - 1];
                    var b = points[p];
                    var center = Vector2.Lerp(a, b, 0.5f);
                    var distance = b - a;
                    var normal = -Vector2.Perpendicular(distance);

                    if (normal.y > 0.0001f)
                    {
                        Debug.DrawRay(center, normal, Color.white);
                        Debug.DrawLine(a, b, Color.cyan);
                    }
                }
            }
            

            
        }    
    }

}
