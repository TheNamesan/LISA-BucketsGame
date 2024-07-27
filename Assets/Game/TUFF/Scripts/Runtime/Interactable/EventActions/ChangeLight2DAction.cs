using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TUFF
{
    public class ChangeLight2DAction : EventAction
    {
        public Light2D light2D;

        [Header("Enabled")]
        public bool keepEnabled = true;
        public bool enabled = false;

        [Header("Color")]
        public bool keepColor = true;
        public Color color = Color.white;

        [Header("Intensity")]
        public bool keepIntensity = true;
        public float intensity = 1;
        public ChangeLight2DAction()
        {
            eventName = "Change Light 2D";
            eventColor = EventGUIColors.character;
        }
        public override void Invoke()
        {
            if (light2D)
            {
                if (!keepEnabled)
                {
                    light2D.enabled = enabled;
                }
                if (!keepColor)
                {
                    light2D.color = color;
                }
                if (!keepIntensity)
                {
                    light2D.intensity = intensity;
                }
            }
            isFinished = true;
        }
    }
}
