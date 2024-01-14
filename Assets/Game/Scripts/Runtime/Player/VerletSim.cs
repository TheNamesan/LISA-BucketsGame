using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame 
{
    [System.Serializable]
    public class VerletPoint
    {
        public Vector2 position;
        public Vector2 prevPosition;
        public bool anchor;
        public float mass = 1;
        public VerletPoint(Vector2 pos, bool anchor)
        {
            position = pos;
            prevPosition = pos;
            this.anchor = anchor;
        }
        public void Update(Vector2 force, float deltaTime)
        {
            if (anchor) return;
            Vector2 vel = position - prevPosition;
            if (mass <= 0) mass = 0.0005f;
            Vector2 acc = force / mass;
            prevPosition = position;
            Vector2 deltaPos = vel + acc * Vector2.one * (deltaTime * deltaTime);
            //if (time > 0) deltaPos.y += Mathf.Sin(time * 10f) * 0.001f;
            position += deltaPos;
        }
        public void Update(Vector2 force, float deltaTime, Vector2 gravity, Vector2 multiplier)
        {
            if (anchor) return;
            Vector2 vel = position - prevPosition;
            if (mass <= 0) mass = 0.0005f;
            Vector2 acc = force / mass;
            acc = Vector2.zero;
            prevPosition = position;
            vel *= multiplier;
            Vector2 deltaPos = vel + ((acc * deltaTime * deltaTime) + ((gravity * deltaTime * deltaTime)));
            //deltaPos *= multiplier;
            position += deltaPos;
        }
        public void Render()
        {
            Debug.DrawRay(position, Vector2.up * 0.5f, Color.green);
        }
    }
    [System.Serializable]
    public class VerletConstraint
    {
        [SerializeReference] public VerletPoint a;
        [SerializeReference] public VerletPoint b;
        public float length;
        public VerletConstraint(VerletPoint a, VerletPoint b, float length)
        {
            this.a = a;
            this.b = b;
            this.length = length;
        }
        public void Update()
        {
            if (a == null || b == null) return;

            var delta = b.position - a.position;
            var distance = Vector2.Distance(a.position, b.position);
            if (distance <= 0) return;
            var difference = length - distance;
            var percent = (difference / distance) / 2f;
            percent = Mathf.Min(percent, 0.5f);
            //Debug.Log(percent);
            Vector2 offset = delta * percent;
            if (!a.anchor) a.position -= offset;
            if (!b.anchor) b.position += offset;

            //Vector2 center = (a.position + b.position) / 2;
            //Vector2 direction = (a.position - b.position).normalized;
            //Vector2 strength = direction * length / 2;
            //if (!a.anchor) a.position = center + strength;
            //if (!b.anchor) b.position = center - strength;
        }
        public void Render()
        {
            Debug.DrawLine(a.position, b.position, Color.white);
        }
    }
    public class VerletSim : MonoBehaviour
    {
        public LineRenderer line;
        public int iterations = 6;
        public float gravityScale = 1f;
        public float constraintLength = 2f;
        public int simulationSpeed = 1;
        public List<VerletPoint> points = new();
        public List<VerletConstraint> constraint = new();
        private void Awake()
        {
            if (iterations <= 0) iterations = 1;
            float length = 1 / iterations;
            Vector2 basePosition = transform.position;
            for (int i = 0; i < iterations; i++)
            {
                bool anchor = i <= 0;
                Vector2 position = basePosition + new Vector2(i + length * iterations, 0);
                points.Add(new VerletPoint(position, anchor));
                if (i > 0)
                    constraint.Add(new VerletConstraint(points[i-1], points[i], constraintLength));
            }
            //points.Add(new VerletPoint(new Vector2(0, 0), true));
            //points.Add(new VerletPoint(new Vector2(0.25f, 0), false));
            //points.Add(new VerletPoint(new Vector2(0.5f, 0), false));
            //points.Add(new VerletPoint(new Vector2(0.75f, 0), false));
            //points.Add(new VerletPoint(new Vector2(1f, 0), false));
            //constraint.Add(new VerletConstraint(points[0], points[1], constraintLength));
            //constraint.Add(new VerletConstraint(points[1], points[2], constraintLength));
            //constraint.Add(new VerletConstraint(points[2], points[3], constraintLength));
            //constraint.Add(new VerletConstraint(points[3], points[4], constraintLength));
            if (line)
            {
                line.positionCount = points.Count;
            }
        }
        private void FixedUpdate()
        {
            for (int i = 0; i < simulationSpeed; i++)
            {
                foreach (VerletPoint point in points)
                {
                    point.Update(Physics2D.gravity * gravityScale, Time.fixedDeltaTime);
                }
                foreach (VerletConstraint constraint in constraint)
                {
                    constraint.length = constraintLength;
                    constraint.Update();
                }
            }
            if (line)
            {
                Vector3[] vectors = new Vector3[points.Count];
                for (int i = 0; i < points.Count; i++)
                {
                    vectors[i] = points[i].position;
                }
                line.SetPositions(vectors);
            }
        }
        private void Update()
        {
            foreach (VerletPoint point in points)
            {
                point.Render();
            }
            foreach (VerletConstraint constraint in constraint)
            {
                constraint.Render();
            }
        }
    }
}

