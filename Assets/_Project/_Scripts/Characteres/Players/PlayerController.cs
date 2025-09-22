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
    public float jumpDampingAdd = 0.2f;
    private float defailtDamping;
    [Header("Dash")]
    public float dashSpeed = 15f;

    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    public bool isDash = false;
    public float dashTimeLeft;
    public float dashCooldownTime;
    void Start()
    {
        animator = GetComponent<Animator>();
        defaultGravity = GetComponent<Rigidbody2D>().gravityScale;
        defailtDamping = GetComponent<Rigidbody2D>().linearDamping;
    }


    void Update()
    {
        PlayerMove();
        PlayerJumping();
        UpdateJumpAndFall();
        Dash();
    }
    void PlayerMove()
    {
        // di chuyen theo truc X
        var x = Input.GetAxis("Horizontal");
        transform.Translate(new Vector2(x * PlayerSpeed * Time.deltaTime, 0));
        //animation dung yen
        if (isGround == true && animator != null)
        {

            animator.SetBool("isIdie", x == 0);
            animator.SetBool("isRunning", x != 0);

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
    void UpdateJumpAndFall()
    {
        if (!isGround)
        {
            if (GetComponent<Rigidbody2D>().linearVelocity.y > 0.1f)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isFalling", false);
                GetComponent<Rigidbody2D>().linearDamping += jumpDampingAdd;
            }
            else if (GetComponent<Rigidbody2D>().linearVelocity.y < -0.1f)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
                GetComponent<Rigidbody2D>().linearDamping = defailtDamping;
                GetComponent<Rigidbody2D>().gravityScale = defaultGravity + fallGravityAdd;
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