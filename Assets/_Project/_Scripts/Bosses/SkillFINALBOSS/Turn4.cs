using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Turn4 : MonoBehaviour
{
    public GameObject BombPre;
    public float force = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     void OnEnable()
    {
        StartCoroutine(spawm());
    }
    IEnumerator spawm()
    {
        while (true)
        {
            spawmBomb();
            yield return new WaitForSeconds(1f);
        }
    }

    void spawmBomb()
    {
        var obj = Instantiate(BombPre, transform.position, Quaternion.identity);
        float angle = Random.Range(0f, 360f);
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        obj.GetComponent<Rigidbody2D>().AddForce(dir * force, ForceMode2D.Impulse);
    }
}
