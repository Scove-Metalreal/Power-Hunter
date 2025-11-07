using UnityEngine;

public class Spike : MonoBehaviour
{
    public float speedSpike = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var dir = Quaternion.Euler(0, 0, Random.Range(-20, 20)) * Vector2.up;
        GetComponent<Rigidbody2D>().linearVelocity = dir.normalized * speedSpike;
        Destroy(gameObject, 2f);
    }
}
