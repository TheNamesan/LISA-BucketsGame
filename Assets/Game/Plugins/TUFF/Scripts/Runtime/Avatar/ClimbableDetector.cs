using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ClimbableDetector : MonoBehaviour
    {
        public LayerMask climbableLayers;
        public Collider2D col;
        public Collider2D climbable;
        public Bounds currentBounds = new Bounds();
        public TerrainProperties terrainProperties;
        public bool ropeContact = false;
        public Vector2[] climbablePoints = new Vector2[4];
        public Vector2[] lastClimbPoints = new Vector2[4];
        public Vector2 pointsCenter;
    
        private void Awake()
        {
            col = GetComponent<Collider2D>();
        }
        private void FixedUpdate()
        {
            currentBounds = col.bounds;
            Vector2 size = new Vector2(currentBounds.size.x, currentBounds.size.y);
            RaycastHit2D collision = Physics2D.BoxCast(currentBounds.center, size, 0f, Vector2.down, 0f, climbableLayers);
            if (collision) //OnTriggerEnter / stay
            {
                if (collision.collider is CompositeCollider2D composite)
                {
                    for (int i = 0; i < composite.pathCount; i++)
                    {
                        Vector2[] points = new Vector2[composite.GetPathPointCount(i)];
                        composite.GetPath(i, points);

                        for (int p = 0; p < points.Length; p++)
                        {
                            points[p] += (Vector2)composite.transform.position;
                        }

                        Vector2 center = Vector2.Lerp(points[2], points[0], 0.5f);
                        Bounds bound = new Bounds(center, new Vector2(Vector2.Distance(points[3], points[0]) + 0.15f, Vector2.Distance(points[3], points[2]) + 0.15f));

                        if (bound.Intersects(currentBounds))
                        {
                            Debug.DrawLine(points[0], points[1], Color.red, 1f);
                            Debug.DrawLine(points[1], points[2], Color.red, 1f);
                            Debug.DrawLine(points[2], points[3], Color.red, 1f);
                            Debug.DrawLine(points[3], points[0], Color.red, 1f);
                            climbablePoints = new Vector2[composite.GetPathPointCount(i)];
                            System.Array.Copy(points, climbablePoints, points.Length);
                            pointsCenter = center;
                            climbable = composite;
                        }
                    }
                    collision.collider?.TryGetComponent(out terrainProperties);
                }
            }
            else
            {
                climbable = null;
                terrainProperties = null;
                climbablePoints = new Vector2[0];
                pointsCenter = Vector2.zero;
            }
        }

        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if (((1 << collision.gameObject.layer) & climbableLayers) != 0)
        //    {
        //        if (collision is CompositeCollider2D)
        //        {
        //            CompositeCollider2D composite = (CompositeCollider2D)collision;
        //            for (int i = 0; i < composite.pathCount; i++)
        //            {
        //                Vector2[] points = new Vector2[composite.GetPathPointCount(i)];
        //                composite.GetPath(i, points);

        //                for (int p = 0; p < points.Length; p++)
        //                {
        //                    points[p] += (Vector2)composite.transform.position;
        //                }

        //                Vector2 center = Vector2.Lerp(points[2], points[0], 0.5f);
        //                Bounds bound = new Bounds(center, new Vector2(Vector2.Distance(points[3], points[0]) + 0.15f, Vector2.Distance(points[3], points[2]) + 0.15f));

        //                if (bound.Intersects(col.bounds))
        //                {
        //                    Debug.DrawLine(points[0], points[1], Color.red, 1f);
        //                    Debug.DrawLine(points[1], points[2], Color.red, 1f);
        //                    Debug.DrawLine(points[2], points[3], Color.red, 1f);
        //                    Debug.DrawLine(points[3], points[0], Color.red, 1f);
        //                    climbablePoints = new Vector2[composite.GetPathPointCount(i)];
        //                    //System.Array.Copy(climbablePoints, lastClimbPoints, climbablePoints.Length);
        //                    System.Array.Copy(points, climbablePoints, points.Length);
        //                    pointsCenter = center;
        //                    climbable = composite;
        //                }
        //            }
        //            collision?.TryGetComponent(out terrainProperties);
        //        }
        //    }
        //}

        //private void OnTriggerStay2D(Collider2D collision)
        //{
        //    if (climbable != null && climbable == collision) // if climbable is still last collision
        //    {
        //        if(climbable is CompositeCollider2D)
        //        {
        //            CompositeCollider2D composite = (CompositeCollider2D)collision;
        //            for (int i = 0; i < composite.pathCount; i++)
        //            {
        //                Vector2[] points = new Vector2[composite.GetPathPointCount(i)];
        //                composite.GetPath(i, points);

        //                for (int p = 0; p < points.Length; p++)
        //                {
        //                    points[p] += (Vector2)composite.transform.position;
        //                }
        //                Debug.DrawLine(points[0], points[1], Color.cyan);
        //                Debug.DrawLine(points[1], points[2], Color.cyan);
        //                Debug.DrawLine(points[2], points[3], Color.cyan);
        //                Debug.DrawLine(points[3], points[0], Color.cyan);

        //                Vector2 center = Vector2.Lerp(points[2], points[0], 0.5f);
        //                Bounds bound = new Bounds(center, new Vector2(Vector2.Distance(points[3], points[0]) + 0.15f, Vector2.Distance(points[3], points[2]) + 0.15f));

        //                if (bound.Intersects(col.bounds))
        //                {
        //                    Debug.DrawLine(points[0], points[1], Color.red);
        //                    Debug.DrawLine(points[1], points[2], Color.red);
        //                    Debug.DrawLine(points[2], points[3], Color.red);
        //                    Debug.DrawLine(points[3], points[0], Color.red);
        //                    climbablePoints = new Vector2[composite.GetPathPointCount(i)];

        //                    System.Array.Copy(points, climbablePoints, points.Length); // Update points
        //                    pointsCenter = center;
        //                    climbable = composite;
        //                }
        //            }
        //        }
        //    }
        //}

        //private void OnTriggerExit2D(Collider2D collision)
        //{
        //    if (climbable != null)
        //    {
        //        if(climbable == collision)
        //        {
        //            climbable = null;
        //            terrainProperties = null;
        //            //System.Array.Copy(climbablePoints, lastClimbPoints, climbablePoints.Length);
        //            climbablePoints = new Vector2[0];
        //            pointsCenter = Vector2.zero;
        //        }
        //    }
        //}

        public bool IntersectsLastClimbPoints()
        {
            if (climbable == null) return false;
            if (lastClimbPoints.Length < 3) return false;
            Vector2[] points = lastClimbPoints;
            Vector2 center = Vector2.Lerp(points[2], points[0], 0.5f);
            Bounds bound = new Bounds(center, new Vector2(
                Vector2.Distance(points[3], points[0]) + (currentBounds.size.x * 1F) + Physics2D.defaultContactOffset, 
                Vector2.Distance(points[3], points[2]) + (currentBounds.size.y * 1F) + Physics2D.defaultContactOffset));
            Color color = Color.magenta;

            Debug.DrawLine(new Vector2(center.x + bound.extents.x, center.y - bound.extents.y),
                new Vector2(center.x + bound.extents.x, center.y + bound.extents.y), color, 1f);
            Debug.DrawLine(new Vector2(center.x + bound.extents.x, center.y + bound.extents.y),
                new Vector2(center.x - bound.extents.x, center.y + bound.extents.y), color, 1f);
            Debug.DrawLine(new Vector2(center.x - bound.extents.x, center.y + bound.extents.y),
                new Vector2(center.x - bound.extents.x, center.y - bound.extents.y), color, 1f);
            Debug.DrawLine(new Vector2(center.x - bound.extents.x, center.y - bound.extents.y),
                new Vector2(center.x + bound.extents.x, center.y - bound.extents.y), color, 1f);

            bool intersects = bound.Intersects(currentBounds);

            center = currentBounds.center;
            bound = currentBounds;
            Debug.DrawLine(new Vector2(center.x + bound.extents.x, center.y - bound.extents.y),
                new Vector2(center.x + bound.extents.x, center.y + bound.extents.y), Color.yellow, 1f);
            Debug.DrawLine(new Vector2(center.x + bound.extents.x, center.y + bound.extents.y),
                new Vector2(center.x - bound.extents.x, center.y + bound.extents.y), Color.yellow, 1f);
            Debug.DrawLine(new Vector2(center.x - bound.extents.x, center.y + bound.extents.y),
                new Vector2(center.x - bound.extents.x, center.y - bound.extents.y), Color.yellow, 1f);
            Debug.DrawLine(new Vector2(center.x - bound.extents.x, center.y - bound.extents.y),
                new Vector2(center.x + bound.extents.x, center.y - bound.extents.y), Color.yellow, 1f);

            
            //if (!intersects) Debug.LogError(intersects);
            //Debug.Log(intersects);
            return intersects;
        }

        public void SetLastPoints()
        {
            System.Array.Copy(climbablePoints, lastClimbPoints, climbablePoints.Length);
        }
    }
}