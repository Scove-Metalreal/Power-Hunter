using UnityEngine;

public class Platform : MonoBehaviour
{
    public float speed = 1.0f;

    public Transform a;
    public Transform b;
    private Transform c;
    private bool check = true;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (check)
            c = a.transform;
        else
            c = b.transform;
        plapla(c);

    }
    void plapla(Transform current)
    {
        transform.position = Vector3.MoveTowards(transform.position, current.position, speed * Time.deltaTime);

        var distanceToTarget = Vector3.Distance(transform.position, current.position);


        if (distanceToTarget < 0.01f)
            check = !check;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
