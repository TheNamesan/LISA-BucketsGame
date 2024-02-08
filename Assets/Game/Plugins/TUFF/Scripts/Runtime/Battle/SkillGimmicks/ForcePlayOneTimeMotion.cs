using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUFF
{

    public class ForcePlayOneTimeMotion : MonoBehaviour
    {
        public List<PlayMotionElement> motions = new List<PlayMotionElement>();

        public void PlayMotionIndex(int index)
        {
            if (index < 0 || index >= motions.Count) return;
            if (motions[index].imageRef == null) return;
            motions[index].Play();
        }
    }

    [System.Serializable]
    public class PlayMotionElement
    {
        public Image imageRef;
        public MotionOneTimeType type = MotionOneTimeType.Twitch;
        public void Play()
        {
            var motion = new TUFFMotion();
            motion.PlayOneTimeMotion(imageRef, type);
        }
    }
}
