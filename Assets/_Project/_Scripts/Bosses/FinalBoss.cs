using System.Collections;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    public PlayerCollision playerCollision;
    public int Turn;
    public bool isAttack = false;
    
    public bool isTurn1 = false;
    public GameObject turn1Prefab;
    
    public bool isTurnRunning = false;
    public int enemiesPerTurn = 5;
    
    void Start()
    {
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
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                }
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

}
