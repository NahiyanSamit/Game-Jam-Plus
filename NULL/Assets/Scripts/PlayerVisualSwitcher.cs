using UnityEngine;

public class PlayerVisualSwitcher : MonoBehaviour
{
    [Header("Child Models")]
    public GameObject simpleCube;      // Drag your Cube child here
    public GameObject characterArt;    // Drag your Character Art child here

    void Start()
    {
        // Ensure we start with the cube enabled and art disabled
        // (Unless we already saved that we unlocked it, but keeping it simple for now)
        UpdateVisuals(false); 
    }

    // Helper function to handle the swapping
    public void UpdateVisuals(bool hasUnlockedArt)
    {
        if (hasUnlockedArt)
        {
            simpleCube.SetActive(false);
            characterArt.SetActive(true);
        }
        else
        {
            simpleCube.SetActive(true);
            characterArt.SetActive(false);
        }
    }
}