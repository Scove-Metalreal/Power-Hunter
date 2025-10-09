using System.Collections;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    public int Turn;
    public bool isAttack = false;
    public int currentArea;
    void Start()
    {
        currentArea = 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator RandomTurn()
    {

        Turn = Random.Range(0, 4);

        switch (Turn)
        {
            case 0:
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

    IEnumerator turn1()
    {

    }

}
