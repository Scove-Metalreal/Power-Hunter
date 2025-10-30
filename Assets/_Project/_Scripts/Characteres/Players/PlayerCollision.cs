using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Cần thiết cho việc làm việc với UI như GameObject và các thành phần UI.

// Lớp này chịu trách nhiệm xử lý các va chạm của người chơi với các đối tượng khác trong game,
// bao gồm cả va chạm với môi trường, kẻ địch, và các trigger.
public class PlayerCollision : MonoBehaviour
{
    // --- THAM CHIẾU COMPONENT ---
    private PlayerController
        playerController; // Tham chiếu đến script PlayerController để điều khiển hành vi và trạng thái của người chơi.

    private PlayerStat
        playerStat; // Tham chiếu đến script PlayerStat để quản lý các chỉ số như máu và khả năng hồi phục.

    public GameManager
        gameManager; // Tham chiếu đến GameManager để quản lý các trạng thái tổng thể của trò chơi (ví dụ: kết thúc game).

    [Header("Knockback Settings")] // Đánh dấu các thuộc tính liên quan đến thiết lập Knockback.
    public float KnockBackSpeed = 10f; // Tốc độ mà người chơi bị văng ra khi nhận sát thương.

    [Header("UI")] // Đánh dấu các thuộc tính liên quan đến giao diện người dùng.
    public GameObject LoseUIPanel; // Tham chiếu đến GameObject chứa bảng UI hiển thị khi người chơi thua.

    [Header("Player Lives")] // --- THÊM ---
    public int lifeCount = 3; // Tổng số mạng của người chơi (mặc định 3)

    public float respawnDelay = 1.2f; // Thời gian chờ trước khi hồi sinh

    [Header("Shop Settings")] public GameObject shopUI; // Giao diện shop (gán trong Inspector)
    private bool isNearShop = false; // Kiểm tra xem player có đang trong vùng shop không



    // Hàm Start được gọi một lần khi script được kích hoạt.
    void Start()
    {
        // Lấy các component cần thiết được đính kèm với GameObject Player.
        playerController = GetComponent<PlayerController>();
        playerStat = GetComponent<PlayerStat>();

        // --- THAY ĐỔI: Tìm GameManager trong scene thay vì get component từ Player ---
        // Điều này hữu ích nếu GameManager không được đính kèm trực tiếp lên GameObject Player.
        gameManager = FindAnyObjectByType<GameManager>();

        // Kiểm tra an toàn để tránh lỗi nếu quên gán UI trong Inspector.
        if (LoseUIPanel != null)
        {
            LoseUIPanel.SetActive(false); // Ẩn bảng thua khi bắt đầu.
        }
        else
        {
            Debug.LogWarning("LoseUIPanel chưa được gán trong PlayerCollision!");
        }
    }

    // --- XÓA BỎ: Toàn bộ logic check ground cũ đã được di chuyển sang PlayerController ---
    // Các hàm OnCollisionEnter2D và OnCollisionExit2D đã bị xóa vì logic kiểm tra mặt đất
    // đã được tập trung và xử lý hiệu quả hơn trong `PlayerController.FixedUpdate`.
    // Điều này giúp script này chỉ tập trung vào xử lý va chạm và các hiệu ứng liên quan.

