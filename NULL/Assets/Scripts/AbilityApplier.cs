using UnityEngine;

public class AbilityApplier : MonoBehaviour
{
    public PlayerController playerController;

    void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
    }

    public void ApplyAbilities()
    {
        if (GameManager.Instance == null || playerController == null)
            return;

        // =================== GUN ===================
        if (!GameManager.Instance.HasAbility(AbilityType.Gun))
        {
            if (playerController.gunModel != null)
                playerController.gunModel.SetActive(false);

            if (GameManager.Instance.GetCurrentWeapon() == AbilityType.Gun)
                GameManager.Instance.ChangeWeapon(AbilityType.Punch);
        }


        // =================== UI ===================
        if (UIManager.Instance != null)
        {
            if (GameManager.Instance.HasAbility(AbilityType.UI))
                UIManager.Instance.EnableGameUI();
            else
                UIManager.Instance.DisableGameUI();
        }


        // =================== SOUND ===================
        if (SoundManager.Instance != null && UIManager.Instance != null)
        {
            if (GameManager.Instance.HasAbility(AbilityType.Sound))
                UIManager.Instance.UnlockSound();
            else
                UIManager.Instance.LockSound();
        }


        // =================== BRIGHTNESS ===================
        if (UIManager.Instance != null)
        {
            if (GameManager.Instance.HasAbility(AbilityType.Brightness))
                UIManager.Instance.UnlockBrightness();
            else
                UIManager.Instance.LockBrightness();
        }
    }
}