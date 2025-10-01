using UnityEngine;

public class Trap : MonoBehaviour
{
    public Collider2D hitbox;
    public float activeTime = 0.5f;
    public float cooldownTime = 1.5f;

    private bool isActive = false;

    private void Start()
    {
        if (hitbox != null)
            hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive && collision.CompareTag("Player"))
        {
            StartCoroutine(ActivateTrap());
        }
    }

    private System.Collections.IEnumerator ActivateTrap()
    {
        isActive = true;
        if (hitbox != null)
            hitbox.enabled = true;
        yield return new WaitForSeconds(activeTime);
        if (hitbox != null)
            hitbox.enabled = false;
        yield return new WaitForSeconds(cooldownTime);
        isActive = false;
    }
}
