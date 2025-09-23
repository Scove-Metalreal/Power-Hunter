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
        if(collision.gameObject.CompareTag("Ground"))
        {
            
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
}
