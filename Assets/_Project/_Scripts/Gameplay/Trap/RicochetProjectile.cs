using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class RicochetProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float damage = 10f;
    public float moveSpeed = 5f;
    public int maxBounces = 3;

    private Rigidbody2D rb;
    private int bounceCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // The turret that spawns this will set its initial direction.
        // We give it an initial velocity.
        rb.linearVelocity = transform.up * moveSpeed;

        // Destroy the projectile after some time to prevent it from flying forever
        Destroy(gameObject, 10f); // Self-destruct after 10 seconds
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we hit the player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerCollision player = collision.gameObject.GetComponentInParent<PlayerCollision>();
            if (player != null)
            {
                player.HandleDamageAndKnockback(damage, transform);
            }
            Destroy(gameObject); // Destroy projectile on hitting player
            return; // Stop further execution
        }

        // For any other collision (like walls), increment bounce count
        bounceCount++;
        if (bounceCount >= maxBounces)
        {
            // Optional: Add an explosion/dissipate effect here
            Destroy(gameObject);
        }
    }

    // To ensure speed doesn't decrease over time due to minor physics inaccuracies
    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > 0)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
    }
}
