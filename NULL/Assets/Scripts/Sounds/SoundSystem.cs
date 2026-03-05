using UnityEngine;
using UnityEngine.InputSystem; 
public class SoundSystem : MonoBehaviour
{
    private SystemCollections _systemCollections;
    private BrokenSoundSystem _brokenSoundSystem;
    [SerializeField]private AudioSource _audioSource;
    private bool perfectSound = false;
    [SerializeField] private GameObject _soundOptionUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource.Stop();
        _systemCollections = FindObjectOfType<SystemCollections>();
        _brokenSoundSystem = FindObjectOfType<BrokenSoundSystem>(); 
       // _systemCollections.OnAddedToInventory += PlayPickupSound;
    }

    // Update is called once per frame
    void Update()
    {
        // Only open the sound option UI if the systemCollections reports an added item
        // AND the collected ability type is Sound (prevents other pickups from opening this UI).
        if (_systemCollections != null && _systemCollections.AddedInTheInventory && !perfectSound)
        {
            if (_systemCollections.LastCollectedAbility == AbilityType.Sound && GameManager.Instance != null && GameManager.Instance.HasAbility(AbilityType.Sound))
            {
                if (_brokenSoundSystem != null && _brokenSoundSystem.BrokenSoundPrefab != null)
                    _brokenSoundSystem.BrokenSoundPrefab.SetActive(false);

                if (_soundOptionUI != null)
                    _soundOptionUI.gameObject.SetActive(true);

                perfectSound = true;

                // Reset the flag so subsequent unrelated pickups won't retrigger this
                _systemCollections.AddedInTheInventory = false;
            }
            else
            {
                // Clear the flag so other systems aren't affected.
                _systemCollections.AddedInTheInventory = false;
            }
        }

        EscapeSoundOption();
    }

    private void PlayPickupSound()
    {
        _audioSource.Play();
        Debug.Log("Audio is working");
    }

    private void EscapeSoundOption()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_soundOptionUI != null)
                _soundOptionUI.gameObject.SetActive(false);
        }
    }
}
