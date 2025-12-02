using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public AbilityType abilityToUnlock; 
    public GameObject pickupEffect;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ------------------------------------------------------------
            // 1. Unlock in GameManager (Backend logic)
            // ------------------------------------------------------------
            // This handles Jump, Camera, Punch, etc. automatically!
            if (GameManager.Instance != null)
                GameManager.Instance.UnlockAbility(abilityToUnlock);

            // ------------------------------------------------------------
            // 2. Add Fun Points
            // ------------------------------------------------------------
            if (FunManager.Instance != null)
                FunManager.Instance.AddFun(10f);

            // ------------------------------------------------------------
            // 3. Update Visuals (Art, Texture, Animation)
            // ------------------------------------------------------------
            PlayerVisualSwitcher visuals = other.GetComponent<PlayerVisualSwitcher>();

            if (visuals != null)
            {
                if (abilityToUnlock == AbilityType.CharacterArt) visuals.UnlockArtModel(); 
                if (abilityToUnlock == AbilityType.Texture) visuals.UnlockTexture();
                if (abilityToUnlock == AbilityType.Animation) visuals.UnlockAnimationModel();
            }

            // ------------------------------------------------------------
            // 4. Enable the Main UI Panel
            // ------------------------------------------------------------
            if (abilityToUnlock == AbilityType.UI)
            {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.EnableGameUI();
                }
            }

            // ------------------------------------------------------------
            // 5. NEW: Enable Sound Controls
            // ------------------------------------------------------------
            if (abilityToUnlock == AbilityType.Sound)
            {
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.UnlockSoundControl();
                }
                else
                {
                    Debug.LogWarning("SoundManager is missing from the scene!");
                }
            }
            // ------------------------------------------------------------
            // 5. NEW: Enable Brightness Controls
            // ------------------------------------------------------------
            if (abilityToUnlock == AbilityType.Brightness)
            {
                if (BrightnessManager.Instance != null)
                {
                    BrightnessManager.Instance.BrightnessIconActive();

                }
                else
                {
                    Debug.LogWarning("SoundManager is missing from the scene!");
                }
            }


            // ------------------------------------------------------------
            // 6. Effect & Destroy
            // ------------------------------------------------------------
            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

        // SOUND ability
        if (abilityToUnlock == AbilityType.Sound)
        {
            UIManager.Instance.UnlockSound();
        }

        // SETTINGS ability
        if (abilityToUnlock == AbilityType.Settings)
        {
            UIManager.Instance.UnlockSettings();
        }

        // EXIT ability
        if (abilityToUnlock == AbilityType.Exit)
        {
            UIManager.Instance.UnlockExit();
        }

    }

}