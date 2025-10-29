using UnityEngine;

public class SpikeLV2 : MonoBehaviour
{
    public float speed = 7f;
    public float distance = 2f;

    private Vector2 startPos;
    private Vector2 targetPos;
    private bool hasMoved = false;

    void Start()
    {
        startPos = transform.position;                     
        targetPos = startPos - new Vector2(0, distance);    
        transform.position = targetPos;                     
    }

    void Update()
    {
        
        if (!hasMoved)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos, speed * Time.deltaTime);

            
            if (Vector2.Distance(transform.position, startPos) < 0.01f)
            {
                hasMoved = true;
            }
        }
    }
}
