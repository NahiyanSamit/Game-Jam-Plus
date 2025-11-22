using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Settings")]
    public int value = 1;
    public float rotateSpeed = 100f; // <--- Speed of rotation

    [Header("Effects")]
    public GameObject collectEffect; 
    public AudioClip collectSound;   

    // 1. NEW: This makes the coin spin every frame
    void Update()
    {
        // Rotate around the Y axis (Up)
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddCoin(value);

            if (SoundManager.Instance != null && collectSound != null)
                SoundManager.Instance.PlaySFX(collectSound);

            if (collectEffect != null)
                Instantiate(collectEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}