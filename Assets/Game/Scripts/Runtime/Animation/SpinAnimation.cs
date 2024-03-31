using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class SpinAnimation : MonoBehaviour
    {
        public float speed = 1f;
        public void Update()
        {
            transform.rotation = Quaternion.Euler(0, 0, Time.time * speed);
        }
    }
}

