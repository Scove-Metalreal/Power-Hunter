using System.Collections;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    public PlayerCollision playerCollision;
    public int Turn;
    public bool isAttack = false;
    
    
    public GameObject turn1Prefab;
    public Transform player;
    public bool isTurnRunning = false;
    public int enemiesPerTurn = 5;
    public GameObject Turn2;
    public GameObject Turn3;
    public GameObject Turn4;
    public GameObject Turn4pre;
    public Transform positionSpawmTurn4;
    private Transform currentPlayer;
    public Transform PointA;
    public Transform PointB;
    private Animator anim;
    public Vector3 currenLocalScale;
    public Turn1New turn1New;
    void Start()
    {
        anim = GetComponent<Animator>();
        Turn2.SetActive(false);
        Turn3.SetActive(false);
        Turn4.SetActive(false);
        StartCoroutine(RandomTurn());
        transform.position = PointA.position;
        currenLocalScale = transform.localScale;
        turn1New = GetComponent<Turn1New>();
        if (turn1New == null)
        {
            turn1New = FindAnyObjectByType<Turn1New>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentPlayer = player.transform;
        
    }
    IEnumerator RandomTurn()
    {

        while (true)
        {
            
            if (!isTurnRunning)
            {
                Turn = Random.Range(0, 5);
                anim.SetTrigger("Skill");
                isTurnRunning = true;
                switch (Turn)
                {
                    case 0:
                        yield return StartCoroutine(turn1New.DownSpawm());
                        yield return new WaitForSeconds(2f);
                        yield return StartCoroutine(turn1New.UpSpawm());
                        AudioManager.Instance.PlayBossSkill1();
                        break;
                        
                    case 1:
                        Turn2.SetActive(true);
                        yield return new WaitForSeconds(8f);
                        Turn2.SetActive(false);
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
                }
                Tele();
                isTurnRunning = false;
            }
            yield return null;
        }
    }
    void Tele()
    {
        float distanceToA = Vector3.Distance(transform.position, PointA.position);
        float distanceToB = Vector3.Distance(transform.position, PointB.position);

        if (distanceToA < 0.1f)
        {
            anim.SetTrigger("isTeleport");
            transform.position = PointB.position;
            transform.localScale = new Vector3(-Mathf.Abs(currenLocalScale.x), currenLocalScale.y, currenLocalScale.z);
        }
        else if (distanceToB < 0.1f)
        {
            anim.SetTrigger("isTeleport");
            transform.position = PointA.position;
            transform.localScale = new Vector3(Mathf.Abs(currenLocalScale.x), currenLocalScale.y, currenLocalScale.z);
        }
    }
    
    void SpawmTurn4()
    {

        Vector3 spawnPos = currentPlayer.position + currentPlayer.up * 2.2f;
        Instantiate(Turn4pre, spawnPos, player.rotation);

    }

}
