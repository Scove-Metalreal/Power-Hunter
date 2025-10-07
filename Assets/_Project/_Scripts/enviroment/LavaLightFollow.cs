using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LavaLightFollow : MonoBehaviour
{
    public Light2D lavaLight;
    public ParticleSystem ps;

    void Update()
    {
        if (ps.particleCount > 0)
        {
            var particles = new ParticleSystem.Particle[ps.particleCount];
            ps.GetParticles(particles);
           
            lavaLight.transform.position = particles[0].position;
        }
    }
}
