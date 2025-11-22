using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangingMusic : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> audioClips;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnSoundChanged(int val)
    {
        switch (val)
        {
            case 0:
                audioSource.resource = audioClips[0];
                break;

            case 1:
                audioSource.resource = audioClips[1];
                break;

            case 2:
                audioSource.resource = audioClips[2];
                break;
            case 3:
                audioSource.resource = audioClips[3];
                break;
        }

        audioSource.Play();
    }
}
