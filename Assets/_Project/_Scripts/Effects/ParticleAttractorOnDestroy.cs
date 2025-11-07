using UnityEngine;
using System.Collections;

/// <summary>
/// Gắn script này vào một GameObject có Particle System.
/// Khi bạn muốn hủy GameObject này, hãy gọi hàm StartDestroySequence() từ một script khác
/// thay vì gọi Destroy(gameObject) trực tiếp.
/// Script sẽ tạo hiệu ứng hút các hạt về một điểm trước khi thực sự hủy GameObject.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class ParticleAttractorOnDestroy : MonoBehaviour
{
    [Tooltip("Điểm các hạt sẽ tụ lại. Nếu để trống, sẽ tụ lại tại vị trí của chính Particle System.")]
    public Transform attractionTarget;

    [Tooltip("Tốc độ các hạt bị hút về điểm tụ.")]
    public float attractionSpeed = 5f;

    [Tooltip("Thời gian tối đa cho hiệu ứng trước khi tự hủy (để tránh bị kẹt).")]
    public float maxDuration = 2f;

    private ParticleSystem ps;
    private Rigidbody2D rb;
    private bool isDestroying = false;

    // Chức năng này sẽ xuất hiện khi bạn click vào dấu 3 chấm "..." trên component này trong Inspector KHI ĐANG CHẠY GAME.
    [ContextMenu("Test Destroy Effect")]
    private void TestEffect()
    {
        if (Application.isPlaying)
        {
            StartDestroySequence();
        }
        else
        {
            Debug.LogWarning("Bạn chỉ có thể test hiệu ứng này khi đang ở trong Play Mode.");
        }
    }

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        rb = GetComponent<Rigidbody2D>(); // Lấy Rigidbody2D (nếu có)
    }

    /// <summary>
    /// Bắt đầu chuỗi hiệu ứng hút và sau đó hủy GameObject.
    /// </summary>
    public void StartDestroySequence()
    {
        if (isDestroying || !this.enabled) return;

        isDestroying = true;

        // --- SỬA LỖI: Vô hiệu hóa vật lý để object không bị rơi xuống ---
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector2.zero;
        }

        StartCoroutine(AttractAndDestroyCoroutine());
    }

    private IEnumerator AttractAndDestroyCoroutine()
    {
        // --- SỬA LỖI: Tắt trọng lực của chính Particle System ---
        var mainModule = ps.main;
        mainModule.gravityModifier = 0;

        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        Vector3 targetPosition = (attractionTarget != null) ? attractionTarget.position : transform.position;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.main.maxParticles];
        
        float timer = 0f;

        while (timer < maxDuration)
        {
            int numParticlesAlive = ps.GetParticles(particles);
            if (numParticlesAlive == 0) 
            {
                break; // Thoát sớm nếu không còn hạt nào
            }

            for (int i = 0; i < numParticlesAlive; i++)
            {
                // Di chuyển hạt về phía mục tiêu với tốc độ không đổi
                particles[i].position = Vector3.MoveTowards(particles[i].position, targetPosition, attractionSpeed * Time.deltaTime);
            }

            ps.SetParticles(particles, numParticlesAlive);

            timer += Time.deltaTime;
            yield return null;
        }

        // Hủy GameObject sau khi hiệu ứng hoàn tất
        Destroy(gameObject);
    }
}
