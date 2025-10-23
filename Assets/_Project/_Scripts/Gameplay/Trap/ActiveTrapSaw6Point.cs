using UnityEngine;

public class ActiveTrapSaw6Point : MonoBehaviour
{
    public GameObject Trap;
    void Start()
    {
        Trap.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Trap.SetActive(true);
        }
    }
}
