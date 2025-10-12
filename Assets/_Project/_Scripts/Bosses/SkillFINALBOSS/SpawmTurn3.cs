using System.Collections;
using UnityEngine;

public class SpawmTurn3 : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPre;
    public float speedBullet =7f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(spawm());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator spawm()
    {
        while (true)
        {
            bullet();
            yield return new WaitForSeconds(1f);
        }
    }
    
    void bullet()
    {
        Vector2 huong = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(huong.y, huong.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle + 180f);
        var bullet = Instantiate(bulletPre, transform.position, rotation);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = huong * 7f;
        Destroy(bullet,3f);
    }
}
