using UnityEngine;

public class HealthBarFollow : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position;
    }
}