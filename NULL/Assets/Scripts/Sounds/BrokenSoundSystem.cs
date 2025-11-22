using UnityEngine;
using System;
public class BrokenSoundSystem : MonoBehaviour
{
    private SystemCollections _systemCollections;
    [SerializeField] private AudioSource audioSource;
    private bool brokensound = true,brokenSoundCollector;
    [SerializeField] private GameObject brokenSoundPrefab;
    public event Action BrokenSystemInventory;

    [SerializeField] private GameObject tmSequence,funnyUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject BrokenSoundPrefab
    {
        get => brokenSoundPrefab;
        set => brokenSoundPrefab = value;
    }
    void Start()
    {
        audioSource.Stop();
        _systemCollections = FindObjectOfType<SystemCollections>();
       // _systemCollections.OnAddedToInventory += EarDestroySound;
    }

    // Update is called once per frame
    void Update()
    {
        EarDestroySound();
    }

    private void EarDestroySound()
    {
        if (brokenSoundCollector)
        {
            brokenSoundPrefab.SetActive(true);
            brokenSoundCollector = false;
            Debug.Log("Broken Sound Collector is working");
            tmSequence.SetActive(true);
            funnyUI.SetActive(true);
        }

    }
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            brokenSoundCollector = true;
            BrokenSystemInventory?.Invoke();
        }
    }
     
}