using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<AbilityType> unlockedAbilities = new List<AbilityType>();

    [Header("Permanent Abilities (Will NOT be lost)")]
    public List<AbilityType> permanentAbilities = new List<AbilityType>()
    {
        AbilityType.Jump,
        AbilityType.Camera,
        AbilityType.UI,
        AbilityType.Settings
    };

    private AbilityType _currentWeapon;

    [Header("Economy")]
    public int coinCount = 0;
    [SerializeField] public int gunPrice = 50;
    [SerializeField] private GameObject rifle;

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
            return;
        }

        if (rifle == null)
            rifle = GameObject.FindGameObjectWithTag("Rifle");
    }

    void Update()
    {
        if (rifle == null)
            rifle = GameObject.FindGameObjectWithTag("Rifle");
    }

    // ================= ABILITY =================

    public void UnlockAbility(AbilityType ability)
    {
        if (!unlockedAbilities.Contains(ability))
        {
            if (ability == AbilityType.Gun && coinCount < gunPrice)
                return;

            unlockedAbilities.Add(ability);
            Debug.Log("Unlocked: " + ability);
        }
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

    public bool IsPermanent(AbilityType ability)
    {
        return permanentAbilities.Contains(ability);
    }

    // ================= WEAPON =================

    public void ChangeWeapon(AbilityType weapon)
    {
        if (HasAbility(weapon))
            _currentWeapon = weapon;
    }

    public AbilityType GetCurrentWeapon()
    {
        return _currentWeapon;
    }

    // ================= COIN =================

    public void AddCoin(int amount)
    {
        coinCount += amount;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateCoinDisplay(coinCount);
    }

    // ================= SHOP =================

    public void BuyGun()
    {
        if (coinCount < gunPrice)
            return;

        coinCount -= gunPrice;
        UnlockAbility(AbilityType.Gun);

        if (rifle != null)
            rifle.GetComponent<BoxCollider>().enabled = true;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateCoinDisplay(coinCount);
    }

    // ================= DEATH PENALTY =================

    public void RemoveRandomAbilities(int count)
    {
        List<AbilityType> removable = new List<AbilityType>();

        foreach (var ability in unlockedAbilities)
        {
            if (!IsPermanent(ability))
                removable.Add(ability);
        }

        int removeCount = Mathf.Min(count, removable.Count);

        for (int i = 0; i < removeCount; i++)
        {
            int index = Random.Range(0, removable.Count);
            AbilityType removed = removable[index];

            unlockedAbilities.Remove(removed);
            removable.RemoveAt(index);

            Debug.Log("Lost Ability: " + removed);
        }

        if (!HasAbility(AbilityType.Gun))
            _currentWeapon = AbilityType.Punch;
    }
}
