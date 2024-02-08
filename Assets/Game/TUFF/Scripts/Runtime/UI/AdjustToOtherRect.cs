using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{
    [ExecuteInEditMode]
    public class AdjustToOtherRect : MonoBehaviour
    {
        public RectTransform originRect;
        public RectTransform destinationRect;
        public float minWidth = 0;
        public float minHeight = 0;
        public Vector2 offset = new Vector2();
        public Vector2 padding = new Vector2();
        // Start is called before the first frame update
        void Start()
        {
            Adjust();
        }

        void LateUpdate()
        {
            Adjust();
        }
        public void Adjust()
        {
            if (originRect == null || destinationRect == null) return;
            float sizeX = Mathf.Max(minWidth, originRect.sizeDelta.x + padding.x * 2) + offset.x;
            float sizeY = Mathf.Max(minHeight, originRect.sizeDelta.y + padding.y * 2) + offset.y;
            destinationRect.sizeDelta = new Vector2(sizeX, sizeY);
        }
        public Vector2 GetSizeWithoutOffset()
        {
            if (originRect == null || destinationRect == null) return Vector2.zero;
            float sizeX = Mathf.Max(minWidth, originRect.sizeDelta.x + padding.x * 2);
            float sizeY = Mathf.Max(minHeight, originRect.sizeDelta.y + padding.y * 2);
            return new Vector2(sizeX, sizeY);
        }
    }
}
