using UnityEngine;
using System.Collections;

// Script này điều khiển hành vi của một kẻ địch đơn giản: đi tuần tra và tấn công khi người chơi lại gần.
public class Enemy : MonoBehaviour
{
    // --- CÁC BIẾN THAM CHIẾU VÀ TRẠNG THÁI ---
    private Animator anim; // Tham chiếu đến component Animator để điều khiển animation.
    private int attackCount = 0; // Biến đếm để thay đổi giữa các đòn tấn công (2 lần đánh thường, 1 lần đánh xa).
    
    // Cờ trạng thái, được bật lên (thường từ một script khác, ví dụ trigger collider) khi người chơi ở trong tầm đánh.
    public bool isCollidingWithPlayer = false; 

    // --- CÁC BIẾN DI CHUYỂN VÀ VẬT LÝ ---
    public float moveSpeed = 2f; // Tốc độ di chuyển của enemy.
    private bool Left = false, Right = false; // Cờ để xác định hướng di chuyển/tấn công hiện tại.
    private bool isGrounded = false; // Cờ kiểm tra xem enemy có đang đứng trên mặt đất không.
    private Rigidbody2D rb; // Tham chiếu đến component Rigidbody2D để điều khiển vật lý.
    
    // Cờ trạng thái CỰC KỲ QUAN TRỌNG, dùng để 'khóa' hành động của enemy khi nó đang tấn công, tránh việc nó vừa đi vừa đánh.
    private bool isattack = false; 

    // --- CÁC BIẾN CHO VIỆC TẤN CÔNG ---
    public GameObject HitBoxAttack1; // Prefab của hitbox cho đòn đánh thường (melee).
    public GameObject HitBoxAttack2; // Prefab của hitbox cho đòn đánh xa (projectile/đạn).
    private GameObject VARHitBoxAttack1; // Biến để lưu trữ hitbox đòn 1 được tạo ra trong game, để có thể hủy nó đi.
    private GameObject VARHitBoxAttack2; // Biến để lưu trữ hitbox đòn 2 được tạo ra trong game.
    public Transform AttackSpawm; // Vị trí để sinh ra hitbox của đòn đánh 1.
    public Transform AttackSpawm2; // Vị trí để sinh ra hitbox của đòn đánh 2.
    public float Attack2BulletSpeed; // Tốc độ của viên đạn trong đòn đánh 2.
    public Transform Parent; // Dùng để làm cha cho hitbox, giúp hitbox di chuyển theo enemy hoặc giữ cho Hierarchy gọn gàng.

    void Start()
    {
        // Lấy các component cần thiết khi game bắt đầu.
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // Bắt đầu vòng lặp hành vi chính của enemy.
        StartCoroutine(MoveRoutine());
    }

