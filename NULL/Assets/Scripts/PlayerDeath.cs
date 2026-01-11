using UnityEngine;
using System.Collections;
using SmallHedge.SoundManager;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private AudioClip deathSound;

    private Animator animator;
    private Health health;
    private PlayerController controller;
    private bool isDead = false;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isDead) return;

        if (health != null && health.CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (SoundManager.Instance != null && deathSound != null)
            SoundManager.Instance.PlaySFX(deathSound);

        if (animator != null)
            animator.SetTrigger("Death");

        // ✅ NO DisableControls() call
        controller.enabled = false;

        StartCoroutine(DeathDelay());
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(3f);
        DeathPanelUI.Instance.Show();
    }
}