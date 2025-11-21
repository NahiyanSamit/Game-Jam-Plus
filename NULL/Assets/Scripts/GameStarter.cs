using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameStarter : MonoBehaviour
{
    public Button playButton;
    public GameObject LiePannel;
    public TextMeshProUGUI Text;
    public Button tryAgainButton;

    public int sceneIndex = 1; 

    private int step = 0;

    public void OnPlayClicked()
    {
        playButton.interactable = false; 
        LiePannel.SetActive(true);
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        Text.text = "Loading Fun...";
        yield return new WaitForSeconds(2f);

        Text.text = "0% Fun found.";
        tryAgainButton.gameObject.SetActive(true);  // show first Try Again button
    }

    // Called when player presses Try Again
    public void OnTryAgainPressed()
    {
        tryAgainButton.gameObject.SetActive(false);

        if (step == 0)
        {
            StartCoroutine(TryAgainStep1());
        }
        else if (step == 1)
        {
            StartCoroutine(TryAgainStep2());
        }

        step++;
    }

    IEnumerator TryAgainStep1()
    {
        Text.text = "Trying again...";
        yield return new WaitForSeconds(3.0f);

        Text.text = "Still no fun.";
        yield return new WaitForSeconds(0.7f);

        tryAgainButton.gameObject.SetActive(true); // show Try Again again
    }

    IEnumerator TryAgainStep2()
    {
        Text.text = "Ok, 1% fun detected.";
        yield return new WaitForSeconds(1.5f);

        Text.text = "Fine, starting the NON-fun version.";
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(sceneIndex);
    }

}
