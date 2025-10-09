using UnityEngine;

// Lớp Enemy quản lý hành vi của kẻ địch, bao gồm di chuyển, phát hiện người chơi,
// tấn công, nhận sát thương và trạng thái của nó thông qua một State Machine.
public class Enemy : MonoBehaviour
{
    // Hệ thống State Machine: Định nghĩa các trạng thái khác nhau mà enemy có thể ở.
    public enum EnemyState { Spawning, Patrolling, Chasing, Attacking, Hit, Dying }
    private EnemyState currentState; // Biến lưu trạng thái hiện tại của enemy.

    [Header("Movement Stats")] // Đánh dấu các thuộc tính liên quan đến thống kê di chuyển.
    public float patrolSpeed = 1.5f; // Tốc độ di chuyển khi enemy đang tuần tra.
    public float chaseSpeed = 3f;    // Tốc độ di chuyển khi enemy đang đuổi theo người chơi.

    [Header("AI Settings")] // Đánh dấu các thuộc tính liên quan đến cài đặt Trí tuệ Nhân tạo.
    public float detectionRange = 8f; // Tầm nhìn của enemy: Khoảng cách mà enemy có thể phát hiện người chơi.
    public float attackRange = 1.5f; // Tầm đánh cận chiến: Khoảng cách mà enemy có thể tấn công người chơi.
    public LayerMask whatIsPlayer; // Layer mask để xác định những gì được coi là người chơi.
    public LayerMask whatBlocksSight; // Layer mask để xác định các vật cản tầm nhìn (ví dụ: Tường, Đất).
    public float loseSightTime = 3f; // Thời gian enemy sẽ từ bỏ việc truy đuổi nếu không thấy người chơi nữa.

    [Header("Gravity Settings")] // Đánh dấu các thuộc tính liên quan đến trọng lực tùy chỉnh.
    public float gravityStrength = 50f; // Sức mạnh của trọng lực tùy chỉnh được áp dụng cho enemy.
    private Vector2 gravityDirection; // Hướng của trọng lực tùy chỉnh (được thiết lập khi khởi tạo).

    [Header("Ground & Wall Check")] // Đánh dấu các thuộc tính liên quan đến việc kiểm tra mặt đất và tường.
    public Transform groundCheckPoint; // Điểm kiểm tra mặt đất.
    public Transform wallCheckPoint; // Điểm kiểm tra tường phía trước enemy.
    public float checkDistance = 0.3f; // Tăng nhẹ giá trị mặc định để kiểm tra chính xác hơn.
    public LayerMask whatIsGround; // Layer mask để xác định những gì được coi là mặt đất (cũng có thể bao gồm tường).
    private bool isGrounded; // Cờ trạng thái, true nếu enemy đang chạm đất.
    private bool isTouchingWall; // Cờ trạng thái, true nếu enemy đang chạm tường.

    // Components & References
    private Rigidbody2D rb; // Tham chiếu đến thành phần Rigidbody2D để xử lý vật lý.
    private Animator anim;  // Tham chiếu đến thành phần Animator để điều khiển animation.
    private Transform playerTransform; // Tham chiếu đến transform của Player để biết vị trí của người chơi.
    private float timeSinceLastSeenPlayer; // Bộ đếm thời gian để enemy từ bỏ việc truy đuổi nếu không thấy người chơi.
    private Vector2 moveDirection = Vector2.right; // Hướng di chuyển cục bộ của enemy (mặc định là sang phải).

    // Hàm Awake được gọi trước Start, dùng để khởi tạo các tham chiếu component.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 0; // Vô hiệu hóa trọng lực mặc định của Unity để sử dụng trọng lực tùy chỉnh.

        // Tìm GameObject có tag "Player" và lấy Transform của nó.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    // Hàm Initialize được gọi bởi Spawner hoặc script khác để thiết lập enemy.
    // gravityDir: Hướng trọng lực mà enemy sẽ chịu.
    public void Initialize(Vector2 gravityDir)
    {
        gravityDirection = gravityDir.normalized; // Chuẩn hóa vector hướng trọng lực.
        
        // Tính toán góc xoay dựa trên hướng trọng lực để enemy đứng thẳng.
        // `Mathf.Atan2` trả về góc (radian) giữa trục X dương và điểm (x, y).
        // Chúng ta muốn `transform.up` chỉ về hướng ngược lại của trọng lực (`-gravityDirection`).
        // Góc tính toán sẽ là góc giữa trục X và `(-gravityDirection.y, -gravityDirection.x)`.
        float angle = Mathf.Atan2(-gravityDirection.y, -gravityDirection.x) * Mathf.Rad2Deg;
        // Cộng hoặc trừ 90 độ để điều chỉnh hướng "lên" của enemy tùy thuộc vào hệ trục.
        // Ở đây -90 độ dường như phù hợp với cách enemy được thiết lập.
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        
        // Thiết lập hướng di chuyển ban đầu. Nếu trọng lực hướng xuống, hướng di chuyển ban đầu có thể là phải.
        // Nếu trọng lực hướng sang trái, hướng di chuyển ban đầu có thể là lên.
        // Điều này có thể cần điều chỉnh thêm dựa trên hệ thống rotate và hướng mặc định của enemy.
        if (gravityDirection == Vector2.down) moveDirection = Vector2.right;
        else if (gravityDirection == Vector2.up) moveDirection = Vector2.left;
        else if (gravityDirection == Vector2.left) moveDirection = Vector2.up;
        else if (gravityDirection == Vector2.right) moveDirection = Vector2.down;
        
        SetState(EnemyState.Spawning); // Bắt đầu vòng đời của Enemy ở trạng thái Spawning.
    }

