using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerController playerController;
    private Enemy enemy;
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        enemy = FindAnyObjectByType<Enemy>();
    }


    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            playerController.CanMove = true;
            playerController.isGround = true;
            if (playerController.animator != null)
            {
                playerController.animator.SetBool("isJumping", false);



            }
        }

    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            playerController.isGround = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
         if(collision.gameObject.CompareTag("DeathZone"))
        {
            PlayerDead();
        }
        if (collision.CompareTag("FindPlayer"))
        {
            enemy.isCollidingWithPlayer = true;
            enemy.DoAttack();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("FindPlayer"))
        {
            enemy.isCollidingWithPlayer = false;

        }
    }
    void PlayerDead()
    {
        if (playerController.animator != null)
        {
            playerController.animator.SetBool("isDead",true);
            playerController.CanMove=false;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }
    public void EndGameAnimation()
    {
        Time.timeScale = 0f;
    }
}