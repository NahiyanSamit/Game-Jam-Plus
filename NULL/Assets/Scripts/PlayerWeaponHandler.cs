using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    [Header("Gun")]
    public GameObject gunModel; 

    void Start()
    {
        UpdateGunState();
    }

    void Update()
    {
        UpdateGunState();
    }

    void UpdateGunState()
    {
        if (!gunModel) return;

        if (GameManager.Instance != null &&
            GameManager.Instance.HasAbility(AbilityType.Gun))
        {
            if (!gunModel.activeSelf)
                gunModel.SetActive(true);
        }
        else
        {
            if (gunModel.activeSelf)
                gunModel.SetActive(false);
        }
    }
}