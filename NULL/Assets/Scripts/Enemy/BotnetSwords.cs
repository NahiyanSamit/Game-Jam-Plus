using UnityEngine;

public class BotnetSwords : MonoBehaviour
{
    [SerializeField] private int damage = 4;

    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit the player
        if (!other.CompareTag("Player")) return;

        // Get Health from Player or its parent
        Health playerHealth = other.GetComponentInParent<Health>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("Player hit by sword");
        }
    }
}