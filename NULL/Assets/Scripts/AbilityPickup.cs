using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public AbilityType abilityToUnlock;
    public GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (abilityToUnlock == AbilityType.Gun)
        {
            Debug.Log("Gun must be purchased from shop.");
            return;
        }

        // Unlock ability
        GameManager.Instance?.UnlockAbility(abilityToUnlock);

        // 🔥 CAMERA FOLLOW TRIGGER (THIS WAS MISSING)
        if (abilityToUnlock == AbilityType.Camera)
        {
            if (Camera.main != null)
            {
                CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
                if (cam != null)
                {
                    cam.StartFollowing(other.transform);
                    Debug.Log("Camera started following player.");
                }
                else
                {
                    Debug.LogError("CameraFollow not found on Main Camera!");
                }
            }
        }

        // Fun points
        FunManager.Instance?.AddFun(10f);

        // Visuals
        PlayerVisualSwitcher visuals = other.GetComponent<PlayerVisualSwitcher>();
        if (visuals != null)
        {
            if (abilityToUnlock == AbilityType.CharacterArt) visuals.UnlockArtModel();
            if (abilityToUnlock == AbilityType.Texture) visuals.UnlockTexture();
            if (abilityToUnlock == AbilityType.Animation) visuals.UnlockAnimationModel();
        }

        // UI-related abilities
        if (abilityToUnlock == AbilityType.UI)
            UIManager.Instance?.EnableGameUI();

        if (abilityToUnlock == AbilityType.Sound)
            UIManager.Instance?.UnlockSound();

        if (abilityToUnlock == AbilityType.Settings)
            UIManager.Instance?.UnlockSettings();

        if (abilityToUnlock == AbilityType.Exit)
            UIManager.Instance?.UnlockExit();

        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
