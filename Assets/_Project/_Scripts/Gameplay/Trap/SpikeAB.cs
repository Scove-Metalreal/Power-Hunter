using UnityEngine;

public class SpikeAB : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;

    private Vector3 target;
    public GameObject Door1;
    [SerializeField] private Transform player;
    void Awake()
    {
        if (pointA == null || pointB == null) return;
        transform.position = pointA.position;
        target = pointB.position;
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }
    }

    void Update()
    {
        if (Door1.activeSelf == true)
        {
            if (pointA == null || pointB == null) return;

            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) <= 0.05f)
                target = target == pointA.position ? pointB.position : pointA.position;
        }
        else
        {
            if (player == null) return;
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }
}
