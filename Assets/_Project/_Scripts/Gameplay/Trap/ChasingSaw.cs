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

            // After fully stopped, it might just sit there or be destroyed
            if (currentMoveSpeed < 0.1f)
            {
                Destroy(gameObject, 2f); // Destroy after 2 seconds of being stopped
            }
            return; // Don't execute chase logic if stopping
        }

        if (playerTransform == null)
        {
            BeginStopping();
            return;
        }

        // --- Ground Following Logic ---
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);

        if (groundHit.collider == null)
        {
            // No ground beneath, so it should fall
            rb.gravityScale = 2f; // Apply gravity
            BeginStopping(); // Stop trying to chase
            return;
        }

        // Adjust position to hover slightly above the ground
        transform.position = (Vector2)groundHit.point + (groundHit.normal * (GetComponent<CircleCollider2D>().radius + groundOffset));

        // Determine movement direction along the ground surface
        float moveDirection = (playerTransform.position.x > transform.position.x) ? 1f : -1f;
        Vector2 moveVector = new Vector2(groundHit.normal.y, -groundHit.normal.x) * moveDirection;

        // Apply velocity
        rb.linearVelocity = moveVector * currentMoveSpeed;
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
