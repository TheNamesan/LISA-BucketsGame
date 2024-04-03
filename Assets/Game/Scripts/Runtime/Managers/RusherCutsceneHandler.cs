using UnityEngine;

namespace BucketsGame
{
    public class RusherCutsceneHandler : MonoBehaviour
    {
        public Rusher target;
        public bool setActive = true;
        public GameObject[] objectsToToggleWhenRusher = new GameObject[0];

        public void Awake()
        {
            A_CheckForRusherMode();
        }

        private void A_CheckForRusherMode()
        {
            if (BucketsGameManager.IsRusher())
            {
                for (int i = 0; i < objectsToToggleWhenRusher.Length; i++)
                {
                    var obj = objectsToToggleWhenRusher[i];
                    if (obj) obj.SetActive(setActive);
                }
            }
        }

        public void Start()
        {
            if (target)
            {
                if (BucketsGameManager.IsRusher())
                    target.hp = 999;
            }
        }
    }
}

