using UnityEngine;
using UnityEngine.Tilemaps; // Thêm dòng này để sử dụng TilemapRenderer

public class GlowBreathingEffect : MonoBehaviour
{
    [Header("Tên thuộc tính trong Shader")]
    [Tooltip("Điền chính xác tên thuộc tính màu trong Shader Graph, ví dụ: Color1, Color2")]
    public string colorPropertyName = "Color1";

    [Header("Thiết lập hiệu ứng thở")]
    [Tooltip("Cường độ sáng thấp nhất (lúc thở ra)")]
    public float minIntensity = 1.5f;

    [Tooltip("Cường độ sáng cao nhất (lúc hít vào)")]
    public float maxIntensity = 3.5f;

    [Tooltip("Tốc độ của nhịp thở")]
    public float breathingSpeed = 1.0f;

    // --- Biến nội bộ ---
    private Material materialInstance;
    private Color baseColor;
    private int propertyID;

    void Start()
    {
        // ----- THAY ĐỔI DUY NHẤT LÀ Ở ĐÂY -----
        // Lấy TilemapRenderer thay vì SpriteRenderer
        TilemapRenderer tilemapRenderer = GetComponent<TilemapRenderer>();

        if (tilemapRenderer == null)
        {
            Debug.LogError("Không tìm thấy TilemapRenderer trên GameObject này!");
            this.enabled = false;
            return;
        }

        // Phần còn lại giữ nguyên
        materialInstance = tilemapRenderer.material;
        propertyID = Shader.PropertyToID(colorPropertyName);

        if (materialInstance.HasProperty(propertyID))
        {
            baseColor = materialInstance.GetColor(propertyID);
        }
        else
        {
            Debug.LogError("Material không có thuộc tính tên là: " + colorPropertyName);
            this.enabled = false;
        }
    }

    void Update()
    {
        // Logic tạo hiệu ứng thở giữ nguyên, không cần thay đổi
        float sinWave = Mathf.Sin(Time.time * breathingSpeed);
        float normalizedValue = (sinWave + 1f) / 2f;
        float currentIntensity = Mathf.Lerp(minIntensity, maxIntensity, normalizedValue);
        Color finalGlowColor = baseColor * currentIntensity;
        materialInstance.SetColor(propertyID, finalGlowColor);
    }
}