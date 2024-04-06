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
        public bool TryKill(Vector2 launchDir)
        {
            if (!callback) return false;
            launchDir.Normalize();
            return callback.TryKill(launchDir);
        }
        public bool Kill(Vector2 launchDir)
        {
            if (!callback) return false;
            launchDir.Normalize();
            return callback.Kill(launchDir);
        }
        public bool Collision(Vector2 launchDir)
        {
            if (!callback) return false;
            launchDir.Normalize();
            return callback.Hurt(launchDir);
        }
        public void SetInvulnerable(bool value)
        {
            m_invulnerable = value;
        }
        private void OnDrawGizmos()
        {
            if (col)
                Gizmos.DrawWireCube(col.bounds.center, col.size);
        }
    }
}

