using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CollapsingPlatform : MonoBehaviour
{
    [Header("Collapse Settings")]
    [Tooltip("Delay after the player lands before the platform starts shaking.")]
    [SerializeField] private float shakeDelay = 0.5f;
    [Tooltip("How long the platform shakes before collapsing.")]
    [SerializeField] private float shakeDuration = 1f;
    [Tooltip("Delay after the platform starts falling before it is destroyed.")]
    [SerializeField] private float destroyDelay = 3f;
    [Tooltip("Time in seconds after collapsing before the platform reappears.")]
    [SerializeField] private float respawnTime = 60f;

    [Header("Shake Effect")]
    [Tooltip("How violently the platform shakes.")]
    [SerializeField] private float shakeMagnitude = 0.05f;

    [Header("Movement Settings")]
    [Tooltip("If true, the platform will move between its start position and Point B.")]
    public bool canMove = false;
    [Tooltip("The target destination for the moving platform (Point B).")]
    public Transform pointB;
    [Tooltip("The speed at which the platform moves.")]
    public float moveSpeed = 2f;
    [Tooltip("If true, the platform won't move until the player lands on it first.")]
    public bool waitForPlayerStart = false;
    [Tooltip("How long the platform waits at each endpoint before moving again.")]
    public float pauseDuration = 1f;

    // --- Private State ---
    private Animator animator;
    private Rigidbody2D rb;
    private Vector3 pointA; // Start position
    private Collider2D[] colliders; // Cached colliders
    private bool isCollapsing = false;
    private bool _hasBeenActivated = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        pointA = transform.position;
        colliders = GetComponents<Collider2D>(); // Cache colliders for resetting

        // Start moving immediately if it's configured to do so.
        if (canMove && !waitForPlayerStart)
        {
            _hasBeenActivated = true;
            StartCoroutine(MovementSequence());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Parent the player to the platform for smooth movement
            collision.transform.SetParent(transform);

            // If movement hasn't started yet, start it.
            if (canMove && waitForPlayerStart && !_hasBeenActivated)
            {
                _hasBeenActivated = true;
                StartCoroutine(MovementSequence());
            }

            // Trigger collapse sequence if it hasn't started yet.
            if (!isCollapsing && collision.contacts[0].normal.y < -0.5)
            {
                isCollapsing = true;
                StartCoroutine(CollapseSequence());
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Unparent the player when they leave the platform
            collision.transform.SetParent(null);
        }
    }

    private IEnumerator MovementSequence()
    {
        if (pointB == null) 
        {
            Debug.LogError("Point B is not set for moving platform!", gameObject);
            yield break; // Stop the coroutine if pointB is not assigned
        }

        Vector3 target = pointB.position;
        while (true) // Infinite loop for back-and-forth movement
        {
            // Stop moving if the platform starts collapsing
            if (isCollapsing) yield break;

            // Move towards the current target
            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                if (isCollapsing) yield break; // Check again inside the loop for immediate stop
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null; // Wait for the next frame
            }

            // Wait at the endpoint
            yield return new WaitForSeconds(pauseDuration);

            // Swap target
            target = (target == pointA) ? pointB.position : pointA;
        }
    }

    private IEnumerator CollapseSequence()
    {
        yield return new WaitForSeconds(shakeDelay);

        float timer = 0;
        Vector3 startPos = transform.position; // Use current position as base for shaking
        while (timer < shakeDuration)
        {
            float xOffset = Random.Range(-shakeMagnitude, shakeMagnitude);
            float yOffset = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.position = startPos + new Vector3(xOffset, yOffset, 0);

            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;

        animator.SetTrigger("Collapse");

        // Unparent the player before the platform falls
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Player"))
            {
                child.SetParent(null);
            }
        }

        // Disable colliders so objects fall through
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // Make the platform fall
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.isKinematic = false;

        // Start the reset process instead of destroying the object
        StartCoroutine(ResetAfterDelay(destroyDelay, respawnTime));
    }

    /// <summary>
    /// Hides the platform after it falls, waits for a delay, then resets it.
    /// </summary>
    private IEnumerator ResetAfterDelay(float fallDelay, float resetDelay)
    {
        // 1. Wait for the platform to fall off-screen
        yield return new WaitForSeconds(fallDelay);

        // 2. Hide the platform
        gameObject.SetActive(false);

        // 3. Wait for the respawn timer
        yield return new WaitForSeconds(resetDelay);

        // 4. Reset the platform to its initial state
        ResetPlatform();
    }

    /// <summary>
    /// Resets the platform to its original position and state.
    /// </summary>
    public void ResetPlatform()
    {
        // Stop any running coroutines to prevent conflicts
        StopAllCoroutines();

        // Reset state flags
        isCollapsing = false;
        _hasBeenActivated = false;

        // Reset position and rotation
        transform.position = pointA;
        transform.rotation = Quaternion.identity;

        // Reset Rigidbody to be kinematic and static
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Re-enable all colliders
        foreach (var col in colliders)
        {
            col.enabled = true;
        }

        // Reset the animator to its initial state
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        // Ensure the GameObject is active
        gameObject.SetActive(true);

        // Restart the movement coroutine if the platform is set to move automatically
        if (canMove && !waitForPlayerStart)
        {
            _hasBeenActivated = true;
            StartCoroutine(MovementSequence());
        }
    }
}
