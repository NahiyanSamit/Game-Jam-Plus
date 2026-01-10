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
    public float showPanelDelay = 3f;

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
            Die();
    }

    void Die()
    {
        isDead = true;

        playerController.enabled = false;

        if (animator)
            animator.SetTrigger("Death");

        StartCoroutine(ShowPanelAfterDelay());
    }

    IEnumerator ShowPanelAfterDelay()
    {
        yield return new WaitForSeconds(showPanelDelay);
        deathPanel.Show();   
    }

    // UI BUTTON → CONTINUE
    public void ContinueGame()
    {
        health.ResetHealth();  

        if (respawnPoint)
            transform.position = respawnPoint.position;

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