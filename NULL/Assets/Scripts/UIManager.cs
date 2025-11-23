using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject mainUIPanel; // Drag your 'SoundPanel' or 'GamePanel' here
    public IconButton soundButton;
    public IconButton settingsButton;
    public IconButton exitButton;

    [Header("Text Elements")]
    public TMP_Text coinText; // <--- NEW: Drag your Coin Text object here

    public int coin = 0;
    void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 1. Hide the UI when the game starts
        if (mainUIPanel != null)
        {
            mainUIPanel.SetActive(false);
        }

        // 2. Initialize Coin Text to 0
        UpdateCoinDisplay(0);

        exitButton.AssignAction(ExitGame);
    }

    // Function to turn the UI on (called by AbilityPickup)
    public void EnableGameUI()
    {
        if (mainUIPanel != null)
        {
            mainUIPanel.SetActive(true);
            Debug.Log("UI Enabled!");
        }
    }

    // --- NEW: UPDATE COIN TEXT ---
    // Called by GameManager whenever we get money or buy a gun
    public void UpdateCoinDisplay(int coins)
    {
        if (coinText != null)
        {
            coin += coins;
            coinText.text = "COINS: " + coins.ToString();
        }
    }

    public bool HasEnoughCoins(int required)
    {
        return coin >= required;
    }

    public void ExitGame()
    {
        Debug.Log("GameExit");
        Application.Quit();
    }

    public void UnlockSound()
    {
        soundButton.SetUnlocked(true);
    }

    public void UnlockSettings()
    {
        settingsButton.SetUnlocked(true);
    }

    public void UnlockExit()
    {
        exitButton.SetUnlocked(true);
    }

}