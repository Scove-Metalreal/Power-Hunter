using System.Collections;
using UnityEngine;

public class Turn2 : MonoBehaviour
{
    public GameObject Bullet;
    public float bulletSpeed = 5f;
    private int AddRotation = 0;
    void Start()
    {
        StartCoroutine(TurnAround());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator TurnAround()
    {
        while (true)
        {
            
            for (int i = 0; i < 360; i = i + 45)
            {
                SpawmBullet(i = AddRotation);
            }
            AddRotation += 10;
            yield return new WaitForSeconds(1f);
            
        }
       
    }
    void SpawmBullet(float angle)
    {
        Quaternion quaternion = Quaternion.Euler(0, 0, angle);
        var bullet = Instantiate(Bullet, transform.position, quaternion);
        Vector2 direction = quaternion * Vector2.right;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed ;
        
    }
}
