using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class AvatarDestroyer : MonoBehaviour
    {
        private void Awake()
        {
            if (FollowerInstance.player != null)
            {
                Destroy(FollowerInstance.player.gameObject);
            }
        }

    }
}
