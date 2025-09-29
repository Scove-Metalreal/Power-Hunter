using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Stats")]
    public float PlayerSpeed = 5f;
    public float jumpForce = 5f;
    private Rigidbody2D rb;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public bool isGround = true;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    [HideInInspector] public bool isDash = false;
    private float dashTimeLeft;
    private float dashCooldownTime;

    [Header("Gravity")]
    public bool GrafityDown;
    public bool GrafityUp;
    public bool GrafityLeft;
    public bool GrafityRight;
    private float defaultGravityScale;
    public float fallGravityAdd = 1f;

    [Header("State")]
    public int Direction = 1;
    private bool isChangingGravity = false; // BIẾN TRẠNG THÁI MỚI, THAY THẾ CHO CanMove

    // Components
    [HideInInspector] public Animator animator;
    private PlayerStat playerStat;
    
    // Hàm này giúp bạn thấy được vòng tròn check ground trong cửa sổ Scene
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerStat = GetComponent<PlayerStat>();
        defaultGravityScale = rb.gravityScale;
        
        GrafityDown = true;
    }

    void Update()
    {
        // Nếu đang trong quá trình đổi trọng lực, ta sẽ kiểm tra xem đã tiếp đất chưa
        if (isChangingGravity)
        {
            if (isGround)
            {
                // Khi đã tiếp đất, kết thúc quá trình đổi trọng lực và cho phép di chuyển lại
                isChangingGravity = false;
                rb.gravityScale = defaultGravityScale; // Trả gravity scale về bình thường
            }
            // Không làm gì khác cho đến khi tiếp đất
            return; 
        }

        // Nếu không đổi trọng lực, xử lý input như bình thường
        HandleMovementAndDash();
        Flip();
        UpdateAnimationState(); // Gọi hàm cập nhật animation tập trung
    }

    void FixedUpdate()
    {
        // Luôn kiểm tra isGround trong FixedUpdate để đảm bảo tính ổn định vật lý
        isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    
    // --- TẬP TRUNG LOGIC INPUT VÀO MỘT HÀM ---
    void HandleMovementAndDash()
    {
        if (isDash)
        {
            // Xử lý khi đang Dash
            if (dashTimeLeft > 0)
            {
                // Logic dash của bạn, nhưng cần được điều chỉnh cho các hướng trọng lực
                Vector2 dashVelocity = Vector2.zero;
                if (GrafityDown) dashVelocity = new Vector2(Direction * dashSpeed, 0);
                if (GrafityUp) dashVelocity = new Vector2(-Direction * dashSpeed, 0);
                if (GrafityLeft) dashVelocity = new Vector2(0, -Direction * dashSpeed);
                if (GrafityRight) dashVelocity = new Vector2(0, Direction * dashSpeed);
                rb.linearVelocity = dashVelocity;

                dashTimeLeft -= Time.deltaTime;
            }
            else
            {
                isDash = false;
                rb.linearVelocity = Vector2.zero; // Dừng lại sau khi dash xong
                animator.ResetTrigger("isDashing");
            }
        }
        else
        {
            // Xử lý di chuyển và nhảy khi không Dash
            PlayerMove();
            PlayerJumping();

            // Xử lý input Dash
            if (dashCooldownTime > 0) dashCooldownTime -= Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTime <= 0 && playerStat.StaminaPlayer >= 20)
            {
                playerStat.UseStamina(20);
                isDash = true;
                dashTimeLeft = dashTime;
                dashCooldownTime = dashCooldown;
                animator.SetTrigger("isDashing");
            }
        }
    }

    void PlayerMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        
        // Điều chỉnh hướng di chuyển dựa trên trọng lực
        if (GrafityDown || GrafityUp)
        {
            float moveDirection = GrafityDown ? 1f : -1f;
            rb.linearVelocity = new Vector2(x * PlayerSpeed * moveDirection, rb.linearVelocity.y);
        }
        else if (GrafityLeft || GrafityRight)
        {
            float moveDirection = GrafityRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, x * PlayerSpeed * moveDirection);
        }

        // Cập nhật hướng nhìn
        if (x > 0) Direction = 1;
        if (x < 0) Direction = -1;
    }

    void PlayerJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Vector2 jumpDirection = Vector2.zero;
            if (GrafityDown) jumpDirection = Vector2.up;
            if (GrafityUp) jumpDirection = Vector2.down;
            if (GrafityLeft) jumpDirection = Vector2.right;
            if (GrafityRight) jumpDirection = Vector2.left;

            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        }
    }

    // --- HÀM CẬP NHẬT ANIMATION TẬP TRUNG ---
    void UpdateAnimationState()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (isGround)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);

            if (moveInput != 0 && !isDash)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isIdie", false);
            }
            else
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isIdie", true);
            }
        }
        else // Khi đang ở trên không
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdie", false);
            
            // Xác định đang nhảy lên hay rơi xuống dựa trên vận tốc tương đối với trọng lực
            float verticalVelocity = 0;
            if (GrafityDown) verticalVelocity = rb.linearVelocity.y;
            if (GrafityUp) verticalVelocity = -rb.linearVelocity.y;
            if (GrafityLeft) verticalVelocity = rb.linearVelocity.x;
            if (GrafityRight) verticalVelocity = -rb.linearVelocity.x;

            if (verticalVelocity > 0.1f) // Đang bay lên (ngược trọng lực)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isFalling", false);
            }
            else // Đang rơi xuống
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
            }
        }
    }

    void Flip()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Direction, transform.localScale.y, transform.localScale.z);
    }
    
    // --- HÀM ĐỂ BẮT ĐẦU QUÁ TRÌNH ĐỔI TRỌNG LỰC ---
    public void StartGravityChange()
    {
        isChangingGravity = true; // Bật trạng thái đang đổi trọng lực
        isGround = false;         // Coi như không còn trên mặt đất cũ
        rb.linearVelocity = Vector2.zero; // Reset vận tốc để rơi tự nhiên hơn
        rb.gravityScale = defaultGravityScale * 3f; // Tăng mạnh trọng lực để "hút" xuống nhanh hơn
    }
}