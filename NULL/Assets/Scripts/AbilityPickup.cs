using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public AbilityType abilityToUnlock; 
    public GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.UnlockAbility(abilityToUnlock);
            
            PlayerVisualSwitcher visuals = other.GetComponent<PlayerVisualSwitcher>();

            if (visuals != null)
            {
                // 1. Unlock Art Model
                if (abilityToUnlock == AbilityType.CharacterArt)
                {
                    visuals.UpdateVisuals(true); 
                }
                
                // 2. Unlock Texture
                if (abilityToUnlock == AbilityType.Texture)
                {
                    visuals.UpdateTexture(true);
                }

                // 3. Unlock Animation (NEW)
                if (abilityToUnlock == AbilityType.Animation)
                {
                    visuals.EnableAnimation(true);
                }
            }

            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}