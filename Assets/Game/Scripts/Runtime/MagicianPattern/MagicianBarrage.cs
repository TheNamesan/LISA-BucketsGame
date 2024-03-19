using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BucketsGame
{
    public enum BarrageDirection
    {
        RightToLeft = 0,
        LeftToRight = 1
    }
    public class MagicianBarrage : MagicianPattern
    {
        public BarrageDirection barrageDir = BarrageDirection.RightToLeft;
        public List<MagicianBarragePillar> pillars = new();
        public int timeBetweenPillars = 25;
        [SerializeField] private int m_time = 0;
        [SerializeField] private int m_index = 0;
        public void Awake()
        {
            GetPillars();
        }
        public void Start()
        {
            //Play(); // test;
        }
        private void FixedUpdate()
        {
            SpawnPillars();
        }
        public override void Play()
        {
            m_inUse = true;
            gameObject.SetActive(true);
            HideAllPillars();
            m_time = timeBetweenPillars;
            if (barrageDir == BarrageDirection.RightToLeft) m_index = 0;
            else m_index = pillars.Count - 1;
        }
        private void SpawnPillars()
        {
            bool onEndOfList = (barrageDir == BarrageDirection.RightToLeft && m_index >= pillars.Count) 
                || (barrageDir == BarrageDirection.LeftToRight && m_index < 0);
            if (onEndOfList)
            {
                bool finished = AllPillarsFinished();
                if (finished)
                {
                    ReturnToPool();
                }
                return; 
            }
            if (m_time > 0) m_time--;
            else
            {
                if (pillars[m_index]) pillars[m_index].gameObject.SetActive(true);
                m_time = timeBetweenPillars;
                if (barrageDir == BarrageDirection.RightToLeft) m_index++;
                else m_index--;
            }
        }
        private void HideAllPillars()
        {
            foreach (var pil in pillars) 
                if (pil) pil.gameObject.SetActive(false);
        }
        private bool AllPillarsFinished()
        {
            for (int i = 0; i < pillars.Count; i++)
            {
                if (!pillars[i]) continue;
                if (pillars[i].gameObject.activeInHierarchy) return false;
            }
            return true;
        }
        private void GetPillars()
        {
            //Debug.LogWarning("GAA");
            var p = GetComponentsInChildren<MagicianBarragePillar>(true);
            //Debug.Log(p.Length);
            for (int i = 0; i < p.Length; i++)
                pillars.Add(p[i]);
        }
    }
}

