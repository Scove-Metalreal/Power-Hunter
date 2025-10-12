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
    void Start()
    {
        Turn2.SetActive(false);
        Turn3.SetActive(false);
        StartCoroutine(RandomTurn());
    }

    // Update is called once per frame
    void Update()
    {
        
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
                isTurnRunning = false;
            }
            yield return null;
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

        Vector3 spawnPos = player.position + player.up * 2.2f;
        Instantiate(Turn4pre, spawnPos, player.rotation);
    }

}
