using System.Collections;
using UnityEngine;

public class Turn1New : MonoBehaviour
{
    public Transform[] Down;
    public Transform[] Up;
    public GameObject turn1_new_pre;
    public float Turn1Speed = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator DownSpawm()
    {
        foreach (Transform t in Down)
        {
            var spawm = Instantiate(turn1_new_pre, t.position, t.rotation);
            spawm.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * Turn1Speed ;
            yield return new WaitForSeconds(1f);
        }
    }
    public IEnumerator UpSpawm()
    {
        foreach (Transform t in Up)
        {
            var spawm = Instantiate(turn1_new_pre, t.position, t.rotation);
            spawm.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * Turn1Speed ;
            yield return new WaitForSeconds(1f);
        }
    }
}
