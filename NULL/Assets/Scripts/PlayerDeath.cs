using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private Animator animator;
    private Health health;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();

        if (animator == null)
            Debug.LogError("Animator not found in children!");

        if (health == null)
            Debug.LogError("Health not found in children!");
    }

    void Update()
    {
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (health == null || animator == null) return;

        if (!isDead && health.CurrentHealth <= 0)
        {
            isDead = true;
            animator.SetTrigger("Death");
        }
    }
}