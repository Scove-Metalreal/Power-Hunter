// Gắn script này vào Prefab của vật phẩm Power-up.
// Yêu cầu: Vật phẩm cần có Rigidbody2D và một Collider2D (được set là "Is Trigger").

using UnityEngine;
using TMPro; // Thư viện để làm việc với TextMeshPro UI

public class Item : MonoBehaviour
{
    [Header("Hiệu ứng Bay")]
    [Tooltip("Biên độ bay lên xuống của vật phẩm.")]
    public float amplitude = 0.25f; // Độ cao mà item sẽ bay lên/xuống so với vị trí gốc
    [Tooltip("Tần số của hiệu ứng bay (càng cao càng nhanh).")]
    public float frequency = 1f;    // Tốc độ của chu kỳ bay lên xuống

    [Header("Thuộc tính Vật phẩm")]
    [Tooltip("Lượng 'power' mà vật phẩm này cộng cho người chơi.")]
    public int powerValue = 1;
    [Tooltip("Thời gian (giây) vật phẩm sẽ tồn tại trước khi tự biến mất.")]
    public float lifeTime = 300f; // 5 phút = 300 giây

    [Header("Vật lý")]
    [Tooltip("Lực hút của trọng lực tác động lên vật phẩm.")]
    public float gravityStrength = 50f;
    private Vector2 gravityDirection; // Hướng trọng lực, được nhận từ Enemy

    // Biến nội bộ
    private Vector3 startPos;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Vật phẩm " + gameObject.name + " thiếu component Rigidbody2D!");
            return;
        }
        rb.gravityScale = 0; // Chúng ta sẽ tự quản lý trọng lực
    }

    void Start()
    {
        // Hủy vật phẩm sau khoảng thời gian lifeTime
        Destroy(gameObject, lifeTime);

        // Lưu vị trí ban đầu để tạo hiệu ứng bay xung quanh nó
        startPos = transform.position;
    }

    // Hàm này được gọi bởi Enemy khi nó tạo ra vật phẩm
    public void Initialize(Vector2 gravityDir)
    {
        gravityDirection = gravityDir.normalized;
        // Xoay vật phẩm để nó "đứng" đúng hướng với trọng lực
        // Tương tự như cách Enemy được xoay
        float angle = Mathf.Atan2(-gravityDirection.y, -gravityDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        
        // Cập nhật lại vị trí bắt đầu sau khi đã xoay
        startPos = transform.position;
    }

    void Update()
    {
        // Tạo hiệu ứng bay lên xuống mượt mà
        // Chúng ta sẽ tính toán vị trí bay dựa trên trục "lên" của vật phẩm, bất kể hướng trọng lực
        Vector3 bobbingOffset = transform.up * Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPos + bobbingOffset;
    }
    
    void FixedUpdate()
    {
        // Áp dụng lực hấp dẫn tùy chỉnh
        if (rb != null && gravityDirection != Vector2.zero)
        {
            rb.AddForce(gravityDirection * gravityStrength);
        }
    }

    // This method is now public so it can be called from PlayerCollision.cs
    public void Pickup(GameObject player)
    {
        PlayerStat playerStat = player.GetComponent<PlayerStat>();
        if (playerStat != null)
        {
            playerStat.AddPowerValue(powerValue);
            Debug.Log("Player picked up item. Added " + powerValue + " power. New total: " + playerStat.PowerValue);

            // Optional: Play sound or show effect here
            // SoundManager.Instance.PlayPickupSound();
            // EffectManager.Instance.ShowPickupEffect(transform.position);

            // Destroy the item after pickup
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Player object does not have a PlayerStat component!");
        }
    }
}