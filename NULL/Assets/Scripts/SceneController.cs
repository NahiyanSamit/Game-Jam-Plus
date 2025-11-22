using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [Header("References")]
    public GameObject player;               
    public GameObject mainSceneEnvironment; 

    [Header("UI References")]
    public CanvasGroup fadePanel; // Drag your 'FadePanel' here!

    [Header("Settings")]
    public float fadeDuration = 1f; // How long it takes to go dark

    private string _currentSubScene = ""; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LoadNextLevel(string sceneToLoad, Vector3 spawnPosition, bool hideMainEnv)
    {
        StartCoroutine(LoadRoutine(sceneToLoad, spawnPosition, hideMainEnv));
    }

    IEnumerator LoadRoutine(string newSceneName, Vector3 spawnPos, bool hideMainEnv)
    {
        // 1. FADE TO BLACK
        // We block input so player can't move while loading
        if (fadePanel != null) fadePanel.blocksRaycasts = true; 
        yield return StartCoroutine(Fade(1f)); // Target Alpha 1 (Black)

        // --- PLAYER IS NOW BLIND, WE CAN DO THE UGLY WORK ---

        // 2. Unload old sub-scene
        if (!string.IsNullOrEmpty(_currentSubScene))
        {
            yield return SceneManager.UnloadSceneAsync(_currentSubScene);
        }

        // 3. Hide Main Environment if needed
        if (hideMainEnv && mainSceneEnvironment != null)
        {
            mainSceneEnvironment.SetActive(false);
        }

        // 4. Load new Scene
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
        while (!loadOp.isDone) yield return null;

        // 5. Teleport Player (While screen is still black)
        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true; // Stop physics momentarily

            player.transform.position = spawnPos;
            Physics.SyncTransforms(); 

            if (rb) rb.isKinematic = false;
        }
        
        // 6. Update Tracker
        _currentSubScene = newSceneName;

        // --- WORK DONE, OPEN EYES ---

        // 7. FADE TO CLEAR
        yield return StartCoroutine(Fade(0f)); // Target Alpha 0 (Clear)
        if (fadePanel != null) fadePanel.blocksRaycasts = false;
    }

    // This little helper handles the math of smooth fading
    IEnumerator Fade(float targetAlpha)
    {
        if (fadePanel == null) yield break;

        float startAlpha = fadePanel.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadePanel.alpha = alpha;
            yield return null;
        }
        fadePanel.alpha = targetAlpha;
    }
}