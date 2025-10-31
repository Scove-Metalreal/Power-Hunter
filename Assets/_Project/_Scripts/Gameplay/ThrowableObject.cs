using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ThrowableObject : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("The UI element (e.g., a Canvas with Text) that shows the 'Press E' prompt. Should be a child of this object.")]
    [SerializeField] private GameObject pickupPrompt;

    public bool IsHeld { get; private set; } = false;
    private bool isThrown = false;

    private Rigidbody2D rb;
    private Collider2D col;
    private int originalLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        originalLayer = gameObject.layer; // Store the original layer

        if (pickupPrompt != null)
        {
            pickupPrompt.SetActive(false);
        }
    }

    public void ShowPrompt()
    {
        if (pickupPrompt != null && !IsHeld)
        {
            pickupPrompt.SetActive(true);
        }
    }

    public void HidePrompt()
    {
        if (pickupPrompt != null)
        {
            pickupPrompt.SetActive(false);
        }
    }

    public void OnPickup(Transform holdPoint)
    {
        IsHeld = true;
        isThrown = false;
        rb.isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("HeldObject"); // Switch to HeldObject layer

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        HidePrompt();
    }

    public void OnDrop()
    {
        IsHeld = false;
        isThrown = false;
        transform.SetParent(null);
        gameObject.layer = originalLayer; // Switch back to original layer
        rb.isKinematic = false;
    }

    public void OnThrow(Vector2 direction, float force)
    {
        IsHeld = false;
        isThrown = true;
        transform.SetParent(null);
        gameObject.layer = originalLayer; // Switch back to original layer
        rb.isKinematic = false;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only process collisions if the object was actively thrown
        if (isThrown)
        {
            // After hitting anything (wall, ground, enemy), it stops being a projectile
            isThrown = false;

            // Example: Check if we hit an enemy to deal damage
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Throwable object hit an enemy: " + collision.gameObject.name);
                // You would get the enemy's health component and deal damage here
            }
        }
    }
}
