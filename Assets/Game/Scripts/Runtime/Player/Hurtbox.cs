using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class Hurtbox : MonoBehaviour
    {
        public MovingEntity callback;
        public BoxCollider2D col;
        public Team team;
        public bool invulnerable { get => m_invulnerable; }
        [SerializeField] private bool m_invulnerable = false;
        public bool Collision(Vector2 launchDir)
        {
            if (!callback) return false;
            return callback.Hurt(launchDir);
        }
        public void SetInvulnerable(bool value)
        {
            m_invulnerable = value;
        }
    }
}

