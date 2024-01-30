using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{

    public class HideRenderer : MonoBehaviour
    {
        public Renderer render;
        void Start()
        {
            if (render) render.enabled = false;
        }
    }
}
