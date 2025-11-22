using UnityEngine;
using TMPro;
using System.Collections;

public class TMPSequentialDisplay : MonoBehaviour
{
    [SerializeField] private GameObject[] tmpTexts;  // Array of TMP GameObjects
    [SerializeField] private float delayBetween = 1f; // Optional delay between lines
    [SerializeField] private Canvas funnyUI;
    private int currentIndex = 0;
    [SerializeField] private AudioSource scaryAudioSource;

    void Start()
    {
        // Make sure all texts start inactive
        foreach (var go in tmpTexts)
            go.SetActive(false);

        StartCoroutine(DisplayTexts());
        
    }

    private IEnumerator DisplayTexts()
    {
        while (currentIndex < tmpTexts.Length)
        {
            tmpTexts[currentIndex].SetActive(true);

            // Wait for delay or player input
            float timer = 0f;
            while (timer < delayBetween)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                    break;

                timer += Time.deltaTime;
                yield return null;
            }

            // Optionally hide previous text
            tmpTexts[currentIndex].SetActive(false);

            currentIndex++;
        }
        scaryAudioSource.Play();
        funnyUI.gameObject.SetActive(false);
    }
}