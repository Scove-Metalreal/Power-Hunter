using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyState { Spawning, Patrolling, Chasing, Attacking, Hit, Dying }
    private EnemyState currentState;

    [Header("Movement Stats")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float jumpForce = 8f;

    [Header("AI Settings")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;
    public LayerMask whatIsPlayer;
    public LayerMask whatBlocksSight;
    public float loseSightTime = 3f;
    public float jumpCooldown = 2f;
    [Tooltip("Khoảng cách 'nhìn về phía trước' để phát hiện rìa.")]
    public float ledgeCheckDistance = 0.5f;

    [Header("Random Jump Settings")]
    [Tooltip("Cho phép enemy nhảy ngẫu nhiên khi đang đi tuần.")]
    public bool canRandomlyJump = true;
    [Tooltip("Thời gian tối thiểu giữa các lần nhảy ngẫu nhiên.")]
    public float minTimeBetweenRandomJumps = 3f;
    [Tooltip("Thời gian tối đa giữa các lần nhảy ngẫu nhiên.")]
    public float maxTimeBetweenRandomJumps = 7f;

    [Header("Health Stats")]
    public float maxHealth = 50f;
    public float knockbackForce = 5f;
    private float currentHealth;
    private bool isDead = false;

    [Header("Item Drop")]
    public GameObject itemDropPrefab;
    [Range(0f, 1f)]
    public float dropChance = 0.3f;

    [Header("Attack Settings")]
    public GameObject attack1HitboxPrefab;
    public Transform attack1Point;
    public GameObject attack2HitboxPrefab;
    public Transform attack2Point;
    public float attackCooldown = 2f;
    private float timeSinceLastAttack = 0f;
    private float timeSinceLastJump = 0f;
    private bool isAttacking = false;

    [Header("Gravity Settings")]
    public float gravityStrength = 50f;
    private Vector2 gravityDirection;

    [Header("Ground & Wall Check")]
    public Transform groundCheckPoint;
    public Transform wallCheckPoint;
    public float checkDistance = 0.1f;
    public LayerMask whatIsGround;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isAtLedge;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform playerTransform;
    private float timeSinceLastSeenPlayer;
    private float randomJumpTimer;
    private float nextRandomJumpTime;

    // Animation parameters
    private float walkAnimTimer = 0f;
    private float walkAnimDuration = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 0;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        timeSinceLastAttack = attackCooldown;
        timeSinceLastJump = jumpCooldown;
        currentHealth = maxHealth;
        SetNextRandomJumpTime();
    }

    public void Initialize(Vector2 gravityDir)
    {
        gravityDirection = gravityDir.normalized;
        float angle = Mathf.Atan2(-gravityDirection.y, -gravityDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        SetState(EnemyState.Spawning);
    }

    void Update()
    {
        // DEBUG: In ra state hiện tại mỗi frame để theo dõi
        Debug.Log($"[Enemy State] Current State: {currentState}");

        if (isDead) return;
        if (timeSinceLastAttack < attackCooldown) timeSinceLastAttack += Time.deltaTime;
        if (timeSinceLastJump < jumpCooldown) timeSinceLastJump += Time.deltaTime;
        randomJumpTimer += Time.deltaTime;

        if (!isAttacking && currentState != EnemyState.Hit)
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

    void FixedUpdate()
    {
        if (isDead) return;
        rb.AddForce(gravityDirection * gravityStrength);
        CheckEnvironment();
        HandleMovement();
    }

    private void HandleStateTransitions()
    {
        if (!CanSeePlayer())
        {
            timeSinceLastSeenPlayer += Time.deltaTime;
            if (timeSinceLastSeenPlayer > loseSightTime && currentState != EnemyState.Patrolling)
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
                if (distanceToPlayer <= attackRange) SetState(EnemyState.Attacking);
                else SetState(EnemyState.Chasing);
                break;
            case EnemyState.Attacking:
                if (distanceToPlayer > attackRange) SetState(EnemyState.Chasing);
                break;
        }
    }

    private void HandleMovement()
    {
        // Ngừng di chuyển trong các trạng thái này
        if (isAttacking || currentState == EnemyState.Attacking || isDead || currentState == EnemyState.Hit)
        {
            // Khi bị hit, không thực hiện logic quay mặt về phía player
            if (currentState != EnemyState.Hit)
            {
                rb.linearVelocity = Vector2.zero;
                if (playerTransform != null && !isDead)
                {
                    Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
                    float directionDot = Vector2.Dot(directionToPlayer, transform.right);
                    if (directionDot < 0) Flip();
                }
            }
            return;
        }

        Vector2 targetHorizontalVelocity;

        if (currentState == EnemyState.Patrolling)
        {
            if (isGrounded && (isTouchingWall || isAtLedge))
            {
                Flip();
            }
            if (canRandomlyJump && isGrounded && !isTouchingWall && !isAtLedge && randomJumpTimer >= nextRandomJumpTime)
            {
                Debug.Log($"[Enemy AI] Random jump condition met. Attempting to jump.");
                Jump();
                SetNextRandomJumpTime();
            }
            targetHorizontalVelocity = transform.right * patrolSpeed;
        }
        else if (currentState == EnemyState.Chasing)
        {
            if (playerTransform != null)
            {
                if (isGrounded && isTouchingWall && timeSinceLastJump >= jumpCooldown)
                {
                    Debug.Log($"[Enemy AI] Wall jump condition met: isGrounded=true, isTouchingWall=true, jumpCooldownPassed=true. Attempting to jump.");
                    Jump();
                }
                else
                {
                    Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
                    float directionDot = Vector2.Dot(directionToPlayer, transform.right);
                    if (directionDot < 0)
                    {
                        Flip();
                    }
                }
                targetHorizontalVelocity = transform.right * chaseSpeed;
            }
            else
            {
                targetHorizontalVelocity = Vector2.zero;
            }
        }
        else
        {
            targetHorizontalVelocity = Vector2.zero;
        }
        
        Vector2 velocityAlongGravity = Vector2.Dot(rb.linearVelocity, gravityDirection) * gravityDirection;
        rb.linearVelocity = targetHorizontalVelocity + velocityAlongGravity;
    }

    private void Jump()
    {
        timeSinceLastJump = 0f;
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        Debug.Log($"[Enemy AI] Jumped with force {jumpForce}!");
    }

    private void SetNextRandomJumpTime()
    {
        nextRandomJumpTime = Random.Range(minTimeBetweenRandomJumps, maxTimeBetweenRandomJumps);
        randomJumpTimer = 0f;
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
        anim.SetInteger("AttackID", attackChoice); // Đặt ID cho animation attack (1 hoặc 2)
        anim.SetTrigger("Attack"); // Trigger trạng thái Attack chung
    }
    
    public void FinishAttack()
    {
        isAttacking = false;
    }

    public void FinishHit()
    {
        Debug.Log("[Enemy] FinishHit event called. Deciding next action...");
        // After getting hit, the enemy should immediately be aware of the player.
        if (CanSeePlayer())
        {
            Debug.Log("[Enemy] Player is visible. Switching to Chasing state.");
            SetState(EnemyState.Chasing);
        }
        else
        {
            Debug.Log("[Enemy] Player not visible. Switching to Patrolling state.");
            SetState(EnemyState.Patrolling);
        }
    }

    private void SetState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (currentState)
    	{
            case EnemyState.Spawning:
                anim.SetTrigger("Spawning");
                break;
            case EnemyState.Hit:
                anim.SetTrigger("Hit");
                isAttacking = false; // Ngắt animation attack nếu đang diễn ra
                rb.linearVelocity = Vector2.zero; // Dừng di chuyển ngay lập tức
                break;
        }
    }

    private void UpdateAnimator()
    {
        // --- LOGIC ANIMATION MỚI ---
        // Tính toán tốc độ di chuyển theo phương ngang của enemy
        float horizontalSpeed = Vector2.Dot(rb.linearVelocity, transform.right);
        float speed = Mathf.Abs(horizontalSpeed);
        // Gửi giá trị tốc độ (luôn dương) vào Animator, dùng để chuyển giữa Idle và Walk
        anim.SetFloat("Speed", speed);

        // Logic để chuyển đổi giữa Walk 1 và Walk 2
        if (speed > 0.1f) // Nếu đang di chuyển
        {
            walkAnimTimer += Time.deltaTime;
            if (walkAnimTimer >= walkAnimDuration)
            {
                walkAnimTimer = 0f;
                // Đặt thời gian ngẫu nhiên cho chu kỳ đi bộ tiếp theo
                walkAnimDuration = Random.Range(3f, 6f); 
                // Chọn animation đi bộ
                int walkChoice = Random.Range(1, 3); // 1 hoặc 2
                anim.SetInteger("WalkType", walkChoice);
            }
        }
        else
        {
            // Reset timer nếu không di chuyển
            walkAnimTimer = 0f;
            walkAnimDuration = 0f; // Reset để lần di chuyển tới sẽ chọn walk animation mới
        }
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
        Vector3 ledgeCheckStartPoint = groundCheckPoint.position + (transform.right * ledgeCheckDistance);
        isAtLedge = !Physics2D.Raycast(ledgeCheckStartPoint, gravityDirection, checkDistance * 2, whatIsGround);
    }

    private void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
    }

    public void FinishSpawning()
    {
        SetState(EnemyState.Patrolling);
    }
    
        public void TakeDamage(float damage, Transform damageSource)
    
        {
    
            if (isDead || currentState == EnemyState.Hit) return; // Không nhận thêm sát thương khi đang bị Hit
    
            currentHealth -= damage;
    
            
    
            if (currentHealth <= 0)
    
            {
    
                Die();
    
            }
    
            else
    
            {
    
                SetState(EnemyState.Hit);
    
                // --- Knockback Logic ---
    
                Vector2 knockbackDir = (transform.position - damageSource.position).normalized;
    
                rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
    
                Debug.Log($"[Enemy] Took {damage} damage and was knocked back from {damageSource.name}.");
    
            }
    
        }
    
    
    
            private void Die()
            {
                isDead = true;
                Debug.Log($"[Enemy] '{gameObject.name}' has died. Waiting for death animation to finish.");
                int deathChoice = Random.Range(1, 3);
                anim.SetInteger("DeathID", deathChoice);
                anim.SetTrigger("Death");
                GetComponent<Collider2D>().enabled = false;
                // this.enabled = false; // REMOVED: This can prevent Animation Events from firing. The isDead flag handles stopping logic.
                rb.bodyType = RigidbodyType2D.Static;
                HandleItemDrop();
                // The GameObject will now be destroyed by an animation event calling EndDead().
            }
        
            private void HandleItemDrop()
            {
                if (itemDropPrefab == null)
                {
                    Debug.Log($"[Enemy Item Drop] No item prefab assigned for {gameObject.name}. Skipping drop.");
                    return;
                }
        
                Debug.Log($"[Enemy Item Drop] Checking drop for {gameObject.name}. Chance: {dropChance * 100}%. Dropping item.");
                if (Random.value <= dropChance)
                {
                    Debug.Log($"[Enemy Item Drop] Success! Dropping item.");
                    GameObject droppedItemObject = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
                    Item itemScript = droppedItemObject.GetComponent<Item>();
                    if (itemScript != null)
                    {
                        itemScript.Initialize(gravityDirection);
                    }
                }
                else
                {
                    Debug.Log($"[Enemy Item Drop] Failed. Unlucky!");
                }
            }
        
            private void OnTriggerEnter2D(Collider2D collision)
            {
            
            if (collision.CompareTag("PlayerHitbox"))
    
            {
    
                Hitbox hitbox = collision.GetComponent<Hitbox>();
    
                if (hitbox != null)
    
                {
    
                    TakeDamage(hitbox.damage, collision.transform);
    
                }
    
            }
    
        }
    
    
    
        private void OnDrawGizmos()
    
        {
    
            if (groundCheckPoint != null) Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + (Vector3)(gravityDirection * checkDistance));
    
            if (wallCheckPoint != null) Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + transform.right * checkDistance);
    
            if (attack1Point != null) Gizmos.DrawWireSphere(attack1Point.position, 0.2f);
    
            if (attack2Point != null) Gizmos.DrawWireSphere(attack2Point.position, 0.2f);
    
        }
    
    
    
    	public void EndDead() {
    
            Debug.Log($"[Enemy] '{gameObject.name}' death animation finished. Destroying GameObject.");
    
            Destroy(gameObject);
    
    	}
    
    }
    
    