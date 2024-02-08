using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class StateVisual : MonoBehaviour
    {
        [Tooltip("Set all values to 1 if no coloration.")]
        public Color graphicColoration = Color.white;
        public Sprite graphicOverlay = null;

        public ActiveState activeState = null;
        public GraphicHandler graphicHandler;

        public void InitializeVisual(ActiveState activeState)
        {
            this.activeState = activeState;
            graphicHandler = activeState.user.imageReference;
        }
    }
}
