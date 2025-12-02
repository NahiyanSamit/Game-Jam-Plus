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
    }

    // Update is called once per frame
    void Update()
    {
        GetTheSliderValue();
        
    }

    public void GetTheSliderValue()
    {
        // brightnessManager.Brightness = brightnessSlider.value;
        brightnessManager.SetBrightness(brightnessSlider.value);
    }
}
