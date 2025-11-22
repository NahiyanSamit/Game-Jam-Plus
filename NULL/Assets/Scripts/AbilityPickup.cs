using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public AbilityType abilityToUnlock; 
    public GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Unlock in GameManager
            GameManager.Instance.UnlockAbility(abilityToUnlock);

            // 2. Update Visuals
            FunManager.Instance.AddFun(5f);
            PlayerVisualSwitcher visuals = other.GetComponent<PlayerVisualSwitcher>();

            if (visuals != null)
            {
                // A. Unlock Static Model
                if (abilityToUnlock == AbilityType.CharacterArt)
                {
                    visuals.UnlockArtModel(); 
                }
                
                // B. Unlock Texture
                if (abilityToUnlock == AbilityType.Texture)
                {
                    visuals.UnlockTexture();
                }

                // C. Unlock Animation (Main Character)
                if (abilityToUnlock == AbilityType.Animation)
                {
                    visuals.UnlockAnimationModel();
                }
            }

            // 3. Pickup Effect
            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}