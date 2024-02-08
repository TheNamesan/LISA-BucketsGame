using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace TUFF
{
    public class TUFFOutline : Shadow
    {
        [Range(2, 100)]
        public int outlinePrecisionX = 4;
        [Range(2, 100)]
        public int outlinePrecisionY = 4;
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            var verts = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(verts);

            var neededCpacity = verts.Count * 5;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;

            var start = 0;
            var end = verts.Count;

            for(int y = 0; y < outlinePrecisionY; y++)
            {
                for (int x = 0; x < outlinePrecisionX; x++)
                {
                    float distanceX = Mathf.Lerp(-effectDistance.x, effectDistance.x, Mathf.InverseLerp(0, outlinePrecisionX - 1, x));
                    float distanceY = Mathf.Lerp(-effectDistance.y, effectDistance.y, Mathf.InverseLerp(0, outlinePrecisionY - 1, y));
                    ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, distanceX, distanceY);
                    start = end;
                    end = verts.Count;
                }
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
            ListPool<UIVertex>.Release(verts);
        }
    }
}

