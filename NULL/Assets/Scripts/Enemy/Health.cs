using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    
    void Awake()
    {
        ResetHealth(); // cleaner init
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    // ✅ REQUIRED BY PlayerDeath
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}