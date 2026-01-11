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

        // 🔻 Remove abilities
        if (GameManager.Instance != null)
            GameManager.Instance.RemoveRandomAbilities(abilitiesToLose);

        // ✅ Restore player (UNCHANGED)
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
        }

        // ⭐⭐⭐ THIS IS THE ONLY NEW PART ⭐⭐⭐
        AbilityApplier applier = FindFirstObjectByType<AbilityApplier>();
        if (applier != null)
            applier.ApplyAbilities();
    }
}