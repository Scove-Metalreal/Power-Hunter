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
    
    // <<< THÊM MỚI: Các biến quản lý máu >>>
    [Header("Health Stats")]
    public float maxHealth = 50f;
    private float currentHealth;
    private bool isDead = false; // Cờ để đảm bảo hàm Die() chỉ được gọi một lần

    [Header("Attack Settings")]
    // Sát thương giờ sẽ được thiết lập trực tiếp trên Prefab của Hitbox.

    // --- Attack 1 ---
    public GameObject attack1HitboxPrefab;
    public Transform attack1Point;

    // --- Attack 2 ---
    public GameObject attack2HitboxPrefab;
    public Transform attack2Point;

    public float attackCooldown = 2f;
    private float timeSinceLastAttack = 0f;
    private bool isAttacking = false;

    [Header("Gravity Settings")] // Đánh dấu các thuộc tính liên quan đến trọng lực tùy chỉnh.
    public float gravityStrength = 50f; // Sức mạnh của trọng lực tùy chỉnh được áp dụng cho enemy.
    private Vector2 gravityDirection; // Hướng của trọng lực tùy chỉnh (được thiết lập khi khởi tạo).

    [Header("Ground & Wall Check")] // Đánh dấu các thuộc tính liên quan đến việc kiểm tra mặt đất và tường.
    public Transform groundCheckPoint; // Điểm kiểm tra mặt đất.
    public Transform wallCheckPoint; // Điểm kiểm tra tường phía trước enemy.
    public float checkDistance = 0.1f;
    public LayerMask whatIsGround; // Layer mask để xác định những gì được coi là mặt đất (cũng có thể bao gồm tường).
    private bool isGrounded; // Cờ trạng thái, true nếu enemy đang chạm đất.
    private bool isTouchingWall; // Cờ trạng thái, true nếu enemy đang chạm tường.

    // Components & References
    private Rigidbody2D rb; // Tham chiếu đến thành phần Rigidbody2D để xử lý vật lý.
    private Animator anim;  // Tham chiếu đến thành phần Animator để điều khiển animation.
    private Transform playerTransform; // Tham chiếu đến transform của Player để biết vị trí của người chơi.
    private float timeSinceLastSeenPlayer; // Bộ đếm thời gian để enemy từ bỏ việc truy đuổi nếu không thấy người chơi.
    private Vector2 moveDirection = Vector2.right;

    // Hàm Awake được gọi trước Start, dùng để khởi tạo các tham chiếu component.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 0;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        
        timeSinceLastAttack = attackCooldown;
        
        // <<< THÊM MỚI: Khởi tạo máu >>>
        currentHealth = maxHealth;
    }

    // Hàm Initialize được gọi bởi Spawner hoặc script khác để thiết lập enemy.
    public void Initialize(Vector2 gravityDir)
    {
        gravityDirection = gravityDir.normalized;
        float angle = Mathf.Atan2(-gravityDirection.y, -gravityDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        SetState(EnemyState.Spawning);
    }

    // Hàm Update được gọi mỗi frame.
    void Update()
    {
        if (timeSinceLastAttack < attackCooldown)
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        if (!isAttacking)
        {
            HandleStateTransitions();
        }
        
        if (currentState == EnemyState.Attacking && !isAttacking)
        {
            if (timeSinceLastAttack >= attackCooldown)
            {
                PerformRandomAttack();
            }
        }

        UpdateAnimator();
    }

    // Hàm FixedUpdate được gọi theo khoảng thời gian vật lý cố định.
    void FixedUpdate()
    {
        rb.AddForce(gravityDirection * gravityStrength);
        CheckEnvironment();
        HandleMovement();
    }

    private void HandleStateTransitions()
    {
        if (!CanSeePlayer())
        {
            timeSinceLastSeenPlayer += Time.deltaTime;
            if (timeSinceLastSeenPlayer > loseSightTime)
            {
                SetState(EnemyState.Patrolling);
            }
            return;
        }

        timeSinceLastSeenPlayer = 0;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case EnemyState.Patrolling:
            case EnemyState.Chasing:
                if (distanceToPlayer <= attackRange)
                {
                    SetState(EnemyState.Attacking);
                }
                else
                {
                    SetState(EnemyState.Chasing);
                }
                break;

            case EnemyState.Attacking:
                if (distanceToPlayer > attackRange)
                {
                    SetState(EnemyState.Chasing);
                }
                break;
        }
    }

    private void HandleMovement()
    {
        if (currentState == EnemyState.Attacking)
        {
            rb.linearVelocity = Vector2.zero;
            if (playerTransform != null)
            {
                Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
                float directionDot = Vector2.Dot(directionToPlayer, transform.right);
                if (directionDot < 0) Flip();
            }
            return;
        }

        Vector2 currentVelocity = rb.linearVelocity;
        Vector2 moveAxis = transform.right;
        Vector2 targetHorizontalVelocity = Vector2.zero;

        if (currentState == EnemyState.Patrolling)
        {
            if (isGrounded)
            {
                if (isTouchingWall) Flip();
                targetHorizontalVelocity = moveAxis * patrolSpeed;
            }
        }
        else if (currentState == EnemyState.Chasing)
        {
            if (playerTransform != null)
            {
                Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
                float directionDot = Vector2.Dot(directionToPlayer, moveAxis);
                if (directionDot < 0) Flip();
                targetHorizontalVelocity = moveAxis * chaseSpeed;
            }
        }
        
        Vector2 velocityAlongGravity = Vector2.Dot(currentVelocity, gravityDirection) * gravityDirection;
        rb.linearVelocity = targetHorizontalVelocity + velocityAlongGravity;
    }

    public void SpawnAttack1Hitbox()
    {
        if (attack1HitboxPrefab == null || attack1Point == null) return;
        GameObject hitboxInstance = Instantiate(attack1HitboxPrefab, attack1Point.position, attack1Point.rotation);
        Destroy(hitboxInstance, 0.3f);
    }

    public void SpawnAttack2Hitbox()
    {
        if (attack2HitboxPrefab == null || attack2Point == null) return;
        GameObject hitboxInstance = Instantiate(attack2HitboxPrefab, attack2Point.position, attack2Point.rotation);
        Destroy(hitboxInstance, 0.5f);
    }

    private void PerformRandomAttack()
    {
        isAttacking = true;
        timeSinceLastAttack = 0f;

        int attackChoice = Random.Range(1, 3);
        if (attackChoice == 1)
        {
            anim.SetTrigger("Attack1");
        }
        else
        {
            anim.SetTrigger("Attack2");
        }
    }
    
    public void FinishAttack()
    {
        isAttacking = false;
    }

    private void SetState(EnemyState newState)
    {
        if (currentState == newState) return;
        
        Debug.Log("<color=yellow>ĐỔI TRẠNG THÁI: Từ " + currentState + " sang " + newState + "</color>");
        currentState = newState;
    }

    private void UpdateAnimator()
    {
        float horizontalSpeed = Vector2.Dot(rb.linearVelocity, transform.right);
        bool isMoving = Mathf.Abs(horizontalSpeed) > 0.1f && isGrounded;
        anim.SetBool("isWalking", isMoving);
    }

    private bool CanSeePlayer()
    {
        if (playerTransform == null) return false;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (playerTransform.position - transform.position).normalized, detectionRange, whatBlocksSight | whatIsPlayer);
            return hit.collider != null && (whatIsPlayer.value & (1 << hit.collider.gameObject.layer)) > 0;
        }
        return false;
    }

    private void CheckEnvironment()
    {
        isGrounded = Physics2D.Raycast(groundCheckPoint.position, gravityDirection, checkDistance, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheckPoint.position, transform.right, checkDistance, whatIsGround);
    }

    private void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        moveDirection = -moveDirection;
    }

    public void FinishSpawning()
    {
        Debug.Log("<color=green>Animation Event 'FinishSpawning' ĐÃ ĐƯỢC GỌI!</color>");
        SetState(EnemyState.Patrolling);
    }
    
    // <<< THÊM MỚI: Hàm để nhận sát thương >>>
    public void TakeDamage(float damage)
    {
        // Nếu đã chết rồi thì không nhận thêm sát thương
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " nhận " + damage + " sát thương, còn lại " + currentHealth + " máu.");

        // Kích hoạt animation bị đánh
        anim.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // <<< THÊM MỚI: Hàm xử lý khi chết >>>
    private void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " đã bị tiêu diệt.");
        
        // Kích hoạt animation chết
        anim.SetTrigger("Die");

        // Vô hiệu hóa các component để quái không thể di chuyển hay tấn công nữa
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false; // Vô hiệu hóa chính script Enemy.cs này
        rb.bodyType = RigidbodyType2D.Static; // Ngăn không cho xác chết bị đẩy đi

        Destroy(gameObject);
    }

    // <<< THÊM MỚI: Thêm logic va chạm >>>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem có va chạm với hitbox của người chơi không
        if (collision.CompareTag("PlayerHitbox"))
        {
            // Lấy script Hitbox từ đối tượng va chạm để biết sát thương
            Hitbox hitbox = collision.GetComponent<Hitbox>();
            if (hitbox != null)
            {
                // Gọi hàm nhận sát thương của chính mình
                TakeDamage(hitbox.damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + (Vector3)(gravityDirection * checkDistance));
        }
        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + transform.right * checkDistance);
        }

        if (attack1Point != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attack1Point.position, 0.2f);
        }
        if (attack2Point != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(attack2Point.position, 0.2f);
        }
    }
}