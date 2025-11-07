using UnityEngine;

// This script should be placed on a child GameObject of the player.
// This child GameObject must have a Collider2D set as a trigger,
// representing the player's "hurtbox" (the area that can take damage).
public class PlayerHurtbox : MonoBehaviour
{
    // Reference to the main collision handler on the parent object.
    private PlayerCollision playerCollisionHandler;

    void Start()
    {
        // Find the PlayerCollision script on the parent GameObject.
        playerCollisionHandler = GetComponentInParent<PlayerCollision>();
        if (playerCollisionHandler == null)
        {
            Debug.LogError("PlayerHurtbox could not find PlayerCollision script on parent object!", gameObject);
        }

        // Ensure the collider on this object is a trigger.
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError("PlayerHurtbox requires a Collider2D component on the same GameObject.", gameObject);
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning("The collider on PlayerHurtbox should be set to 'Is Trigger'. Forcing it now.", gameObject);
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the handler is not found, do nothing.
        if (playerCollisionHandler == null) return;

        // --- DAMAGE LOGIC MOVED FROM PlayerCollision.cs ---
        Debug.Log($"Player Hurtbox triggered with GameObject: '{collision.gameObject.name}' which has tag: '{collision.tag}'");

        // Xử lý va chạm với vùng chết (DeathZone).
        if (collision.CompareTag("DeathZone"))
        {
            playerCollisionHandler.HandleDamageAndKnockback(20f, collision.transform);
        }

        if (collision.CompareTag("Minus100Heath"))
        {
            playerCollisionHandler.HandleDamageAndKnockback(100f, collision.transform);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayLava();
        }

        // Khi người chơi va chạm với hitbox của kẻ địch.
        if (collision.CompareTag("EnemyHitbox"))
        {
            Hitbox hitbox = collision.GetComponent<Hitbox>();
            if (hitbox != null)
            {
                Debug.Log("<color=cyan>HURTBOX: Player collided with EnemyHitbox!</color>");
                playerCollisionHandler.HandleDamageAndKnockback(hitbox.damage, collision.transform);
            }
            else
            {
                Debug.LogWarning("Found 'EnemyHitbox' tag but no Hitbox component on: " + collision.gameObject.name);
            }
        }

        // Va chạm với bẫy gai
        if (collision.CompareTag("SpikyTrap"))
        {
            SpikyTrap trap = collision.GetComponentInParent<SpikyTrap>();
            if (trap != null)
            {
                playerCollisionHandler.HandleDamageAndKnockback(trap.damage, collision.transform);
                if (AudioManager.Instance != null) AudioManager.Instance.PlayTrap();
            }
            else
            {
                Debug.LogError($"GameObject '{collision.gameObject.name}' has the 'SpikyTrap' tag, but the 'SpikyTrap.cs' script is missing!");
            }
        }
    }
}
