using UnityEngine;
using UnityEngine.UI;

public class BrightnessUi : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;
    private BrightnessManager brightnessManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        brightnessManager = FindObjectOfType<BrightnessManager>();

        // Listen to slider change
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
    }

    // Called when brightness icon is clicked
    void OnBrightnessChanged(float value)
    {
        // 🔒 Check ability before applying
        if (!GameManager.Instance.HasAbility(AbilityType.Brightness))
        {
            MessageManager.Instance.ShowMessage("Item Not Found", 1.2f);
            brightnessSlider.value = brightnessManager.Brightness; // reset
            return;
        }

        brightnessManager.SetBrightness(value);
    }
}
