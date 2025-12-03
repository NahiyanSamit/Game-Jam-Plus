using System;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<AbilityType> unlockedAbilities = new List<AbilityType>();

    [Header("Economy")]
    public int coinCount = 0;
    public int gunPrice = 50; // Gun costs 5 coins
    [SerializeField] private GameObject rifle;
    void Awake()
    {
        if(rifle == null)
            rifle = GameObject.FindGameObjectWithTag("Rifle");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
       
    }

    private void Update()
    {
        if (rifle == null)
        {
            rifle = GameObject.FindGameObjectWithTag("Rifle");

        }


    }

    public void UnlockAbility(AbilityType ability)
    {
        if (!unlockedAbilities.Contains(ability))
        {
            unlockedAbilities.Add(ability);
        }
    }

    public bool HasAbility(AbilityType ability)
    {
        return unlockedAbilities.Contains(ability);
    }

    // --- NEW: COIN SYSTEM ---
    public void AddCoin(int amount)
    {
        coinCount += amount;
        Debug.Log("Coins: " + coinCount);
        
        // Update UI
        if (UIManager.Instance != null) 
            UIManager.Instance.UpdateCoinDisplay(coinCount);
    }

    // --- NEW: SHOP SYSTEM ---
    public void BuyGun()
    {
        if (coinCount >= gunPrice)
        {
            rifle.gameObject.GetComponent<BoxCollider>().enabled = true;
            coinCount -= gunPrice;
            UnlockAbility(AbilityType.Gun);
            
            if (UIManager.Instance != null) 
                UIManager.Instance.UpdateCoinDisplay(coinCount);
                
            Debug.Log("Gun Purchased!");
        }
        else
        {
            rifle.gameObject.GetComponent<Collider>().isTrigger= false;
            rifle.gameObject.GetComponent<BoxCollider>().enabled = false;
            Debug.Log("Not enough coins! Need " + gunPrice);
            
        }
    }
}