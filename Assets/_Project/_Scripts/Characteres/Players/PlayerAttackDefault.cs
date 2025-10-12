using UnityEngine;

// Lớp này xử lý các đòn tấn công cơ bản của người chơi, bao gồm cả hệ thống combo đơn giản.
public class PlayerAttackDefault : MonoBehaviour
{
    [SerializeField] private Transform ATTSPAWM; // Vị trí (Transform) nơi hitbox tấn công sẽ được tạo ra.

    public PlayerController playerController; // Tham chiếu đến script PlayerController để truy cập Animator và các trạng thái khác của người chơi.
    public Transform Attack; // Transform cha để quản lý các hitbox tấn công.

    [Header("Prefabs")] // Đánh dấu các thuộc tính liên quan đến Prefab.
    public GameObject Slash1Hitbox; // Prefab cho hitbox của đòn tấn công đầu tiên (Slash 1).
    public GameObject Slash2Hitbox; // Prefab cho hitbox của đòn tấn công thứ hai (Slash 2 - Combo).
    public GameObject DashAttackHitbox; // Prefab cho hitbox của đòn tấn công sau khi dash.
    
    [Header("Attack State")]
    private bool isAttacking = false; // Cờ trạng thái, true nếu người chơi đang trong quá trình thực hiện một đòn tấn công.
    private bool isAttackSequenceStarted = false; // Cờ trạng thái mới: true nếu người chơi đang trong một chuỗi tấn công (giữ chuột).

    private GameObject currentHitbox; // Biến chung để lưu trữ hitbox hiện tại, thay thế cho varhitbox1.
    [Header("SoundEffect")]
    public AudioSource SfxSource;
    public AudioClip SwingSlash1;
    public AudioClip SwingSlash2;
    public AudioClip SwingSlash3;
    public AudioClip SwingSlash4;
    public AudioClip SwingSlash5;
    
    // Hàm Update được gọi mỗi frame. Dùng để xử lý input và logic dựa trên thời gian.
    void Update()
    {
        // Không cho phép tấn công thường khi đang dash hoặc đang tụ lực dash.
        if (playerController.isDash || playerController.isChargingDash)
        {
            isAttackSequenceStarted = false; // Reset chuỗi tấn công nếu dash ngắt.
            return;
        }
        
        // Lấy trạng thái input của chuột.
        bool isMouseDown = Input.GetMouseButtonDown(0);
        bool isMouseHeld = Input.GetMouseButton(0);
        bool isMouseUp = Input.GetMouseButtonUp(0);

        // --- LOGIC MỚI ---

        // Nếu đang trong animation tấn công, không nhận input mới.
        // Chỉ kiểm tra nếu người chơi nhả chuột để kết thúc chuỗi tấn công.
        if (isAttacking)
        {
            if (isMouseUp)
            {
                isAttackSequenceStarted = false;
            }
            return;
        }

        // Nếu người chơi đang giữ chuột
        if (isMouseHeld)
        {
            // Nếu chuỗi tấn công CHƯA bắt đầu (đây là lần nhấn đầu tiên hoặc một lần click mới)
            // -> Thực hiện đòn Slash1.
            if (!isAttackSequenceStarted)
            {
                isAttackSequenceStarted = true;
                PlaySlash1();
            }
            // Nếu chuỗi tấn công ĐÃ bắt đầu (người chơi vẫn đang giữ chuột sau đòn đánh trước)
            // -> Thực hiện đòn DoubleSlash lặp lại.
            else
            {
                PlayDoubleSlash();
            }
        }
        
        // Nếu người chơi nhả chuột, kết thúc chuỗi tấn công.
        if (isMouseUp)
        {
            isAttackSequenceStarted = false;
        }
    }

    // Hàm thực hiện đòn tấn công Slash 1.
    void PlaySlash1()
    {
        isAttacking = true; // Đánh dấu đang tấn công để khóa input.
        playerController.animator.SetTrigger("Slash1"); // Kích hoạt animation.
    }

    // Hàm thực hiện đòn tấn công Double Slash.
    void PlayDoubleSlash()
    {
        isAttacking = true; // Đánh dấu đang tấn công để khóa input.
        playerController.animator.SetTrigger("DoubleSlash"); // Kích hoạt animation.
    }
    
    // Hàm thực hiện đòn tấn công Dash.
    public void PerformDashAttack()
    {
        isAttacking = true;
        playerController.animator.SetTrigger("DashAttack");
    }

