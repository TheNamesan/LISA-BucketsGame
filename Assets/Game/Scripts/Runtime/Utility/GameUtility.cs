using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BucketsGame
{
    public static class GameUtility
    {
        public static void KillTween(ref Tween tween)
        {
            tween?.Kill();
            tween = null;
        }
    }
}
