using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
    // --- THAM CHIẾU COMPONENT ---
    private PlayerController playerController;
    private PlayerStat playerStat;
    public GameManager gameManager;
    [Header("Knockback Settings")]
    public float KnockBackSpeed = 10f;

    [Header("UI")]
    public GameObject LoseUIPanel;
    
    
    void Start()
    {
        // Lấy các component cần thiết từ chính Player
        playerController = GetComponent<PlayerController>();
        playerStat = GetComponent<PlayerStat>();
        gameManager = GetComponent<GameManager>();
        // Kiểm tra an toàn để tránh lỗi nếu quên gán UI
        if (LoseUIPanel != null)
        {
            LoseUIPanel.SetActive(false);
        }
    }

    // --- XÓA BỎ: Toàn bộ logic check ground cũ đã được di chuyển sang PlayerController ---
    // Hàm OnCollisionEnter2D và OnCollisionExit2D đã bị xóa vì không còn cần thiết.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Xử lý va chạm với vùng chết (DeathZone)
        if (collision.CompareTag("DeathZone"))
        {
            HandleDeathZoneCollision();
        }

        // TỐI ƯU: Xử lý va chạm với vùng tìm kiếm của Enemy
        if (collision.CompareTag("FindPlayer"))
        {
            // Lấy component Enemy trực tiếp từ đối tượng va chạm, thay vì tìm kiếm cả scene
            Enemy enemy = collision.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.isCollidingWithPlayer = true;
                enemy.DoAttack();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // TỐI ƯU: Xử lý khi thoát khỏi vùng tìm kiếm của Enemy
        if (collision.CompareTag("FindPlayer"))
        {
            Enemy enemy = collision.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.isCollidingWithPlayer = false;
            }
        }
    }

    private void HandleDeathZoneCollision()
    {
        playerStat.TakeDamage(20f);
        
        // Dừng di chuyển của người chơi để thực hiện knockback
        // (Giả sử PlayerController có biến isChangingGravity hoặc tương tự để khóa di chuyển)
        playerController.enabled = false; // Tạm thời vô hiệu hóa script để ngăn input

        // --- ĐƠN GIẢN HÓA LOGIC KNOCKBACK ---
        // Hướng văng ra sẽ luôn ngược lại với hướng trọng lực hiện tại
        Vector2 knockbackDir = Vector2.zero;
        if (playerController.GrafityDown) knockbackDir = Vector2.up;
        else if (playerController.GrafityUp) knockbackDir = Vector2.down;
        else if (playerController.GrafityLeft) knockbackDir = Vector2.right;
        else if (playerController.GrafityRight) knockbackDir = Vector2.left;

        // Reset vận tốc cũ và áp dụng lực văng ra
        var rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDir * KnockBackSpeed, ForceMode2D.Impulse);

        StartCoroutine(RecoverFromKnockback(0.4f));

        if (playerStat.HeathPlayer <= 0)
        {
            PlayerDead();
        }
    }

    IEnumerator RecoverFromKnockback(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerController.enabled = true; // Kích hoạt lại script PlayerController
    }

    void PlayerDead()
    {
        if (playerController.animator != null)
        {
            playerController.animator.SetBool("isDead", true);
        }
        this.enabled = false; // Vô hiệu hóa script này
        playerController.enabled = false; // Vô hiệu hóa script điều khiển
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }

    // Các hàm này được gọi từ Animation Event trên animation "Dead"
    public void StartDeadAnimation()
    {
        Time.timeScale = 0f;
    }

    public void EndDeadAnimation()
    {
        
        Destroy(gameObject);
        if (LoseUIPanel != null)
        {
            LoseUIPanel.SetActive(true);
            gameManager.isGameEnd = true;
            gameManager.gamePauseUI.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

    }
}