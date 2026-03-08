using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Health health;

    [SerializeField] private bool isPlayer;
    [SerializeField][Range(1f,40f)] private float fillSmoothSpeed = 8f;

    private float targetFill;

    public void SetHealth(Health newHealth)
    {
        health = newHealth;
        UpdateHealthBarInstant();
    }

    void Update()
    {
        if (health == null) return;

        UpdateHealthBar();

        // destroy health bar when enemy dies
        if (!isPlayer && health.CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdateHealthBar()
    {
        if (fillImage == null) return;

        targetFill = Mathf.Clamp01((float)health.CurrentHealth / health.MaxHealth);

        float next = Mathf.MoveTowards(fillImage.fillAmount, targetFill, Time.deltaTime * fillSmoothSpeed);
        fillImage.fillAmount = next;

        UpdateColorByHealth(next);
    }

    void UpdateHealthBarInstant()
    {
        if (health == null || fillImage == null) return;

        float normalized = Mathf.Clamp01((float)health.CurrentHealth / health.MaxHealth);
        fillImage.fillAmount = normalized;

        UpdateColorByHealth(normalized);
    }

    void UpdateColorByHealth(float normalized)
    {
        Color targetColor;

        if (isPlayer)
        {
            if (normalized >= 0.75f) targetColor = Color.green;
            else if (normalized >= 0.5f) targetColor = Color.yellow;
            else if (normalized >= 0.25f) targetColor = new Color(1f,0.65f,0f);
            else targetColor = Color.red;
        }
        else
        {
            if (normalized >= 0.75f) targetColor = Color.red;
            else if (normalized >= 0.5f) targetColor = new Color(1f,0.65f,0f);
            else if (normalized >= 0.25f) targetColor = Color.yellow;
            else targetColor = Color.green;
        }

        fillImage.color = Color.Lerp(fillImage.color, targetColor, Time.deltaTime * fillSmoothSpeed);
    }
}