using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerStat playerStat;

    [Header("Knockback Settings")]
    public float KnockBackSpeed = 10f;

    [Header("Player Lives")]
    public int lifeCount = 3;
    public float respawnDelay = 1.2f;

    [Header("Shop Settings")]
    public GameObject shopUI;
    private bool isNearShop = false;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerStat = GetComponent<PlayerStat>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GroundTrap"))
        {
            FindAnyObjectByType<FallingGround>().TriggerCollapse();
            FindAnyObjectByType<CameraShake>().StartCoroutine(FindAnyObjectByType<CameraShake>().Shake());
        }

        if (collision.CompareTag("DeathZone"))
        {
            HandleDamageAndKnockback(20f, collision.transform);
        }

        if (collision.CompareTag("EnemyHitbox"))
        {
            Hitbox hitbox = collision.GetComponent<Hitbox>();
            if (hitbox != null)
            {
                HandleDamageAndKnockback(hitbox.damage, collision.transform);
            }
        }

        if (collision.CompareTag("SpikeTrap"))
        {
            SpikeTrap spikeTrap = collision.GetComponent<SpikeTrap>();
            if (spikeTrap != null)
            {
                spikeTrap.ActiceSpikeTrap();
            }
        }

        if (collision.gameObject.CompareTag("Gate"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (collision.CompareTag("Shop"))
        {
            isNearShop = true;
            Transform pressEInShop = collision.transform.Find("Canvas");
            if (pressEInShop != null)
            {
                pressEInShop.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Shop"))
        {
            isNearShop = false;
            Transform pressEInShop = collision.transform.Find("Canvas");
            if (pressEInShop != null)
            {
                pressEInShop.gameObject.SetActive(false);
            }
            if (shopUI != null) shopUI.SetActive(false);
        }
    }

    private void HandleDamageAndKnockback(float damage, Transform damageSource)
    {
        playerStat.TakeDamage(damage);

        if (playerController != null) playerController.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce((transform.position - damageSource.position).normalized * KnockBackSpeed, ForceMode2D.Impulse);
        }

        StartCoroutine(RecoverFromKnockback(0.4f));

        if (playerStat.HeathPlayer <= 0)
        {
            lifeCount--; // This seems to be old logic, PlayerStat now handles lives. You may want to remove this.
            if (playerStat.CurrentLives > 0) // Using the new lives system from PlayerStat
            {
                StartCoroutine(RespawnPlayer());
            }
            else
            {
                PlayerDead();
            }
        }
    }

    IEnumerator RecoverFromKnockback(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerController != null) playerController.enabled = true;
    }

    IEnumerator RespawnPlayer()
    {
        this.enabled = false;
        if (playerController != null) playerController.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // This should also be handled by a central manager, but we'll leave it for now.
        Vector3 respawnPos = CheckPoint.GetRespawnPosition();
        transform.position = respawnPos;
        // Health is now reset inside PlayerStat's HandlePlayerDeath method.

        if (playerController != null)
        {
            playerController.animator.SetBool("isDead", false);
            playerController.enabled = true;
        }
        this.enabled = true;
    }

    void PlayerDead()
    {
        if (playerController.animator != null)
        {
            playerController.animator.SetBool("isDead", true);
        }
        this.enabled = false;
        if (playerController != null) playerController.enabled = false;
    }

    // --- ANIMATION EVENT ---
    public void StartDeadAnimation()
    {
        Time.timeScale = 0f;
    }

    // --- ANIMATION EVENT ---
    public void EndDeadAnimation()
    {
        // Destroy the player object
        Destroy(gameObject);

        // Simply tell the GameManager to handle the game over state
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowGameOverUI();
        }
        else
        {
            Debug.LogError("GameManager instance not found when trying to show Game Over UI!");
        }
    }

    void Update()
    {
        if (isNearShop && Input.GetKeyDown(KeyCode.E))
        {
            if (shopUI != null)
            {
                shopUI.SetActive(!shopUI.activeSelf);
            }
        }
    }
}
