using System;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private EnemyAI enemyAI;

    private void Awake()
    {
        currentHealth = maxHealth;
        enemyAI = GetComponent<EnemyAI>();
    }

    void takeDamage(float damage)
    {
        if (currentHealth <= 0) return; // Nếu đã chết thì không nhận thêm sát thương
        
        currentHealth -= damage;

        if (currentHealth > 0)
        {
            // còn sống
            enemyAI.TriggerHurtState();
        }
        else
        {
            // hết máu
            enemyAI.TriggerDeathState();
        }
    }
}
