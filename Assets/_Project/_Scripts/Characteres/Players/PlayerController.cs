using UnityEngine;

// Lớp này chịu trách nhiệm điều khiển hành vi của người chơi trong trò chơi.
// Nó bao gồm di chuyển, nhảy (với khả năng tùy chỉnh độ cao), dash (có tụ lực và tấn công dash),
// thay đổi trọng lực và cập nhật animation.
public class PlayerController : MonoBehaviour
{
    [Header("Movement Stats")] // Đánh dấu các thuộc tính liên quan đến thống kê di chuyển.
    public float PlayerSpeed = 5f; // Tốc độ di chuyển cơ bản của người chơi.
    public float jumpForce = 5f;   // Lực tác động khi người chơi nhảy.
    public float jumpStaminaCost = 10f; // Chi phí stamina cho mỗi lần nhảy.
    [Range(0f, 1f)] // Tạo một thanh trượt trong Inspector từ 0 đến 1 cho giá trị này.
    public float jumpCutMultiplier = 0.5f; // Tỷ lệ giảm vận tốc khi nhả nút nhảy (giúp tùy chỉnh độ cao nhảy).

    private Rigidbody2D rb;         // Tham chiếu đến thành phần Rigidbody2D của người chơi để xử lý vật lý.

    [Header("Ground Check")] // Đánh dấu các thuộc tính liên quan đến việc kiểm tra mặt đất.
    public Transform groundCheck;  // Transform điểm dùng để kiểm tra xem người chơi có đang chạm đất hay không.
    public float groundCheckRadius = 0.2f; // Bán kính của vòng tròn kiểm tra mặt đất.
    public LayerMask groundLayer;  // Layer mask để xác định những gì được coi là mặt đất.
    public bool isGround = true;   // Biến trạng thái, true nếu người chơi đang chạm đất, false nếu không.

    [Header("Dash")] // Đánh dấu các thuộc tính liên quan đến khả năng Dash.
    public float minDashSpeed = 10f;       // Tốc độ dash tối thiểu (khi không tụ lực).
    public float maxDashSpeed = 20f;       // Tốc độ dash tối đa (khi tụ lực đầy).
    public float timeToMaxCharge = 1f;     // Thời gian cần để tụ lực dash đạt mức tối đa.
    public float dashTime = 0.2f;    // Thời gian người chơi thực hiện dash (thời gian di chuyển với tốc độ dash).
    public float dashStaminaCost = 20f; // Chi phí stamina cho mỗi lần dash.
    [HideInInspector] public bool isDash = false; // Biến trạng thái, true nếu người chơi đang trong quá trình dash (di chuyển).
    [HideInInspector] public bool isChargingDash = false; // Trạng thái người chơi đang giữ nút để tụ lực dash.
    private float dashTimeLeft;      // Thời gian còn lại để thực hiện dash (khi `isDash` là true).
    private float dashChargeTimer;         // Bộ đếm thời gian đang giữ nút để tụ lực dash.
    private bool isFullyChargedDash = false; // Cờ kiểm tra dash đã được tụ lực đầy hay chưa.
    private bool hasPerformedDashAttack = false; // Cờ để đảm bảo attack chỉ gọi 1 lần trong quá trình dash.

    [Header("Gravity")] // Đánh dấu các thuộc tính liên quan đến trọng lực.
    public bool GrafityDown = true;  // Cho phép trọng lực hướng xuống (mặc định).
    public bool GrafityUp = false;   // Cho phép trọng lực hướng lên.
    public bool GrafityLeft = false; // Cho phép trọng lực hướng sang trái.
    public bool GrafityRight = false;// Cho phép trọng lực hướng sang phải.
    private float defaultGravityScale; // Lưu trữ giá trị gravity scale mặc định của Rigidbody2D để có thể phục hồi.
    public float fallGravityAdd = 1f; // Lượng trọng lực tăng thêm khi rơi xuống (để làm cho cảm giác rơi nhanh hơn).

