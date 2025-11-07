using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrapTrigger : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("The prefab to spawn when triggered (e.g., the ChasingSaw).")]
    [SerializeField] private GameObject prefabToSpawn;
    [Tooltip("The location where the prefab will be spawned.")]
    [SerializeField] private Transform spawnPoint;

    [Header("Settings")]
    [Tooltip("If true, this trigger will only work once.")]
    [SerializeField] private bool oneTimeUse = true;

    private bool hasBeenTriggered = false;

    private void Start()
    {
        // Ensure the collider is a trigger so the player can pass through it
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (oneTimeUse && hasBeenTriggered)
        {
            return; // Don't do anything if it's a one-time trigger that has already been used
        }

        if (collision.CompareTag("Player"))
        {
            if (prefabToSpawn != null && spawnPoint != null)
            {
                Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
                hasBeenTriggered = true;

                // Disable the trigger object itself after use to be safe
                if (oneTimeUse)
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("TrapTrigger is missing a prefab or spawn point!", gameObject);
            }
        }
    }
}
