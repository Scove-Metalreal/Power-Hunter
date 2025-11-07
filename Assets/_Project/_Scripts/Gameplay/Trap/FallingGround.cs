using UnityEngine;

public class FallingGround : MonoBehaviour
{
    public float delayBeforeFall = 1f;
    private Rigidbody2D rb;
    public ParticleSystem collapseEffect;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static; // ban ??u ??ng yên
    }

    public void TriggerCollapse()
    {
        StartCoroutine(FallAfterDelay());
    }

    private System.Collections.IEnumerator FallAfterDelay()
    {
        

      yield return new WaitForSeconds(delayBeforeFall);
        rb.bodyType = RigidbodyType2D.Dynamic; // b?t gravity -> r?i
        
        Instantiate(collapseEffect, transform.position, Quaternion.identity);

    }
}
