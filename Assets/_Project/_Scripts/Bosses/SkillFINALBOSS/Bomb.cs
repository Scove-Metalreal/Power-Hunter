using UnityEngine;
using static UnityEngine.LightAnchor;

public class Bomb : MonoBehaviour
{
    private PlayerController playerController;
    public float bombForce = 3f;
    public Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        playerController = FindAnyObjectByType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Turn4FX"))
        {
            anim.SetTrigger("Active");
            if (playerController.GrafityDown)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-4, 4), Random.Range(2, 4)) * bombForce, ForceMode2D.Impulse);
            }
            if (playerController.GrafityUp)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-4, 4),- Random.Range(2, 4)) * bombForce, ForceMode2D.Impulse);
            }
        
            if (playerController.GrafityLeft)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(2, 4), Random.Range(-4, 4)) * bombForce, ForceMode2D.Impulse);
            }
            }
            if (playerController.GrafityRight)
            {
                 GetComponent<Rigidbody2D>().AddForce(new Vector2(-Random.Range(2, 4), Random.Range(-4, 4)) * bombForce, ForceMode2D.Impulse);
             }
                
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            anim.SetTrigger("Active");
        }
    }
}
