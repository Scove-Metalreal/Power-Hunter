using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageZone : MonoBehaviour
{
    [Tooltip("How many damage points are dealt per second.")]
    public float damagePerSecond = 10f;

    private PlayerStat playerToDamage;

    private void Start()
    {
        // Ensure the collider is set to be a trigger automatically.
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // When player enters the zone, get their PlayerStat component.
        if (collision.CompareTag("Player"))
        {
            playerToDamage = collision.GetComponentInParent<PlayerStat>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // When player leaves, clear the reference so they stop taking damage.
        if (collision.CompareTag("Player"))
        {
            playerToDamage = null;
        }
    }

    private void Update()
    {
        // If a player is currently in the zone, deal damage over time.
        if (playerToDamage != null)
        {
            // Call the TakeDamage function from the PlayerStat script, scaled by time.
            playerToDamage.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}