    [Header("State")] // Đánh dấu các thuộc thuộc tính liên quan đến trạng thái chung của người chơi.
    public int Direction = 1; // Hướng nhìn hiện tại của người chơi (1 cho phải, -1 cho trái).
    private bool isChangingGravity = false; // BIẾN TRẠNG THÁI MỚI: true nếu người chơi đang trong quá trình thay đổi trọng lực, false nếu không.

    // Components
    [HideInInspector] public Animator animator; // Tham chiếu đến thành phần Animator để điều khiển animation.
    private PlayerStat playerStat;              // Tham chiếu đến thành phần PlayerStat để quản lý các chỉ số của người chơi (ví dụ: Stamina).
    private PlayerAttackDefault playerAttack; // Tham chiếu đến script tấn công để có thể ngắt tấn công khi dash hoặc thực hiện attack dash.

    // Public Getters for other scripts
    public Rigidbody2D Rigidbody => rb;
    public float DefaultGravityScale => defaultGravityScale;
    
    // Hàm này giúp bạn thấy được vòng tròn check ground trong cửa sổ Scene.
    // Rất hữu ích để debug hoặc điều chỉnh groundCheckRadius và groundCheck position.
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red; // Màu sắc của gizmo.
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); // Vẽ một hình cầu rỗng tại vị trí groundCheck với bán kính đã định.
        }
    }

    // Hàm này được gọi một lần khi script được kích hoạt.
    void Start()
    {
        // Lấy các thành phần cần thiết được đính kèm với GameObject này.
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerStat = GetComponent<PlayerStat>();
        playerAttack = GetComponent<PlayerAttackDefault>(); // Lấy component PlayerAttackDefault khi game bắt đầu.
        
        // Lưu trữ giá trị gravity scale mặc định để có thể sử dụng lại sau này.
        defaultGravityScale = rb.gravityScale;
        
        // Đặt trọng lực mặc định là hướng xuống khi bắt đầu trò chơi.
        GrafityDown = true;
    }

    // Hàm này được gọi mỗi frame. Thích hợp cho việc xử lý input và logic không phụ thuộc vào tốc độ khung hình.
    void Update()
    {
        // Nếu đang trong quá trình đổi trọng lực, chỉ kiểm tra xem người chơi đã tiếp đất chưa.
        if (isChangingGravity)
        {
            // Kiểm tra xem người chơi có đang chạm đất không.
            if (isGround)
            {
                // Khi đã tiếp đất, kết thúc quá trình đổi trọng lực.
                isChangingGravity = false;
                rb.gravityScale = defaultGravityScale; // Trả gravity scale về giá trị mặc định.
                // Gravity có thể cần được reset lại hướng tại đây tùy thuộc vào logic thay đổi trọng lực cụ thể.
            }
            // Nếu chưa tiếp đất, không làm gì khác cho đến khi điều kiện trên thỏa mãn.
            return; 
        }
        
        HandleJumping(); // Xử lý logic nhảy, bao gồm cả việc nhấn và nhả nút.
        HandleDash();    // Xử lý logic dash, bao gồm cả tụ lực và thực thi.
        
        // Chỉ cho phép di chuyển nếu không đang dash hoặc đang tụ lực dash.
        if (!isDash && !isChargingDash)
        {
            PlayerMove();
        }

        Flip(); // Xoay nhân vật theo hướng di chuyển.
        UpdateAnimationState(); // Cập nhật trạng thái animation dựa trên các điều kiện hiện tại.
    }

    // Hàm này được gọi mỗi bước thời gian vật lý cố định. Thích hợp cho việc xử lý vật lý.
    void FixedUpdate()
    {
        // Luôn kiểm tra `isGround` trong `FixedUpdate` để đảm bảo tính ổn định vật lý.
        // `Physics2D.OverlapCircle` trả về true nếu có bất kỳ Collider nào thuộc `groundLayer` nằm trong vòng tròn được định nghĩa.
        // Lưu ý: `layerMask: groundLayer` là cú pháp C# 8.0+ cho named arguments. Nếu dùng phiên bản cũ hơn, chỉ cần `groundLayer`.
        isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, layerMask: groundLayer);
    }
    
    // // Hàm này quản lý toàn bộ các hành động dựa trên input người chơi, bao gồm di chuyển và dash.
    // // <<< ĐÃ BỊ THAY THẾ BỞI HandleDash() và PlayerMove() gọi trực tiếp trong Update()
    // void HandleMovementAndDash()
    // {
    //     if (isDash)
    //     {
    //         if (dashTimeLeft > 0)
    //         {
    //             // Logic dash của bạn.
    //             Vector2 dashVelocity = Vector2.zero;
    //             if (GrafityDown) dashVelocity = new Vector2(Direction * dashSpeed, 0);
    //             if (GrafityUp) dashVelocity = new Vector2(-Direction * dashSpeed, 0);
    //             if (GrafityLeft) dashVelocity = new Vector2(0, -Direction * dashSpeed);
    //             if (GrafityRight) dashVelocity = new Vector2(0, Direction * dashSpeed);
    //             
    //             rb.linearVelocity = dashVelocity; // <<< THAY ĐỔI: Sử dụng rb.velocity thay cho linearVelocity cho nhất quán.
    //             dashTimeLeft -= Time.deltaTime;
    //         }
    //         else
    //         {
    //             isDash = false;
    //             rb.linearVelocity = Vector2.zero;
    //             animator.ResetTrigger("isDashing");
    //         }
    //     }
    //     else // Xử lý di chuyển và nhảy khi không ở trạng thái Dash.
    //     {
    //         PlayerMove();
    //         PlayerJumping(); // <<< SẼ BỊ LOẠI BỎ HOẶC THAY ĐỔI CHO PHÙ HỢP VỚI HandleJumping()
    //
    //         // <<< XÓA: Toàn bộ logic xử lý cooldown của Dash đã được xóa bỏ.
    //         // if (dashCooldownTime > 0) dashCooldownTime -= Time.deltaTime;
    //         
    //         // <<< THAY ĐỔI: Kiểm tra input để bắt đầu Dash mà không cần check cooldown.
    //         // Điều kiện: phím LeftShift được nhấn và đủ Stamina.
    //         if (Input.GetKeyDown(KeyCode.LeftShift) && playerStat.StaminaPlayer >= dashStaminaCost)
    //         {
    //             // <<< THAY ĐỔI: Gọi hàm ngắt tấn công ngay trước khi Dash.
    //             if (playerAttack != null) 
    //             {
    //                 playerAttack.InterruptAttack();
    //             }
    //             
    //             playerStat.UseStamina(dashStaminaCost); // Sử dụng Stamina.
    //             isDash = true;
    //             dashTimeLeft = dashTime;
    //             // dashCooldownTime = dashCooldown; // <<< XÓA: Không còn thiết lập cooldown.
    //             animator.SetTrigger("isDashing");
    //         }
    //     }
    // }
    
    // <<< HÀM MỚI: Xử lý toàn bộ logic Dash Tụ Lực
    // Hàm này quản lý việc bắt đầu, đang tụ lực, và thực thi hành động Dash.
    void HandleDash()
    {
        // Nếu người chơi đang trong trạng thái dash (đang di chuyển với tốc độ dash).
        if (isDash)
        {
            // <<< THAY ĐỔI: Logic kích hoạt đòn đánh giữa chừng trong quá trình dash.
            // Nếu là dash tụ lực đầy, chưa thực hiện đòn đánh, và thời gian dash còn lại chỉ còn 25%...
            if (isFullyChargedDash && !hasPerformedDashAttack && dashTimeLeft <= dashTime * 0.25f)
            {
                // Gọi hàm thực hiện tấn công dash từ script PlayerAttackDefault.
                playerAttack.PerformDashAttack();
                hasPerformedDashAttack = true; // Đánh dấu là đã thực hiện để không gọi lại trong lần dash này.
            }

            // Giảm thời gian dash còn lại.
            if (dashTimeLeft > 0)
            {
                dashTimeLeft -= Time.deltaTime;
            }
            else // Nếu thời gian dash đã hết.
            {
                isDash = false; // Kết thúc trạng thái dash.
                rb.linearVelocity = Vector2.zero; // Dừng người chơi lại.
                // <<< SỬA LỖI: Khôi phục trọng lực khi dash kết thúc.
                rb.gravityScale = defaultGravityScale; 
                animator.ResetTrigger("isDashing"); // Reset trigger animation dash.
                
                // Reset lại các cờ trạng thái khi dash kết thúc.
                if (isFullyChargedDash)
                {
                    isFullyChargedDash = false; // Reset cờ dash đầy.
                }
            }
            return; // Dừng xử lý tiếp theo khi đang trong trạng thái dash.
        }

        // Bắt đầu quá trình tụ lực dash.
        // Điều kiện: Nhấn phím Dash, còn đủ Stamina, và chưa đang tụ lực.
        if (Input.GetKeyDown(KeyCode.LeftShift) && playerStat.StaminaPlayer >= dashStaminaCost && !isChargingDash)
        {
            isChargingDash = true; // Bắt đầu trạng thái tụ lực.
            dashChargeTimer = 0f;  // Reset bộ đếm thời gian tụ lực.

            // <<< SỬA LỖI: Chỉ dừng người chơi khi họ đang ở trên mặt đất.
            // Tránh làm mất động lượng nếu đang bay.
            if (isGround)
            {
                rb.linearVelocity = Vector2.zero; // Dừng người chơi lại để tập trung tụ lực.
            }
        }

        // Nếu đang trong quá trình tụ lực và vẫn giữ nút Dash.
        if (isChargingDash && Input.GetKey(KeyCode.LeftShift))
        {
            dashChargeTimer += Time.deltaTime; // Tăng bộ đếm thời gian tụ lực.
            // Nếu thời gian tụ lực đạt mức tối đa.
            if (dashChargeTimer >= timeToMaxCharge)
            {
                isFullyChargedDash = true; // Đánh dấu dash đã tụ lực đầy.
                ExecuteDash(maxDashSpeed); // Thực thi dash ngay lập tức với tốc độ tối đa.
            }
        }

        // Nếu nhả nút Dash khi đang trong quá trình tụ lực.
        if (Input.GetKeyUp(KeyCode.LeftShift) && isChargingDash)
        {
            isFullyChargedDash = false; // Dash không được tụ lực đầy.
            // Tính toán tỷ lệ tụ lực dựa trên thời gian đã giữ nút.
            float chargeRatio = Mathf.Clamp01(dashChargeTimer / timeToMaxCharge);
            // Nội suy (Lerp) để tính tốc độ dash hiện tại giữa min và max speed.
            float currentDashSpeed = Mathf.Lerp(minDashSpeed, maxDashSpeed, chargeRatio);
            ExecuteDash(currentDashSpeed); // Thực thi dash với tốc độ đã tính.
        }
    }
    
    // <<< HÀM MỚI: Thực thi hành động dash.
    // Hàm này được gọi khi dash bắt đầu (tự động hoặc sau khi nhả nút).
    void ExecuteDash(float speed)
    {
        isChargingDash = false; // Kết thúc trạng thái tụ lực.
        
        // Nếu có script PlayerAttack, ngắt đòn tấn công hiện tại để ưu tiên dash.
        if (playerAttack != null) 
        {
            playerAttack.InterruptAttack();
        }

        playerStat.UseStamina(dashStaminaCost); // Sử dụng stamina cho hành động dash.
        isDash = true; // Bắt đầu trạng thái dash.
        dashTimeLeft = dashTime; // Thiết lập thời gian dash còn lại.
        hasPerformedDashAttack = false; // Reset cờ đã thực hiện attack mỗi khi bắt đầu dash mới.
        // <<< SỬA LỖI: Tắt trọng lực khi bắt đầu dash để đảm bảo dash bay theo đường thẳng.
        rb.gravityScale = 0f; 
        animator.SetTrigger("isDashing"); // Kích hoạt animation dash.

        // Áp dụng vận tốc dash dựa trên hướng trọng lực hiện tại và hướng nhìn.
        Vector2 dashVelocity = Vector2.zero;
        if (GrafityDown) dashVelocity = new Vector2(Direction * speed, 0);
        if (GrafityUp) dashVelocity = new Vector2(-Direction * speed, 0);
        if (GrafityLeft) dashVelocity = new Vector2(0, -Direction * speed);
        if (GrafityRight) dashVelocity = new Vector2(0, Direction * speed);
        rb.linearVelocity = dashVelocity; // Áp dụng vận tốc dash.
    }

    // Hàm này xử lý logic di chuyển của người chơi.
    void PlayerMove()
    {
        // Lấy input từ trục ngang (phím A/D hoặc mũi tên trái/phải).
        float x = Input.GetAxisRaw("Horizontal");
        
        // Điều chỉnh hướng di chuyển dựa trên trọng lực hiện tại.
        if (GrafityDown || GrafityUp) // Nếu trọng lực là dọc (xuống hoặc lên).
        {
            // Xác định hệ số nhân cho hướng di chuyển: 1 cho trọng lực xuống, -1 cho trọng lực lên.
            float moveDirection = GrafityDown ? 1f : -1f;
            // Cập nhật vận tốc ngang của Rigidbody2D. Vận tốc dọc được giữ nguyên.
            rb.linearVelocity = new Vector2(x * PlayerSpeed * moveDirection, rb.linearVelocity.y);
        }
        else if (GrafityLeft || GrafityRight) // Nếu trọng lực là ngang (trái hoặc phải).
        {
            // Xác định hệ số nhân cho hướng di chuyển: 1 cho trọng lực phải, -1 cho trọng lực trái.
            float moveDirection = GrafityRight ? 1f : -1f;
            // Cập nhật vận tốc dọc của Rigidbody2D. Vận tốc ngang được giữ nguyên.
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, x * PlayerSpeed * moveDirection);
        }

        // Cập nhật biến `Direction` dựa trên input để nhân vật xoay đúng hướng.
        if (x > 0) Direction = 1;  // Nếu di chuyển sang phải.
        if (x < 0) Direction = -1; // Nếu di chuyển sang trái.
    }
    
    // <<< HÀM ĐƯỢC CẬP NHẬT: Xử lý Nhảy Tùy Chỉnh Độ Cao
    // Hàm này xử lý logic nhảy của người chơi, bao gồm cả việc cắt giảm độ cao nhảy khi nhả nút sớm.
    void HandleJumping()
    {
        // Bắt đầu nhảy: Kiểm tra nút Space, đang trên mặt đất, và đủ Stamina.
        if (Input.GetKeyDown(KeyCode.Space) && isGround && playerStat.StaminaPlayer >= jumpStaminaCost)
        {
            playerStat.UseStamina(jumpStaminaCost); // Trừ Stamina khi nhảy.
            rb.linearVelocity = Vector2.zero; // Reset vận tốc để lực nhảy nhất quán, không bị ảnh hưởng bởi vận tốc rơi.

            Vector2 jumpDirection = Vector2.zero;
            // Xác định hướng nhảy dựa trên trọng lực hiện tại.
            if (GrafityDown) { rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); jumpDirection = Vector2.up; } // Reset vận tốc Y khi trọng lực xuống.
            if (GrafityUp)   { rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); jumpDirection = Vector2.down; } // Reset vận tốc Y khi trọng lực lên.
            if (GrafityLeft) { rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); jumpDirection = Vector2.right; } // Reset vận tốc X khi trọng lực trái.
            if (GrafityRight){ rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); jumpDirection = Vector2.left; } // Reset vận tốc X khi trọng lực phải.
            
            // Áp dụng lực nhảy.
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        }

        // Cắt giảm độ cao nhảy khi nhả nút Space khi đang bay lên.
        if (Input.GetKeyUp(KeyCode.Space))
        {
            bool isMovingUp = false;
            // Kiểm tra xem người chơi có đang di chuyển lên (hoặc ra xa bề mặt) hay không.
            if (GrafityDown && rb.linearVelocity.y > 0) isMovingUp = true;
            if (GrafityUp && rb.linearVelocity.y < 0) isMovingUp = true;
            if (GrafityLeft && rb.linearVelocity.x > 0) isMovingUp = true;
            if (GrafityRight && rb.linearVelocity.x < 0) isMovingUp = true;

            // Nếu đang di chuyển lên và nhả nút Space.
            if (isMovingUp)
            {
                // Giảm vận tốc theo hướng nhảy để nhân vật không bay cao được nữa.
                // Sử dụng biến tùy chỉnh `jumpCutMultiplier` thay vì giá trị 0.5f cố định.
                if (GrafityDown || GrafityUp) rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
                if (GrafityLeft || GrafityRight) rb.linearVelocity = new Vector2(rb.linearVelocity.x * jumpCutMultiplier, rb.linearVelocity.y);
            }
        }
    }


    // // Hàm này xử lý logic nhảy của người chơi.
    // // <<< ĐÃ BỊ THAY THẾ VÀ TÍCH HỢP VÀO HandleJumping()
    // void PlayerJumping()
    // {
    //     // <<< THAY ĐỔI: Thêm điều kiện kiểm tra Stamina trước khi nhảy.
    //     if (Input.GetKeyDown(KeyCode.Space) && isGround && playerStat.StaminaPlayer >= jumpStaminaCost)
    //     {
    //         playerStat.UseStamina(jumpStaminaCost); // <<< THAY ĐỔI: Trừ stamina khi nhảy.
    //
    //         Vector2 jumpDirection = Vector2.zero;
    //         if (GrafityDown) jumpDirection = Vector2.up;
    //         if (GrafityUp) jumpDirection = Vector2.down;
    //         if (GrafityLeft) jumpDirection = Vector2.right;
    //         if (GrafityRight) jumpDirection = Vector2.left;
    //
    //         rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
    //     }
    // }

    // Hàm này quản lý việc chuyển đổi giữa các trạng thái animation (idle, running, jumping, falling).
    static AudioManager.SoundType lastSound = AudioManager.SoundType.None;
    void UpdateAnimationState()
    {
        // Lấy input từ trục ngang để xác định xem người chơi có đang di chuyển hay không.
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Lưu trạng thái âm thanh trước đó để tránh gọi lại liên tục
        

        // Xử lý animation khi người chơi đang ở trên mặt đất.
        if (isGround)
        {
            animator.SetBool("isJumping", false); // Đảm bảo animation nhảy tắt.
            animator.SetBool("isFalling", false); // Đảm bảo animation rơi tắt.

            // Nếu có input di chuyển và không ở trạng thái dash hoặc đang tụ lực dash.
            if (moveInput != 0 && !isDash && !isChargingDash)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isIdie", false);

                if (lastSound != AudioManager.SoundType.Walk)
                {
                    AudioManager.Instance.PlayWalk(); // phát âm bước chân
                    lastSound = AudioManager.SoundType.Walk;
                }
            }
            else // Nếu không di chuyển hoặc đang dash/tụ lực dash.
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isIdie", true);

                if (lastSound == AudioManager.SoundType.Walk)
                {
                    AudioManager.Instance.StopCurrentSound(); // dừng âm bước chân
                    lastSound = AudioManager.SoundType.None;
                }
            }

            // Khi vừa tiếp đất (từ nhảy hoặc rơi xuống)
            if (lastSound == AudioManager.SoundType.Jump || lastSound == AudioManager.SoundType.Fall)
            {
                AudioManager.Instance.PlayHitTower(); // hiệu ứng tiếp đất
                lastSound = AudioManager.SoundType.Other;
            }
        }
        else // Xử lý animation khi người chơi đang ở trên không (nhảy hoặc rơi).
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdie", false);

            float verticalVelocity = 0;
            if (GrafityDown) verticalVelocity = rb.linearVelocity.y;
            if (GrafityUp) verticalVelocity = -rb.linearVelocity.y;
            if (GrafityLeft) verticalVelocity = rb.linearVelocity.x;
            if (GrafityRight) verticalVelocity = -rb.linearVelocity.x;

            if (verticalVelocity > 0.1f) // đang bay lên
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isFalling", false);

                if (lastSound != AudioManager.SoundType.Jump)
                {
                    AudioManager.Instance.PlayJump(); // âm nhảy
                    lastSound = AudioManager.SoundType.Jump;
                }
            }
            else // đang rơi
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);

                if (lastSound != AudioManager.SoundType.Fall)
                {
                    AudioManager.Instance.PlayFall(); // âm rơi
                    lastSound = AudioManager.SoundType.Fall;
                }
            }
        }

        // Nếu đang dash hoặc tụ lực dash
        if (isDash || isChargingDash)
        {
            if (lastSound != AudioManager.SoundType.BossSkill)
            {
                AudioManager.Instance.PlayBossSkill1(); // âm dash
                lastSound = AudioManager.SoundType.BossSkill;
            }
        }
    }


    // Hàm này chịu trách nhiệm xoay nhân vật theo hướng `Direction`.
    void Flip()
    {
        // Ngăn lật nhân vật khi đang tụ lực dash để giữ hướng nhìn ban đầu.
        if (isChargingDash) return;
        // `transform.localScale.x` được điều chỉnh dựa trên `Direction`.
        // `Mathf.Abs(transform.localScale.x)` đảm bảo chúng ta luôn bắt đầu từ giá trị dương trước khi nhân với Direction.
        // Điều này làm cho nhân vật quay sang trái hoặc phải.
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Direction, transform.localScale.y, transform.localScale.z);
    }
    
    // Hàm này được gọi khi người chơi tương tác với một đối tượng hoặc một sự kiện kích hoạt việc thay đổi trọng lực.
    // Nó bắt đầu một quá trình chuyển đổi trọng lực có thể điều khiển.
    public void StartGravityChange()
    {
        isChangingGravity = true; // Đặt cờ `isChangingGravity` là true để kích hoạt logic trong `Update`.
        isGround = false;         // Ngay lập tức coi như người chơi không còn trên mặt đất cũ để tránh các hành vi không mong muốn.
        rb.linearVelocity = Vector2.zero; // Reset vận tốc để người chơi rơi tự do theo trọng lực mới.
        rb.gravityScale = defaultGravityScale * 3f; // Tăng mạnh gravity scale (ví dụ: gấp 3 lần) để người chơi nhanh chóng "bị hút" về phía trọng lực mới.
        // Sau đó, trong `Update`, khi `isGround` trở thành true (nghĩa là đã tiếp đất theo trọng lực mới),
        // `isChangingGravity` sẽ được đặt lại thành false và `gravityScale` trở về bình thường.
    }

    public void ApplyGravityDirection(GravityDirection direction)
    {
        // This logic is taken from PlayerSkillGrafity.cs to apply a saved gravity state
        switch (direction)
        {
            case GravityDirection.Up:
                Physics2D.gravity = Vector2.up * 9.81f;
                transform.rotation = Quaternion.Euler(0, 0, 180);
                GrafityDown = false; GrafityUp = true; GrafityLeft = false; GrafityRight = false;
                break;
            case GravityDirection.Left:
                Physics2D.gravity = Vector2.left * 9.81f;
                transform.rotation = Quaternion.Euler(0, 0, -90);
                GrafityDown = false; GrafityUp = false; GrafityLeft = true; GrafityRight = false;
                break;
            case GravityDirection.Right:
                Physics2D.gravity = Vector2.right * 9.81f;
                transform.rotation = Quaternion.Euler(0, 0, 90);
                GrafityDown = false; GrafityUp = false; GrafityLeft = false; GrafityRight = true;
                break;
            case GravityDirection.Down:
            default:
                Physics2D.gravity = Vector2.down * 9.81f;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                GrafityDown = true; GrafityUp = false; GrafityLeft = false; GrafityRight = false;
                break;
        }
    }
}