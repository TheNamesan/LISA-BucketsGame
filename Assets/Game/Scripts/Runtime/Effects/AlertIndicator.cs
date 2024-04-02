using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class AlertIndicator : MonoBehaviour
    {
        public Enemy enemy;
        public SpriteRenderer spriteRenderer;
        public List<Sprite> sprites = new();
        public float timeIntervals = 0.1f;
        
        [SerializeField] private float m_time = 0;
        [SerializeField] private int m_index = 0;

        public void OnEnable()
        {
            m_time = timeIntervals;
            UpdateSprite();
        }
        public void Update()
        {
            ShowIndicator();
        }
        private void ShowIndicator()
        {
            if (!enemy) return;
            if (!spriteRenderer) return;
            if (enemy.enemyState == EnemyAIState.Alert && !enemy.dead)
            {
                spriteRenderer.enabled = true;
                m_time += Time.deltaTime;
                if (m_time >= timeIntervals)
                {
                    m_time = 0;
                    bool updated = UpdateSprite();
                    if (updated) m_index++;
                }
            }
            else
            { 
                spriteRenderer.enabled = false;
                m_index = 0;
            }
        }
        private bool UpdateSprite()
        {
            if (!spriteRenderer) return false;
            if (m_index >= sprites.Count) return false;
            spriteRenderer.sprite = sprites[m_index];
            return true;
        }
    }

}
