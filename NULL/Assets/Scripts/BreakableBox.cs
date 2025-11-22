using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    [Header("Settings")]
    public int health = 3; // Set this to 3 in Inspector

    [Header("Effects")]
    public GameObject hitEffect;   // Plays every time you punch (Dust/Sparks)
    public GameObject breakEffect; // Plays only when it finally breaks (Explosion)

    // New function: We call this "TakeDamage" instead of just "Smash"
    public void TakeDamage()
    {
        health--; // Decrease health by 1
        
        // Visual Feedback: Play hit effect if we have one
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // Check if dead
        if (health <= 0)
        {
            Die();
        }
        else
        {
            Debug.Log("Box Hit! Health left: " + health);
        }
    }

    void Die()
    {
        // Spawn explosion effect
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        // Destroy the box
        Destroy(gameObject);
    }
}