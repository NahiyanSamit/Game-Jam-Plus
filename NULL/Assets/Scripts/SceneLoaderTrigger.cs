using UnityEngine;

public class SceneLoaderTrigger : MonoBehaviour
{
    [Header("Settings")]
    public string sceneName;           // Name of scene to load (e.g., "Level2")
    public Vector3 playerSpawnPoint;   // Where should the player go? (e.g. 0, 2, 0)
    public bool shouldHideMainLevel;   // Check TRUE only for the very first trigger

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadNextLevel(sceneName, playerSpawnPoint, shouldHideMainLevel);
                Destroy(gameObject); // Remove this trigger so we don't hit it again
            }
        }
    }
}