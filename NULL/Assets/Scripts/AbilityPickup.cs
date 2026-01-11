using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public AbilityType abilityToUnlock;
    public GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // ================= 1. UNLOCK ABILITY =================
        if (GameManager.Instance != null)
            GameManager.Instance.UnlockAbility(abilityToUnlock);

        // ================= 2. FUN POINTS =================
        if (FunManager.Instance != null)
            FunManager.Instance.AddFun(10f);

        // ================= 3. VISUAL SWITCHER =================
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

        // ================= 4. UI ABILITIES =================
        if (abilityToUnlock == AbilityType.UI)
            UIManager.Instance?.EnableGameUI();

        if (abilityToUnlock == AbilityType.Settings)
            UIManager.Instance?.UnlockSettings();

        if (abilityToUnlock == AbilityType.Exit)
            UIManager.Instance?.UnlockExit();

        // ================= 5. SOUND / BRIGHTNESS =================
        if (abilityToUnlock == AbilityType.Sound)
            SoundManager.Instance?.UnlockSoundControl();

        if (abilityToUnlock == AbilityType.Brightness)
            UIManager.Instance?.UnlockBrightness();

        // ================= 6. COMBAT ABILITIES =================
        if (abilityToUnlock == AbilityType.Punch)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.ChangeWeapon(AbilityType.Punch);
        }

        if (abilityToUnlock == AbilityType.Gun)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.BuyGun();

                if (GameManager.Instance.HasAbility(AbilityType.Gun))
                    GameManager.Instance.ChangeWeapon(AbilityType.Gun);
            }
        }

        // ================= 7. APPLY ABILITY EFFECTS =================
        AbilityApplier applier = other.GetComponent<AbilityApplier>();
        if (applier != null)
            applier.ApplyAbilities();

        // ================= 8. EFFECT & DESTROY =================
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
