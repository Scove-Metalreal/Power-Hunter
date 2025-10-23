using System.Collections;
using UnityEngine;

public class RocketUp : MonoBehaviour
{
    public GameObject Rocket;
    public float RocketSpeed = 10f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            spawmRocket();
        }
    }

    void spawmRocket()
    {
        var rocket = Instantiate(Rocket, transform.position, Quaternion.identity);

        rocket.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * RocketSpeed;

        Destroy(rocket.gameObject, 5f);
    }
}
