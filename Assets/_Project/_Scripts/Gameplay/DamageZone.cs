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
        // Khi hurtbox của người chơi đi vào, lấy component PlayerStat
        if (collision.GetComponent<PlayerHurtbox>() != null)
        {
            playerToDamage = collision.GetComponentInParent<PlayerStat>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Khi hurtbox của người chơi rời đi, xóa tham chiếu
        if (collision.GetComponent<PlayerHurtbox>() != null)
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
