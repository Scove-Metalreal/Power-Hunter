using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerController : MonoBehaviour
{
    [Header("Chi So Nhan Vat")]
    public float PlayerSpeed = 5f;
    public int direction = 1;
    public bool isGround = true;
    public float jumpForce = 5f;
    [Header("Khac")]
    public Animator animator;
    public int Direction = 1;
    [Header("Stat Rigibody 2d")]
    private float defaultGravity;
    public float fallGravityAdd = 1f;
    
    
    [Header("Dash")]
    public float dashSpeed = 15f;

    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    public bool isDash = false;
    public float dashTimeLeft;
    public float dashCooldownTime;
    [Header("SkillGrafity")]
    public bool GrafityDown;
    public bool GrafityUp;
    public bool GrafityLeft;
    public bool GrafityRight;
    void Start()
    {
        animator = GetComponent<Animator>();
        defaultGravity = GetComponent<Rigidbody2D>().gravityScale;
        
        GrafityDown = true;
        GrafityUp = false;
        GrafityLeft = false;
        GrafityRight = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }


    void Update()
    {
        
        if (GrafityDown == true)
        {
            PlayerMove();
            PlayerJumping();
        }
        if (GrafityUp == true)
        {
            PlayerMove();
            PlayerJumpingUp();
        }
        if (GrafityLeft == true)
        {
            PlayerMove();
            PlayerJumpingLeft();
        }
        if (GrafityRight == true)
        {
            PlayerMove();
            PlayerJumpingRight();
        }
        
        UpdateJumpAndFall();
        Dash();
    }
    void PlayerMove()
    {
        // di chuyen theo truc X
        float x = Input.GetAxisRaw("Horizontal");
        float moveInput = 0f;
        Vector2 velocity = GetComponent<Rigidbody2D>().linearVelocity;

        if (GrafityDown) 
        {
            velocity.x = x * PlayerSpeed;
            moveInput = x;
        }
        else if (GrafityUp) 
        {
            velocity.x = -x * PlayerSpeed;
            moveInput = x;
        }
        else if (GrafityLeft) 
        {
            velocity.y = -x * PlayerSpeed;
            moveInput = x;
        }
        else if (GrafityRight) 
        {
            velocity.y = x * PlayerSpeed;
            moveInput = x;
        }
        GetComponent<Rigidbody2D>().linearVelocity = velocity;

        //animation dung yen
        if (isGround == true && animator != null)
        {

            animator.SetBool("isIdie", moveInput == 0);
            animator.SetBool("isRunning", moveInput != 0);
            

        }
        //Huong theo huong di chuyen
        Vector3 currentScale = transform.localScale;//LocalScale hien tai
        if (x > 0)
        {
            Direction = 1;//mat phai
            transform.localScale = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);// phai
        }
        if (x < 0)
        {
            Direction = -1;//mat trai
            transform.localScale = new Vector3(-Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);// trai
        }
    }
    void PlayerJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround == true)// khi an Space va tren mat dat
        {
            // dang khong o tren mat dat
            isGround = false;
            // anition nhay
            if (animator != null)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isRunning", false);

            }
            // nhay
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }
    void PlayerJumpingUp()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround == true)// khi an Space va tren mat dat
        {
            // dang khong o tren mat dat
            isGround = false;
            // anition nhay
            if (animator != null)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isRunning", false);

            }
            // nhay
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -jumpForce), ForceMode2D.Impulse);
        }
    }
    
    void PlayerJumpingLeft()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround == true)// khi an Space va tren mat dat
        {
            // dang khong o tren mat dat
            isGround = false;
            // anition nhay
            if (animator != null)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isRunning", false);

            }
            // nhay
            GetComponent<Rigidbody2D>().AddForce(new Vector2(jumpForce,0 ), ForceMode2D.Impulse);
        }
    }
    
    void PlayerJumpingRight()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround == true)// khi an Space va tren mat dat
        {
            // dang khong o tren mat dat
            isGround = false;
            // anition nhay
            if (animator != null)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isRunning", false);

            }
            // nhay
            GetComponent<Rigidbody2D>().AddForce(new Vector2(-jumpForce,0 ), ForceMode2D.Impulse);
        }
    }
    void UpdateJumpAndFall()
    {
        if (!isGround)
        {
            if (GetComponent<Rigidbody2D>().linearVelocity.y > 0.1f)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isFalling", false);
                
            }
            else if (GetComponent<Rigidbody2D>().linearVelocity.y < -0.01f)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
                
                GetComponent<Rigidbody2D>().gravityScale = defaultGravity + fallGravityAdd;
            }
            else
            {
                isGround = true;
            }
        }
        else
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            GetComponent<Rigidbody2D>().gravityScale = defaultGravity;
            


        }
    }
    void Dash()
    {
        if (dashCooldownTime > 0)
        {
            dashCooldownTime -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTime <= 0 && isDash == false)
        {
            isDash = true;
            dashTimeLeft = dashTime;
            dashCooldownTime = dashCooldown;
            animator.SetTrigger("isDashing");
        }
        if (isDash == true)
        {
            if (dashTimeLeft > 0)
            {
                GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Direction * dashSpeed, GetComponent<Rigidbody2D>().linearVelocity.y);

                dashTimeLeft -= Time.deltaTime;
            }
            else
            {
                isDash = false;
                GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, GetComponent<Rigidbody2D>().linearVelocity.y);
                

            }
        }
    }

}