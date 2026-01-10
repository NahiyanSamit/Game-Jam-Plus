using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Abilities")]
    public List<AbilityType> unlockedAbilities = new List<AbilityType>();

    private AbilityType _currentWeapon = AbilityType.Punch; // default fallback

    [Header("Economy")]
    public int coinCount = 0;
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

    // ================= ABILITY =================

    public void UnlockAbility(AbilityType ability)
    {
        if (!unlockedAbilities.Contains(ability))
        {
            unlockedAbilities.Add(ability);
            Debug.Log("Unlocked: " + ability);
        }
    }

    public bool HasAbility(AbilityType ability)
    {
        return unlockedAbilities.Contains(ability);
    }

    // ================= WEAPON =================

    public void ChangeWeapon(AbilityType weaponType)
    {
        // Only allow weapon abilities here
        if (!HasAbility(weaponType))
        {
            Debug.LogWarning("Weapon not unlocked: " + weaponType);
            return;
        }

        _currentWeapon = weaponType;
        Debug.Log("Equipped weapon: " + _currentWeapon);
    }

    public AbilityType GetCurrentWeapon()
    {
        return _currentWeapon;
    }

    // ================= ECONOMY =================

    public void AddCoin(int amount)
    {
        coinCount += amount;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateCoinDisplay(coinCount);
    }

    public void BuyGun()
    {
        if (coinCount < gunPrice)
        {
            Debug.Log("Not enough coins to buy gun");
            return;
        }

        coinCount -= gunPrice;
        UnlockAbility(AbilityType.Gun);
        ChangeWeapon(AbilityType.Gun);

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateCoinDisplay(coinCount);

        Debug.Log("Gun purchased successfully");
    }

    // ================= PERMANENT ABILITY =================

    public bool IsPermanentAbility(AbilityType ability)
    {
        switch (ability)
        {
            case AbilityType.Jump:
            case AbilityType.Texture:
            case AbilityType.Animation:
            case AbilityType.Camera:
            case AbilityType.CharacterArt:
                return true;

            default:
                return false;
        }
    }

    public void RemoveRandomNonPermanentAbilities(int amount)
    {
        List<AbilityType> removable = unlockedAbilities
            .Where(a => !IsPermanentAbility(a))
            .ToList();

        int removeCount = Mathf.Min(amount, removable.Count);

        for (int i = 0; i < removeCount; i++)
        {
            int index = Random.Range(0, removable.Count);
            AbilityType removed = removable[index];

            unlockedAbilities.Remove(removed);
            removable.RemoveAt(index);

            Debug.Log("Lost ability: " + removed);

            // If gun lost, fallback to punch
            if (removed == AbilityType.Gun)
                _currentWeapon = AbilityType.Punch;
        }
    }
}
