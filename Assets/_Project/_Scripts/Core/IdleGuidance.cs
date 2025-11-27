using UnityEngine;
using UnityEngine.AI; // Required for NavMesh pathfinding
using System.Collections;

/// <summary>
/// Shows a blinking particle effect that follows a path from a starting object (A) to a destination (B),
/// avoiding obstacles defined in a NavMesh. The effect triggers when 'targetA' is idle.
/// </summary>
public class IdleGuidance : MonoBehaviour
{
    [Header("Targets")]
    [Tooltip("The starting point of the particle trail, typically the player.")]
    public Transform targetA;

    [Tooltip("The destination point the particle trail should lead to.")]
    public Transform targetB;

    [Header("Effect Settings")]
    [Tooltip("The Particle System prefab to use for the trail. Particles should have zero speed and a short lifetime.")]
    public ParticleSystem guidanceParticles;

    [Tooltip("The time in seconds 'targetA' must be idle before the effect starts.")]
    public float idleThresholdTime = 5.0f;

    [Tooltip("The duration of one on/off cycle for the blinking effect.")]
    public float blinkInterval = 1.5f;

    [Header("Pathfinding Settings")]
    [Tooltip("How many particles to draw per meter of the path.")]
    public float particleDensity = 10f;

    // --- Private State ---
    private Vector3 lastPosition;
    private float idleTimer = 0f;
    private Coroutine guidanceCoroutine;
    private ParticleSystem particleInstance;
    private NavMeshPath path;

    void Start()
    {
        if (targetA == null || targetB == null || guidanceParticles == null)
        {
            Debug.LogError("IdleGuidance script is missing one or more required references. Disabling script.", this);
            this.enabled = false;
            return;
        }

        // Create an instance of the particle system and initialize the path object.
        particleInstance = Instantiate(guidanceParticles, transform);
        particleInstance.Stop();
        path = new NavMeshPath();

        lastPosition = targetA.position;
    }

    void Update()
    {
        // --- Idle Detection ---
        if (Vector3.Distance(targetA.position, lastPosition) > 0.01f)
        {
            idleTimer = 0f;
            if (guidanceCoroutine != null)
            {
                StopCoroutine(guidanceCoroutine);
                guidanceCoroutine = null;
                particleInstance.Clear(); // Clear any visible particles
            }
        }
        else
        {
            idleTimer += Time.deltaTime;
        }
        lastPosition = targetA.position;

        // --- Effect Trigger ---
        if (idleTimer >= idleThresholdTime && guidanceCoroutine == null)
        {
            guidanceCoroutine = StartCoroutine(BlinkingGuidanceRoutine());
        }
    }

    /// <summary>
    /// Calculates a path and emits particles along it in a blinking pattern.
    /// </summary>
    private IEnumerator BlinkingGuidanceRoutine()
    {
        while (true)
        {
            // Calculate a path from A to B on the NavMesh.
            bool pathFound = NavMesh.CalculatePath(targetA.position, targetB.position, NavMesh.AllAreas, path);

            if (pathFound && path.corners.Length > 1)
            {
                // --- Dynamically Adjust Particle Lifetime ---
                var mainModule = particleInstance.main;
                float distance = Vector3.Distance(targetA.position, targetB.position);
                if (mainModule.startSpeed.constant > 0)
                {
                    // --- THIS IS THE CORRECTED LINE ---
                    // Instead of assigning a float directly, we create a new MinMaxCurve.
                    mainModule.startLifetime = new ParticleSystem.MinMaxCurve(distance / mainModule.startSpeed.constant);
                }

                // --- "On" Phase: Draw the path with particles ---
                particleInstance.Clear(); // Clear particles from the previous blink

                // Iterate through each segment of the path (between two corners)
                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    Vector3 startPoint = path.corners[i];
                    Vector3 endPoint = path.corners[i + 1];
                    float segmentDistance = Vector3.Distance(startPoint, endPoint);
                    
                    // Determine how many particles to place on this segment based on density.
                    int particleCount = Mathf.CeilToInt(segmentDistance * particleDensity);

                    for (int j = 0; j < particleCount; j++)
                    {
                        // Find the position for the new particle along the segment.
                        Vector3 particlePosition = Vector3.Lerp(startPoint, endPoint, (float)j / particleCount);
                        
                        // Manually emit one particle at the calculated position.
                        var emitParams = new ParticleSystem.EmitParams { position = particlePosition };
                        particleInstance.Emit(emitParams, 1);
                    }
                }

                yield return new WaitForSeconds(blinkInterval / 2);

                // --- "Off" Phase: Clear the particles ---
                particleInstance.Clear();
                yield return new WaitForSeconds(blinkInterval / 2);
            }
            else
            {
                // If no path can be found, wait before trying again.
                Debug.LogWarning("IdleGuidance: Could not find a NavMesh path from A to B.");
                yield return new WaitForSeconds(blinkInterval);
            }
        }
    }

    private void OnDisable()
    {
        if (particleInstance != null)
        {
            Destroy(particleInstance.gameObject);
        }
    }
}
