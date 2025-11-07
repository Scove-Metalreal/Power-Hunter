using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HealingZone : MonoBehaviour
{
    [Tooltip("How many health points are restored per second.")]
    public float healPerSecond = 20f;

    [Tooltip("The maximum health a player can have.")]
    public float maxHealth = 100f;

    private PlayerStat playerToHeal;

    private void Start()
    {
        // Ensure the collider is set to be a trigger automatically.
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // When player enters the zone, get their PlayerStat component.
        // Using GetComponentInParent for safety, in case the collider is on a child object.
        if (collision.CompareTag("Player"))
        {
            playerToHeal = collision.GetComponentInParent<PlayerStat>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // When player leaves, clear the reference so they stop healing.
        if (collision.CompareTag("Player"))
        {
            playerToHeal = null;
        }
    }

    private void Update()
    {
        // If a player is currently in the zone, heal them over time.
        if (playerToHeal != null)
        {
            // Add health scaled by the time that has passed since the last frame.
            playerToHeal.HeathPlayer += healPerSecond * Time.deltaTime;

            // Clamp the health so it doesn't go over the maximum value.
            if (playerToHeal.HeathPlayer > maxHealth)
            {
                playerToHeal.HeathPlayer = maxHealth;
            }
        }
    }
}
