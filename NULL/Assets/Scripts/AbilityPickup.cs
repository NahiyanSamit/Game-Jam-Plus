using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public AbilityType abilityToUnlock; 
    public GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Unlock in GameManager (Backend)
            GameManager.Instance.UnlockAbility(abilityToUnlock);

            // 2. CHECK: Is this the item that unlocks the art?
            if (abilityToUnlock == AbilityType.CharacterArt)
            {
                // Try to find the visual switcher on the player
                PlayerVisualSwitcher visuals = other.GetComponent<PlayerVisualSwitcher>();
                
                if (visuals != null)
                {
                    visuals.UpdateVisuals(true); // Turn on the art!
                }
            }

            // 3. Visual Effects for the pickup itself
            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}