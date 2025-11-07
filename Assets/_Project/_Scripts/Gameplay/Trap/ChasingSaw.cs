using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class ChasingSaw : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float chaseDuration = 4f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundOffset = 0.1f; // How high to hover above ground

    [Header("Visuals & Damage")]
    [SerializeField] private Transform visualsTransform; // The part that will spin
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float damage = 25f;

    // --- Private State ---
    private Rigidbody2D rb;
    private Transform playerTransform;
    private bool isStopping = false;
    private float currentMoveSpeed;
    private float currentRotationSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // We control position manually, no gravity needed while chasing

        currentMoveSpeed = moveSpeed;
        currentRotationSpeed = rotationSpeed;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("ChasingSaw Error: Cannot find a GameObject with the 'Player' tag. The saw will not move. Please ensure your player object is tagged correctly.", gameObject);
            BeginStopping(); // Stop immediately if no player is found
        }

        // Start the countdown to stop chasing
        Invoke(nameof(BeginStopping), chaseDuration);
    }

    void Update()
    {
        // Handle visual rotation
        if (visualsTransform != null)
        {
            visualsTransform.Rotate(0, 0, -currentRotationSpeed * Time.deltaTime);
        }

        // Handle deceleration of rotation speed when stopping
        if (isStopping)
        {
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0f, Time.deltaTime * 2f); // The '2f' is deceleration rate
        }
    }

    void FixedUpdate()
    {
        if (isStopping)
        {
            // Decelerate movement
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, 0f, Time.deltaTime * 2f);
            rb.linearVelocity = rb.linearVelocity.normalized * currentMoveSpeed;

            if (currentMoveSpeed < 0.1f)
            {
                Destroy(gameObject, 2f);
            }
            return; // Don't execute chase logic if stopping
        }

        if (playerTransform == null)
        {
            Debug.LogError("PlayerTransform became NULL in FixedUpdate. Stopping.");
            BeginStopping();
            return;
        }

        // --- Ground Following Logic ---
        Debug.Log("Running Ground Check...");
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);

        if (groundHit.collider == null)
        {
            Debug.LogError("Ground Check FAILED. No ground detected beneath the saw. Check 'Ground Layer' in Inspector and spawn position.");
            rb.gravityScale = 2f; // Apply gravity
            BeginStopping(); // Stop trying to chase
            return;
        }
        else
        {
            Debug.Log($"<color=green>Ground Check SUCCESS. Hit '{groundHit.collider.name}'</color>");
        }

        // Adjust position to hover slightly above the ground
        transform.position = (Vector2)groundHit.point + (groundHit.normal * (GetComponent<CircleCollider2D>().radius + groundOffset));

        // Determine movement direction along the ground surface
        float moveDirection = (playerTransform.position.x > transform.position.x) ? 1f : -1f;
        Vector2 moveVector = new Vector2(groundHit.normal.y, -groundHit.normal.x) * moveDirection;
        Debug.Log($"MoveDirection: {moveDirection}, GroundNormal: {groundHit.normal}, MoveVector: {moveVector}");

        // Apply velocity
        rb.linearVelocity = moveVector * currentMoveSpeed;
        Debug.Log($"<color=cyan>Final Velocity set to: {rb.linearVelocity}</color>");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerCollision player = collision.gameObject.GetComponentInParent<PlayerCollision>();
            if (player != null)
            {
                player.HandleDamageAndKnockback(damage, transform);
            }
        }
    }

    void BeginStopping()
    {
        isStopping = true;
    }
}
