using UnityEngine;

public class PlayerVisualSwitcher : MonoBehaviour
{
    [Header("Child Objects")]
    public GameObject simpleCube;      // Drag 'Cube' here
    public GameObject characterArt;    // Drag 'boy01_Body_Geo' here

    [Header("Texture")]
    public Material texturedMaterial;  // Drag your final material here

    private Animator _artAnimator;

    void Start()
    {
        // 1. Find the Animator on boy01_Body_Geo
        if (characterArt != null)
        {
            _artAnimator = characterArt.GetComponent<Animator>();
        }

        // 2. Start: Cube ON, Boy OFF, Animation FROZEN
        UpdateVisuals(false);
        EnableAnimation(false);
    }

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

    public void UpdateTexture(bool hasUnlockedTexture)
    {
        if (hasUnlockedTexture)
        {
            // Apply texture to boy01_Body_Geo
            Renderer rend = characterArt.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = texturedMaterial;
            }
        }
    }

    public void EnableAnimation(bool hasUnlockedAnim)
    {
        // This turns the Animator component ON or OFF
        if (_artAnimator != null)
        {
            _artAnimator.enabled = hasUnlockedAnim;
        }
    }
}