using UnityEngine;
﻿
﻿// Lớp này chịu trách nhiệm điều khiển hành vi của người chơi trong trò chơi.
﻿// Nó bao gồm di chuyển, nhảy (với khả năng tùy chỉnh độ cao), dash (có tụ lực và tấn công dash),
﻿// thay đổi trọng lực và cập nhật animation.
﻿public class PlayerController : MonoBehaviour
﻿{
﻿    [Header("Movement Stats")] // Đánh dấu các thuộc tính liên quan đến thống kê di chuyển.
﻿    public float PlayerSpeed = 5f; // Tốc độ di chuyển cơ bản của người chơi.
﻿    public float jumpForce = 5f;   // Lực tác động khi người chơi nhảy.
﻿    public float jumpStaminaCost = 10f; // Chi phí stamina cho mỗi lần nhảy.
﻿    [Range(0f, 1f)] // Tạo một thanh trượt trong Inspector từ 0 đến 1 cho giá trị này.
﻿    public float jumpCutMultiplier = 0.5f; // Tỷ lệ giảm vận tốc khi nhả nút nhảy (giúp tùy chỉnh độ cao nhảy).
﻿
﻿    private Rigidbody2D rb;         // Tham chiếu đến thành phần Rigidbody2D của người chơi để xử lý vật lý.
﻿
﻿    [Header("Ground Check")] // Đánh dấu các thuộc tính liên quan đến việc kiểm tra mặt đất.
﻿    public Transform groundCheck;  // Transform điểm dùng để kiểm tra xem người chơi có đang chạm đất hay không.
﻿    public float groundCheckRadius = 0.2f; // Bán kính này sẽ được thay đổi động.
﻿    public LayerMask groundLayer;  // Layer mask để xác định những gì được coi là mặt đất.
﻿    public bool isGround = true;   // Biến trạng thái, true nếu người chơi đang chạm đất, false nếu không.
﻿    [Tooltip("Bán kính check đất khi không có nhảy tường.")]
﻿    public float groundCheckDefaultRadius = 0.2f;
﻿    [Tooltip("Bán kính check đất khi đã có kỹ năng nhảy tường.")]
﻿    public float groundCheckWallJumpRadius = 0.4f;
﻿
﻿
﻿    [Header("Dash")] // Đánh dấu các thuộc tính liên quan đến khả năng Dash.
﻿    public float minDashSpeed = 10f;       // Tốc độ dash tối thiểu (khi không tụ lực).
﻿    public float maxDashSpeed = 20f;       // Tốc độ dash tối đa (khi tụ lực đầy).
﻿    public float timeToMaxCharge = 1f;     // Thời gian cần để tụ lực dash đạt mức tối đa.
﻿    public float dashTime = 0.2f;    // Thời gian người chơi thực hiện dash (thời gian di chuyển với tốc độ dash).
﻿    public float dashStaminaCost = 20f; // Chi phí stamina cho mỗi lần dash.
﻿    [HideInInspector] public bool isDash = false; // Biến trạng thái, true nếu người chơi đang trong quá trình dash (di chuyển).
﻿    [HideInInspector] public bool isChargingDash = false; // Trạng thái người chơi đang giữ nút để tụ lực dash.
﻿    private float dashTimeLeft;      // Thời gian còn lại để thực hiện dash (khi `isDash` là true).
﻿    private float dashChargeTimer;         // Bộ đếm thời gian đang giữ nút để tụ lực dash.
﻿    private bool isFullyChargedDash = false; // Cờ kiểm tra dash đã được tụ lực đầy hay chưa.
﻿    private bool hasPerformedDashAttack = false; // Cờ để đảm bảo attack chỉ gọi 1 lần trong quá trình dash.
﻿
﻿    [Header("Gravity")] // Đánh dấu các thuộc tính liên quan đến trọng lực.
﻿    public bool GrafityDown = true;  // Cho phép trọng lực hướng xuống (mặc định).
﻿    public bool GrafityUp = false;   // Cho phép trọng lực hướng lên.
﻿    public bool GrafityLeft = false; // Cho phép trọng lực hướng sang trái.
﻿    public bool GrafityRight = false;// Cho phép trọng lực hướng sang phải.
﻿    private float defaultGravityScale; // Lưu trữ giá trị gravity scale mặc định của Rigidbody2D để có thể phục hồi.
﻿    public float fallGravityAdd = 1f; // Lượng trọng lực tăng thêm khi rơi xuống (để làm cho cảm giác rơi nhanh hơn).
﻿
﻿    [Header("State")] // Đánh dấu các thuộc thuộc tính liên quan đến trạng thái chung của người chơi.
﻿    public int Direction = 1; // Hướng nhìn hiện tại của người chơi (1 cho phải, -1 cho trái).
﻿    private bool isChangingGravity = false; // BIẾN TRẠNG THÁI MỚI: true nếu người chơi đang trong quá trình thay đổi trọng lực, false nếu không.
﻿
﻿    // Components
﻿    [HideInInspector] public Animator animator; // Tham chiếu đến thành phần Animator để điều khiển animation.
﻿    private PlayerStat playerStat;              // Tham chiếu đến thành phần PlayerStat để quản lý các chỉ số của người chơi (ví dụ: Stamina).
﻿    private PlayerAttackDefault playerAttack; // Tham chiếu đến script tấn công để có thể ngắt tấn công khi dash hoặc thực hiện attack dash.
﻿
﻿    // Public Getters for other scripts
﻿    public Rigidbody2D Rigidbody => rb;
﻿    public float DefaultGravityScale => defaultGravityScale;
﻿    
﻿    // Hàm này giúp bạn thấy được vòng tròn check ground trong cửa sổ Scene.
﻿    // Rất hữu ích để debug hoặc điều chỉnh groundCheckRadius và groundCheck position.
﻿    void OnDrawGizmos()
﻿    {
﻿        if (groundCheck != null)
﻿        {
﻿            Gizmos.color = Color.red; // Màu sắc của gizmo.
﻿            // Vẽ vòng tròn với bán kính hiện tại, giúp dễ dàng debug
﻿            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
﻿        }
﻿    }
﻿
﻿    // Hàm này được gọi một lần khi script được kích hoạt.
﻿    void Start()
﻿    {
﻿        // Lấy các thành phần cần thiết được đính kèm với GameObject này.
﻿        rb = GetComponent<Rigidbody2D>();
﻿        animator = GetComponent<Animator>();
﻿        playerStat = GetComponent<PlayerStat>();
﻿        playerAttack = GetComponent<PlayerAttackDefault>(); // Lấy component PlayerAttackDefault khi game bắt đầu.
﻿        
﻿        // Lưu trữ giá trị gravity scale mặc định để có thể sử dụng lại sau này.
﻿        defaultGravityScale = rb.gravityScale;
﻿        
﻿        // Đặt trọng lực mặc định là hướng xuống khi bắt đầu trò chơi.
﻿        GrafityDown = true;
﻿    }
﻿
﻿    // Hàm này được gọi mỗi frame. Thích hợp cho việc xử lý input và logic không phụ thuộc vào tốc độ khung hình.
﻿    void Update()
﻿    {
﻿        UpdateGroundCheckRadius(); // Cập nhật bán kính check đất cho tính năng nhảy tường.
﻿
﻿        // Nếu đang trong quá trình đổi trọng lực, chỉ kiểm tra xem người chơi đã tiếp đất chưa.
﻿        if (isChangingGravity)
﻿        {
﻿            // Kiểm tra xem người chơi có đang chạm đất không.
﻿            if (isGround)
﻿            {
﻿                // Khi đã tiếp đất, kết thúc quá trình đổi trọng lực.
﻿                isChangingGravity = false;
﻿                rb.gravityScale = defaultGravityScale; // Trả gravity scale về giá trị mặc định.
﻿            }
﻿            return; 
﻿        }
﻿        
﻿        HandleJumping(); // Xử lý logic nhảy, bao gồm cả việc nhấn và nhả nút.
﻿        HandleDash();    // Xử lý logic dash, bao gồm cả tụ lực và thực thi.
﻿        
﻿        if (!isDash && !isChargingDash)
﻿        {
﻿            PlayerMove();
﻿        }
﻿
﻿        Flip(); // Xoay nhân vật theo hướng di chuyển.
﻿        UpdateAnimationState(); // Cập nhật trạng thái animation dựa trên các điều kiện hiện tại.
﻿    }
﻿
﻿    // <<< HÀM MỚI >>>
﻿    // Cập nhật bán kính của điểm check đất dựa trên việc người chơi có kỹ năng nhảy tường hay không.
﻿    void UpdateGroundCheckRadius()
﻿    {
﻿        if (playerStat != null && playerStat.hasWallJump)
﻿        {
﻿            groundCheckRadius = groundCheckWallJumpRadius;
﻿        }
﻿        else
﻿        {
﻿            groundCheckRadius = groundCheckDefaultRadius;
﻿        }
﻿    }
﻿
﻿    // Hàm này được gọi mỗi bước thời gian vật lý cố định. Thích hợp cho việc xử lý vật lý.
﻿    void FixedUpdate()
﻿    {
﻿        // Luôn kiểm tra `isGround` trong `FixedUpdate` để đảm bảo tính ổn định vật lý.
﻿        // Bán kính `groundCheckRadius` đã được cập nhật trong Update(), nên ở đây nó sẽ dùng giá trị đúng.
﻿        isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, layerMask: groundLayer);
﻿    }
﻿    
﻿    void HandleDash()
﻿    {
﻿        if (isDash)
﻿        {
﻿            if (isFullyChargedDash && !hasPerformedDashAttack && dashTimeLeft <= dashTime * 0.25f)
﻿            {
﻿                playerAttack.PerformDashAttack();
﻿                hasPerformedDashAttack = true;
﻿            }
﻿
﻿            if (dashTimeLeft > 0)
﻿            {
﻿                dashTimeLeft -= Time.deltaTime;
﻿            }
﻿            else
﻿            {
﻿                isDash = false;
﻿                rb.linearVelocity = Vector2.zero;
﻿                rb.gravityScale = defaultGravityScale; 
﻿                animator.ResetTrigger("isDashing");
﻿                
﻿                if (isFullyChargedDash)
﻿                {
﻿                    isFullyChargedDash = false;
﻿                }
﻿            }
﻿            return;
﻿        }
﻿
﻿        if (Input.GetKeyDown(KeyCode.LeftShift) && playerStat.StaminaPlayer >= dashStaminaCost && !isChargingDash)
﻿        {
﻿            isChargingDash = true;
﻿            dashChargeTimer = 0f;
﻿
﻿            if (isGround)
﻿            {
﻿                rb.linearVelocity = Vector2.zero;
﻿            }
﻿        }
﻿
﻿        if (isChargingDash && Input.GetKey(KeyCode.LeftShift))
﻿        {
﻿            dashChargeTimer += Time.deltaTime;
﻿            if (dashChargeTimer >= timeToMaxCharge)
﻿            {
﻿                isFullyChargedDash = true;
﻿                ExecuteDash(maxDashSpeed);
﻿            }
﻿        }
﻿
﻿        if (Input.GetKeyUp(KeyCode.LeftShift) && isChargingDash)
﻿        {
﻿            isFullyChargedDash = false;
﻿            float chargeRatio = Mathf.Clamp01(dashChargeTimer / timeToMaxCharge);
﻿            float currentDashSpeed = Mathf.Lerp(minDashSpeed, maxDashSpeed, chargeRatio);
﻿            ExecuteDash(currentDashSpeed);
﻿        }
﻿    }
﻿    
﻿    void ExecuteDash(float speed)
﻿    {
﻿        isChargingDash = false;
﻿        
﻿        if (playerAttack != null) 
﻿        {
﻿            playerAttack.InterruptAttack();
﻿        }
﻿
﻿        playerStat.UseStamina(dashStaminaCost);
﻿        isDash = true;
﻿        dashTimeLeft = dashTime;
﻿        hasPerformedDashAttack = false;
﻿        rb.gravityScale = 0f; 
﻿        animator.SetTrigger("isDashing");
﻿
﻿        Vector2 dashVelocity = Vector2.zero;
﻿        if (GrafityDown) dashVelocity = new Vector2(Direction * speed, 0);
﻿        if (GrafityUp) dashVelocity = new Vector2(-Direction * speed, 0);
﻿        if (GrafityLeft) dashVelocity = new Vector2(0, -Direction * speed);
﻿        if (GrafityRight) dashVelocity = new Vector2(0, Direction * speed);
﻿        rb.linearVelocity = dashVelocity;
﻿    }
﻿
﻿    void PlayerMove()
﻿    {
﻿        float x = Input.GetAxisRaw("Horizontal");
﻿        
﻿        if (GrafityDown || GrafityUp)
﻿        {
﻿            float moveDirection = GrafityDown ? 1f : -1f;
﻿            rb.linearVelocity = new Vector2(x * PlayerSpeed * moveDirection, rb.linearVelocity.y);
﻿        }
﻿        else if (GrafityLeft || GrafityRight)
﻿        {
﻿            float moveDirection = GrafityRight ? 1f : -1f;
﻿            rb.linearVelocity = new Vector2(rb.linearVelocity.x, x * PlayerSpeed * moveDirection);
﻿        }
﻿
﻿        if (x > 0) Direction = 1;
﻿        if (x < 0) Direction = -1;
﻿    }
﻿    
﻿    void HandleJumping()
﻿    {
﻿        if (Input.GetKeyDown(KeyCode.Space) && isGround && playerStat.StaminaPlayer >= jumpStaminaCost)
﻿        {
﻿            playerStat.UseStamina(jumpStaminaCost);
﻿            rb.linearVelocity = Vector2.zero;
﻿
﻿            Vector2 jumpDirection = Vector2.zero;
﻿            if (GrafityDown) { rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); jumpDirection = Vector2.up; }
﻿            if (GrafityUp)   { rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); jumpDirection = Vector2.down; }
﻿            if (GrafityLeft) { rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); jumpDirection = Vector2.right; }
﻿            if (GrafityRight){ rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); jumpDirection = Vector2.left; }
﻿            
﻿            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
﻿        }
﻿
﻿        if (Input.GetKeyUp(KeyCode.Space))
﻿        {
﻿            bool isMovingUp = false;
﻿            if (GrafityDown && rb.linearVelocity.y > 0) isMovingUp = true;
﻿            if (GrafityUp && rb.linearVelocity.y < 0) isMovingUp = true;
﻿            if (GrafityLeft && rb.linearVelocity.x > 0) isMovingUp = true;
﻿            if (GrafityRight && rb.linearVelocity.x < 0) isMovingUp = true;
﻿
﻿            if (isMovingUp)
﻿            {
﻿                if (GrafityDown || GrafityUp) rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
﻿                if (GrafityLeft || GrafityRight) rb.linearVelocity = new Vector2(rb.linearVelocity.x * jumpCutMultiplier, rb.linearVelocity.y);
﻿            }
﻿        }
﻿    }
﻿
﻿    void UpdateAnimationState()
﻿    {
﻿        float moveInput = Input.GetAxisRaw("Horizontal");
﻿
﻿        if (isGround)
﻿        {
﻿            animator.SetBool("isJumping", false);
﻿            animator.SetBool("isFalling", false);
﻿            
﻿            if (moveInput != 0 && !isDash && !isChargingDash)
﻿            {
﻿                animator.SetBool("isRunning", true);
﻿                animator.SetBool("isIdie", false);
﻿            }
﻿            else
﻿            {
﻿                animator.SetBool("isRunning", false);
﻿                animator.SetBool("isIdie", true);
﻿            }
﻿        }
﻿        else
﻿        {
﻿            animator.SetBool("isRunning", false);
﻿            animator.SetBool("isIdie", false);
﻿            
﻿            float verticalVelocity = 0;
﻿            if (GrafityDown) verticalVelocity = rb.linearVelocity.y;
﻿            if (GrafityUp) verticalVelocity = -rb.linearVelocity.y;
﻿            if (GrafityLeft) verticalVelocity = rb.linearVelocity.x;
﻿            if (GrafityRight) verticalVelocity = -rb.linearVelocity.x;
﻿
﻿            if (verticalVelocity > 0.1f)
﻿            {
﻿                animator.SetBool("isJumping", true);
﻿                animator.SetBool("isFalling", false);
﻿            }
﻿            else
﻿            {
﻿                animator.SetBool("isJumping", false);
﻿                animator.SetBool("isFalling", true);
﻿            }
﻿        }
﻿    }
﻿
﻿    void Flip()
﻿    {
﻿        if (isChargingDash) return;
﻿        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Direction, transform.localScale.y, transform.localScale.z);
﻿    }
﻿    
﻿    public void StartGravityChange()
﻿    {
﻿        isChangingGravity = true;
﻿        isGround = false;
﻿        rb.linearVelocity = Vector2.zero;
﻿        rb.gravityScale = defaultGravityScale * 3f;
﻿    }
﻿
﻿    public void ApplyGravityDirection(GravityDirection direction)
﻿    {
﻿        switch (direction)
﻿        {
﻿            case GravityDirection.Up:
﻿                Physics2D.gravity = Vector2.up * 9.81f;
﻿                transform.rotation = Quaternion.Euler(0, 0, 180);
﻿                GrafityDown = false; GrafityUp = true; GrafityLeft = false; GrafityRight = false;
﻿                break;
﻿            case GravityDirection.Left:
﻿                Physics2D.gravity = Vector2.left * 9.81f;
﻿                transform.rotation = Quaternion.Euler(0, 0, -90);
﻿                GrafityDown = false; GrafityUp = false; GrafityLeft = true; GrafityRight = false;
﻿                break;
﻿            case GravityDirection.Right:
﻿                Physics2D.gravity = Vector2.right * 9.81f;
﻿                transform.rotation = Quaternion.Euler(0, 0, 90);
﻿                GrafityDown = false; GrafityUp = false; GrafityLeft = false; GrafityRight = true;
﻿                break;
﻿            case GravityDirection.Down:
﻿            default:
﻿                Physics2D.gravity = Vector2.down * 9.81f;
﻿                transform.rotation = Quaternion.Euler(0, 0, 0);
﻿                GrafityDown = true; GrafityUp = false; GrafityLeft = false; GrafityRight = false;
﻿                break;
﻿        }
﻿    }
﻿}
﻿