    // Hàm Update được gọi mỗi frame. Dùng để xử lý logic dựa trên input và trạng thái không phụ thuộc vào vật lý.
    void Update()
    {
        Debug.Log("Current State: " + currentState);
        // <<< LOGIC MỚI: Chỉ xử lý thay đổi trạng thái trong Update.
        HandleStateTransitions();
        // Cập nhật Animator dựa trên trạng thái và hành vi hiện tại.
        UpdateAnimator();
    }

    // Hàm FixedUpdate được gọi theo khoảng thời gian vật lý cố định. Thích hợp cho các phép tính vật lý.
    void FixedUpdate()
    {
        // <<< LOGIC MỚI: Toàn bộ vật lý được chuyển vào FixedUpdate.
        // Chỉ áp dụng trọng lực nếu không ở trên mặt đất để tránh bị "lún" vào mặt đất.
        // Khi enemy chạm đất, trọng lực sẽ được giữ ở mức tối thiểu hoặc 0 để tránh bị kẹt.
        if (!isGrounded)
        {
            rb.AddForce(gravityDirection * gravityStrength);
            Debug.Log("Apply custom garvity: " + gravityDirection * gravityStrength);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            Debug.Log("Enemy is grounded.");
        }
        // Xử lý di chuyển và kiểm tra môi trường.
        HandleMovement();
    }
    
    // <<< LOGIC MỚI: Chỉ xử lý thay đổi trạng thái trong Update.
    // Hàm này quản lý việc chuyển đổi giữa các trạng thái AI của enemy.
    private void HandleStateTransitions()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                // Nếu thấy người chơi, chuyển sang Chasing.
                if (CanSeePlayer()) SetState(EnemyState.Chasing);
                break;

            case EnemyState.Chasing:
                // Nếu vẫn thấy người chơi:
                if (CanSeePlayer())
                {
                    timeSinceLastSeenPlayer = 0; // Reset thời gian quên người chơi.
                    float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                    // Nếu trong tầm đánh, chuyển sang Attacking.
                    if (distanceToPlayer <= attackRange) SetState(EnemyState.Attacking);
                }
                else // Nếu không còn thấy người chơi:
                {
                    timeSinceLastSeenPlayer += Time.deltaTime; // Tăng thời gian kể từ lần cuối nhìn thấy.
                    if (timeSinceLastSeenPlayer > loseSightTime) SetState(EnemyState.Patrolling); // Nếu quá thời gian, quay về tuần tra.
                }
                break;

            case EnemyState.Attacking:
                // Tạm thời quay về Chasing sau khi tấn công (sẽ cải tiến sau).
                // Logic tấn công sẽ cần một Animation Event để chuyển trạng thái sau khi tấn công xong.
                SetState(EnemyState.Chasing);
                break;

