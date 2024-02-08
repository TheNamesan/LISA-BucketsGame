using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace TUFF
{
    public class PartyEXPReadoutElement : MonoBehaviour
    {
        protected bool m_animIsFinished = false;
        public bool animIsFinished { get { return m_animIsFinished; } }

        protected bool m_animIsRunning = false;
        public bool animIsRunning { get { return m_animIsRunning; } }
        public Image unitGraphic;
        public TMP_Text nameText;
        public TMP_Text expText;
        public BarHandler bar;

        private Tween tween;
        private float animDuration = 0.75f;

        public void InitiateAnimation(PartyMember member)
        {
            unitGraphic.sprite = member.GetGraphic();
            nameText.text = member.GetName();
            expText.text = $"+{LISAUtility.IntToString(BattleManager.instance.expCollected)}{TUFFSettings.expText}";
            float currentValue = PartyMember.GetNextLevelProgress(member.prevLevel, member.job, member.prevExp);
            float targetValue = PartyMember.GetNextLevelProgress(member.prevLevel, member.job, member.exp);
            KillTween();
            OnUpdate(currentValue);
            m_animIsRunning = false;
            tween = DOTween
                .To(val => OnUpdate(val), currentValue, targetValue, animDuration)
                .SetEase(Ease.InQuad)
                .SetDelay(0.5f)
                .OnComplete(() => {
                    m_animIsFinished = true;
                    m_animIsRunning = false;
                });
        }
        public void StopAnimation()
        {
            tween?.Complete();
            KillTween();
        }
        private void OnUpdate(float fillAmount)
        {
            if(fillAmount < 1f) bar.SetValue(fillAmount, $"{LISAUtility.FloatToString(fillAmount * 100f, "F2")}%");
            else bar.SetValue(fillAmount, $"{TUFFSettings.levelUpText}");
            m_animIsRunning = true;
        }
        private void OnDestroy()
        {
            KillTween();
        }
        protected void KillTween()
        {
            tween?.Kill();
            tween = null;
        }
    }
}

