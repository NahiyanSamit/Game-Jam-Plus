using UnityEngine;
using UnityEngine.UI; // <--- Needed for Slider

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("References")]
    public AudioSource musicSource;      // Drag your Audio Source here
    public GameObject soundUIContainer;  // Drag the Panel holding the Icon and Slider
    public Slider volumeSlider;          // Drag the Slider here

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 1. Hide the UI initially
        if (soundUIContainer != null)
            soundUIContainer.SetActive(false);

        // 2. Set volume to 0 initially (Muted)
        if (musicSource != null)
        {
            musicSource.volume = 0f;
            // Ensure the clip is looping and playing, but silent
            musicSource.loop = true; 
            musicSource.Play();
        }
    }

    // Call this when picking up the item
    public void UnlockSoundControl()
    {
        // Show the UI
        if (soundUIContainer != null)
            soundUIContainer.SetActive(true);

        // Set default volume to 50% so player hears it immediately
        if (musicSource != null && volumeSlider != null)
        {
            musicSource.volume = 0.5f;
            volumeSlider.value = 0.5f;
        }

        Debug.Log("Sound Unlocked!");
    }

    // Link this function to the Slider's "On Value Changed" event
    public void SetVolume(float vol)
    {
        if (musicSource != null)
        {
            musicSource.volume = vol;
        }
    }
}