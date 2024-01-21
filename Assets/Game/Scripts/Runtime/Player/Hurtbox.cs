using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Hurtbox : MonoBehaviour
    {
        public PlayerController callback;
        public BoxCollider2D col;
        public Team team;
        public void Collision(Vector2 impactPoint)
        {
            callback?.Hurt(impactPoint);
        }
    }
}

