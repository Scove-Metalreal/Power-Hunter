using UnityEngine;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
    private PlayerController playerController;
    private Enemy enemy;
    private PlayerStat playerStat;
    [Header("PlayerTakeDamage")]
    public float TakeDamage = 20;
    [Header("UI")]
    public GameObject LoseUIPanel;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        enemy = FindAnyObjectByType<Enemy>();
        playerStat = GetComponent<PlayerStat>();
        LoseUIPanel.SetActive(false);
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
            playerStat.TakeDamage(20f);

            if (playerStat.HeathPlayer <= 0)
            {
                PlayerDead();
            }
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
    public void StartDeadAnimation()
    {
        Time.timeScale = 0f;
    }
    public void EndDeadAnimation()
    {
        Destroy(gameObject);
        LoseUIPanel.SetActive(true);
    }
}