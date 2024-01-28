using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class SceneTerrainTracker : MonoBehaviour
    {
        public List<Collider2D> groundColliders = new();

        private void Update()
        {
            for (int c = 0; c < groundColliders.Count; c++)
            {
                if (groundColliders[c] is CompositeCollider2D composite)
                {
                    for (int j = 0; j < composite.pathCount; j++)
                    {
                        Vector2[] points = new Vector2[composite.GetPathPointCount(j)];
                        composite.GetPath(j, points);

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
    }
}
