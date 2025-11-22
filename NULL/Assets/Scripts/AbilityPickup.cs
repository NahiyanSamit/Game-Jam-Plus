using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public AbilityType abilityToUnlock; 
    public GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Unlock in GameManager (Backend logic)
            if (GameManager.Instance != null)
                GameManager.Instance.UnlockAbility(abilityToUnlock);

            // 2. Add Fun Points
            if (FunManager.Instance != null)
                FunManager.Instance.AddFun(5f);

            // 3. Update Visuals (Art, Texture, Animation)
            PlayerVisualSwitcher visuals = other.GetComponent<PlayerVisualSwitcher>();

            if (visuals != null)
            {
                if (abilityToUnlock == AbilityType.CharacterArt) visuals.UnlockArtModel(); 
                if (abilityToUnlock == AbilityType.Texture) visuals.UnlockTexture();
                if (abilityToUnlock == AbilityType.Animation) visuals.UnlockAnimationModel();
            }

            // ------------------------------------------------------------
            // 4. NEW: Enable the UI Panel inside the current scene
            // ------------------------------------------------------------
            if (abilityToUnlock == AbilityType.UI)
            {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.EnableGameUI();
                }
                else
                {
                    Debug.LogError("No UIManager found in the scene! Did you create it?");
                }
            }

            // 5. Pickup Effect
            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}