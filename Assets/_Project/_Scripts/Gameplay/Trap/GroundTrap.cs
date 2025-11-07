using UnityEngine;

public class GroundTrap : MonoBehaviour
{
    public GameObject SpikePre;
    public Transform spikeBody; 
    public float speedSpike = 5f;

    private GameObject currentSpike;
    private Vector3 startPos;

    public void ActiceSpikeTrap()
    {
        
        currentSpike = Instantiate(SpikePre, transform.position, Quaternion.identity);
        startPos = transform.position;

        
        Rigidbody2D rb = currentSpike.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.down * speedSpike;
    }

    void Update()
    {
        if (currentSpike != null && spikeBody != null)
        {
            // Tính khoảng cách rơi
            float distance = Vector2.Distance(startPos, currentSpike.transform.position);

            // Kéo dài phần đen theo khoảng rơi
            spikeBody.localScale = new Vector3(
                spikeBody.localScale.x,
                distance, // chiều dài theo Y
                spikeBody.localScale.z
            );
        }
    }
}
