using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class NextRoomCheck : MonoBehaviour
    {
        public Collider2D col;

        public void FixedUpdate()
        {
            if (SceneProperties.instance)
            {
                col.enabled = SceneProperties.instance.nextRoomAvailable;
            }
        }
    }
}
