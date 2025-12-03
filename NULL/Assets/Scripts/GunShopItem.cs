using UnityEngine;

public class GunShopItem : MonoBehaviour
{
    [Header("Price Settings")]
    public int gunPrice = 50;
    public AbilityType abilityToUnlock = AbilityType.Gun;

    [Header("Shop Visuals")]
    public GameObject tableGunModel; // Drag the gun mesh ON THE TABLE here

    private void OnTriggerEnter(Collider other)
    {
        // We check if the PLAYER touched the table trigger
        if (other.CompareTag("Player"))
        {
            // 1. Check if we already have the gun (Don't buy it twice!)
            if (GameManager.Instance.HasAbility(abilityToUnlock))
            {
                Debug.Log("You already own the gun!");
                return;
            }

            // 2. Check if we have 50 coins
            if (GameManager.Instance.coinCount >= gunPrice)
            {
                GetComponent<Collider>().isTrigger = true; 
                BuyGun();
            }
            else
            {
               GetComponent<Collider>().isTrigger = false; 
                Debug.Log("Not enough cash! Need " + gunPrice);
                
                
            }
        }
    }

    void BuyGun()
    {
        // A. Deduct 50 Coins
        GameManager.Instance.AddCoin(-gunPrice);

        // B. Tell GameManager the gun is unlocked
        // (Your PlayerController in the Main Scene is listening for this!)
        GameManager.Instance.UnlockAbility(abilityToUnlock);

        // C. Hide the gun ON THE TABLE (because we bought it)
        if (tableGunModel != null)
        {
            tableGunModel.SetActive(false);
        }

        // D. Disable this trigger so we can't buy it again
        GetComponent<Collider>().enabled = false;

        Debug.Log("Gun Purchased! Player should see it now.");
    }
}