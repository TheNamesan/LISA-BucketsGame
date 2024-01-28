using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

namespace BucketsGame
{
    public class BackgroundColorHandler : MonoBehaviour
    {
        public Tween colorTween;
        public Tilemap tilemap;
        public Color targetColor = Color.black;
        public float duration = 0.25f;
        private Color m_originalColor;

        private void Awake()
        {
            if (!tilemap) tilemap = GetComponent<Tilemap>();
            if (tilemap)
            {
                m_originalColor = tilemap.color;
            }
        }
        private void Update()
        {
            ColorUpdate();
        }
        private void ColorUpdate()
        {
            if (GameManager.instance.focusMode)
            {
                PlayTween();
            }
            else if (colorTween != null)
            {
                PlayTween(true);
            }
        }
        public void PlayTween(bool backwards = false)
        {
            if (colorTween == null) GenerateTween();
            if (colorTween == null) return;
            if (backwards)
            {
                if ((colorTween.IsPlaying() && colorTween.isBackwards)
                    || colorTween.fullPosition <= 0) return;
                Debug.Log("Playing Backwards");
                colorTween.PlayBackwards();
            }
            else
            {
                if ((colorTween.IsPlaying() && !colorTween.isBackwards)
                    || colorTween.IsComplete()) return;
                Debug.Log("Playing");
                colorTween.PlayForward();
            }
        }
        private void GenerateTween()
        {
            if (!tilemap) return;
            colorTween = DOTween.To(() => tilemap.color, value => tilemap.color = value, targetColor, duration)
                .From(m_originalColor)
                .SetUpdate(true)
                .SetAutoKill(false);
        }
    }
}
