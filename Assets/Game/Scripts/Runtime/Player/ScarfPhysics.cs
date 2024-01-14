using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BucketsGame
{
    public class ScarfPhysics : MonoBehaviour
    {
        public LineRenderer line;
        public Transform anchorPoint;
        public int iterations = 12;
        public float gravityScale = 1f;
        public float mass = 10;
        public int simulationSpeed = 1;
        public float totalLength = 0.5f;
        public Vector2 awakeNormal = new Vector2(0, -1);
        public Vector2 velocityMultiplier = new Vector2(0.5f, 0.9f);
        public List<VerletPoint> points = new();
        public List<VerletConstraint> constraint = new();
        private float timeCount = 0;
        private void Awake()
        {
            if (iterations <= 0) iterations = 1;
            float length = totalLength / (float)iterations;
            Vector2 basePosition = transform.position;
            if (anchorPoint) basePosition = anchorPoint.position;
            for (int i = 0; i < iterations; i++)
            {
                bool anchor = i <= 0;
                Vector2 position = basePosition + awakeNormal * (totalLength * i);
                points.Add(new VerletPoint(position, anchor));
                if (i > 0)
                    constraint.Add(new VerletConstraint(points[i - 1], points[i], length));
            }
            if (line) line.positionCount = points.Count;
        }
        private void FixedUpdate()
        {
            
        }
        private void Update()
        {
            Simulation();
            foreach (VerletPoint point in points)
            {
                point.Render();
            }
            foreach (VerletConstraint constraint in constraint)
            {
                constraint.Render();
            }
            if (line && anchorPoint)
            {
                line.SetPosition(0, anchorPoint.position);
            }
        }

        private void Simulation()
        {
            float fps = 1.0F / Time.deltaTime;
            int simSpeed = (int)Mathf.Lerp(10, simulationSpeed, 1 - Mathf.InverseLerp(50, 300, (int)fps));
            //deltaTime = deltaTime / simSpeed;
            float deltaTime = Time.fixedDeltaTime - Time.deltaTime;
            Vector2 gravity = Physics2D.gravity * gravityScale;
            //gravity = new Vector2(0, Mathf.Lerp(Physics2D.gravity.y * gravityScale, 0, Mathf.InverseLerp(0, Time.fixedDeltaTime, Time.deltaTime)));
            
            if (timeCount < Time.fixedDeltaTime)
            {
                gravity = Physics2D.gravity * gravityScale * Time.deltaTime;
                timeCount += Time.deltaTime;
            }
            else
            {
                timeCount = 0;
            }
            for (int j = 0; j < points.Count; j++)
            {
                ApplyVelocity(gravity, Time.fixedDeltaTime, j);
            }

            for (int i = 0; i < simulationSpeed; i++)
            {
                foreach (VerletConstraint constraint in constraint)
                {
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

        private void ApplyVelocity(Vector2 gravity, float deltaTime, int j)
        {
            points[j].mass = mass;
            if (anchorPoint && j == 0)
                points[j].position = anchorPoint.position;
            else points[j].Update(Vector2.zero, deltaTime, gravity, velocityMultiplier);
        }

        
    }
}

