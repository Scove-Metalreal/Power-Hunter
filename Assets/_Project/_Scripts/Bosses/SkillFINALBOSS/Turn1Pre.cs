using UnityEngine;

public class Turn1Pre : MonoBehaviour
{
    public GameObject fatherColision;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BlockTurn1")) 
        {
            Destroy(fatherColision);
        }
    }
}
