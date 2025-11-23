using UnityEngine;
using TMPro;

public class DistanceDisplay : MonoBehaviour
{
    public Transform player;
    public Transform targetBox;     // environment box
    public TextMeshProUGUI distanceText;
    public int requiredCoin = 50;

    void Update()
    {
        if (player == null || targetBox == null || distanceText == null) return;

        // Only show distance if player has enough coins
        if (UIManager.Instance != null && UIManager.Instance.HasEnoughCoins(requiredCoin))
        {
            float distance = Vector3.Distance(player.position, targetBox.position);
            distanceText.text = "Distance: " + distance.ToString("F1") + "m";
            distanceText.gameObject.SetActive(true); // ensure it's visible
        }
        else
        {
            distanceText.gameObject.SetActive(false); // hide if not enough coins
        }
    }
}

