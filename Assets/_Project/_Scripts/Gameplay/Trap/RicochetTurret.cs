using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class RicochetTurret : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("The projectile prefab to be fired.")]
    public GameObject projectilePrefab;
    [Tooltip("The part of the turret that rotates.")]
    public Transform rotatingPart;
    [Tooltip("The exact point where projectiles will be spawned. Should be a child of the rotating part.")]
    public Transform firePoint;

    [Header("Targeting & Firing")]
    [Tooltip("The radius within which the turret detects the player.")]
    public float detectionRadius = 10f;
    [Tooltip("How fast the turret turns to aim at the player.")]
    public float aimSpeed = 5f;
    [Tooltip("How many projectiles to fire per second when player is in range.")]
    public float fireRate = 1f;

    // --- Private State ---
    private Transform playerTransform;
    private bool isPlayerInRange = false;
    private float fireCooldown = 0f;

    private void Start()
    {
        // Setup the detection collider automatically
        CircleCollider2D detectionCollider = GetComponent<CircleCollider2D>();
        detectionCollider.isTrigger = true;
        detectionCollider.radius = detectionRadius;

        if (rotatingPart == null)
        {
            rotatingPart = transform; // Default to rotating the whole turret if not set
        }

        if (firePoint == null)
        {
            firePoint = rotatingPart; // Default to the rotating part if not set
        }
    }

    private void Update()
    {
        // Cooldown timer always ticks down
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }

        if (isPlayerInRange && playerTransform != null)
        {
            // --- Aiming Logic ---
            Vector2 directionToPlayer = playerTransform.position - rotatingPart.position;
            float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f; // -90 because transform.up is our 'forward'
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            // Use Slerp for smooth rotation
            rotatingPart.rotation = Quaternion.Slerp(rotatingPart.rotation, targetRotation, aimSpeed * Time.deltaTime);

            // --- Firing Logic ---
            // Check if turret is aimed roughly at the player before firing
            float angleDifference = Quaternion.Angle(rotatingPart.rotation, targetRotation);
            if (angleDifference < 10f && fireCooldown <= 0) // Only fire if aimed and cooldown is over
            {
                Fire();
            }
        }
    }

    private void Fire()
    {
        if (projectilePrefab == null) return;

        // Reset cooldown
        fireCooldown = 1f / fireRate;

        // Spawn the projectile at the fire point's position and rotation
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerTransform = null;
        }
    }

    // Optional: Draw the detection radius in the editor for easier setup
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
