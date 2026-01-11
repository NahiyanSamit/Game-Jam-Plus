using UnityEngine;
using System.Collections;

public class PlayerDeath : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public Animator animator;
    public DeathContinuePanel deathPanel;
    public Transform respawnPoint;

    [Header("Settings")]
    public float panelDelay = 3f;

    private Health health;
    private bool isDead;

    void Awake()
    {
        health = GetComponent<Health>();

        if (!playerController)
            playerController = GetComponent<PlayerController>();

        if (!animator)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (isDead || health == null) return;

        if (health.CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        playerController.enabled = false;
        animator.SetTrigger("Death");

        StartCoroutine(OpenPanel());
    }

    IEnumerator OpenPanel()
    {
        yield return new WaitForSeconds(panelDelay);
        deathPanel.Show();
    }

    // UI BUTTON → CONTINUE
    public void ContinueGame()
    {
        // 1. Lose abilities
        GameManager.Instance.LoseRandomAbilities();

        // 2. Respawn
        health.ResetHealth();
        transform.position = respawnPoint.position;

        // 3. APPLY ability changes to player
        playerController.ApplyAbilitiesFromGameManager();

        // 4. Resume
        playerController.enabled = true;
        deathPanel.Hide();
        isDead = false;
    }


    // UI BUTTON → QUIT
    public void QuitGame()
    {
        Application.Quit();
    }
}