    // Hàm để ngắt tấn công.
    public void InterruptAttack()
    {
        // Nếu đang trong một đòn đánh, hãy hủy nó.
        if (isAttacking)
        {
            isAttacking = false; // Mở khóa input ngay lập tức.
            
            // Reset các trigger để animation tấn công không bị "nhớ" và chạy sau khi dash.
            playerController.animator.ResetTrigger("Slash1");
            playerController.animator.ResetTrigger("DoubleSlash");
            playerController.animator.ResetTrigger("DashAttack");

            // Hủy hitbox đang tồn tại (nếu có) để tránh "hitbox ma".
            if (currentHitbox != null)
            {
                Destroy(currentHitbox);
            }
        }
    }
    
    
    // -------- Animation Events --------
    // Các hàm này được gọi trực tiếp từ các Animation Event trên các clip animation tấn công.

    // Sự kiện animation: Tạo hitbox cho đòn Slash 1.
    public void SpawnSlash1()
    {
        // <<< THAY ĐỔI: Sử dụng rotation của ATTSPAWM thay vì Quaternion.identity >>>
        currentHitbox = Instantiate(Slash1Hitbox, ATTSPAWM.position, ATTSPAWM.rotation);
        currentHitbox.transform.localScale = transform.localScale; // <<< THÊM DÒNG NÀY
        currentHitbox.transform.SetParent(Attack);
    }

    // Sự kiện animation: Hoàn thành đòn Slash 1, xóa hitbox.
    public void CompletedSlash1()
    {
        if (currentHitbox != null)
        {
            Destroy(currentHitbox);
        }
    }

    // Sự kiện animation: Tạo hitbox cho đòn Slash 2 (Double Slash).
    public void SpawnSlash2()
    {
        // <<< THAY ĐỔI: Sử dụng rotation của ATTSPAWM thay vì Quaternion.identity >>>
        var hitbox = Instantiate(Slash2Hitbox, ATTSPAWM.position, ATTSPAWM.rotation);
        hitbox.transform.localScale = transform.localScale; // <<< THÊM DÒNG NÀY
        hitbox.transform.SetParent(Attack);
        Destroy(hitbox, 0.3f);
    }
    
    // Animation Event cho Dash Attack
    public void SpawnDashAttackHitbox()
    {
        // Đảm bảo bạn đã gán Prefab trong Inspector.
        if (DashAttackHitbox != null)
        {
            // <<< THAY ĐỔI: Sử dụng rotation của ATTSPAWM thay vì Quaternion.identity >>>
            var hitbox = Instantiate(DashAttackHitbox, ATTSPAWM.position, ATTSPAWM.rotation);
            hitbox.transform.localScale = transform.localScale; // <<< THÊM DÒNG NÀY
            hitbox.transform.SetParent(Attack);
            Destroy(hitbox, 0.35f); // Thời gian tồn tại của hitbox có thể tùy chỉnh.
        }
    }

    // Sự kiện animation: Kết thúc một đòn tấn công (cả Slash 1 và Double Slash).
    public void EndAttack()
    {
        // Reset các trigger animation tấn công.
        playerController.animator.ResetTrigger("Slash1");
        playerController.animator.ResetTrigger("DoubleSlash");
        playerController.animator.ResetTrigger("DashAttack");
        
        isAttacking = false; // Đánh dấu đã kết thúc tấn công, cho phép nhận input mới ở frame tiếp theo.

        // Cập nhật lại trạng thái idle/running sau khi tấn công xong.
        if (playerController.isGround)
        {
            var x = Input.GetAxis("Horizontal");
            playerController.animator.SetBool("isIdie", x == 0);
            playerController.animator.SetBool("isRunning", x != 0);
        }
    }

    public void SoundEffectSlash1()
    {
        int RandomSFX = Random.Range(1, 5);
        switch (RandomSFX)
        {
            case (1):
                SfxSource.PlayOneShot(SwingSlash1);
                break;
            case (2):
                SfxSource.PlayOneShot(SwingSlash2);
                break;
            case (3):
                SfxSource.PlayOneShot(SwingSlash3);
                break;
            case (4):
                SfxSource.PlayOneShot(SwingSlash4);
                break;
            case (5):
                SfxSource.PlayOneShot(SwingSlash5);
                break;
        }
    }
}