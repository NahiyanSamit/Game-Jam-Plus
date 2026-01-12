using UnityEngine;

public class BotnetSwords : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        DamageDealer dealer = GetComponent<DamageDealer>();
        Health playerHealth = other.GetComponentInParent<Health>();

        if (dealer != null && playerHealth != null)
            playerHealth.TakeDamage(dealer.Damage);
    }
}