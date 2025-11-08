using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class RicochetProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float damage = 10f;
    public float moveSpeed = 5f;
    public int maxBounces = 3;

    [Header("Duplication")]
    [Tooltip("Bật/tắt tính năng nhân đôi khi va chạm.")]
    public bool canDuplicate = false;
    [Tooltip("Số lần tối đa mà viên đạn này có thể tự nhân đôi.")]
    public int maxDuplications = 1;
    [Tooltip("Độ ngẫu nhiên thêm vào hướng bay của bản sao.")]
    public float duplicationRandomness = 1.5f;

    [Header("Explosion")]
    [Tooltip("Bật/tắt tính năng phát nổ khi va chạm đất.")]
    public bool canExplodeOnBounce = false;
    [Tooltip("Prefab hiệu ứng nổ sẽ được tạo ra.")]
    public GameObject explosionEffectPrefab;
    [Tooltip("Bán kính của vụ nổ.")]
    public float explosionRadius = 3f;
    [Tooltip("Lực đẩy của vụ nổ tác động lên các vật thể khác.")]
    public float explosionForce = 100f;
    [Tooltip("Sát thương gây ra cho người chơi trong bán kính vụ nổ.")]
    public float explosionDamage = 20f;

    private Rigidbody2D rb;
    private int bounceCount = 0;
    private int duplicationCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * moveSpeed;
        Destroy(gameObject, 10f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            return;
        }

        // Xử lý các sự kiện khi va chạm với "Ground"
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Xử lý nhân đôi
            if (canDuplicate && duplicationCount < maxDuplications)
            {
                HandleDuplication();
            }

            // Xử lý phát nổ
            if (canExplodeOnBounce)
            {
                PerformExplosion();
            }
        }

        // Xử lý nảy (bounce)
        bounceCount++;
        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
        }
    }

    void HandleDuplication()
    {
        duplicationCount++;
        
        GameObject cloneObj = Instantiate(gameObject, transform.position, transform.rotation);
        RicochetProjectile cloneScript = cloneObj.GetComponent<RicochetProjectile>();
        Rigidbody2D cloneRb = cloneObj.GetComponent<Rigidbody2D>();

        if (cloneScript != null && cloneRb != null)
        {
            cloneScript.canDuplicate = false;
            Vector2 randomForce = Random.insideUnitCircle * duplicationRandomness;
            cloneRb.AddForce(randomForce, ForceMode2D.Impulse);
        }
    }

    void PerformExplosion()
    {
        // Tạo hiệu ứng nổ
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Tìm tất cả các đối tượng trong bán kính
        Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hitCollider in collidersInRadius)
        {
            // Gây sát thương cho người chơi
            PlayerHurtbox hurtbox = hitCollider.GetComponent<PlayerHurtbox>();
            if (hurtbox != null)
            {
                PlayerCollision player = hurtbox.GetComponentInParent<PlayerCollision>();
                if (player != null)
                {
                    player.HandleDamageAndKnockback(explosionDamage, transform);
                }
            }

            // Tác động lực lên các vật thể khác
            Rigidbody2D hitRb = hitCollider.GetComponent<Rigidbody2D>();
            if (hitRb != null)
            {
                Vector2 direction = hitCollider.transform.position - transform.position;
                if (direction.magnitude > 0)
                {
                    float forceFalloff = 1 - (direction.magnitude / explosionRadius);
                    hitRb.AddForce(direction.normalized * explosionForce * forceFalloff, ForceMode2D.Impulse);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > 0)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
    }
}
