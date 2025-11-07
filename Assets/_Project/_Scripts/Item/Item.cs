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

    [Header("Hiệu ứng Quay")]
    [Tooltip("Tốc độ quay của vật phẩm (độ/giây). Quay thuận kim đồng hồ.")]
    public float rotationSpeed = 50f;

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
    private Vector3 bobbingAxis; // Trục bay lên xuống, được lưu lại để không bị ảnh hưởng bởi hiệu ứng quay

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

        // Lưu vị trí và trục bay ban đầu
        startPos = transform.position;
        bobbingAxis = transform.up; // Lưu lại trục "lên" ban đầu
    }

    // Hàm này được gọi bởi Enemy khi nó tạo ra vật phẩm
    public void Initialize(Vector2 gravityDir)
    {
        gravityDirection = gravityDir.normalized;
        // Xoay vật phẩm để nó "đứng" đúng hướng với trọng lực
        float angle = Mathf.Atan2(-gravityDirection.y, -gravityDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        
        // Cập nhật lại vị trí và trục bay sau khi đã xoay
        startPos = transform.position;
        bobbingAxis = transform.up; // Lưu lại trục "lên" mới sau khi xoay
    }

    void Update()
    {
        // --- Quay vật phẩm ---
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

        // --- Bay lên xuống (nhấp nhô) ---
        // Sử dụng trục `bobbingAxis` đã lưu để hiệu ứng nhấp nhô không bị ảnh hưởng bởi việc quay
        Vector3 bobbingOffset = bobbingAxis * Mathf.Sin(Time.time * frequency) * amplitude;
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

            // --- LOGIC MỚI: Kích hoạt hiệu ứng tan biến và tự hủy ---

            // Vô hiệu hóa collider ngay lập tức để không thể nhặt lại
            GetComponent<Collider2D>().enabled = false;
            
            // Tắt hiệu ứng nhấp nhô và quay của script này
            this.enabled = false; 

            ParticleAttractorOnDestroy attractor = null;
            if (transform.parent != null)
            {
                attractor = transform.parent.GetComponent<ParticleAttractorOnDestroy>();
            }

            if (attractor != null)
            {
                // Nếu tìm thấy script hiệu ứng trên object cha, kích hoạt nó.
                // Script đó sẽ tự hủy object cha (và cả item này) sau khi chạy xong.
                attractor.StartDestroySequence();
            }
            else if (transform.parent != null)
            {
                // Nếu không có script hiệu ứng, nhưng có object cha, hủy cha như cũ.
                Destroy(transform.parent.gameObject);
            }
            else
            {
                // Nếu không có cha, chỉ hủy chính nó.
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("Player object does not have a PlayerStat component!");
        }
    }
}