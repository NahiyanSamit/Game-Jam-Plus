using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Drag your Main UI Panel here")]
    public GameObject mainUIPanel;

    void Awake()
    {
        // Singleton setup so we can call it from anywhere
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 1. Hide the UI when the game starts
        if (mainUIPanel != null)
        {
            mainUIPanel.SetActive(false);
        }
    }

    // 2. Function to turn it on
    public void EnableGameUI()
    {
        if (mainUIPanel != null)
        {
            mainUIPanel.SetActive(true);
            Debug.Log("UI Enabled!");
        }
    }
}