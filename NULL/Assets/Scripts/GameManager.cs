using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Abilities")]
    public List<AbilityType> unlockedAbilities = new List<AbilityType>();

    [Header("Economy")]
    public int coinCount;
    public int gunPrice = 50;

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

    // ================= ABILITIES =================

    public void UnlockAbility(AbilityType ability)
    {
        if (unlockedAbilities.Contains(ability)) return;

        unlockedAbilities.Add(ability);
        Debug.Log("Unlocked Ability: " + ability);
    }

    public void LockAbility(AbilityType ability)
    {
        if (unlockedAbilities.Contains(ability))
            unlockedAbilities.Remove(ability);
    }

    public bool HasAbility(AbilityType ability)
    {
        return unlockedAbilities.Contains(ability);
    }

    // ================= ECONOMY =================

    public void AddCoin(int amount)
    {
        coinCount += amount;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateCoinDisplay(coinCount);

        Debug.Log("Coins: " + coinCount);
    }

    // ================= SHOP =================

    public bool BuyGun()
    {
        if (HasAbility(AbilityType.Gun))
        {
            Debug.Log("Gun already owned.");
            return false;
        }

        if (coinCount < gunPrice)
        {
            Debug.Log("Not enough coins! Need: " + gunPrice);
            return false;
        }

        coinCount -= gunPrice;
        UnlockAbility(AbilityType.Gun);

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateCoinDisplay(coinCount);

        Debug.Log("Gun Purchased! Remaining coins: " + coinCount);
        return true;
    }
}