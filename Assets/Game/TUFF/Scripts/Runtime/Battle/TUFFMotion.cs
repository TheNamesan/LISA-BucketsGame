using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TUFF
{
    public enum MotionType
    {
        Appear = 0,
        Idle = 1,
        KO = 2,
        OneTime = 3
    }
    public enum MotionKOType
    {
        None = 0,
        FadeOut = 1,
        Custom = 2
    }
    public enum MotionOneTimeType
    {
        None = 0,
        TwitchLight = 1,
        Twitch = 2,
        TwitchIntense = 3,
        FrontAction = 4,
        RoundLeft = 5,
        RoundRight = 6
    }
    public class TUFFMotion
    {
        public bool isFinished = false;
        protected Tween oneTimeTween;
        protected Sequence oneTimeSequence;
        protected Vector2 orgPosition;
        protected Image curImgRef;
        public void PlayOneTimeMotion(Image imgRef, MotionOneTimeType type)
        {
            BattleManager.instance.StartCoroutine(PlayDefaultOneTimeMotion(imgRef, type));
        }
        protected IEnumerator PlayDefaultOneTimeMotion(Image imgRef, MotionOneTimeType type)
        {
            if (curImgRef != imgRef)
            {
                curImgRef = imgRef;
                orgPosition = imgRef.transform.position;
                Debug.Log("Reassigned position");
            }
            KillTween();
            switch (type)
            {
                case MotionOneTimeType.TwitchLight:
                    oneTimeTween = imgRef.transform.DOShakePosition(0.15f, new Vector2(10, 0), 20, 90, false, false)
                        .OnComplete(() => { imgRef.transform.position = orgPosition; });
                    break;
                case MotionOneTimeType.Twitch:
                    oneTimeTween = imgRef.transform.DOShakePosition(0.15f, new Vector2(25, 0), 25, 90, false, false)
                        .OnComplete(() => { imgRef.transform.position = orgPosition; } ) ;
                    break;
                case MotionOneTimeType.TwitchIntense:
                    Debug.Log("Position: " + orgPosition);
                    oneTimeTween = imgRef.transform.DOShakePosition(0.65f, new Vector2(25, 0), 50, 90, false, false)
                        .OnComplete(() => { imgRef.transform.position = orgPosition; });
                    break;
                case MotionOneTimeType.FrontAction:
                    oneTimeTween = imgRef.transform.DOScale(1.5f, 0.25f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
                    break;
                case MotionOneTimeType.RoundLeft:
                    RoundLeft(imgRef);
                    break;
                case MotionOneTimeType.RoundRight:
                    RoundRight(imgRef);
                    break;

                default:
                    break;
            }
            yield break;
        }

        private void RoundLeft(Image imgRef)
        {
            var move1 = new Vector3(orgPosition.x - 235, orgPosition.y - 235);
            var move2 = new Vector3(orgPosition.x + 235, orgPosition.y - 235);
            var move3 = new Vector3(orgPosition.x, orgPosition.y);
            var duration = 0.25f;
            var orgScale = imgRef.transform.localScale;
            var flip = new Vector3(orgScale.x * -1, orgScale.y, orgScale.z);
            oneTimeSequence = DOTween.Sequence();
            oneTimeSequence.Append(imgRef.rectTransform.DOMove(move1, duration).SetEase(Ease.Linear));
            oneTimeSequence.Append(imgRef.rectTransform.DOMove(move2, duration).SetEase(Ease.Linear).OnStart(() => { imgRef.transform.localScale = flip; }));
            oneTimeSequence.Append(imgRef.rectTransform.DOMove(move3, duration).SetEase(Ease.Linear).OnStart(() => { imgRef.transform.localScale = orgScale; }));
            oneTimeSequence.OnComplete(() =>
            {
                imgRef.transform.position = orgPosition;
                imgRef.transform.localScale = orgScale;
            });
        }
        private void RoundRight(Image imgRef)
        {
            var move1 = new Vector3(orgPosition.x + 235, orgPosition.y - 235);
            var move2 = new Vector3(orgPosition.x - 235, orgPosition.y - 235);
            var move3 = new Vector3(orgPosition.x, orgPosition.y);
            var duration = 0.25f;
            var orgScale = imgRef.transform.localScale;
            var flip = new Vector3(orgScale.x * -1, orgScale.y, orgScale.z);
            oneTimeSequence = DOTween.Sequence();
            oneTimeSequence.Append(imgRef.rectTransform.DOMove(move1, duration).SetEase(Ease.Linear).OnStart(() => { imgRef.transform.localScale = flip; }));
            oneTimeSequence.Append(imgRef.rectTransform.DOMove(move2, duration).SetEase(Ease.Linear).OnStart(() => { imgRef.transform.localScale = orgScale; }));
            oneTimeSequence.Append(imgRef.rectTransform.DOMove(move3, duration).SetEase(Ease.Linear).OnStart(() => { imgRef.transform.localScale = flip; }));
            oneTimeSequence.OnComplete(() =>
            {
                imgRef.transform.position = orgPosition;
                imgRef.transform.localScale = orgScale;
            });
        }

        public void KillTween()
        {
            oneTimeTween?.Kill(true); 
            oneTimeTween = null;
            oneTimeSequence.Kill(true);
            oneTimeSequence = null;
        }
        public void PlayKOMotion(EnemyInstance enemyInstance)
        {
            BattleManager.instance.StartCoroutine(PlayDefaultKOMotion(enemyInstance));
        }
        protected IEnumerator PlayDefaultKOMotion(EnemyInstance enemyInstance)
        {
            var imgRef = enemyInstance.imageReference;
            switch (enemyInstance.enemyRef.KOMotion)
            {
                case MotionKOType.FadeOut:
                    AudioManager.instance.PlaySFX(TUFFSettings.enemyKOSFX);
                    enemyInstance.playedKOAnimation = true;
                    imgRef.userImage.DOFade(0f, 0.5f).From(imgRef.userImage.color.a);
                    isFinished = true;
                    break;
                default:
                    break;
                    
            }
            yield break;
        }

    }
}