    // Hàm OnTriggerEnter2D được gọi khi một Collider khác đi vào Trigger Collider của đối tượng này.
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("GroundTrap"))
        {
            FindAnyObjectByType<FallingGround>().TriggerCollapse();
            // FindAnyObjectByType<CameraShake>().StartCoroutine(FindAnyObjectByType<CameraShake>().Shake());
        }

        // Xử lý va chạm với vùng chết (DeathZone).
        if (collision.CompareTag("DeathZone"))
        {
            // Dùng lại logic knockback và sát thương chung để xử lý va chạm DeathZone.
            // Lượng sát thương là 20f. Transform của DeathZone được truyền vào để tính hướng knockback.
            HandleDamageAndKnockback(20f, collision.transform);
        }

        // <<< LOGIC MỚI: Xử lý va chạm với hitbox của Enemy
        // Khi người chơi va chạm với hitbox của kẻ địch.
        if (collision.CompareTag("EnemyHitbox"))
        {
            // Lấy component Hitbox từ đối tượng va chạm để biết thông tin (ví dụ: sát thương).
            Hitbox hitbox = collision.GetComponent<Hitbox>();
            if (hitbox != null)
            {
                Debug.Log("<color=cyan>PLAYER: Đã va chạm với EnemyHitbox!</color>"); // <<< THÊM DÒNG NÀY
                Debug.Log("PLAYER: Đọc được " + hitbox.damage + " sát thương từ hitbox.");
                // Gọi hàm xử lý sát thương và knockback với thông tin từ hitbox.
                HandleDamageAndKnockback(hitbox.damage, collision.transform);
            }
            else
            {
                Debug.LogWarning("Found 'EnemyHitbox' tag but no Hitbox component on: " + collision.gameObject.name);
            }
        }

        // <<< LOGIC MỚI: Xử lý va chạm với vật phẩm (Item)
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                item.Pickup(this.gameObject);
            }
        }

        if (collision.CompareTag("SpikyTrap"))
        {
            SpikyTrap trap = collision.GetComponent<SpikyTrap>();
            if (trap != null)
            {
                HandleDamageAndKnockback(trap.damage, collision.transform);
            }
        }

        if (collision.CompareTag("Shop"))
        {
            isNearShop = true;

            // Tìm UI "Press E" nằm trong Shop (nếu có)
            Transform pressEInShop = collision.transform.Find("Canvas");
            if (pressEInShop != null)
            {
            }
        }
    }
    

    // Hàm OnTriggerExit2D được gọi khi một Collider khác rời khỏi Trigger Collider của đối tượng này.
    private void OnTriggerExit2D(Collider2D collision)
    {
        // <<< LOGIC CŨ ĐÃ BỊ LOẠI BỎ (hoặc điều chỉnh)
        // TỐI ƯU: Xử lý khi thoát khỏi vùng tìm kiếm của Enemy
        // if (collision.CompareTag("FindPlayer"))
        // {
        //     Enemy enemy = collision.GetComponentInParent<Enemy>();
        //     if (enemy != null)
        //     {
        //         enemy.isCollidingWithPlayer = false;
        //     }
        // }

        // --- THÊM MỚI: Khi player rời khỏi vùng shop ---
        if (collision.CompareTag("Shop"))
        {
            isNearShop = false;

            // Ẩn lại UI “Press E” của shop
            Transform pressEInShop = collision.transform.Find("Canvas");
            if (pressEInShop != null)
            {
                pressEInShop.gameObject.SetActive(false);
            }

            if (shopUI != null) shopUI.SetActive(false);
        }
    }

    // <<< HÀM MỚI: Gom logic nhận sát thương và knockback vào một chỗ
    // Hàm này xử lý việc người chơi nhận sát thương và bị đẩy lùi (knockback).
    // damage: Lượng sát thương nhận vào.
    // damageSource: Transform của đối tượng gây sát thương, dùng để tính hướng knockback.
    private void HandleDamageAndKnockback(float damage, Transform damageSource)
    {
        // Trừ máu người chơi.
        playerStat.TakeDamage(damage);

        // Tạm thời vô hiệu hóa script PlayerController để ngăn input và hành động khác trong khi bị knockback.
        if (playerController != null) playerController.enabled = false;

        // Tính toán hướng văng ra: Luôn đẩy người chơi ra xa nguồn sát thương.
        Vector2 knockbackDir = (transform.position - damageSource.position).normalized;

        // Lấy Rigidbody2D của người chơi để áp dụng lực.
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Reset vận tốc hiện tại để lực knockback có hiệu lực hoàn toàn.
            rb.AddForce(knockbackDir * KnockBackSpeed, ForceMode2D.Impulse); // Áp dụng lực knockback.
        }
        else
        {
            Debug.LogWarning("Rigidbody2D not found for knockback on: " + gameObject.name);
        }

        // Bắt đầu Coroutine để xử lý việc phục hồi sau knockback.
        StartCoroutine(RecoverFromKnockback(0.4f)); // Thời gian phục hồi là 0.4 giây.

        // --- THÊM: Xử lý mạng sống và respawn ---
        if (playerStat.HeathPlayer <= 0)
        {
            lifeCount--;
            Debug.Log("Player mất 1 mạng, còn lại: " + lifeCount);

            if (lifeCount > 0)
            {
                StartCoroutine(RespawnPlayer());
            }
        }
    }

    // Coroutine để phục hồi sau khi Knockback.
    IEnumerator RecoverFromKnockback(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerController != null) playerController.enabled = true;
    }

    // --- THÊM: Coroutine hồi sinh người chơi ---
    IEnumerator RespawnPlayer()
    {
        this.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        Vector3 respawnPos = CheckPoint.GetRespawnPosition();
        if (respawnPos == Vector3.zero)
        {
            Debug.LogWarning("Không có checkpoint nào, respawn về vị trí (0,0,0)");
        }

        transform.position = respawnPos;
        playerStat.HeathPlayer = 100f; // --- SỬA LẠI: Hồi máu đầy theo cách 3 ---
        Debug.Log("Hồi sinh tại checkpoint: " + respawnPos);

        if (playerController != null)
        {
            playerController.animator.SetBool("isDead", false);
            playerController.enabled = true;
        }

        this.enabled = true;
    }

    // Hàm xử lý khi người chơi chết.
    void PlayerDead()
    {
        // Nếu có Animator, bật cờ "isDead" để kích hoạt animation chết.
        if (playerController.animator != null)
        {
            playerController.animator.SetBool("isDead", true);
        }

        this.enabled = false;
        if (playerController != null) playerController.enabled = false;
    }

    // --- CÁC HÀM NÀY ĐƯỢC GỌI TỪ ANIMATION EVENT TRÊN ANIMATION "Dead" ---
    public void StartDeadAnimation()
    {
        Time.timeScale = 0f;
    }

    public void EndDeadAnimation()
    {
        Destroy(gameObject);

        if (LoseUIPanel != null && gameManager != null)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            LoseUIPanel.SetActive(true);
            gameManager.isGameEnd = true;

            if (gameManager.gamePauseUI != null)
            {
                gameManager.gamePauseUI.SetActive(false);
            }
        }
        else
        {
            if (LoseUIPanel == null) Debug.LogWarning("LoseUIPanel is null when EndDeadAnimation is called.");
            if (gameManager == null) Debug.LogWarning("GameManager is null when EndDeadAnimation is called.");
        }
    }

    void Update()
    {
        // --- THÊM LOGIC MỞ SHOP ---
        // Kiểm tra khi người chơi đang trong vùng shop và nhấn phím E
        if (isNearShop && Input.GetKeyDown(KeyCode.E))
        {
            if (shopUI != null)
            {
                bool isActive = shopUI.activeSelf;
                shopUI.SetActive(!isActive); // Bật / tắt shop
            }
            else
            {
                Debug.LogWarning("ShopUI chưa được gán trong PlayerCollision!");
            }
        }
    }
}