using UnityEngine;

public class Turn1 : MonoBehaviour
{
    public float animSpeed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"{transform.position}");
    }

    public void Event1Turn4()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y - 0.6f), animSpeed * Time.deltaTime);
    }
    public void Event2Turn4()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y - 1.6f), (animSpeed-5) * Time.deltaTime);
    }
    public void EndAnimationClip()
    {
        Destroy(gameObject);
    }
}
