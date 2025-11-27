using System.Collections;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    public PlayerCollision playerCollision;
    public int Turn;
    public bool isTurnRunning = false;
    
    public GameObject Turn2;
    public GameObject Turn3;
    public GameObject Turn4;
    private Animator anim;
    public Vector3 currenLocalScale;
    public Turn1New turn1New;
    
    [SerializeField] float moveSpeed;
    [SerializeField] Vector2 moveDirection = new Vector2(1f, 0.25f);
    [SerializeField] GameObject rightCheck, roofCheck, groundCheck;
    [SerializeField] Vector2 rightCheckSize, roofCheckSize, groundCheckSize;
    [SerializeField] LayerMask groundLayer, platform  ;
    [SerializeField]bool goingUp = true;
    private bool touchedGround, touchedRoof, touchedRight;
    private Rigidbody2D EnemyRB;
    void Start()
    {
        EnemyRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        Turn2.SetActive(false);
        Turn3.SetActive(false);
        Turn4.SetActive(false);
        StartCoroutine(RandomTurn());
        
        turn1New = GetComponent<Turn1New>();
        if (turn1New == null)
        {
            turn1New = FindAnyObjectByType<Turn1New>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        HitLogic();
    }

    void FixedUpdate()
    {
        EnemyRB.linearVelocity = moveDirection * moveSpeed;
    }
    IEnumerator RandomTurn()
    {

        while (true)
        {
            
            if (!isTurnRunning)
            {
                Turn = Random.Range(0, 6);
                anim.SetTrigger("Skill");
                isTurnRunning = true;
                switch (Turn)
                {
                    case 0:
                        Turn2.SetActive(true);
                        yield return StartCoroutine(turn1New.DownSpawm());
                        yield return StartCoroutine(turn1New.LeftSpawm());
                        yield return new WaitForSeconds(2f);
                        yield return StartCoroutine(turn1New.UpSpawm());
                        yield return StartCoroutine(turn1New.RightSpawm());
                        Turn2.SetActive(false);
                        AudioManager.Instance.PlayBossSkill1();
                        break;
                        
                    case 1:
                        Turn2.SetActive(true);
                        Turn4.SetActive(true);
                        yield return new WaitForSeconds(8f);
                        Turn2.SetActive(false);
                        Turn4.SetActive(false);
                        AudioManager.Instance.PlayBossSkill2();
                        break;
                    case 2:
                        Turn3.SetActive(true);
                        yield return new WaitForSeconds(8f);
                        Turn3.SetActive(false);
                        AudioManager.Instance.PlayBossSkill3();
                        break;
                    case 3:
                        Turn4.SetActive(true);
                        yield return new WaitForSeconds(8f);
                        Turn4.SetActive(false);
                        AudioManager.Instance.PlayBossSkill1();
                        break;
                    case 4:
                        yield return new WaitForSeconds(4f);
                        AudioManager.Instance.PlayBossSkill3();
                        break;
                    case 5:
                        Turn4.SetActive(true);
                        yield return StartCoroutine(turn1New.DownSpawm());
                        yield return StartCoroutine(turn1New.LeftSpawm());
                        yield return new WaitForSeconds(2f);
                        yield return StartCoroutine(turn1New.UpSpawm());
                        yield return StartCoroutine(turn1New.RightSpawm());
                        Turn4.SetActive(false);
                        AudioManager.Instance.PlayBossSkill1();
                        break;
                }
                
                isTurnRunning = false;
            }
            yield return null;
        }
    }
    
    void HitLogic()
    {
        touchedRight = HitDetector(rightCheck, rightCheckSize, (groundLayer | platform));
        touchedRoof = HitDetector(roofCheck, roofCheckSize, (groundLayer | platform));
        touchedGround = HitDetector(groundCheck, groundCheckSize, (groundLayer | platform ));

        if (touchedRight)
        {
            Flip();
        }
        if (touchedRoof && goingUp)
        {
            ChangeYDirection();
        }
        if (touchedGround && !goingUp)
        {
            ChangeYDirection();
        }
    }

    bool HitDetector(GameObject gameObject, Vector2 size, LayerMask layer)
    {
        return Physics2D.OverlapBox(gameObject.transform.position, size, 0f, layer);
    }

    void ChangeYDirection()
    {
        moveDirection.y = -moveDirection.y;
        goingUp = !goingUp;
    }

    void Flip()
    {
        transform.Rotate(new Vector2(0, 180));
        moveDirection.x = -moveDirection.x;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groundCheck.transform.position, groundCheckSize);
        Gizmos.DrawWireCube(roofCheck.transform.position, roofCheckSize);
        Gizmos.DrawWireCube(rightCheck.transform.position, rightCheckSize);
    }

}
