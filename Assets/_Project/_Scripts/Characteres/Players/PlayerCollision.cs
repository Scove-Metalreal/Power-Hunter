using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerController playerController;
    void Start()
    {
        playerController = GetComponent<PlayerController>();
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
    }

    void PlayerDead()
    {
        if (playerController.animator != null)
        {
            playerController.animator.SetBool("isDead",true);
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }
    public void EndGameAnimation()
    {
        Time.timeScale = 0f;
    }
}