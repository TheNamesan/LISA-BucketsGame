using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class HideRendererOnStart : MonoBehaviour
    {
        [Tooltip("Reference to the Renderer component.")]
        public Renderer render;
        void Start()
        {
            if(render != null) render.enabled = false;
        }
    }
}
