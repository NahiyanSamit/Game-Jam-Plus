using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Health health;
    // Optional: assign the UI Slider component from your HealthBar Canvas (not the Canvas itself)
    [SerializeField] private Slider slider;
    // Optional container: you can drag the Canvas GameObject here and the script will try to find a Slider inside it
    [SerializeField] private GameObject sliderContainer;

    // If this health bar is for the player, enable color changes (green -> yellow -> orange -> red)
    [SerializeField] private bool isPlayer;

    // Smooth animation speed for the fill and color
    [SerializeField][Range(1f, 40f)] private float fillSmoothSpeed = 8f;

    private float _targetFill = 1f;

    void Start()
    {
        if (health == null)
            health = GetComponentInParent<Health>();

        // If a container (Canvas or parent GameObject) was assigned but not the Slider itself, try to find the Slider inside it
        if (slider == null && sliderContainer != null)
        {
            slider = sliderContainer.GetComponent<Slider>();
            if (slider == null)
                slider = sliderContainer.GetComponentInChildren<Slider>();
        }


        UpdateHealthBarInstant();
    }

    void Update()
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (health == null || fillImage == null) return;

        _targetFill = Mathf.Clamp01((float)health.CurrentHealth / health.MaxHealth);

        // Smoothly animate fill amount toward the target
        float current = fillImage.fillAmount;
        float next = Mathf.MoveTowards(current, _targetFill, Time.deltaTime * fillSmoothSpeed);
        fillImage.fillAmount = next;

        // Update optional Slider (attach the Slider component from the Canvas to the `slider` field)
        if (slider != null)
        {
            slider.maxValue = health.MaxHealth;
            slider.value = health.CurrentHealth;
        }

        // If this is the player's bar, update color according to thresholds
        // Now we pass isPlayer so UpdateColorByHealth can invert mapping for enemies
        UpdateColorByHealth(next, isPlayer);
    }

    // Instant set (used on Start or when you need an immediate refresh)
    void UpdateHealthBarInstant()
    {
        if (health == null || fillImage == null) return;
        float normalized = Mathf.Clamp01((float)health.CurrentHealth / health.MaxHealth);
        fillImage.fillAmount = normalized;

        if (slider != null)
        {
            slider.maxValue = health.MaxHealth;
            slider.value = health.CurrentHealth;
        }

        UpdateColorByHealth(normalized, isPlayer);
    }

    void UpdateColorByHealth(float normalized, bool isPlayerBar)
    {
        // For player: thresholds: >=75% green, >=50% yellow, >=25% orange, else red
        // For enemy: inverse -> >=75% red, >=50% orange, >=25% yellow, else green
        Color targetColor;
        if (isPlayerBar)
        {
            if (normalized >= 0.75f)
                targetColor = Color.green;
            else if (normalized >= 0.5f)
                targetColor = Color.yellow;
            else if (normalized >= 0.25f)
                targetColor = new Color(1f, 0.65f, 0f); // orange-ish
            else
                targetColor = Color.red;
        }
        else
        {
            if (normalized >= 0.75f)
                targetColor = Color.red;
            else if (normalized >= 0.5f)
                targetColor = new Color(1f, 0.65f, 0f); // orange-ish
            else if (normalized >= 0.25f)
                targetColor = Color.yellow;
            else
                targetColor = Color.green;
        }

        // Smoothly interpolate the fill color toward the target color
        fillImage.color = Color.Lerp(fillImage.color, targetColor, Time.deltaTime * (fillSmoothSpeed * 0.5f));
    }

    // Public helper so other systems can force an immediate refresh
    public void Refresh()
    {
        UpdateHealthBarInstant();
    }
}