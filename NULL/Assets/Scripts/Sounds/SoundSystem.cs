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
        if(_systemCollections.AddedInTheInventory && !perfectSound)
        {
            _brokenSoundSystem.BrokenSoundPrefab.SetActive(false);
            _soundOptionUI.gameObject.SetActive(true);
            perfectSound=true;
          
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
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            _soundOptionUI.gameObject.SetActive(false);
        }
    }
}
