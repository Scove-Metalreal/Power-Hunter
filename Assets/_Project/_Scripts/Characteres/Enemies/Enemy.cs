using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private Animator anim;
    private int attackCount = 0;
    public bool isCollidingWithPlayer = false;

    public float moveSpeed = 2f;
    public GameObject HitBoxAttack1;
    public GameObject HitBoxAttack2;
    private GameObject VARHitBoxAttack1;
    private GameObject VARHitBoxAttack2;
    public Transform AttackSpawm;
    public Transform AttackSpawm2;
    public float Attack2BulletSpeed;
    private bool Left=false, Right = false;
    private bool isGrounded = false;    
    public Transform Parent;
    private Rigidbody2D rb;
    private bool isattack= false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {

            if (!isGrounded || anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("isWalking", false);
                yield return null;
                continue;
            }

            
            anim.SetBool("isWalking", true);
            transform.localScale = new Vector3(1, 1, 1);
            Left = true;
            Right = false;
            float t = 0;
            while (t < 2f && isGrounded && isattack == false)
            {
                rb.linearVelocity = Vector2.left * moveSpeed;
                t += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(2f);

            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);
            yield return new WaitForSeconds(2f);

            
            anim.SetBool("isWalking", true);
            transform.localScale = new Vector3(-1, 1, 1);
            Left = false;
            Right = true;
            t = 0;
            while (t < 2f && isGrounded && isattack == false)
            {
                rb.linearVelocity = Vector2.right * moveSpeed;
                t += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(2f);

            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);
            yield return new WaitForSeconds(2f);
        }
    }

    void Update()
    {
        
        if (isCollidingWithPlayer && isattack == false)
        {
            DoAttack();
        }
    }

    public void DoAttack()
    {
        isattack = true;
        rb.linearVelocity = Vector2.zero;
        if (attackCount < 2)
        {
            anim.SetTrigger("isAttack1");
            attackCount++;
        }
        else
        {
            anim.SetTrigger("isAttack2");
            attackCount = 0; 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    public void Attack1Hitbox()
    {
        VARHitBoxAttack1 = Instantiate(HitBoxAttack1, AttackSpawm.position,Quaternion.identity);
        VARHitBoxAttack1.transform.SetParent(Parent);
    }
    public void EndAttack1()
    {
        Destroy(VARHitBoxAttack1);
        isattack = false;
    }
    public void Attack2Hitbox()
    {
        VARHitBoxAttack2 = Instantiate(HitBoxAttack2, AttackSpawm2.position,Quaternion.identity);
        if(Left == true)
        {
            VARHitBoxAttack2.GetComponent<Rigidbody2D>().linearVelocity = Vector2.left * Attack2BulletSpeed;
        }
        if(Right == true)
        {
            VARHitBoxAttack2.GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * Attack2BulletSpeed;
        }
        Destroy(VARHitBoxAttack2,1f);
        
    }
    public void EndAttack2()
    {
        
        isattack = false;
    }

}
