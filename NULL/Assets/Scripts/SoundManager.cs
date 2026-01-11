using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource; // Drag your Music AudioSource here
    public AudioSource sfxSource;   // Drag your SFX AudioSource here

    [Header("UI References")]
    public GameObject soundUIContainer; // The Panel holding the sliders
    public Slider musicSlider;          // Drag Music Slider here
    public Slider sfxSlider;            // Drag SFX Slider here

    // Variable to track if sound has been collected/unlocked
    private bool _isSoundActive = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 1. Hide UI initially
        if (soundUIContainer != null) 
            soundUIContainer.SetActive(false);

        // 2. STOP MUSIC INITIALLY
        // We ensure music is silent until the item is picked up
        if (musicSource != null)
        {
            musicSource.Stop(); 
            musicSource.volume = 0.5f; 
        }

        if (sfxSource != null) sfxSource.volume = 0.5f;

        // 3. AUTO-CONNECT SLIDERS (The Code Fix)
        // This connects the sliders to the volume functions automatically.
        // You don't need to set up "On Value Changed" in the Inspector.
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    // --- UNLOCK LOGIC ---
    public void UnlockSoundControl()
    {
        // 1. Enable the sound system gate
        _isSoundActive = true;

        // 2. Start the Music NOW
        if (musicSource != null)
        {
            musicSource.Play();
        }

        // 3. Show UI
        if (soundUIContainer != null) 
            soundUIContainer.SetActive(true);
        
        // 4. Sync sliders to match the code's volume
        if (musicSlider != null) musicSlider.value = musicSource.volume;
        if (sfxSlider != null) sfxSlider.value = sfxSource.volume;

        Debug.Log("Sound System Activated!");
    }

    // --- VOLUME CONTROL ---
    public void SetMusicVolume(float vol)
    {
        if (musicSource != null) musicSource.volume = vol;
    }

    public void SetSFXVolume(float vol)
    {
        if (sfxSource != null) sfxSource.volume = vol;
    }

    // --- PLAY SFX ---
    // Called by PlayerController
    public void PlaySFX(AudioClip clip)
    {
        // If sound hasn't been unlocked yet, do nothing
        if (!_isSoundActive) return;

        if (sfxSource != null && clip != null)
        {
            // Randomize pitch slightly for realism (0.9 to 1.1)
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
            
            // Play the sound
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void LockSoundControl()
    {
        _isSoundActive = false;

        // Stop music
        if (musicSource != null)
            musicSource.Stop();

        // Hide UI
        if (soundUIContainer != null)
            soundUIContainer.SetActive(false);

        Debug.Log("Sound System Locked!");
    }

}