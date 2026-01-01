using UnityEngine;

public class CameraAbilityGate : MonoBehaviour
{
    public CameraFollow cameraFollow;
    public Transform player;

    void Update()
    {
        if (!cameraFollow || !player) return;

        if (GameManager.Instance.HasAbility(AbilityType.Camera))
        {
            cameraFollow.target = player;
            enabled = false; // run once
        }
    }
}