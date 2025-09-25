using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Enum (liệt kê) các trạng thái của AI, giúp code cực kỳ rõ ràng
    public enum EnemyState { Idle, Patrol, Chase, Attack, Hurt, Dead }
    
    [Header("Trạng thái hiện tại")]
    [SerializeField] private EnemyState currentState;

    [Header("Thông số di chuyển & Tuần tra")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public Transform[] patrolPoints;
    public float waitAtPatrolPointTime = 2f; // Thời gian chờ tại mỗi điểm
    private int currentPatrolPointIndex = 0;
    private float waitTimer;

    [Header("Thông số phát hiện & Tấn công")]
    public float sightRange = 10f; // Tầm nhìn
    public float attackRange = 1.5f; // Tầm đánh
    public float attackCooldown = 2f; // Thời gian nghỉ giữa các đòn đánh
    private float lastAttackTime;
    private Transform playerTransform;

    [Header("Tham chiếu")]
    public Transform eyePosition; // Vị trí "mắt" để bắn Raycast
    public LayerMask whatIsPlayer; // Layer của Player
    
    // Các component
    private Animator anim;
    private Rigidbody2D rb;
    private int facingDirection = 1; // 1 = phải, -1 = trái

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    
    void Start()
    {
        // Khởi đầu ở trạng thái đi tuần tra
        ChangeState(EnemyState.Patrol);
    }

    void Update()
    {
        if (currentState == EnemyState.Dead || playerTransform == null)
        {
            rb.linearVelocity = Vector2.zero;
            return; // Nếu đã chết hoặc không tìm thấy player thì không làm gì cả
        }

        // "Bộ não" của AI: chạy logic tương ứng với trạng thái hiện tại
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;
            case EnemyState.Patrol:
                HandlePatrolState();
                break;
            case EnemyState.Chase:
                HandleChaseState();
                break;
            case EnemyState.Attack:
                HandleAttackState();
                break;
            case EnemyState.Hurt:
                // Trạng thái Hurt sẽ được xử lý chủ yếu bằng animation và Animation Event
                break;
        }
    }

    // --- LOGIC CỦA TỪNG TRẠNG THÁI ---

    private void HandleIdleState()
    {
        rb.linearVelocity = Vector2.zero; // Đứng yên
        waitTimer -= Time.deltaTime;

        // Nếu thấy người chơi, đuổi theo ngay
        if (CanSeePlayer())
        {
            ChangeState(EnemyState.Chase);
            return;
        }
        
        // Hết thời gian chờ, quay lại đi tuần
        if (waitTimer <= 0)
        {
            ChangeState(EnemyState.Patrol);
        }
    }
    
    private void HandlePatrolState()
    {
        // Nếu thấy người chơi, đuổi theo
        if (CanSeePlayer())
        {
            ChangeState(EnemyState.Chase);
            return;
        }
        
        // Di chuyển đến điểm tuần tra
        Transform targetPoint = patrolPoints[currentPatrolPointIndex];
        if (Vector2.Distance(transform.position, targetPoint.position) < 1f)
        {
            // Khi đến điểm, chuyển sang trạng thái Idle để chờ
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
            ChangeState(EnemyState.Idle);
            return;
        }

        // Di chuyển
        Vector2 moveDirection = (targetPoint.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(moveDirection.x * patrolSpeed, rb.linearVelocity.y);
        FlipTowards(moveDirection.x);
    }

    private void HandleChaseState()
    {
        // Mất dấu người chơi, quay lại đi tuần
        if (!CanSeePlayer())
        {
            ChangeState(EnemyState.Patrol);
            return;
        }
        
        // Người chơi trong tầm đánh, chuyển sang tấn công
        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            ChangeState(EnemyState.Attack);
            return;
        }
        
        // Đuổi theo người chơi
        Vector2 moveDirection = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(moveDirection.x * chaseSpeed, rb.linearVelocity.y);
        FlipTowards(moveDirection.x);
    }

    private void HandleAttackState()
    {
        rb.linearVelocity = Vector2.zero; // Dừng lại khi tấn công
        FlipTowards(playerTransform.position.x - transform.position.x);

        if (Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            
            // Random giữa 2 đòn đánh để khó đoán hơn
            if (Random.Range(0, 2) == 0)
            {
                anim.SetTrigger("Attack1");
            }
            else
            {
                anim.SetTrigger("Attack2");
            }
        }
        
        // Nếu player chạy ra khỏi tầm đánh, đuổi theo tiếp
        if (Vector2.Distance(transform.position, playerTransform.position) > attackRange)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    // --- CÁC HÀM KÍCH HOẠT VÀ TIỆN ÍCH ---

    public void TriggerHurtState()
    {
        ChangeState(EnemyState.Hurt);
        anim.SetTrigger("Hurt");
    }

    public void TriggerDeathState()
    {
        ChangeState(EnemyState.Dead);
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false; // Tắt collider để không cản đường
        
        // Random animation chết
        if (Random.Range(0, 2) == 0)
        {
            anim.SetTrigger("Death1");
        }
        else
        {
            anim.SetTrigger("Death2");
        }
        
        Destroy(gameObject, 3f); // Hủy đối tượng sau 3 giây
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;
        
        // Cập nhật Animator dựa trên trạng thái
        switch (currentState)
        {
            case EnemyState.Idle:
                waitTimer = waitAtPatrolPointTime;
                anim.SetFloat("Speed", 0f);
                break;
            case EnemyState.Patrol:
                anim.SetFloat("Speed", 1f);
                break;
            case EnemyState.Chase:
                anim.SetFloat("Speed", 2f);
                break;
            case EnemyState.Attack:
                anim.SetFloat("Speed", 0f);
                break;
            case EnemyState.Hurt:
                rb.linearVelocity = Vector2.zero;
                anim.SetFloat("Speed", 0f);
                break;
            case EnemyState.Dead:
                anim.SetFloat("Speed", 0f);
                break;
        }
    }

    private bool CanSeePlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(eyePosition.position, new Vector2(facingDirection, 0), sightRange, whatIsPlayer);
        return hit.collider != null;
    }

    private void FlipTowards(float directionX)
    {
        if (directionX > 0 && facingDirection == -1)
        {
            facingDirection = 1;
            transform.localScale = new Vector3(-1, 1, 1); // Quay phải
        }
        else if (directionX < 0 && facingDirection == 1)
        {
            facingDirection = -1;
            transform.localScale = new Vector3(1, 1, 1); // Quay trái
        }
    }
    
    // HÀM NÀY SẼ ĐƯỢC GỌI BẰNG ANIMATION EVENT TỪ CUỐI CÁC ANIMATION ATTACK VÀ HURT
    public void AnimationFinished()
    {
        // Sau khi bị thương hoặc tấn công xong, quay lại đuổi theo người chơi
        if(currentState == EnemyState.Hurt || currentState == EnemyState.Attack)
        {
            ChangeState(EnemyState.Chase);
        }
    }
}