using UnityEngine;

public class DeathPanelUI : MonoBehaviour
{
    public static DeathPanelUI Instance;

    [Header("Respawn Point")]
    public Transform respawnPoint;   // assign in Inspector

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

        // 🔻 Ability penalty
        if (GameManager.Instance != null)
            GameManager.Instance.RemoveRandomAbilities(5);

        // ✅ Safe player restore (NO custom methods)
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
    }
}