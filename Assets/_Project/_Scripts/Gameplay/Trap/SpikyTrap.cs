using System.Collections;
using UnityEngine;

public class SpikyTrap : MonoBehaviour
{
    public enum TrapMode { Static, Retractable }

    [Header("General Settings")]
    [Tooltip("Select the behavior of the trap.")]
    public TrapMode mode = TrapMode.Static;
    [Tooltip("Damage dealt by the trap.")]
    public float damage = 10f;

    [Header("Retractable Mode Settings")]
    [Tooltip("The part of the trap that will move up and down.")]
    public Transform spikeObject;
    [Tooltip("Time in seconds to wait before the spike shoots up.")]
    public float activationDelay = 0.5f;
    [Tooltip("How high the spike will move up from its original position.")]
    public float spikeHeight = 1.0f;
    [Tooltip("Speed at which the spike moves.")]
    public float spikeSpeed = 10f;
    [Tooltip("Time in seconds the spike stays up before retracting.")]
    public float stayUpDuration = 1.0f;

    private Vector3 initialSpikePosition;
    private bool isPlayerInRange = false;
    private bool isTrapActive = false;

    void Start()
    {
        if (mode == TrapMode.Retractable)
        {
            if (spikeObject == null)
            {
                Debug.LogError("Spike Object is not assigned for the Retractable trap. Please assign it in the Inspector.");
                enabled = false;
                return;
            }
            initialSpikePosition = spikeObject.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (mode == TrapMode.Retractable && other.CompareTag("Player") && !isTrapActive)
        {
            isPlayerInRange = true;
            StartCoroutine(ActivateTrap());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (mode == TrapMode.Retractable && other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (mode == TrapMode.Static && collision.gameObject.CompareTag("Player"))
        {
            // In Static mode, deal damage on contact
            // You would call a method on the player to deal damage, for example:
            // collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Debug.Log("Player hit by static trap. Damage: " + damage);
        }
    }

    private IEnumerator ActivateTrap()
    {
        isTrapActive = true;

        // Wait for the activation delay
        yield return new WaitForSeconds(activationDelay);

        // Only proceed if the player is still in range after the delay
        if (isPlayerInRange)
        {
            Vector3 targetPosition = initialSpikePosition + new Vector3(0, spikeHeight, 0);

            // Move spike up
            while (Vector3.Distance(spikeObject.position, targetPosition) > 0.01f)
            {
                spikeObject.position = Vector3.MoveTowards(spikeObject.position, targetPosition, spikeSpeed * Time.deltaTime);
                yield return null;
            }
            spikeObject.position = targetPosition;

            // At this point, the spike is up and can deal damage.
            // We can add a separate collider on the spike itself to handle damage,
            // or check for collision here. For simplicity, we assume a collider on the spike.
            Debug.Log("Spike is up! Dealing damage.");

            // Wait for the specified duration
            yield return new WaitForSeconds(stayUpDuration);

            // Retract spike
            while (Vector3.Distance(spikeObject.position, initialSpikePosition) > 0.01f)
            {
                spikeObject.position = Vector3.MoveTowards(spikeObject.position, initialSpikePosition, spikeSpeed * Time.deltaTime);
                yield return null;
            }
            spikeObject.position = initialSpikePosition;
        }

        isTrapActive = false;
    }
}
