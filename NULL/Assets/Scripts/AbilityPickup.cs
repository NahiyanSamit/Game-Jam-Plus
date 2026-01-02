using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public AbilityType abilityToUnlock;
    public GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // ===================== GUN (SPECIAL CASE) =====================
        // Gun must be bought, not picked like others
        if (abilityToUnlock == AbilityType.Gun)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.BuyGun();

                // If gun purchase failed (not enough coins)
                if (!GameManager.Instance.HasAbility(AbilityType.Gun))
                    return;
            }
        }
        else
        {
            // ===================== NORMAL ABILITIES =====================
            if (GameManager.Instance != null)
                GameManager.Instance.UnlockAbility(abilityToUnlock);
        }

        // ===================== FUN POINTS =====================
        if (FunManager.Instance != null)
            FunManager.Instance.AddFun(10f);

        // ===================== VISUALS =====================
        PlayerVisualSwitcher visuals = other.GetComponent<PlayerVisualSwitcher>();
        if (visuals != null)
        {
            if (abilityToUnlock == AbilityType.CharacterArt)
                visuals.UnlockArtModel();

            if (abilityToUnlock == AbilityType.Texture)
                visuals.UnlockTexture();

            if (abilityToUnlock == AbilityType.Animation)
                visuals.UnlockAnimationModel();
        }

        // ===================== UI =====================
        if (abilityToUnlock == AbilityType.UI)
            UIManager.Instance?.EnableGameUI();

        if (abilityToUnlock == AbilityType.Sound)
        {
            UIManager.Instance?.UnlockSound();
            SoundManager.Instance?.UnlockSoundControl();
        }

        if (abilityToUnlock == AbilityType.Settings)
            UIManager.Instance?.UnlockSettings();

        if (abilityToUnlock == AbilityType.Exit)
            UIManager.Instance?.UnlockExit();

        if (abilityToUnlock == AbilityType.Brightness)
            BrightnessManager.Instance?.BrightnessIconActive();

        // ===================== CAMERA FOLLOW =====================
        if (abilityToUnlock == AbilityType.Camera)
        {
            if (Camera.main != null)
            {
                CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
                if (cam != null)
                    cam.StartFollowing(other.transform);
            }
        }

        // ===================== EFFECT =====================
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
