using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public AbilityType abilityToUnlock;
    public GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // ================= GUN =================
        if (abilityToUnlock == AbilityType.Gun)
        {
            if (GameManager.Instance == null)
                return;

            GameManager.Instance.BuyGun();

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
                    UIManager.Instance.UnlockBrightness();

                }
                else
                {
                    Debug.LogWarning("SoundManager is missing from the scene!");
                }
            }
            
            if (abilityToUnlock == AbilityType.Punch)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ChangeWeapon(abilityToUnlock);
                    if (!GameManager.Instance.HasAbility(abilityToUnlock))
                    {
                        return;
                    }
                }
                else
                {
                    Debug.LogWarning("SoundManager is missing from the scene!");
                }
            }

            // ------------------------------------------------------------
            // 5. NEW: Buy Gun 
            // ------------------------------------------------------------
            if (abilityToUnlock == AbilityType.Gun)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.BuyGun();
                    GameManager.Instance.ChangeWeapon(abilityToUnlock);
                    if (!GameManager.Instance.HasAbility(abilityToUnlock))
                    {
                        return;
                    }
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

        // 🔥 CRITICAL FIX #1 — EQUIP PUNCH WHEN PICKED
        if (abilityToUnlock == AbilityType.Punch)
        {
            GameManager.Instance.ChangeWeapon(AbilityType.Punch);
        }

        // 🔥 CRITICAL FIX #2 — SYNC PLAYER STATE
        other.GetComponent<PlayerController>()
             ?.ApplyAbilitiesFromGameManager();

        // ================= FUN =================
        FunManager.Instance?.AddFun(10f);

        // ================= VISUAL =================
        PlayerVisualSwitcher visuals =
            other.GetComponent<PlayerVisualSwitcher>();

        if (visuals != null)
        {
            if (abilityToUnlock == AbilityType.CharacterArt)
                visuals.UnlockArtModel();
            if (abilityToUnlock == AbilityType.Texture)
                visuals.UnlockTexture();
            if (abilityToUnlock == AbilityType.Animation)
                visuals.UnlockAnimationModel();
        }

        // BRIGHTNESS ability
        if (abilityToUnlock == AbilityType.Brightness)
        {
            UIManager.Instance.UnlockBrightness();
        }

        // EXIT ability
        if (abilityToUnlock == AbilityType.Exit)
        {
            case AbilityType.UI:
                UIManager.Instance?.EnableGameUI();
                break;

            case AbilityType.Sound:
                UIManager.Instance?.UnlockSound();
                SoundManager.Instance?.UnlockSoundControl();
                break;

            case AbilityType.Settings:
                UIManager.Instance?.UnlockSettings();
                break;

            case AbilityType.Exit:
                UIManager.Instance?.UnlockExit();
                break;

            case AbilityType.Brightness:
                BrightnessManager.Instance?.BrightnessIconActive();
                break;
        }

        // ================= EFFECT =================
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
