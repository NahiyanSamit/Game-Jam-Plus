using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject mainUIPanel; 
    public IconButton soundButton;
    public IconButton settingsButton;
    public IconButton exitButton;
    public IconButton brightnessIcon;

    [Header("Text Elements")]
    public TMP_Text coinText; 

    public int coin = 0;
    public bool isBrightnessUnlocked = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (mainUIPanel != null)
            mainUIPanel.SetActive(false);

        UpdateCoinDisplay(0);
    }

    public void EnableGameUI()
    {
        if (mainUIPanel != null)
        {
            mainUIPanel.SetActive(true);
            Debug.Log("UI Enabled!");
        }
    }

    // ✅ NEW (REQUIRED)
    public void DisableGameUI()
    {
        if (mainUIPanel != null)
        {
            mainUIPanel.SetActive(false);
            Debug.Log("UI Disabled!");
        }
    }

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

    public void UnlockSound()
    {
        soundButton.SetUnlocked(true);
    }

    public void UnlockSettings()
    {
        settingsButton.SetUnlocked(true);
    }

    public void UnlockBrightness()
    {
        isBrightnessUnlocked = true;
        brightnessIcon.SetUnlocked(true);
    }

    public void UnlockExit()
    {
        exitButton.SetUnlocked(true);
    }
    
    public void LockSound()
    {
        soundButton.SetUnlocked(false);
    }

    public void LockSettings()
    {
        settingsButton.SetUnlocked(false);
    }

    public void LockBrightness()
    {
        isBrightnessUnlocked = false;
        brightnessIcon.SetUnlocked(false);
    }

    public void LockExit()
    {
        exitButton.SetUnlocked(false);
    }

}