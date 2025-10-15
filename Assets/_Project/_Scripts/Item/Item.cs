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

    // --- Tham chiếu đến UI ---
    // Cách 1: Kéo thả trực tiếp (đơn giản nhưng không linh hoạt)
    // public TextMeshProUGUI powerText; 
    
    // Cách 2: Tìm kiếm theo Tag (linh hoạt hơn)
    private static TextMeshProUGUI powerText; // Dùng static để không phải tìm lại mỗi lần nhặt
    private static int currentPlayerPower = 0; // Biến static để lưu trữ giá trị power chung

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

        // Tìm UI text nếu chưa được tìm thấy
        if (powerText == null)
        {
            GameObject textObject = GameObject.FindGameObjectWithTag("PowerUIText"); // Đặt Tag này cho đối tượng Text trên Canvas
            if (textObject != null)
            {
                powerText = textObject.GetComponent<TextMeshProUGUI>();
                // Khởi tạo giá trị ban đầu cho UI
                powerText.text = "Power: " + currentPlayerPower;
            }
            else
            {
                Debug.LogWarning("Không tìm thấy đối tượng UI với Tag 'PowerUIText'. Hãy chắc chắn bạn đã tạo và gán tag cho nó.");
            }
        }
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

    // Được gọi khi một collider khác đi vào trigger của vật phẩm
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem có phải là người chơi không
        if (collision.CompareTag("Player"))
        {
            Pickup();
        }
    }

    // Xử lý logic khi người chơi nhặt vật phẩm
    void Pickup()
    {
        Debug.Log("Người chơi đã nhặt vật phẩm, tăng " + powerValue + " power!");

        // Tăng chỉ số power
        currentPlayerPower += powerValue;

        // Cập nhật UI
        if (powerText != null)
        {
            powerText.text = "Power: " + currentPlayerPower;
        }
        else
        {
            Debug.LogWarning("Tham chiếu đến Power UI Text không tồn tại!");
        }

        // Có thể thêm hiệu ứng âm thanh hoặc hình ảnh ở đây
        // SoundManager.Instance.PlayPickupSound();
        // EffectManager.Instance.ShowPickupEffect(transform.position);

        // Biến mất sau khi được nhặt
        Destroy(gameObject);
    }
}