    // Coroutine này là bộ não chính của AI, giúp enemy lặp đi lặp lại hành động tuần tra.
    IEnumerator MoveRoutine()
    {
        while (true) // Vòng lặp vô tận để enemy luôn hoạt động.
        {
            // --- ĐIỀU KIỆN AN TOÀN ---
            // Nếu không đứng trên mặt đất HOẶC đang trong animation tấn công, enemy sẽ đứng yên.
            if (!isGrounded || anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                rb.linearVelocity = Vector2.zero; // Dừng di chuyển.
                anim.SetBool("isWalking", false); // Tắt animation đi bộ.
                yield return null; // Chờ đến frame tiếp theo.
                continue; // Bỏ qua phần còn lại của vòng lặp và bắt đầu lại từ đầu.
            }

            // --- GIAI ĐOẠN ĐI BÊN TRÁI ---
            anim.SetBool("isWalking", true); // Bật animation đi bộ.
            transform.localScale = new Vector3(1, 1, 1); // Quay mặt sang trái (giả sử sprite gốc quay sang trái).
            Left = true;
            Right = false;
            float t = 0;
            // Di chuyển sang trái trong vòng 2 giây, nhưng sẽ dừng nếu không còn trên mặt đất hoặc bắt đầu tấn công.
            while (t < 2f && isGrounded && isattack == false)
            {
                rb.linearVelocity = Vector2.left * moveSpeed;
                t += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(2f); // Đợi 2 giây sau khi đi xong.

            rb.linearVelocity = Vector2.zero; // Dừng lại.
            anim.SetBool("isWalking", false);
            yield return new WaitForSeconds(2f); // Đứng yên 2 giây.

            // --- GIAI ĐOẠN ĐI BÊN PHẢI ---
            anim.SetBool("isWalking", true);
            transform.localScale = new Vector3(-1, 1, 1); // Lật sprite để quay mặt sang phải.
            Left = false;
            Right = true;
            t = 0;
            while (t < 2f && isGrounded && isattack == false)
            {
                rb.linearVelocity = Vector2.right * moveSpeed;
                t += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(2f);

            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);
            yield return new WaitForSeconds(2f);
        }
    }

    void Update()
    {
        // Liên tục kiểm tra trong mỗi frame.
        // Nếu người chơi ở trong tầm và enemy không đang bận tấn công.
        if (isCollidingWithPlayer && isattack == false)
        {
            DoAttack(); // Thực hiện tấn công.
        }
    }

    // Hàm quyết định và kích hoạt animation tấn công.
    public void DoAttack()
    {
        isattack = true; // Khóa hành vi di chuyển.
        rb.linearVelocity = Vector2.zero; // Đảm bảo enemy đứng yên khi đánh.

        if (attackCount < 2) // Nếu chưa đánh thường đủ 2 lần
        {
            anim.SetTrigger("isAttack1"); // Kích hoạt animation Attack1.
            attackCount++; // Tăng biến đếm.
        }
        else // Nếu đã đánh thường 2 lần rồi
        {
            anim.SetTrigger("isAttack2"); // Kích hoạt animation Attack2.
            attackCount = 0; // Reset biến đếm để chu trình lặp lại.
        }
    }

    // Hàm được gọi tự động khi có va chạm vật lý.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem có va chạm với đối tượng có tag "Ground" không.
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // HÀM NÀY ĐƯỢC GỌI BẰNG ANIMATION EVENT //
    // Nó được gọi tại thời điểm trong animation mà đòn đánh được tung ra.
    public void Attack1Hitbox()
    {
        // Tạo ra hitbox của đòn đánh 1 tại vị trí đã định.
        VARHitBoxAttack1 = Instantiate(HitBoxAttack1, AttackSpawm.position, Quaternion.identity);
        // Gán hitbox làm con của một đối tượng cha (để di chuyển theo hoặc quản lý).
        VARHitBoxAttack1.transform.SetParent(Parent);
    }

    // HÀM NÀY ĐƯỢC GỌI BẰNG ANIMATION EVENT KHI KẾT THÚC ANIMATION ĐÁNH 1 //
    public void EndAttack1()
    {
        // Hủy hitbox đã tạo.
        Destroy(VARHitBoxAttack1);
        // Mở khóa, cho phép enemy hành động trở lại.
        isattack = false;
    }
    
    // HÀM NÀY ĐƯỢC GỌI BẰNG ANIMATION EVENT //
    public void Attack2Hitbox()
    {
        // Tạo ra viên đạn tại vị trí đã định.
        VARHitBoxAttack2 = Instantiate(HitBoxAttack2, AttackSpawm2.position, Quaternion.identity);
        if (Left == true)
        {
            // Bắn đạn sang trái.
            VARHitBoxAttack2.GetComponent<Rigidbody2D>().linearVelocity = Vector2.left * Attack2BulletSpeed;
        }
        if (Right == true)
        {
            // Bắn đạn sang phải.
            VARHitBoxAttack2.GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * Attack2BulletSpeed;
        }
        // Tự hủy viên đạn sau 1 giây để tránh làm đầy scene.
        Destroy(VARHitBoxAttack2, 1f);
    }

    // HÀM NÀY ĐƯỢC GỌI BẰNG ANIMATION EVENT KHI KẾT THÚC ANIMATION ĐÁNH 2 //
    public void EndAttack2()
    {
        // Mở khóa, cho phép enemy hành động trở lại.
        isattack = false;
    }
}