using UnityEngine;

public class PlayerVisualSwitcher : MonoBehaviour
{
    [Header("Stage 1: Default")]
    public GameObject simpleCube;       // Drag 'Cube' here

    [Header("Stage 2: Art (Static T-Pose)")]
    public GameObject characterArt;     // Drag your static 'boy01' here (No Animator)

    [Header("Stage 3: Animation (Main Character)")]
    public GameObject mainCharacter;    // Drag your 'Main Character' here (With Animator)

    [Header("Texture")]
    public Material texturedMaterial;   // Drag your final material here

    // We store if the texture is unlocked so we can apply it to the Main Character
    // automatically if we switch to it later.
    private bool _textureIsUnlocked = false;

    void Start()
    {
        // 1. Start State: Only Cube is active
        simpleCube.SetActive(true);
        characterArt.SetActive(false);
        mainCharacter.SetActive(false);
    }

    // STAGE 2: Switch from Cube to Static Art
    public void UnlockArtModel()
    {
        simpleCube.SetActive(false);
        characterArt.SetActive(true);
        mainCharacter.SetActive(false);

        // If we collected texture earlier, apply it now
        if (_textureIsUnlocked) ApplyTexture(characterArt);
    }

    // STAGE 3: Switch from Static Art to Main Character (Animation)
    public void UnlockAnimationModel()
    {
        simpleCube.SetActive(false);
        characterArt.SetActive(false);
        mainCharacter.SetActive(true);

        // If we collected texture earlier, apply it now
        if (_textureIsUnlocked) ApplyTexture(mainCharacter);
    }

    // TEXTURE: Apply material to whatever is currently visible
    public void UnlockTexture()
    {
        _textureIsUnlocked = true;

        // Try to apply to both models (so they are ready when swapped)
        ApplyTexture(characterArt);
        ApplyTexture(mainCharacter);
    }

    // Helper function to find Renderer and set Material
    private void ApplyTexture(GameObject targetObj)
    {
        if (targetObj == null) return;

        Renderer r = targetObj.GetComponent<Renderer>();
        // If not on the parent, look in the children (common for imported models)
        if (r == null) r = targetObj.GetComponentInChildren<Renderer>();

        if (r != null)
        {
            r.material = texturedMaterial;
        }
    }
}