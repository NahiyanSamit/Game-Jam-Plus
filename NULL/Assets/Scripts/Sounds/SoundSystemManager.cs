using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SoundSystemManager : MonoBehaviour
{
    private bool brokensystem = true;
    private SystemCollections _systemCollections;
    [SerializeField] private AudioSource audioSource;
    // [SerializeField] private Dropdown sounddropdown;
    [SerializeField] private Slider[] sliders;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _systemCollections = FindObjectOfType<SystemCollections>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_systemCollections.AddedInTheInventory && brokensystem)
        {
            BrokenSoundSystem();
            brokensystem = false;
        }

    }

    public void BrokenSoundSystem()
    {
       audioSource.Play();
    }

    public void PerfectSoundSystem()
    {
        
    }

    public void PrioritySound()
    {
         audioSource.priority= (int)sliders[0].value;
    }

    public void VolumeOfSound()
    {
        audioSource.volume = sliders[1].value;
    }
    public void PitchOfSound()
    {
        audioSource.pitch = sliders[2].value;
    }   
    public void StereoOfSound()
    {
        audioSource.panStereo = sliders[3].value;
    }  
    public void SpatialBlendOfSound()
    {
        audioSource.spatialBlend = sliders[4].value;
    } 
    public void ReverbZoneMixOfSound()
    {
        audioSource.reverbZoneMix = sliders[5].value;
    }
    
}
