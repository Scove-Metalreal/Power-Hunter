using System.Collections;
using UnityEngine;

public class SpawmRocket : MonoBehaviour
{
    public GameObject rocket;
    public float speedRocket = 7f;
    void Start()
    {
        StartCoroutine(spawmr());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator spawmr()
    {
        while (true)
        {
            Spawm();
            yield return new WaitForSeconds(2f);
        }
    }
    void Spawm()
    {
        var Srocket = Instantiate(rocket,transform.position,Quaternion.identity);
        Srocket.GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * speedRocket;
        Destroy(Srocket,4.5f);
    }
}
