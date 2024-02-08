using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TUFF
{
    public class EnemyBarHandler : BarHandler
    {
        public ActiveStatesHUD statesHUD;
        [HideInInspector] public BattleHUD hudCallback;
        [HideInInspector] public EnemyInstance target;
        private const float depleteDuration = 0.5f;
        private const float secondsToWaitToDestroy = 1f;
        private Tween tween;
        private IEnumerator destroyCoroutine = null;
        private float currentValue;
        private float targetValue;
        private float targetMaxValue;
        private string format = null;
        private bool forceShow = false;
        public void ShowBar(EnemyInstance target, BattleHUD hudCallback, bool infDisplayTime = false, string format = "F0")
        {
            this.target = target;
            this.hudCallback = hudCallback;
            this.forceShow = infDisplayTime;
            this.format = format;

            var rect = GetComponent<RectTransform>();
            rect.position = target.imageReference.GetOverlayPosition();
            rect.anchoredPosition -= Vector2.up * 250f;

            targetValue = target.HP;
            targetMaxValue = target.GetMaxHP();
            float prevValue = target.prevHP;
            
            if(destroyCoroutine != null) StopCoroutine(destroyCoroutine);
            destroyCoroutine = WaitToDestroy();
            if (tween != null)
            {
                prevValue = currentValue;
                tween?.Kill();
                tween = null;
            }
            else
            {
                statesHUD.InitializeHUD();
            }
            statesHUD.UpdateStates(target.states);
            if (infDisplayTime)
            {
                OnUpdate(targetValue);
            }
            else
            {
                OnUpdate(prevValue);
                tween = DOTween
                    .To(val => OnUpdate(val), prevValue, targetValue, depleteDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => {
                        DestroyBar();
                    });
            }    
        }
        public void QuickUpdate()
        {
            if (target == null) return;
            statesHUD.UpdateStates(target.states);
        }
        private void OnUpdate(float currentValue)
        {
            fill.fillAmount = Mathf.Clamp(currentValue / targetMaxValue, 0f, 1f);
            valueText.text = LISAUtility.FloatToString(currentValue, format);
            UpdateFillColor();
            this.currentValue = currentValue;
        }
        private void DestroyBar()
        {
            if(!forceShow) StartCoroutine(destroyCoroutine);
        }
        private IEnumerator WaitToDestroy()
        {
            yield return new WaitForSeconds(secondsToWaitToDestroy);
            tween?.Kill();
            tween = null;
            hudCallback.RemoveEnemyHPBar(this);
        }
        private void OnDestroy()
        {
            tween?.Kill();
            tween = null;
        }
    }
}
