using UnityEngine;

public class DeathPanelUI : MonoBehaviour
{
    public static DeathPanelUI Instance;

    [Header("Respawn Point")]
    public Transform respawnPoint;

    [Header("Death Penalty")]
    [SerializeField] private int abilitiesToLose = 5;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    public void OnContinue()
    {
        Hide();

        if (GameManager.Instance != null)
            GameManager.Instance.RemoveRandomAbilities(abilitiesToLose);

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.enabled = true;

            if (respawnPoint != null)
                player.transform.position = respawnPoint.position;
            else
                player.transform.position = Vector3.up * 2f;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = Vector3.zero;

            // ⭐⭐⭐ RESET HEALTH (THIS FIXES IT) ⭐⭐⭐
            Health health = player.GetComponent<Health>();
            if (health != null)
                health.ResetHealth();
            
            PlayerDeath death = player.GetComponent<PlayerDeath>();
            if (death != null)
                death.ResetDeathState();
            death.ResetAnimator();
        }

        // ⭐ Ensure abilities affect UI / gun / sound
        AbilityApplier applier = FindFirstObjectByType<AbilityApplier>();
        if (applier != null)
            applier.ApplyAbilities();
    }

}