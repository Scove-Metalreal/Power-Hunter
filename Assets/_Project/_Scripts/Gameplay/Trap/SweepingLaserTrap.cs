using Unity.AppUI.Core;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SweepingLaserTrap : MonoBehaviour
{
    [Header("Laser Settings")]
    [Tooltip("Damage dealt per second while the player is in the laser.")]
    public float damagePerSecond = 50f;

    [Tooltip("The speed at which the laser rotates in degrees per second.")]
    public float rotationSpeed = 45f;

    [Tooltip("The maximum distance the laser can travel if it doesn't hit anything.")]
    public float maxDistance = 100f;

    [Tooltip("The width of the laser beam.")]
    public float laserWidth = 0.2f;

    [Header("Layers")]
    [Tooltip("The layer(s) that the laser will be blocked by (e.g., Ground, Walls).")]
    public LayerMask groundLayer;

    [Tooltip("The layer the player is on. The laser will damage anything on this layer.")]
    public LayerMask playerLayer;

    public GameObject lineParticle;

    // --- Private References ---
    private LineRenderer lineRenderer;
    private PlayerCollision _cachedPlayerCollision;
    private PlayerStat _cachedPlayerStat;
    private bool _isPlayerInLaser = false; // Flag to track if we already hit the player

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        
    }

    void Update()
    {
        // Step 1: Rotate the trap
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        Vector2 laserDirection = transform.up;

        // Step 2: Calculate laser endpoint
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, laserDirection, maxDistance, groundLayer);
        Vector2 laserEndPoint = groundHit.collider ? groundHit.point : (Vector2)transform.position + laserDirection * maxDistance;

        // Step 3: Draw the laser
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, laserEndPoint);

        // Step 4: Handle Player Collision (with knockback on first hit)
        float laserLength = Vector2.Distance(transform.position, laserEndPoint);
        Vector2 direction = laserEndPoint - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        lineParticle.transform.position = laserEndPoint;
        lineParticle.transform.rotation = Quaternion.Euler(0,0,angle);
        RaycastHit2D playerHit = Physics2D.Raycast(transform.position, laserDirection, laserLength, playerLayer);

        if (playerHit.collider != null)
        {
            // Cache components on first contact, searching in parent objects.
            if (_cachedPlayerCollision == null)
            {
                _cachedPlayerCollision = playerHit.collider.GetComponentInParent<PlayerCollision>();
                _cachedPlayerStat = playerHit.collider.GetComponentInParent<PlayerStat>();
            }

            // Check if components are valid
            if (_cachedPlayerCollision != null && _cachedPlayerStat != null)
            {
                // If this is the first frame the laser hits the player
                if (!_isPlayerInLaser)
                {
                    // Apply knockback and initial damage
                    _cachedPlayerCollision.HandleDamageAndKnockback(damagePerSecond * Time.deltaTime, transform);
                    _isPlayerInLaser = true; // Set flag to true
                }
                else // If player is already in the laser
                {
                    // Only apply continuous damage without knockback
                    _cachedPlayerStat.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
        }
        else
        {
            // If laser is no longer hitting the player, reset the flag and caches
            if (_isPlayerInLaser)
            {
                _isPlayerInLaser = false;
                _cachedPlayerCollision = null;
                _cachedPlayerStat = null;
            }
        }
    }
}
