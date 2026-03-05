using UnityEngine;
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

    public int coin;
    public bool isBrightnessUnlocked;

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
            coinText.text = "COINS: " + coin.ToString();
        }
    }

    public bool HasEnoughCoins(int required)
    {
        return coin >= required;
    }

    public void UnlockSound()
    {
        // Ensure the main UI is visible so the button can be seen
        if (mainUIPanel != null && !mainUIPanel.activeSelf)
            mainUIPanel.SetActive(true);

        if (soundButton != null)
        {
            if (soundButton.gameObject != null && !soundButton.gameObject.activeSelf)
                soundButton.gameObject.SetActive(true);

            soundButton.SetUnlocked(true);
            Debug.Log("UIManager: Sound button unlocked and shown.");
        }
        else
        {
            Debug.LogWarning("UIManager: soundButton reference is null.");
        }
    }

    public void UnlockSettings()
    {
        if (settingsButton != null)
            settingsButton.SetUnlocked(true);
        else
            Debug.LogWarning("UIManager: settingsButton reference is null.");
    }

    public void UnlockBrightness()
    {
        isBrightnessUnlocked = true;
        if (brightnessIcon != null)
            brightnessIcon.SetUnlocked(true);
        else
            Debug.LogWarning("UIManager: brightnessIcon reference is null.");
    }

    public void UnlockExit()
    {
        if (exitButton != null)
            exitButton.SetUnlocked(true);
        else
            Debug.LogWarning("UIManager: exitButton reference is null.");
    }
    
    public void LockSound()
    {
        if (soundButton != null)
            soundButton.SetUnlocked(false);
    }

    public void LockSettings()
    {
        if (settingsButton != null)
            settingsButton.SetUnlocked(false);
    }

    public void LockBrightness()
    {
        isBrightnessUnlocked = false;
        if (brightnessIcon != null)
            brightnessIcon.SetUnlocked(false);
    }

    public void LockExit()
    {
        if (exitButton != null)
            exitButton.SetUnlocked(false);
    }

}