using UnityEngine;

public class SawTrap6Point : MonoBehaviour
{
    public Transform[] points;  
    public float speed = 3.0f;  

    private int currentIndex = 0;
    private bool isReversing = false;

    void Update()
    {
        if (points == null || points.Length < 2) return;

       
        Transform target = points[currentIndex];

       
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            if (!isReversing)
            {
                currentIndex++;
                if (currentIndex >= points.Length)
                {
                    currentIndex = points.Length - 2; 
                    isReversing = true;
                }
            }
            else
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = 1; 
                    isReversing = false;
                }
            }
        }
    }
}
