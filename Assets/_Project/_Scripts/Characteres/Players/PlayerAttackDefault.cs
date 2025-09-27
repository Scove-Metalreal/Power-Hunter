using UnityEngine;

public class PlayerAttackDefault : MonoBehaviour
{
    [SerializeField] private Transform ATTSPAWM;

    public PlayerController playerController;
    public Transform Attack;

    [Header("Prefabs")]
    public GameObject Slash1Hitbox;
    public GameObject Slash2Hitbox;

    [Header("Combo Settings")]
    private GameObject varhitbox1;
    public float doubleClickTime = 0.25f; 
    private int clickCount = 0;
    private float clickTimer = 0f;

    private bool isAttacking = false;

    void Update()
    {
        if (clickTimer > 0)
            clickTimer -= Time.deltaTime;

        if (Input.GetMouseButton(0) && !isAttacking)
        {
            clickCount++;

            if (clickCount == 1)
            {
                clickTimer = doubleClickTime;
                Invoke(nameof(PlaySlash1), doubleClickTime);
            }
            else if (clickCount == 2 && clickTimer > 0)
            {
                CancelInvoke(nameof(PlaySlash1));
                PlayDoubleSlash();
                clickCount = 0;
            }
        }
    }

    void PlaySlash1()
    {
        if (clickCount == 1 && !isAttacking)
        {
            isAttacking = true;
            playerController.animator.SetTrigger("Slash1");
        }
        clickCount = 0;
    }

    void PlayDoubleSlash()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            playerController.animator.SetTrigger("DoubleSlash");
        }
    }

    // -------- Animation Events --------
    public void SpawnSlash1()
    {
        varhitbox1 = Instantiate(Slash1Hitbox, ATTSPAWM.position, Quaternion.identity);
        varhitbox1.transform.SetParent(Attack);
        
    }
    public void CompletedSlash1()
    {
        Destroy(varhitbox1);
    }

    public void SpawnSlash2()
    {
        var hitbox = Instantiate(Slash2Hitbox, ATTSPAWM.position, Quaternion.identity);
        hitbox.transform.SetParent(Attack);
        Destroy(hitbox, 0.3f);
    }

    public void EndAttack()
    {
        if (playerController.isGround == true && playerController.animator != null) { 
            var x = Input.GetAxis("Horizontal");
            playerController.animator.SetBool("isIdie", x == 0);
            playerController.animator.SetBool("isRunning", x != 0);
            
        }
        playerController.animator.ResetTrigger("Slash1");
        playerController.animator.ResetTrigger("DoubleSlash");
        isAttacking = false;
    }
}
