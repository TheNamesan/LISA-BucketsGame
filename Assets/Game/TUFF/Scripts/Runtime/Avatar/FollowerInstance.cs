using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class FollowerInstance : SceneCharacter
    {
        public static FollowerInstance instance;
        public static FollowerInstance player { get { return instance; } } // Tmp

        private void Awake()
        {
            //if (instance != null)
            //{ Destroy(gameObject); }
            //else
            //{
            //    instance = this;
            //    Debug.Log("Hi", this.gameObject);
            //    DontDestroyOnLoad(gameObject);
            //}
        }
        public void ChangeAnimationPack(AnimationPack animationPack)
        {
            charAnim?.LoadAnimationPack(animationPack);
        }
        public void PlayAnimation(AnimationClip animationClip)
        {
            charAnim?.PlayAnimation(animationClip);
        }
        public void ChangeSprite(Sprite sprite)
        {
            charAnim?.ChangeSprite(sprite);
        }
    }
}
