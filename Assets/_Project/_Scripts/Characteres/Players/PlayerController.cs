using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Stat
    public float PlayerSpeed = 5f;
    public int direction = 1;
    public bool isGround = true;
    public float jumpForce =5f;
    //Other

    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        PlayerMove();
        PlayerJumping();
    }
    void PlayerMove()
    {
        // di chuyen theo truc X
        var x = Input.GetAxis("Horizontal");
        transform.Translate(new Vector2 (x * PlayerSpeed* Time.deltaTime, 0));
        //animation dung yen
        if (isGround == true && animator != null)
        {
            
                animator.SetBool("isIdie", x == 0);
                animator.SetBool("isRunning", x != 0);
            
        }
        //Huong theo huong di chuyen
        Vector3 currentScale= transform.localScale;//LocalScale hien tai
        if (x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(currentScale.x), currentScale.y,currentScale.z);// phai
        }
        if (x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(currentScale.x), currentScale.y,currentScale.z);// trai
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
            GetComponent<Rigidbody2D>().AddForce(new Vector2 (0,jumpForce),ForceMode2D.Impulse);
        }
    }
}
