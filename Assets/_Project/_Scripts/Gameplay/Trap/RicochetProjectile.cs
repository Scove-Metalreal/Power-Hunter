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

        // --- LOGIC NHÂN ĐÔI ĐƯỢC NÂNG CẤP ---
        // Chỉ nhân đôi khi va chạm với "Ground"
        if (canDuplicate && duplicationCount < maxDuplications && collision.gameObject.CompareTag("Ground"))
        {
            duplicationCount++;
            
            GameObject cloneObj = Instantiate(gameObject, transform.position, transform.rotation);
            RicochetProjectile cloneScript = cloneObj.GetComponent<RicochetProjectile>();
            Rigidbody2D cloneRb = cloneObj.GetComponent<Rigidbody2D>();

            if (cloneScript != null && cloneRb != null)
            {
                // Bản sao không thể nhân đôi nữa
                cloneScript.canDuplicate = false;

                // Thêm một lực ngẫu nhiên để tạo hiệu ứng "văng tứ tung".
                // Lực này sẽ được cộng vào vận tốc nảy lên vật lý tự nhiên của bản sao.
                Vector2 randomForce = Random.insideUnitCircle * duplicationRandomness;
                cloneRb.AddForce(randomForce, ForceMode2D.Impulse);
            }
        }
        // --- KẾT THÚC LOGIC MỚI ---

        // Xử lý nảy (bounce)
        bounceCount++;
        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
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
