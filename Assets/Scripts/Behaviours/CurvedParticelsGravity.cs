using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers.Curve;
using Controllers.Curve.Meta;

namespace Behaviours
{
    [RequireComponent(typeof(ParticleSystem))]
    public class CurvedParticelsGravity : MonoBehaviour
    {
        public CurveController curveController;
        public float pull = 5.0f;

        private ParticleSystem system;
        private ParticleSystem.Particle[] particles;

        private void Start()
        {
            Init();
        }

        protected void Init()
        {
            if (system == null)
            {
                system = GetComponent<ParticleSystem>();
            }

            if (particles == null || particles.Length < system.main.maxParticles)
            {
                particles = new ParticleSystem.Particle[system.main.maxParticles];
            }
        }

        private void LateUpdate()
        {
            // GetParticles is allocation free because we reuse the m_Particles buffer between updates
            int numParticlesAlive = system.GetParticles(particles);

            // Change only the particles that are alive
            for (int i = 0; i < numParticlesAlive; i++)
            {
                Vector2 particle2DProjection = new Vector2(transform.position.x, particles[i].position.z);
                float distanceFromOrgin = Vector3.Magnitude(particle2DProjection);
                float distanceFromOrginSquared = distanceFromOrgin * distanceFromOrgin;

                float attractorPositionX = transform.position.x + curveController.BendAmountDiluted.x * distanceFromOrginSquared;
                float attractorPositionY = curveController.BendAmountDiluted.y * distanceFromOrginSquared;
                float attractorPositionZ = transform.position.z + particles[i].position.z;

                Vector3 attractorPosition = new Vector3(attractorPositionX, attractorPositionY, attractorPositionZ);

                float distanceToAttractor = Vector3.Magnitude(attractorPosition - particles[i].position);

                if (distanceToAttractor > 0)
                {
                    particles[i].position = Vector3.Lerp(particles[i].position, attractorPosition, Time.deltaTime * pull);
                }
            }

            // Apply the particle changes to the Particle System
            system.SetParticles(particles, numParticlesAlive);
        }

        private void OnDrawGizmos()
        {
            Init();

            Gizmos.color = Color.gray;

            int numParticlesAlive = system.GetParticles(particles);

            // Change only the particles that are alive
            for (int i = 0; i < numParticlesAlive; i++)
            {
                Vector2 particleProjection = new Vector2(transform.position.x, particles[i].position.z);
                float distanceFromOrgin = Vector3.Magnitude(particleProjection);
                float distanceFromOrginSquared = distanceFromOrgin * distanceFromOrgin;
                Vector3 attractorPosition = new Vector3(transform.position.x + curveController.BendAmountDiluted.x * distanceFromOrginSquared, curveController.BendAmountDiluted.y * distanceFromOrginSquared, transform.position.z + particles[i].position.z);

                Gizmos.DrawWireSphere(attractorPosition, 0.25f);

                Gizmos.DrawLine(particles[i].position, attractorPosition);
            }
        }
    }
}

