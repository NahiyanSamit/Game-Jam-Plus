using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    [Header("Settings")]
    public int health = 3; 

    [Header("Effects")]
    public GameObject hitEffect;   
    public GameObject breakEffect; 

    [Header("Loot")]
    public GameObject coinPrefab; // Drag your Coin Prefab here

    public void TakeDamage()
    {
        health--; 
        
        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (breakEffect != null)
            Instantiate(breakEffect, transform.position, Quaternion.identity);
            
        // --- NEW: SPAWN COIN ---
        if (coinPrefab != null)
        {
            // Spawn the coin slightly above the ground so it doesn't fall through
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            Instantiate(coinPrefab, spawnPos, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}