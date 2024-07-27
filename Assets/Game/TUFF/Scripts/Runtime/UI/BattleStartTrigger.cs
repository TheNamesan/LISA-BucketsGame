using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TUFF
{
    public class BattleStartTrigger : MonoBehaviour
    {
        private Tween tween;

        private const float startSize = 0.25f;
        private const float finalSize = 4.5f;
        private const float duration = 1f;

        public bool isFinished = false;
        public void TriggerBattleStart()
        {
            isFinished = false;
            gameObject.SetActive(true);
            AudioManager.instance.PlaySFX(TUFFSettings.battleStartSFX);
            transform.localScale = new Vector3(startSize, startSize, transform.localScale.z);
            if (tween != null) KillTween();
            tween = transform.DOScale(new Vector3(finalSize, finalSize, transform.localScale.z), duration).SetEase(Ease.InSine)
                .OnComplete(() => { isFinished = true; });
        }
        public void HideBattleStart()
        {
            KillTween();
            isFinished = false;
            UIController.instance.UIFadeScreen.FadeIn(0.25f);
            gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            KillTween();
        }
        private void KillTween()
        {
            tween?.Kill();
            tween = null;
        }
    }
}


