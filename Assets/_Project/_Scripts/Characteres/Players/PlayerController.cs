using UnityEngine;
﻿
﻿public class PlayerController : MonoBehaviour
﻿{
﻿    [Header("Movement Stats")]
﻿    public float PlayerSpeed = 5f;
﻿    public float jumpForce = 5f;
﻿    public float jumpStaminaCost = 10f;
﻿    [Range(0f, 1f)]
﻿    public float jumpCutMultiplier = 0.5f;
﻿
﻿    [Header("Dash Stats")]
﻿    public float minDashSpeed = 10f;
﻿    public float maxDashSpeed = 20f;
﻿    public float timeToMaxCharge = 1f;
﻿    public float dashTime = 0.2f;
﻿    public float dashStaminaCost = 20f;
﻿
﻿    [Header("Wall Jump Stats")]
﻿    public float wallSlidingSpeed = 2f;
﻿    public Vector2 wallJumpForce = new Vector2(5f, 10f);
﻿    public float wallJumpStaminaCost = 15f;
﻿
﻿    [Header("Cooldowns")]
﻿    public float[] jumpCooldownByLevel = { 0f, 0f, 0f }; // Example values, can be tuned
﻿    public float[] dashCooldownByLevel = { 0f, 0f, 0f }; // Example values, can be tuned
﻿    private float jumpCooldownTimer;
﻿    private float dashCooldownTimer;
﻿
﻿    [Header("Checks")]
﻿    public Transform groundCheck;
﻿    public float groundCheckRadius = 0.2f;
﻿    public Transform wallCheck;
﻿    public float wallCheckRadius = 0.2f;
﻿    public LayerMask groundLayer;
﻿
﻿    [Header("Gravity")]
﻿    public bool GrafityDown = true;
﻿    public bool GrafityUp = false;
﻿    public bool GrafityLeft = false;
﻿    public bool GrafityRight = false;
﻿    private float defaultGravityScale;
﻿
﻿    // State
﻿    public int Direction = 1;
﻿    private bool isGround;
﻿    private bool isTouchingWall;
﻿    private bool isWallSliding;
﻿    private bool isDashing;
﻿    private bool isChargingDash;
﻿    private float dashTimeLeft;
﻿    private float dashChargeTimer;
﻿    private bool isFullyChargedDash;
﻿    private bool hasPerformedDashAttack;
﻿
﻿    // Components
﻿    private Rigidbody2D rb;
﻿    private Animator animator;
﻿    private PlayerStat playerStat;
﻿    private PlayerAttackDefault playerAttack;
﻿
﻿    void Start()
﻿    {
﻿        rb = GetComponent<Rigidbody2D>();
﻿        animator = GetComponent<Animator>();
﻿        playerStat = GetComponent<PlayerStat>();
﻿        playerAttack = GetComponent<PlayerAttackDefault>();
﻿        defaultGravityScale = rb.gravityScale;
﻿        GrafityDown = true;
﻿    }
﻿
﻿    void Update()
﻿    {
﻿        UpdateCooldowns();
﻿        HandleInput();
﻿        Flip();
﻿        UpdateAnimationState();
﻿    }
﻿
﻿    void FixedUpdate()
﻿    {
﻿        CheckSurroundings();
﻿        HandleWallSliding();
﻿    }
﻿
﻿    private void UpdateCooldowns()
﻿    {
﻿        if (jumpCooldownTimer > 0)
﻿        {
﻿            jumpCooldownTimer -= Time.deltaTime;
﻿        }
﻿        if (dashCooldownTimer > 0)
﻿        {
﻿            dashCooldownTimer -= Time.deltaTime;
﻿        }
﻿    }
﻿
﻿    private void HandleInput()
﻿    {
﻿        if (isDashing) return;
﻿
﻿        // Movement
﻿        if (!isChargingDash)
﻿        {
﻿            PlayerMove();
﻿        }
﻿
﻿        // Jumping
﻿        if (Input.GetKeyDown(KeyCode.Space))
﻿        {
﻿            if (isGround)
﻿            {
﻿                HandleJumping();
﻿            }
﻿            else if (isTouchingWall && playerStat.hasWallJump)
﻿            {
﻿                HandleWallJumping();
﻿            }
﻿        }
﻿
﻿        // Variable Jump Height
﻿        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0)
﻿        {
﻿            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
﻿        }
﻿
﻿        // Dashing
﻿        HandleDash();
﻿    }
﻿
﻿    private void CheckSurroundings()
﻿    {
﻿        isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
﻿        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, groundLayer);
﻿    }
﻿
﻿    private void HandleWallSliding()
﻿    {
﻿        if (isTouchingWall && !isGround && rb.linearVelocity.y < 0 && playerStat.hasWallJump)
﻿        {
﻿            isWallSliding = true;
﻿            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlidingSpeed);
﻿        }
﻿        else
﻿        {
﻿            isWallSliding = false;
﻿        }
﻿    }
﻿
﻿    private void PlayerMove()
﻿    {
﻿        float x = Input.GetAxisRaw("Horizontal");
﻿        rb.linearVelocity = new Vector2(x * PlayerSpeed, rb.linearVelocity.y);
﻿        if (x != 0)
﻿        {
﻿            Direction = (int)Mathf.Sign(x);
﻿        }
﻿    }
﻿
﻿    private void HandleJumping()
﻿    {
﻿        if (jumpCooldownTimer > 0) return;
﻿        if (playerStat.StaminaPlayer < jumpStaminaCost) return;
﻿
﻿        playerStat.UseStamina(jumpStaminaCost);
﻿        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
﻿        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
﻿        jumpCooldownTimer = jumpCooldownByLevel[playerStat.jumpCooldownLevel];
﻿    }
﻿
﻿    private void HandleWallJumping()
﻿    {
﻿        if (jumpCooldownTimer > 0) return;
﻿        if (playerStat.StaminaPlayer < wallJumpStaminaCost) return;
﻿
﻿        playerStat.UseStamina(wallJumpStaminaCost);
﻿        isWallSliding = false;
﻿        rb.linearVelocity = new Vector2(wallJumpForce.x * -Direction, wallJumpForce.y);
﻿        jumpCooldownTimer = jumpCooldownByLevel[playerStat.jumpCooldownLevel];
﻿    }
﻿
﻿    private void HandleDash()
﻿    {
﻿        if (Input.GetKeyDown(KeyCode.LeftShift) && playerStat.StaminaPlayer >= dashStaminaCost && !isChargingDash)
﻿        {
﻿            isChargingDash = true;
﻿            dashChargeTimer = 0f;
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
﻿
﻿        if (isDashing)
﻿        {
﻿            if (dashTimeLeft > 0)
﻿            {
﻿                dashTimeLeft -= Time.deltaTime;
﻿            }
﻿            else
﻿            {
﻿                isDashing = false;
﻿                rb.gravityScale = defaultGravityScale;
﻿            }
﻿        }
﻿    }
﻿
﻿    void ExecuteDash(float speed)
﻿    {
﻿        if (dashCooldownTimer > 0)
﻿        {
﻿            isChargingDash = false;
﻿            return;
﻿        }
﻿
﻿        isChargingDash = false;
﻿        if (playerAttack != null)
﻿        {
﻿            playerAttack.InterruptAttack();
﻿        }
﻿
﻿        playerStat.UseStamina(dashStaminaCost);
﻿        isDashing = true;
﻿        dashTimeLeft = dashTime;
﻿        rb.gravityScale = 0f;
﻿        rb.linearVelocity = new Vector2(Direction * speed, 0);
﻿        dashCooldownTimer = dashCooldownByLevel[playerStat.dashCooldownLevel];
﻿    }
﻿
﻿    void Flip()
﻿    {
﻿        if (isChargingDash) return;
﻿        transform.localScale = new Vector3(Direction, 1, 1);
﻿    }
﻿
﻿    void UpdateAnimationState()
﻿    {
﻿        animator.SetBool("isJumping", !isGround && !isWallSliding);
﻿        animator.SetBool("isFalling", rb.linearVelocity.y < 0 && !isGround && !isWallSliding);
﻿        animator.SetBool("isRunning", isGround && rb.linearVelocity.x != 0);
﻿        animator.SetBool("isIdie", isGround && rb.linearVelocity.x == 0);
﻿        animator.SetBool("isWallSliding", isWallSliding);
﻿    }
﻿
﻿    void OnDrawGizmos()
﻿    {
﻿        Gizmos.color = Color.red;
﻿        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
﻿        Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
﻿    }
﻿}
﻿