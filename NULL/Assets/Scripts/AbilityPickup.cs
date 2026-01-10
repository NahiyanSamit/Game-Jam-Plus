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

            // If buy failed, stop
            if (!GameManager.Instance.HasAbility(AbilityType.Gun))
                return;
        }
        else
        {
            // ================= NORMAL ABILITY =================
            GameManager.Instance.UnlockAbility(abilityToUnlock);
        }

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

        // ================= UI / SYSTEM =================
        switch (abilityToUnlock)
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
