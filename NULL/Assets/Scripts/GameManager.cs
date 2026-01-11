using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Abilities")]
    public List<AbilityType> unlockedAbilities = new List<AbilityType>();

    [Header("Weapon")]
    [SerializeField] private AbilityType currentWeapon = AbilityType.None;

    [Header("Economy")]
    public int coinCount = 0;
    public int gunPrice = 50;

    [Header("Death Penalty")]
    public int abilityLossCount = 2;

    public List<AbilityType> permanentAbilities = new List<AbilityType>
    {
        AbilityType.Jump,
        AbilityType.Camera,
        AbilityType.CharacterArt,
        AbilityType.Texture,
        AbilityType.Animation
    };

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

    // -------- Ability --------
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

    // -------- Weapon --------
    public void ChangeWeapon(AbilityType weapon)
    {
        if (weapon == AbilityType.None)
        {
            currentWeapon = AbilityType.None;
            return;
        }

        if (HasAbility(weapon))
        {
            currentWeapon = weapon;
            Debug.Log("Equipped: " + weapon);
        }
    }

    public AbilityType GetCurrentWeapon()
    {
        return currentWeapon;
    }

    // -------- Economy --------
    public void AddCoin(int amount)
    {
        coinCount += amount;
        UIManager.Instance?.UpdateCoinDisplay(coinCount);
    }

    public void BuyGun()
    {
        if (HasAbility(AbilityType.Gun)) return;
        if (coinCount < gunPrice) return;

        coinCount -= gunPrice;
        UnlockAbility(AbilityType.Gun);
        ChangeWeapon(AbilityType.Gun);

        UIManager.Instance?.UpdateCoinDisplay(coinCount);
    }

    // -------- Death Ability Loss --------
    public void LoseRandomAbilities()
    {
        List<AbilityType> removable = new List<AbilityType>();

        foreach (var a in unlockedAbilities)
        {
            if (!permanentAbilities.Contains(a))
                removable.Add(a);
        }

        int loss = Mathf.Min(abilityLossCount, removable.Count);

        for (int i = 0; i < loss; i++)
        {
            int index = Random.Range(0, removable.Count);
            AbilityType lost = removable[index];

            removable.RemoveAt(index);
            unlockedAbilities.Remove(lost);

            Debug.Log("Lost ability: " + lost);

            if (currentWeapon == lost)
                currentWeapon = AbilityType.None;
        }
    }
}
