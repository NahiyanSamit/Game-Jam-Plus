using SmallHedge.SoundManager;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
   [SerializeField] private Animator animator;
    private Health health;
    private bool isDead = false;
    private PlayerController playerController;
    void Awake()
    {
       
        animator = GetComponentInChildren<Animator>();
        playerController = GetComponentInParent<PlayerController>();
        health = GetComponent<Health>();

        if (animator == null)
            Debug.LogError("Animator not found in children!");
        if(health == null) return;
        if (health == null)
            Debug.LogError("Health not found in children!");
    }

    void Update()
    {
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (health == null || animator == null || isDead) return;

        if (!isDead && health.CurrentHealth <= 0)
        {
            isDead = true;
            SmallHedge.SoundManager.SoundManager.PlaySound(SoundType.PLAYERDEATH);
            animator.SetTrigger("Death");
            playerController.Respawn();
        }
    }
}