            // Các trạng thái khác (Spawning, Hit, Dying) sẽ có logic riêng.
        }
    }

    // <<< LOGIC MỚI: Toàn bộ vật lý và di chuyển được xử lý trong FixedUpdate.
    // Hàm này thực hiện việc kiểm tra môi trường và áp dụng vận tốc cho enemy.
    private void HandleMovement()
    {
        // Kiểm tra môi trường (đất và tường).
        CheckEnvironment();

        // Xử lý di chuyển dựa trên trạng thái hiện tại.
        if (currentState == EnemyState.Patrolling)
        {
            // Nếu đang tuần tra và chạm đất:
            if (isGrounded)
            {
                // Nếu chạm tường, quay đầu.
                if (isTouchingWall) Flip();
                // Di chuyển theo hướng `transform.right`.
                rb.linearVelocity = transform.right * patrolSpeed;
            }
            else
            {
                // Nếu đang ở trên không, chỉ giữ lại vận tốc theo hướng trọng lực.
                // Điều này ngăn enemy trượt ngang vô hạn khi rơi.
                rb.linearVelocity = Vector2.Dot(rb.linearVelocity, gravityDirection) * gravityDirection;
            }
        }
        else if (currentState == EnemyState.Chasing)
        {
            // Logic đuổi theo sẽ được chỉnh sửa tương tự trong tương lai, tạm thời giữ nguyên.
            // Nếu thấy người chơi:
            if (CanSeePlayer())
            {
                // Tính toán hướng đến người chơi.
                Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
                // Đảm bảo enemy xoay mặt về phía người chơi.
                if (Vector2.Dot(directionToPlayer, transform.right) < 0) Flip();
                // Di chuyển về phía người chơi với tốc độ chaseSpeed.
                rb.linearVelocity = transform.right * chaseSpeed;
            }
            else
            {
                // Nếu không thấy người chơi, dừng lại (hoặc quay về Patrolling sau khi hết loseSightTime).
                rb.linearVelocity = Vector2.zero;
            }
        }
        else // Nếu ở các trạng thái khác (Attacking, Hit, Dying, Spawning)
        {
            // Dừng lại nếu không có logic di chuyển cụ thể cho trạng thái đó.
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    // <<< LOGIC MỚI: Cập nhật Animator dựa trên trạng thái và hành vi hiện tại.
    // Hàm này đặt các biến (bool, float, int) trên Animator Controller.
    private void UpdateAnimator()
    {
        // Chỉ bật animation `isWalking` nếu enemy đang di chuyển và ở trạng thái tuần tra hoặc truy đuổi.
        bool isMoving = (currentState == EnemyState.Patrolling || currentState == EnemyState.Chasing);
        anim.SetBool("isWalking", isMoving); // Đặt trạng thái animation `isWalking`.
    }


    // Hàm kiểm tra xem có thể nhìn thấy người chơi không bằng Raycast.
    private bool CanSeePlayer()
    {
        // Nếu chưa tìm thấy Player, không thể nhìn thấy.
        if (playerTransform == null) return false;
        // Tính khoảng cách đến người chơi.
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        // Chỉ kiểm tra nếu người chơi nằm trong tầm nhìn `detectionRange`.
        if (distanceToPlayer <= detectionRange)
        {
            // Tính toán hướng từ enemy đến người chơi.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (playerTransform.position - transform.position).normalized, detectionRange, whatBlocksSight | whatIsPlayer);
            // Kiểm tra kết quả raycast: có va chạm và va chạm đó là với người chơi không.
            return hit.collider != null && (whatIsPlayer.value & (1 << hit.collider.gameObject.layer)) > 0;
        }
        // Nếu người chơi ngoài tầm nhìn hoặc bị chặn bởi vật cản, thì không nhìn thấy.
        return false;
    }

    // Kiểm tra các yếu tố môi trường như mặt đất và tường bằng Raycast.
    private void CheckEnvironment()
    {
        // Kiểm tra mặt đất: Raycast từ `groundCheckPoint` theo hướng trọng lực (`gravityDirection`).
        isGrounded = Physics2D.Raycast(groundCheckPoint.position, gravityDirection, checkDistance, whatIsGround);
        // Kiểm tra tường: Raycast từ `wallCheckPoint` theo hướng `transform.right` (hướng mặt của enemy).
        // Sử dụng `whatIsGround` để coi tường là vật cản tương tự mặt đất trong trường hợp này.
        isTouchingWall = Physics2D.Raycast(wallCheckPoint.position, transform.right, checkDistance, whatIsGround);
        
        Debug.Log("Is grounded: " + isGrounded + "\nIs touching wall: " + isTouchingWall);
    }

    // Hàm để lật mặt enemy (quay ngang).
    // Hàm này làm cho enemy quay mặt về hướng di chuyển hoặc hướng nhìn mới.
    private void Flip()
    {
        // Xoay enemy 180 độ quanh trục Y cục bộ của nó. Điều này làm cho nó quay mặt sang hướng ngược lại.
        // Lưu ý: Nếu transform của bạn không có scale ban đầu là 1, hoặc có các phép xoay khác, logic này có thể cần điều chỉnh.
        transform.Rotate(0f, 180f, 0f);
        moveDirection = -moveDirection;
    }

    // Hàm thay đổi trạng thái của enemy.
    private void SetState(EnemyState newState)
    {
        // Chỉ thay đổi trạng thái nếu trạng thái mới khác với trạng thái hiện tại.
        if (currentState == newState) return;
        currentState = newState;
    }

    // Hàm này được gọi bởi Animation Event trên animation "Spawning" khi animation kết thúc.
    public void FinishSpawning()
    {
        SetState(EnemyState.Patrolling); // Chuyển sang trạng thái tuần tra sau khi hoàn thành quá trình sinh ra.
    }

    // Vẽ các tia debug để kiểm tra trên Scene View.
    private void OnDrawGizmos()
    {
        // Vẽ tia kiểm tra mặt đất.
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red; // Màu đỏ cho tia kiểm tra đất.
            // Vẽ đường thẳng từ điểm kiểm tra theo hướng trọng lực với khoảng cách checkDistance.
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + (Vector3)(gravityDirection * checkDistance));
        }
        // Vẽ tia kiểm tra tường.
        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.blue; // Màu xanh cho tia kiểm tra tường.
            // Vẽ đường thẳng từ điểm kiểm tra tường theo hướng mặt của enemy (`transform.right`).
            Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + transform.right * checkDistance);
        }
    }
}