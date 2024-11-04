using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LISATheFool
{
    public class LightFlicker : MonoBehaviour
    {
        public Light2D lightReference;
        public float intensityRange = 0.05f;
        public float intensityMinTime = 0.05f;
        public float intensityMaxTime = 0.125f;

        private float baseIntensity = 1f;

        private void OnEnable()
        {
            if (lightReference == null) lightReference = GetComponent<Light2D>();
            if (lightReference == null) return;
            baseIntensity = lightReference.intensity;
            StartCoroutine(Flicker());
        }

        private IEnumerator Flicker()
        {
            while (true)
            {
                lightReference.intensity = Random.Range(baseIntensity - intensityRange, baseIntensity + intensityRange);
                yield return new WaitForSeconds(Random.Range(intensityMinTime, intensityMaxTime));
            }
        }
    }

}
