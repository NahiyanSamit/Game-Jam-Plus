using UnityEngine;
using UnityEngine.SceneManagement;

public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager Instance;

    [Range(0f, 2f)]
    [SerializeField] private float brightness = 1f;
    private Light[] directionalLights;

    public float Brightness => brightness;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindDirectionalLights();
        ApplyBrightness();
    }

    private void FindDirectionalLights()
    {
        Light[] allLights = FindObjectsOfType<Light>();
        directionalLights = System.Array.FindAll(allLights, l => l.type == LightType.Directional);
    }

    public void SetBrightness(float value)
    {
        brightness = value;
        ApplyBrightness();
    }

    private void ApplyBrightness()
    {
        if (directionalLights == null) return;

        foreach (var light in directionalLights)
        {
            if (light != null)
                light.intensity = brightness;
        }
    }
}