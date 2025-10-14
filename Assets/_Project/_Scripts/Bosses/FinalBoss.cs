using System.Collections;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    public PlayerCollision playerCollision;
    public int Turn;
    public bool isAttack = false;
    
    public bool isTurn1 = false;
    public GameObject turn1Prefab;
    public Transform player;
    public bool isTurnRunning = false;
    public int enemiesPerTurn = 5;
    public GameObject Turn2;
    public GameObject Turn3;
    public GameObject Turn4pre;
    public Transform positionSpawmTurn4;
    private Transform currentPlayer;
    public Transform PointA;
    public Transform PointB;
    private Animator anim;
    public Vector3 currenLocalScale;
    void Start()
    {
        anim = GetComponent<Animator>();
        Turn2.SetActive(false);
        Turn3.SetActive(false);
        StartCoroutine(RandomTurn());
        transform.position = PointA.position;
        currenLocalScale = transform.localScale;
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
            if( isTurn1 == true )
            {
                
                
                    yield return StartCoroutine(turn1(playerCollision.currentAreaTranform));
                isTurnRunning = false;
                isTurn1 = false;
                yield return new WaitForSeconds(2f);
            }
            if (!isTurnRunning)
            {
                Turn = Random.Range(0, 5);
                anim.SetTrigger("Skill");
                isTurnRunning = true;
                switch (Turn)
                {
                    case 0:
                        isTurn1 = true;
                        
                        break;
                    case 1:
                        Turn2.SetActive(true);
                        yield return new WaitForSeconds(8f);
                        Turn2.SetActive(false);
                        break;
                    case 2:
                        Turn3.SetActive(true);
                        yield return new WaitForSeconds(8f);
                        Turn3.SetActive(false);
                        break;
                    case 3:
                        for (int i = 0; i < 5;i++)
                        {
                            SpawmTurn4();
                            yield return new WaitForSeconds(2f);
                        }
                        break;
                    case 4:
                        yield return new WaitForSeconds(10f);
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
    IEnumerator turn1(Vector2 AreaPosition)
    {
        
        Instantiate(turn1Prefab, AreaPosition, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Instantiate(turn1Prefab, AreaPosition, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Instantiate(turn1Prefab, AreaPosition, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Instantiate(turn1Prefab, AreaPosition, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Instantiate(turn1Prefab, AreaPosition, Quaternion.identity);
        yield return new WaitForSeconds(2f);    
        
    }
    void SpawmTurn4()
    {

        Vector3 spawnPos = currentPlayer.position + currentPlayer.up * 2.2f;
        Instantiate(Turn4pre, spawnPos, player.rotation);

    }

}
