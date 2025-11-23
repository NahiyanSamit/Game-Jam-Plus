using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Effects")]
    public GameObject hitEffect;  // Blood/Spark effect
    public GameObject deathEffect; // Explosion/Ragdoll

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Enemy Hit! HP: " + currentHealth);

        // Visual Effect
        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);

        // Check Death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Add coins for killing enemy?
        if (GameManager.Instance != null)
            GameManager.Instance.AddCoin(2); // Reward 2 coins

        Destroy(gameObject);
    }
}